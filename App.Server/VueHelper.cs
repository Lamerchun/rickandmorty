using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace us
{
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

		public static void UseViteNodeModules(this IApplicationBuilder app, IWebHostEnvironment env)
		{
			var node_modules_path = "node_modules";

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "_Client/" + node_modules_path + "/.vite")),
				RequestPath = "/" + node_modules_path + "/.vite"
			});
		}
	}
}
