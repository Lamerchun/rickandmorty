using OpenQA.Selenium;

namespace Tests.E2e;

public static class E2eExtensions
{
	public static void GotoRelativeUrl(this IWebDriver instance, string relativeUrl)
	{
		var currentAbsoluteUrl =
			instance.Url;

		var startIndex =
			currentAbsoluteUrl.IndexOf("/", "https://".Length + 1);

		var baseUrl =
			currentAbsoluteUrl[..startIndex];

		instance.Url = baseUrl + relativeUrl;
		E2eHelper.WaitUI();
	}
}
