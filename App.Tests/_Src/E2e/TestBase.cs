using System.Linq;
using NUnit.Framework;

namespace Tests.E2e;

[Category("SkipWhenLiveUnitTesting")]
public abstract class TestBase : ElementHost
{
	#region Construct

	public TestBase() : base(null)
	{
		Driver = Driver = E2eFixtureBase.Driver;
	}

	#endregion

	#region Tools

	public void BrowserSwitchTab(int index)
	{
		var handles = Driver.WindowHandles.ToList();
		Driver.SwitchTo().Window(handles[index]);
	}

	public void BrowserSwitchFirstTab()
		=> BrowserSwitchTab(0);

	#endregion
}
