using System;
using R4;

namespace Tests.Static;

[DisableAutoDiscover]
public class TimeServiceFake : TimeService
{
	private DateTime _UtcNow = DateTime.UtcNow;

	public override DateTime UtcNow => _UtcNow;
	public override DateTimeOffset OffsetUtcNow => new DateTimeOffset(_UtcNow);

	public void SetUtcNow(DateTime utcNow)
		=> _UtcNow = utcNow;

	public void TickMilliSeconds(int milliSeconds)
		=> _UtcNow = _UtcNow.AddMilliseconds(milliSeconds);

	public void TickMinutes(int minutes)
		=> _UtcNow = _UtcNow.AddMinutes(minutes);
}
