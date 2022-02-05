using OpenQA.Selenium;

namespace Tests.E2e;

public abstract class PageBase : ElementHost
{
	protected virtual string Url { get; }

	public PageBase(IWebDriver driver) : base(driver) { }

	protected virtual By ReadyElement { get; }

	public void GotoPage()
	{
		if (string.IsNullOrWhiteSpace(Url))
			Driver!.GotoRelativeUrl(Url!);

		if (ReadyElement != null)
		{
			WillWaitForReadyElement();
			WaitDisplayed(ReadyElement);
		}

		DidGotoPage();
	}

	protected virtual void WillWaitForReadyElement() { }
	protected virtual void DidGotoPage() { }
}
