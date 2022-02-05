using NUnit.Framework;

namespace Tests.E2e.Public;

public class SearchTests : TestBase
{
	[Test]
	public void Home_GraphQL_Live()
	{
		// Arrange
		var homePage = new HomePage(Driver);
		homePage.GotoPage();

		// Act, Assert
		homePage.ClickGraphQL();
		homePage.ClickLive();
		homePage.ResetInput();

		homePage.TypeSearch("ri");
		E2eHelper.WaitApi();
		WaitDisplayed(HomePage.Suggestions);

		homePage.EnterInput();
		WaitDisplayed(HomePage.Results);
	}

	[Test]
	public void Home_GraphQL_Proxy()
	{
		// Arrange
		var homePage = new HomePage(Driver);
		homePage.GotoPage();

		// Act, Assert
		homePage.ClickGraphQL();
		homePage.ClickProxy();
		homePage.ResetInput();

		homePage.TypeSearch("ri");
		E2eHelper.WaitApi();
		WaitDisplayed(HomePage.Suggestions);

		homePage.EnterInput();
		WaitDisplayed(HomePage.Results);
	}

	[Test]
	public void Home_REST_Live()
	{
		// Arrange
		var homePage = new HomePage(Driver);
		homePage.GotoPage();

		// Act, Assert
		homePage.ClickREST();
		homePage.ClickLive();
		homePage.ResetInput();

		homePage.TypeSearch("ri");
		E2eHelper.WaitApi();
		WaitDisplayed(HomePage.Suggestions);

		homePage.EnterInput();
		WaitDisplayed(HomePage.Results);
	}

	[Test]
	public void Home_REST_Proxy()
	{
		// Arrange
		var homePage = new HomePage(Driver);
		homePage.GotoPage();

		// Act, Assert
		homePage.ClickREST();
		homePage.ClickProxy();
		homePage.ResetInput();

		homePage.TypeSearch("ri");
		E2eHelper.WaitApi();
		WaitDisplayed(HomePage.Suggestions);

		homePage.EnterInput();
		WaitDisplayed(HomePage.Results);
	}
}
