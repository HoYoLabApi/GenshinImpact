using HoYoLabApi.Classes;
using HoYoLabApi.Enums;
using HoYoLabApi.interfaces;
using HoYoLabApi.Models;

namespace HoYoLabApi.GenshinImpact;

public abstract class GenshinImpactServiceBase
{
	private static readonly Func<GameData, ClaimRequest> s_codeClaim = gameAcc => new ClaimRequest(
		"sg-hk4e-api", "", null, gameAcc.Region.GetGenshinRegion(), gameAcc
	);

	private static readonly ClaimRequest s_dailyClaim = new(
		"sg-hk4e-api",
		"event/sol/sign",
		"e202102251931481"
	);

	protected readonly IHoYoLabClient Client;

	private readonly AccountSearcher m_accountSearcher;
	private readonly CodesClaimer m_codesClaimer;
	private readonly DailyClaimer m_dailyClaimer;

	protected GenshinImpactServiceBase(IHoYoLabClient client)
	{
		Client = client;
		m_accountSearcher = new AccountSearcher(client);
		m_dailyClaimer = new DailyClaimer(client, s_dailyClaim);
		m_codesClaimer = new CodesClaimer(client, m_accountSearcher);
	}

	public Task<GameData[]> GetGameAccountAsync(ICookies? cookies = null)
	{
		return m_accountSearcher.GetGameAccountAsync(cookies ?? Client.Cookies!, "hk4e_global");
	}

	public Task<IDailyClaimResult> DailyClaimAsync(ICookies cookies)
	{
		return m_dailyClaimer.DailyClaimAsync(cookies);
	}

	public IAsyncEnumerable<IDailyClaimResult> DailiesClaimAsync(ICookies[] cookies,
		CancellationToken? cancellationToken = null)
	{
		return m_dailyClaimer.DailiesClaimAsync(cookies, cancellationToken);
	}

	public async Task<ICodeClaimResult> CodeClaimAsync(ICookies cookies, string code, Region? region = null)
	{
		var gameAcc = await GetGameAccountAsync(cookies).ConfigureAwait(false);
		region ??= gameAcc.First().Region;
		return await m_codesClaimer.CodeClaimAsync(cookies, code, s_codeClaim(gameAcc.First(x => x.Region == region)));
	}

	public async IAsyncEnumerable<ICodeClaimResult> CodesClaimAsync(
		ICookies cookies,
		string[] codes,
		Region? region = null,
		CancellationToken? cancellationToken = null)
	{
		cancellationToken ??= CancellationToken.None;
		var gameAcc = await GetGameAccountAsync(cookies).ConfigureAwait(false);
		region ??= gameAcc.First().Region;
		foreach (var code in codes)
		{
			if (cancellationToken.Value.IsCancellationRequested)
				yield break;

			yield return await CodeClaimAsync(cookies, code, gameAcc.First(x => x.Region == region).Region);
		}
	}
}