using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests.Static;

public abstract class TestBase
{
	public TestContext TestContext { get; set; }

	protected Task<string> ReadTextAsync(string relativePath)
		=> File.ReadAllTextAsync(TestContext.CurrentContext.TestDirectory + "\\" + relativePath);
}
