using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

using us;

var builder = WebApplication.CreateBuilder(args);

builder
	.Services
	.AddMvc()
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

app.Run();

