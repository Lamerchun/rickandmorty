using System;
using System.Linq;
using System.Collections.Generic;

using Moq;
using R4;

namespace Tests.Static;

public interface IUnitTestContainer : ITestContainer
{
	Mock<T> GetOrCreateMock<T>()
		where T : class;

	void RegisterService<T>(T service);
}

public class UnitTestContainer : TestContainerBase, IUnitTestContainer
{
	#region Construct

	private readonly Dictionary<Type, Mock> _Mocks
		= new();

	private readonly Dictionary<Type, object> _Services
		= new();

	#endregion

	#region Tools

	private Mock CreateMock(Type type)
		=> typeof(Mock<>).ConstructGeneric(type) as Mock;

	#endregion

	#region IUnitTestContainer

	public void RegisterService<T>(T service)
	{
		var type = typeof(T);

		var serviceInterfaces =
			type
				.GetInterfaces()
				.Where(x => x.InheritsFrom<ISingletonService>());

		foreach (var serviceInterface in serviceInterfaces.Safe())
			_Services[serviceInterface] = service;

		_Services[type] = service;
	}

	public override object Resolve(Type type)
	{
		if (_Services.ContainsKey(type))
			return _Services[type];

		var existingMock =
			_Mocks.GetSafe(type);

		if (existingMock != null)
			return existingMock.Object;

		var mock = CreateMock(type);
		_Mocks.Add(type, mock);
		return mock.Object;
	}

	public Mock<T> GetOrCreateMock<T>()
		where T : class
	{
		if (!_Mocks.ContainsKey(typeof(T)))
		{
			_Mocks[typeof(T)] =
			   CreateMock(typeof(T)) as Mock<T>;
		}

		return _Mocks[typeof(T)] as Mock<T>;
	}

	#endregion
}

