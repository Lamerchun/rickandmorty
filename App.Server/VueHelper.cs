using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace us;

public static class VueHelper
{
	public static void UseViteRunningCheck(this IApplicationBuilder app)
	{
		app.Use(async (context, next) =>
		{
			if (!IsViteRunning())
			{
				await context.Response.WriteAsync($"Run 'npm run dev' to start Vite server on port {_DevServerPort} first.");
				return;
			}
			await next();
		});
	}

	public static void UseViteProxy(this IApplicationBuilder app)
	{
		var devServerEndpoint = new Uri($"https://localhost:{_DevServerPort}");
		app.UseSpa(x => x.UseProxyToSpaDevelopmentServer(devServerEndpoint));
	}

	private const int _DevServerPort = 3000;

	private static bool IsViteRunning()
		=>
		IPGlobalProperties.GetIPGlobalProperties()
			.GetActiveTcpListeners()
			.Select(x => x.Port)
			.Contains(_DevServerPort);
}
