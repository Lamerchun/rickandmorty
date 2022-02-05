using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace us;

public static class Startup
{
	public static IHost CreateApp(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder
			.Services
			.AddMvc()
			.AddNewtonsoftJson()
			.AddControllersAsServices()
			.ConfigureApiBehaviorOptions(options =>
			{
				options.InvalidModelStateResponseFactory =
					context => new BadRequestObjectResult(context.ModelState);
			});

		builder.Services.AddCompression();
		builder.Services.AddSingleton<ISimpleWebClient>(new SimpleWebClient());

		builder.Services.AddSingleton<IRickAndMortyApiService>(
			x => new RickAndMortyApiService(
				x.GetRequiredService<ILogger<RickAndMortyApiService>>(),
				x.GetRequiredService<IMemoryCache>(),
				x.GetRequiredService<ISimpleWebClient>())
			);

		builder.Services
			.AddGraphQLServer()
			.AddQueryType<CharacterGraphQLQuery>();

		var app = builder.Build();

		app.UseHsts();
		app.UseResponseCompression();
		app.UseDefaultFiles();
		app.UseStaticFiles();

		app
			.UseRouting()
			.UseEndpoints(
				x =>
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

		return app;
	}
}
