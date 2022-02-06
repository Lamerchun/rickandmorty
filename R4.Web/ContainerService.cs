using System;
using Autofac;

namespace R4;

public interface IContainerService : ISingletonService
{
	void InjectProperties<T>(T target);

	T CreateWithPropertiesInjected<T>()
		where T : class, new();

	T Resolve<T>();
	ISingletonService Resolve(Type type);

	ISingletonService ResolveOptional(Type type);

	T ResolveOptional<T>()
		where T : class, ISingletonService;

	ISingletonService ResolveOptionalGeneric(Type genericType, Type genericParameter);
}

public class ContainerService : IContainerService
{
	public ILifetimeScope ILifetimeScope { get; set; }

	public void InjectProperties<T>(T target)
		=> ILifetimeScope.InjectProperties(target);

	public T Resolve<T>()
		=> ILifetimeScope.Resolve<T>();

	public ISingletonService Resolve(Type type)
		=> ILifetimeScope.Resolve(type) as ISingletonService;

	public ISingletonService ResolveOptional(Type type)
		=> ILifetimeScope.ResolveOptional(type) as ISingletonService;

	public T ResolveOptional<T>()
		where T : class, ISingletonService
		=> ILifetimeScope.ResolveOptional<T>();

	public T CreateWithPropertiesInjected<T>() where T : class, new()
	{
		var result = new T();
		InjectProperties(result);
		return result;
	}

	public ISingletonService ResolveOptionalGeneric(Type genericType, Type genericParameter)
	{
		var serviceType =
			genericType.MakeGenericType(new[] { genericParameter });

		return ResolveOptional(serviceType);
	}
}
