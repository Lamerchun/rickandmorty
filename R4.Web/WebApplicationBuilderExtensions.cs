using Microsoft.AspNetCore.Builder;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace R4;

public static class WebApplicationBuilderExtensions
{
	public static void AddAutofac(this WebApplicationBuilder instance)
	{
		instance.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

		instance.Host.ConfigureContainer<ContainerBuilder>(
			(context, builder) =>
			{
				builder.RegisterAssemblies((builder, assembly) => builder.RegisterControllers(assembly));
			});
	}
}
