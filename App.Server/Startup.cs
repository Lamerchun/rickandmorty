using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace us
{
	public class Startup
	{
		public virtual void Configure(
			IApplicationBuilder app,
			IWebHostEnvironment env)
		{
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

			app.UseHttpsRedirection();

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
}
