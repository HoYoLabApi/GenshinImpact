using HoYoLabApi.interfaces;
using HoYoLabApi.Models;

namespace HoyoLabApi.GenshinImpact;

public sealed class CodeClaimRequest : Request
{
	public CodeClaimRequest(string subDomain, string path, ICookies? cookies, IDictionary<string, string>? query = null, IDictionary<string, string>? headers = null)
		: base(subDomain, path, cookies, query, headers)
	{
	}

	public override Uri GetFullUri()
	{
		var invalid = base.GetFullUri().ToString();
		return new Uri(invalid.Replace("hoyolab", "hoyoverse"));
	}
}