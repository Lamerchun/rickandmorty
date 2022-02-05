using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using us;

namespace Tests.E2e;

[SetUpFixture]
public abstract class E2eFixtureBase
{
	#region Construct

	private IHost _Host { get; set; }
	private bool _Headless { get; set; }
	public static IWebDriver Driver { get; private set; }

	private const int _InstancePort = 5081;
	private readonly string _Url = $"https://localhost:{_InstancePort}";

	#endregion

	#region Tools

	private static string FindClosestDirectoryContaining(
		IEnumerable<string> filenames,
		string startDirectory)
	{
		var dir = startDirectory;

		while (true)
		{
			if (filenames.Any(filename => File.Exists(Path.Combine(dir, filename))))
				return dir;

			dir = Directory.GetParent(dir)?.FullName;
			if (string.IsNullOrEmpty(dir))
			{
				throw new FileNotFoundException(
					$"Could not locate a file called '{filenames}' in " +
					$"directory '{startDirectory}' or any parent directory.");
			}
		}
	}

	private IHost CreateWebHost()
	{
		var solutionDir =
			FindClosestDirectoryContaining(
				new[] { "_solutionroot" },
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
				);

		var sitePath = Path.Combine(solutionDir, "App.Server");

		var args = new[]
		{
				"--urls", _Url,
				"--contentroot", sitePath,
				"--environment", "development"
			};

		return Startup.CreateApp(args);
	}

	#endregion

	#region Setup / TearDown

	[OneTimeSetUp]
	public async Task Setup()
	{
		_Host = CreateWebHost();
		await _Host.StartAsync();

		var options = new ChromeOptions();

		_Headless =
			!Debugger.IsAttached;

		if (_Headless)
			options.AddArguments(new[] { "headless", "window-size=1024,786" });

		options.AddArguments("--ignore-certificate-errors");
		Driver = new ChromeDriver(options);
		Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
		Driver.Url = _Url;

		await Task.Delay(50);
	}

	[OneTimeTearDown]
	public async Task TearDown()
	{
		await _Host.StopAsync(TimeSpan.FromSeconds(5));
		await _Host.WaitForShutdownAsync();
		_Host.Dispose();

		Debugger.Break();
		Driver.Dispose();
	}

	#endregion
}
