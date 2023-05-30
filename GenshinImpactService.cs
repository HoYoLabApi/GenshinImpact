using HoYoLabApi;
using HoYoLabApi.interfaces;
using HoYoLabApi.Models;
using HoYoLabApi.Static;

namespace HoyoLabApi.GenshinImpact;

public class GenshinImpactService
{
	private readonly IHoYoLabClient m_client;

	public GenshinImpactService(IHoYoLabClient client)
		=> m_client = client;
	
	public async Task<GameData> GetGameAccountAsync(ICookies cookies)
	{
		return (await m_client.GetGamesArrayAsync(new Request(
			"api-account-os",
			"account/binding/api/getUserGameRolesByCookieToken",
			cookies,
			new Dictionary<string, string>()
			{
				{ "game_biz", "hk4e_global" },
				{ "uid", cookies.AccountId.ToString() },
				{ "sLangKey", cookies.Language.GetLanguageString() },
			}
		)).ConfigureAwait(false)).Data.GameAccounts.FirstOrDefault()!;
	}
	
	public Task<GameData> GetGameAccountAsync(string cookies)
		=> GetGameAccountAsync(cookies.ParseCookies());
	
	public Task<GameData> GetGameAccountAsync()
		=> GetGameAccountAsync(m_client.Cookies);
	
	public async Task<IDailyClaimResult> DailyClaimAsync(ICookies cookies)
	{
		return await m_client.DailyClaimAsync(new Request(
			"sg-hk4e-api",
			"/event/sol/sign",
			cookies,
			new Dictionary<string, string>
			{
				{ "act_id", "e202102251931481" },
				{ "lang", cookies.Language.GetLanguageString() }
			}
		)).ConfigureAwait(false);
	}
	
	private async IAsyncEnumerable<IDailyClaimResult> DailysClaimAsync(ICookies[] cookies, CancellationToken? cancellationToken = null)
	{
		cancellationToken ??= CancellationToken.None;
		foreach (var cookie in cookies)
		{
			if (cancellationToken.Value.IsCancellationRequested)
				yield break;
			
			yield return await DailyClaimAsync(cookie).ConfigureAwait(false);
		}
	}
	
	public IAsyncEnumerable<IDailyClaimResult> DailysClaimAsync(string[] cookies, CancellationToken? cancellationToken = null)
		=> DailysClaimAsync(cookies.Select(x => x.ParseCookies()).ToArray());
	
	public Task<IDailyClaimResult> DailyClaimAsync()
		=> DailyClaimAsync(m_client.Cookies);

	public Task<IDailyClaimResult> DailyClaimAsync(string cookies)
		=> DailyClaimAsync(cookies.ParseCookies());

	public async Task<ICodeClaimResult> CodeClaimAsync(ICookies cookies, string code)
	{
		var gameAcc = await GetGameAccountAsync(cookies).ConfigureAwait(false);
		
		return await m_client.CodeClaimAsync(new CodeClaimRequest(
			"sg-hk4e-api",
			"common/apicdkey/api/webExchangeCdkey",
			cookies,
			new Dictionary<string, string>()
			{
				{ "uid", gameAcc.Uid.ToString() },
				{ "region", gameAcc.Region.GetGenshinRegion() },
				{ "game_biz", gameAcc.Game },
				{ "cdkey", code },
				{ "sLangKey", cookies.Language.GetLanguageString() },
				{ "lang", cookies.Language.GetGenshinLang() },
			},
			new Dictionary<string, string>()
			{
				{ "Referer", "https://genshin.hoyoverse.com" },
				{ "Orig", "https://genshin.hoyoverse.com" }
			}
		)).ConfigureAwait(false);
	}
	
	public Task<ICodeClaimResult> CodeClaimAsync(string code)
		=> CodeClaimAsync(m_client.Cookies, code);

	public async IAsyncEnumerable<ICodeClaimResult> CodesClaimAsync(
		string[] codes,
		CancellationToken? cancellationToken = null)
	{
		cancellationToken ??= CancellationToken.None;
		foreach (var code in codes)
		{
			if (cancellationToken.Value.IsCancellationRequested)
				yield break;
			
			yield return await CodeClaimAsync(code).ConfigureAwait(false);
		}
	}
}