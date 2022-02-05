using System.Threading;

namespace Tests.E2e;

public static class E2eHelper
{
	public static void WaitUI()
		=> Thread.Sleep(250);

	public static void WaitApi()
		=> Thread.Sleep(500);
}
