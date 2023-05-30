using HoYoLabApi.Enums;

namespace HoYoLabApi.GenshinImpact;

public static class Extensions
{
	public static string GetGenshinRegion(this Region region)
	{
		return region switch
		{
			Region.Europe => "os_euro",
			_ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
		};
	}
}