using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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

builder.Services.AddResponseCompression();
builder.Services.AddSingleton<ISimpleWebClient>(new SimpleWebClient());

var app = builder.Build();

app.UseHsts();
app.UseResponseCompression();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(x => x.MapControllers());

if (app.Environment.IsDevelopment())
{
	app.UseViteRunningCheck();
	app.UseDeveloperExceptionPage();
	app.UseViteProxy();
}

app.Run();

