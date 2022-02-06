using System;
using System.Threading;
using System.Threading.Tasks;

namespace R4;

public interface ITimeService : ISingletonService
{
	TimeSpan ClientTimeOffset { get; set; }
	DateTime ToClientLocalDateTime(DateTime dt);
	string ToClientLocalString(DateTime dt, string format);
	string ToClientLocalString(DateTime? dt, string format);
	DateTime ClientLocalNow { get; }

	DateTimeOffset OffsetUtcNow { get; }
	DateTime UtcNow { get; }

	int GetAge(DateTime? birthday, DateTime? calcDay = null);

	DateTime? ParseDateTimeFromPlainInput(string input);
	Task<string> ToGeneralDateShortTimeWithHourHint(DateTime? dt);

	bool AppliesTo(DateTime? from, DateTime? until, DateTime? dt = null);
}

public class TimeService : ITimeService
{
	#region Client Local Time

	public TimeSpan ClientTimeOffset { get; set; }
		= TimeSpan.Zero;

	public DateTime ToClientLocalDateTime(DateTime dt)
		=> dt.ToUniversalTime().Add(ClientTimeOffset);

	public string ToClientLocalString(DateTime dt, string format)
		=> ToClientLocalDateTime(dt).ToString(format);

	public string ToClientLocalString(DateTime? dt, string format)
	{
		if (!dt.HasValue) return "";
		return ToClientLocalString(dt.Value, format);
	}

	public DateTime ClientLocalNow
		=> ToClientLocalDateTime(UtcNow);

	#endregion

	public virtual DateTimeOffset OffsetUtcNow
		=> DateTimeOffset.UtcNow;

	public virtual DateTime UtcNow
		=> DateTime.UtcNow;

	public DateTime? ParseDateTimeFromPlainInput(string input)
	{
		if (!input.HasContent())
			return default;

		if (input.Contains("Z"))
		{
			if (DateTime.TryParse(input, out DateTime systemFormatResult))
				return systemFormatResult;
		}

		var dtToken = input.SplitSafe(" ");

		if (!dtToken.HasContent())
			return null;

		var result = ParseDate(dtToken[0]);

		if (dtToken.Count > 1)
		{
			var time =
				ParseTime(dtToken[1]);

			result = result?.Add(time);
		}

		return result;
	}

	private readonly string[] _DateDelimiters =
		new[] { ",", "-", "/", "." };

	private (int year, int month, int day) GetDateIndexes()
	{
		var yearInput = 1940;
		var monthInput = 11;
		var dayInput = 24;

		var date = new DateTime(yearInput, monthInput, dayInput);
		var str = date.ToString("d");
		var token = str.SplitSafe(_DateDelimiters);

		var year = token.IndexOf(yearInput.ToString());
		var month = token.IndexOf(monthInput.ToString());
		var day = token.IndexOf(dayInput.ToString());

		return (year, month, day);
	}

	private TimeSpan ParseTime(string input)
	{
		var token = input.SplitSafe(new[] { ":", "-" });

		if (!token.HasContent())
			return new TimeSpan();

		var hours = token[0].ToInt32();
		var minutes = 0;
		var seconds = 0;

		if (token.Count > 1)
			minutes = token[1].ToInt32();

		if (token.Count > 2)
			seconds = token[2].ToInt32();

		return new TimeSpan(hours, minutes, seconds);
	}

	private DateTime? ParseDate(string input)
	{
		var now = UtcNow;

		var year = now.Year;
		var months = 1;
		var days = 1;

		var (yearIndex, monthIndex, dayIndex) = GetDateIndexes();
		var token = input.SplitSafe(_DateDelimiters);

		var yearIncluded = token.Count > 2;
		if (!yearIncluded && yearIndex == 0)
			token.Insert(0, now.Year.ToString("0000"));

		if (token.Count >= yearIndex + 1)
			year = token[yearIndex].ToInt32();

		if (token.Count >= monthIndex + 1)
			months = Math.Max(1, token[monthIndex].ToInt32());

		if (token.Count >= dayIndex + 1)
			days = Math.Max(1, token[dayIndex].ToInt32());

		if (year < 1000)
			year += 2000;

		var date = new DateTime(year, 1, 1);
		return date.AddMonths(months - 1).AddDays(days - 1);
	}

	public Task<string> ToGeneralDateShortTimeWithHourHint(DateTime? dt)
	{
		if (dt == null)
			return Task.FromResult(null as string);

		var result = ToClientLocalString(dt, "g");

		if (!Thread.CurrentThread.CurrentUICulture.Name.ContainsIgnoreCase("de"))
			return Task.FromResult(result);

		return Task.FromResult(result + " Uhr");
	}

	public int GetAge(DateTime? birthday, DateTime? calcDay = null)
	{
		if (birthday == null)
			return 0;

		if (calcDay == null) calcDay = UtcNow;

		// get the difference in years
		int result = calcDay.Value.Year - birthday.Value.Year;

		// subtract another year if we're before the
		// birth day in the current year
		if (
			calcDay.Value.Month < birthday.Value.Month ||
			(
			calcDay.Value.Month == birthday.Value.Month &&
			calcDay.Value.Day < birthday.Value.Day)
			)
		{
			result--;
		}

		return result;
	}

	public bool AppliesTo(DateTime? from, DateTime? until, DateTime? dt = null)
	{
		if (dt == null)
			dt = UtcNow;

		if (dt < from)
			return false;

		if (dt > until)
			return false;

		return true;
	}
}
