using NUnit.Framework;
using System;

namespace Tests.E2e.Public;

public class SearchTests : TestBase
{
	[Test]
	[TestCaseSource(typeof(SearchSource), nameof(SearchSource.Prepare))]
	public void Home_Shows_Suggestions_And_Results(Action<HomePage> prepare)
	{
		// Arrange
		var homePage = new HomePage(Driver);
		homePage.GotoPage();

		// Act, Assert
		prepare(homePage);
		homePage.ResetInput();

		homePage.TypeSearch("ri");
		E2eHelper.WaitApi();
		WaitDisplayed(HomePage.Suggestions);

		homePage.EnterInput();
		WaitDisplayed(HomePage.Results);
	}

	public class SearchSource
	{
		public static Action<HomePage>[] Prepare =
			new Action<HomePage>[] {
				homePage =>
				{
					homePage.ClickGraphQL();
					homePage.ClickLive();
				},
				homePage =>
				{
					homePage.ClickGraphQL();
					homePage.ClickProxy();
				},
				homePage =>
				{
					homePage.ClickREST();
					homePage.ClickLive();
				},
				homePage =>
				{
					homePage.ClickREST();
					homePage.ClickProxy();
					E2eHelper.Wait(4_000);
				}
			};
	}
}
