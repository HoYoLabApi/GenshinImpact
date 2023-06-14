using HoYoLabApi.Enums;

namespace HoYoLabApi.GenshinImpact;

public static class Extensions
{
	public static string GetGenshinRegion(this Region region)
	{
		return region switch
		{
			Region.Europe => "os_euro",
			Region.America => "os_usa",
			Region.Asia => "os_asia",
			_ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
		};
	}
}