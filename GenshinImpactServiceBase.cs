using HoYoLabApi.Classes;
using HoYoLabApi.Enums;
using HoYoLabApi.interfaces;
using HoYoLabApi.Models;

namespace HoYoLabApi.GenshinImpact;

public abstract class GenshinImpactServiceBase
{
	private static readonly Func<GameData, ClaimRequest> s_codeClaim = (GameData? gameAcc) => new ClaimRequest(
		"sg-hk4e-api", "", null, gameAcc?.Region.GetGenshinRegion(), gameAcc
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

	public Task<(IGameResponse, Headers)> GetGameAccountAsync(ICookies? cookies = null)
	{
		return m_accountSearcher.GetGameAccountAsync(cookies ?? Client.Cookies!, "hk4e_global");
	}

	public Task<(IDailyClaimResult, Headers)> DailyClaimAsync(ICookies cookies)
	{
		return m_dailyClaimer.DailyClaimAsync(cookies);
	}

	public IAsyncEnumerable<(IDailyClaimResult, Headers)> DailiesClaimAsync(ICookies[] cookies,
		CancellationToken? cancellationToken = null)
	{
		return m_dailyClaimer.DailiesClaimAsync(cookies, cancellationToken);
	}

	private async Task<(ICodeClaimResult, Headers)> CodeClaimAsync(ICookies cookies, string code, GameData acc)
		=> await m_codesClaimer.CodeClaimAsync(cookies, code, s_codeClaim(acc));

	public async Task<(ICodeClaimResult, Headers)> CodeClaimAsync(ICookies cookies, string code, Region? region = null)
	{
		(ICodeClaimResult, Headers) res = (null!, default!);
		await foreach (var resp in CodesClaimAsync(cookies, new[] { code }, region))
			res = resp;

		return res;
	}

	public async IAsyncEnumerable<(ICodeClaimResult, Headers)> CodesClaimAsync(
		ICookies cookies,
		string[] codes,
		Region? region = null,
		CancellationToken? cancellationToken = null)
	{
		cancellationToken ??= CancellationToken.None;
		var (gameAcc, headers) = await GetGameAccountAsync(cookies).ConfigureAwait(false);
		var acc = gameAcc.Code == 0
			? region is not null
				? gameAcc.Data.GameAccounts.First(x => x.Region == region)
				: gameAcc.Data.GameAccounts.First()
			: null;
		
		if (acc is null)
		{
			yield return (new CodeClaimResult(gameAcc.Code, gameAcc.Message), headers);
			yield break;
		}
		
		foreach (var code in codes)
		{
			if (cancellationToken.Value.IsCancellationRequested)
				yield break;

			yield return await CodeClaimAsync(cookies, code, acc);
		}
	}
}