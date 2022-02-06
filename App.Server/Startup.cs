using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using R4;

namespace us;

public static class Startup
{
	public static IHost CreateApp(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.AddAutofac();

		var containingAssemly =
			typeof(Startup).Assembly;

		builder
			.Services
				.AddMvc()
				.AddApplicationPart(containingAssemly)
				.AddControllersAsServices()
				.AddNewtonsoftJson();

		builder.Services.AddCompression();

		builder.Services
			.AddGraphQLServer()
			.AddQueryType<CharacterGraphQLQuery>();

		var app = builder.Build();

		app.UseHsts();
		app.UseResponseCompression();

		app.UseRouting();
		app.UseEndpoints(x =>
		{
			x.MapControllers();
			x.MapGraphQL();
		});

		if (app.Environment.IsDevelopment())
		{
			app.UseViteRunningCheck();
			app.UseDeveloperExceptionPage();
			app.UseViteProxy();
		}

		app.UseDefaultFiles();
		app.UseStaticFiles();

		return app;
	}
}
