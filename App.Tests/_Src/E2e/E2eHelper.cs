using System.Threading;

namespace Tests.E2e;

public static class E2eHelper
{
	public static void WaitUI()
		=> Wait(250);

	public static void WaitApi()
		=> Wait(500);

	public static void Wait(int ms)
		=> Thread.Sleep(ms);
}
