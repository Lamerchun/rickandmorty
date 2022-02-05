using OpenQA.Selenium;

namespace Tests.E2e;

public class HomePage : PageBase
{
	public HomePage(IWebDriver driver) : base(driver) { }

	protected override string Url
		=> "/";

	protected override By ReadyElement
		=> Input;

	public void ClickGraphQL()
		=> ClickSafe(By.XPath("//ul[contains(@class, 'flex')][1]/li[1]"));

	public void ClickREST()
		=> ClickSafe(By.XPath("//ul[contains(@class, 'flex')][1]/li[2]"));

	public void ClickLive()
		=> ClickSafe(By.XPath("//ul[contains(@class, 'flex')][2]/li[1]"));

	public void ClickProxy()
		=> ClickSafe(By.XPath("//ul[contains(@class, 'flex')][2]/li[2]"));

	public void ResetInput()
	{
		SendKeysSafe(Input, Keys.Escape);
		SendKeysSafe(Input, Keys.Escape);
	}

	public void EnterInput()
	{
		SendKeysSafe(Input, Keys.Enter);
	}

	public static By Input
		=> By.XPath("//input");

	public static By Suggestions
		=> By.XPath("//ul[contains(@class, 'absolute')]");

	public static By Results
		=> By.XPath("//table");

	public void TypeSearch(string content)
		=> SendKeysSafe(Input, content);
}
