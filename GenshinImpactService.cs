using HoYoLabApi.Classes;
using HoYoLabApi.interfaces;
using HoYoLabApi.Models;
using HoYoLabApi.Static;

namespace HoYoLabApi.GenshinImpact;

public class GenshinImpactService : ServiceBase
{
	private static readonly Func<GameData, ClaimRequest> s_codeClaim = (GameData? gameAcc)
		=> ClaimRequest.FromData(gameAcc, "sg-hk4e-api", gameAcc?.Region.GetGenshinRegion());

	private static readonly ClaimRequest s_dailyClaim = new ClaimRequest(
		"sg-hk4e-api",
		"event/sol/sign",
		"e202102251931481"
	);
	
	public GenshinImpactService(IHoYoLabClient client) : base(client, s_codeClaim, s_dailyClaim)
	{
	}
	
	public Task<IGameResponse> GetGameAccountAsync(string? cookies = null)
	{
		return base.GetGameAccountAsync(cookies?.ParseCookies() ?? Client.Cookies!, "hk4e_global");
	}
}