using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace us;

public class Startup
{
	public virtual void Configure(
		IApplicationBuilder app,
		IWebHostEnvironment env)
	{
		app.UseResponseCompression();

		if (env.IsDevelopment())
		{
			app.UseViteRunningCheck();
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseHsts();
		}

		app.UseRouting();
		app.UseDefaultFiles();
		app.UseStaticFiles();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});

		if (env.IsDevelopment())
		{
			app.UseViteProxy();
		}
	}

	public virtual void ConfigureServices(IServiceCollection services)
	{
		services.AddCompression();

		var controllerBuilder =
			services
				.AddMvc()
				.AddControllersAsServices()
				.ConfigureApiBehaviorOptions(options =>
				{
					options.InvalidModelStateResponseFactory =
						context => new BadRequestObjectResult(context.ModelState);
				});
	}
}
