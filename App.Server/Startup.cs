using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace us;

public static class Startup
{
	public static IHost CreateApp(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		var containingAssemly =
			typeof(Startup).Assembly;

		builder
			.Services
				.AddMvc()
				.AddApplicationPart(containingAssemly)
				.AddNewtonsoftJson();

		builder.Services.AddCompression();
		builder.Services.AddSingleton<ISimpleWebClient>(new SimpleWebClient());

		builder.Services.AddSingleton<IRickAndMortyApiService>(
			x => new RickAndMortyApiService(
				x.GetRequiredService<IMemoryCache>(),
				x.GetRequiredService<ISimpleWebClient>())
			);

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
		else
		{
			app.UseDefaultFiles();
			app.UseStaticFiles();
		}

		return app;
	}
}
