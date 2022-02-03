using System.IO.Compression;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.ResponseCompression;

namespace us;

public class Startup
{
	public virtual void Configure(
		IApplicationBuilder app,
		IWebHostEnvironment env)
	{
		app.UseHttpsRedirection();
		app.UseResponseCompression();

		if (env.IsDevelopment())
		{
			app.UseViteRunningCheck();
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		var provider = new FileExtensionContentTypeProvider();
		provider.Mappings[".vue"] = "application/javascript";

		app.UseStaticFiles(new StaticFileOptions
		{
			ServeUnknownFileTypes = true,
			DefaultContentType = "application/octet-stream",
			ContentTypeProvider = provider
		});

		app.UseStaticFiles();
		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
			endpoints.MapFallbackToPage("/", "/Index");
			endpoints.MapFallbackToPage("/{page}", "/Index");
			app.UseViteNodeModules(env);
		});

		app.UseViteProxy();
	}

	public virtual void ConfigureServices(IServiceCollection services)
	{
		services.AddResponseCompression(options =>
		{
			var MimeTypes = new[]
			{
					 "text/plain",
					 "text/html",
					 "text/json",
					 "text/css",
					 "font/woff2",
					 "image/png",
					 "image/x-icon",
					 "image/svg+xml",
					 "application/json",
					 "application/javascript",
					 "application/octet-stream"
				 };

			options.EnableForHttps = true;
			options.MimeTypes = MimeTypes;
			options.Providers.Add<GzipCompressionProvider>();
			options.Providers.Add<BrotliCompressionProvider>();
		});

		services.Configure<GzipCompressionProviderOptions>(options =>
		{
			options.Level = CompressionLevel.Optimal;
		});

		services.Configure<BrotliCompressionProviderOptions>(options =>
		{
			options.Level = CompressionLevel.Optimal;
		});

		var controllerBuilder =
			services
				.AddMvc()
				.AddControllersAsServices()
				.ConfigureApiBehaviorOptions(options =>
				{
					options.InvalidModelStateResponseFactory =
						context => new BadRequestObjectResult(context.ModelState);
				});

		controllerBuilder.AddNewtonsoftJson(x =>
		{
			x.SerializerSettings.FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Double;
		});
	}
}

