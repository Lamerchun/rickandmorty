using System;
using R4;

namespace Tests.Static;

[DisableAutoDiscover]
public class ContainerServiceFake : IContainerService
{
	private readonly ITestContainer _TestContainer;

	public ContainerServiceFake(ITestContainer testContainer)
	{
		_TestContainer = testContainer;
	}

	#region IContainerService

	public T CreateWithPropertiesInjected<T>()
		where T : class, new()
		=> _TestContainer.Create<T>();

	public void InjectProperties<T>(T target)
		=> _TestContainer.Inject(target);

	public T Resolve<T>()
		=> (T)_TestContainer.Resolve(typeof(T));

	public ISingletonService Resolve(Type type)
		=> _TestContainer.Resolve(type) as ISingletonService;

	public ISingletonService ResolveOptional(Type type)
		=> _TestContainer.Resolve(type) as ISingletonService;

	T IContainerService.ResolveOptional<T>()
		=> ResolveOptional(typeof(T)) as T;

	public ISingletonService ResolveOptionalGeneric(Type genericType, Type genericParameter)
	{
		var serviceType =
			genericType.MakeGenericType(new[] { genericParameter });

		return ResolveOptional(serviceType);
	}

	#endregion
}
