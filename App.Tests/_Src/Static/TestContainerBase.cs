using System;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Http;
using R4;

namespace Tests.Static;

public interface ITestContainer
{
	DateTime UtcNow { get; set; }
	void TickMinutes(int minutes);
	void TickMilliSeconds(int milliseconds);

	TimeServiceFake TimeServiceFake { get; }
	HttpContextAccessorFake HttpContextAccessorFake { get; }
	IContainerService ContainerServiceFake { get; }

	T Create<T>();
	void Inject<T>(T target);
	object Resolve(Type type);
	T Resolve<T>();
}

public abstract class TestContainerBase : ITestContainer
{
	#region Construct

	public TimeServiceFake TimeServiceFake { get; protected set; }
		= new();

	public HttpContextAccessorFake HttpContextAccessorFake { get; }
		= new();

	public IAssemblyService IAssemblyService { get; }
		= new AssemblyService();

	public TestContainerBase()
	{
		ContainerServiceFake =
			new ContainerServiceFake(this);
	}

	#endregion

	#region Tools

	protected void InjectService(object target, PropertyInfo property)
	{
		if (property.PropertyType == typeof(IContainerService))
		{
			property.SetValue(target, ContainerServiceFake);
			return;
		}
		else if (property.PropertyType == typeof(ITimeService))
		{
			property.SetValue(target, TimeServiceFake);
			return;
		}
		else if (property.PropertyType == typeof(IHttpContextAccessor))
		{
			property.SetValue(target, HttpContextAccessorFake);
			return;
		}
		else if (property.PropertyType == typeof(IAssemblyService))
		{
			property.SetValue(target, IAssemblyService);
			return;
		}

		property.SetValue(target, Resolve(property.PropertyType));
	}

	#endregion

	#region ITestContainer

	public DateTime UtcNow
	{
		get => TimeServiceFake.UtcNow;
		set => TimeServiceFake.SetUtcNow(value);
	}

	public void TickMinutes(int minutes)
		=> TimeServiceFake.TickMinutes(minutes);

	public void TickMilliSeconds(int milliseconds)
		=> TimeServiceFake.TickMilliSeconds(milliseconds);

	public IContainerService ContainerServiceFake { get; private set; }

	public T Create<T>()
	{
		var result = Activator.CreateInstance<T>();
		Inject(result);
		return result;
	}

	public void Inject<T>(T target)
	{
		var interfacesToInject =
			new[]
			{
					typeof(ISingletonService),
					typeof(IHttpContextAccessor)
			};

		var propertiesToInject =
			target.GetType()
				.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => interfacesToInject.Any(interf => x.PropertyType.InheritsFrom(interf)));

		foreach (var property in propertiesToInject)
			InjectService(target, property);
	}

	public abstract object Resolve(Type type);

	public T Resolve<T>()
		=> (T)Resolve(typeof(T));

	#endregion
}
