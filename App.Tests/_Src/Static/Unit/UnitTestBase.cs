using NUnit.Framework;
using Tests.Static;
using R4;

namespace Tests.Unit;

public abstract class UnitTestBase : TestBase
{
	#region Construct

	protected IUnitTestContainer Container { get; private set; }

	[SetUp]
	public virtual void SetUp()
	{
		Container = new UnitTestContainer();
		Container.Inject(this);
	}

	#endregion
}

public abstract class UnitTestBase<TService> : UnitTestBase
	where TService : class, new()
{
	public ITimeService ITimeService { get; set; }
	protected TService Service { get; private set; }

	public override void SetUp()
	{
		base.SetUp();
		WillCreateService();
		Service = Container.Create<TService>();
	}

	public virtual void WillCreateService() { }
}
