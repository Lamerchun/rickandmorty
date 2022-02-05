using System;
using System.IO;
using System.Diagnostics;

using NUnit.Framework;
using OpenQA.Selenium;

namespace Tests.E2e;

public abstract class ElementHost
{
	public IWebDriver Driver { get; set; }

	public ElementHost(IWebDriver driver)
	{
		Driver = driver;
	}

	protected IWebElement Element(By query)
		=> Driver.FindElement(query);

	protected void ClickSafe(By query)
		=> ClickSafe(Driver, query);

	protected void ClickSafe(ISearchContext searchContext, By query)
	{
		WaitDisplayed(searchContext, query).Click();
		E2eHelper.WaitUI();
	}

	protected void ClickApiSafe(By query)
		=> ClickApiSafe(Driver, query);

	protected void ClickApiSafe(ISearchContext searchContext, By query)
	{
		WaitDisplayed(searchContext, query).Click();
		E2eHelper.WaitApi();
	}

	protected void SendKeysSafe(By query, string value)
		=> SendKeysSafe(WaitDisplayed(query), value);

	protected void SendKeysSafe(IWebElement element, string value)
	{
		element.SendKeys(value);
		E2eHelper.WaitUI();
	}

	public void SubmitSafe(By query)
	{
		SendKeysSafe(query, Keys.Enter);
		E2eHelper.WaitApi();
	}

	protected IWebElement WaitDisplayed(By query)
		=> WaitDisplayed(Driver, query);

	private static bool _FirstWait = true;

	protected IWebElement WaitDisplayed(ISearchContext searchContext, By query)
	{
		var maxLoops = 2;

		if (_FirstWait)
		{
			_FirstWait = false;
			maxLoops = 2;
		}

		if (Debugger.IsAttached)
			maxLoops = 1;

		for (var i = 0; i < maxLoops; i++)
		{
			try
			{
				var result = searchContext.FindElement(query);
				if (result.Displayed)
					return result;
			}
			catch { }

			E2eHelper.WaitUI();
		}

		TakeScreenshot();
		throw new Exception("Waiting for Element failed.");
	}

	protected void TakeScreenshot()
	{
		var cam = Driver as ITakesScreenshot;
		var screenshot = cam.GetScreenshot();

		var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshot.png");
		screenshot.SaveAsFile(file, ScreenshotImageFormat.Png);
		TestContext.AddTestAttachment(file, $"Error: Waiting for Element failed.");
	}
}
