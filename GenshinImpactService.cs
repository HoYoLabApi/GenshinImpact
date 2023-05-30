using HoYoLabApi.Classes;
using HoYoLabApi.interfaces;
using HoYoLabApi.Models;
using HoYoLabApi.Static;

namespace HoYoLabApi.GenshinImpact;

public class GenshinImpactService : GenshinImpactServiceBase
{
	public GenshinImpactService(IHoYoLabClient client) : base(client)
	{
	}
	
	public async Task DailiesClaimAsync(ICookies[] cookies)
	{
		await foreach (var _ in DailiesClaimAsync(cookies, null))
		{
		}
	}
	
	public async Task CodesClaimAsync(ICookies cookies, string[] codes)
	{
		await foreach (var _ in CodesClaimAsync(cookies, codes, null))
		{
		}
	}
	
	public Task<IDailyClaimResult> DailyClaimAsync(string cookies)
		=> base.DailyClaimAsync(cookies.ParseCookies());
	
	public Task<IDailyClaimResult> DailyClaimAsync()
		=> base.DailyClaimAsync(Client.Cookies!);

	public Task<ICodeClaimResult> CodeClaimAsync(string code)
		=> base.CodeClaimAsync(Client.Cookies!, code);
	
	public IAsyncEnumerable<IDailyClaimResult> DailiesClaimAsync(string[] cookies, CancellationToken? cancellationToken)
		=> base.DailiesClaimAsync(cookies.Select(x => x.ParseCookies()).ToArray(), cancellationToken);

	public Task CodesClaimAsync(string cookies, string[] codes)
		=> CodesClaimAsync(cookies.ParseCookies(), codes);

	public Task DailiesClaimAsync(string[] cookies)
		=> DailiesClaimAsync(cookies.Select(x => x.ParseCookies()).ToArray());

	public IAsyncEnumerable<ICodeClaimResult> CodesClaimAsync(
		string cookies,
		string[] codes,
		CancellationToken? cancellationToken)
		=> base.CodesClaimAsync(cookies.ParseCookies(), codes, cancellationToken);

	public IAsyncEnumerable<ICodeClaimResult> CodesClaimAsync(
		string[] codes,
		CancellationToken? cancellationToken)
		=> base.CodesClaimAsync(Client.Cookies!, codes, cancellationToken);
}