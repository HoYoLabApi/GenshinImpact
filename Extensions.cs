using HoYoLabApi.Enums;

namespace HoyoLabApi.GenshinImpact;

public static class Extensions
{
	public static string GetGenshinLang(this Language language)
	{
		return language switch
		{
			Language.English => "en",
			Language.Russian => "ru",
			_ => "en"
		};
	}
	
	public static string GetGenshinRegion(this Region region)
	{
		return region switch
		{
			Region.Europe => "os_euro",
			_ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
		};
	}
}