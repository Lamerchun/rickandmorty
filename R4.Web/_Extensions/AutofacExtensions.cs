using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

using Autofac;
using Autofac.Builder;

namespace R4;

public static class CommonAutofacExtensions
{
	public static void RegisterControllers(this ContainerBuilder instance, Assembly assembly)
	{
		instance
			.RegisterAssemblyTypes(assembly)
			.Where(x => x.InheritsFrom<ControllerBase>())
			.AsSelf()
			.InstancePerLifetimeScope()
			.PropertiesAutowired();
	}

	public static void RegisterAssemblies(this ContainerBuilder instance, Action<ContainerBuilder, Assembly> callback = null)
	{
		foreach (var assembly in AssemblyService.GetCodeAssemblies())
		{
			instance.RegisterServices(assembly);
			callback?.Invoke(instance, assembly);
		}
	}

	public static void RegisterServices(this ContainerBuilder instance, Assembly assembly)
	{
		instance
			.RegisterAssemblyTypes(assembly)
			.Where(x => !x.IsAbstract && x.HasInterface<ISingletonService>() && !x.HasAttribute<DisableAutoDiscoverAttribute>())
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance()
			.PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
			.AutoActivate();
	}

	public static void RegisterAutowired<T>(this ContainerBuilder instance)
	{
		instance
			.RegisterType<T>()
			.Config();
	}

	public static void RegisterAutowired<T>(this ContainerBuilder instance, T serviceInstance)
		where T : class
	{
		instance
			.RegisterInstance(serviceInstance)
			.Config();
	}

	private static IRegistrationBuilder<TLimit, TConcreteActivatorData, SingleRegistrationStyle> Config<TLimit, TConcreteActivatorData>(this IRegistrationBuilder<TLimit, TConcreteActivatorData, SingleRegistrationStyle> registration)
		where TConcreteActivatorData : IConcreteActivatorData
		=>
		registration
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance()
			.PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
			.AutoActivate();
}
