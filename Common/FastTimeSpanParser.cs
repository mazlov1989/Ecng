namespace Ecng.Common
{
	using System;

	public class FastTimeSpanParser
	{
		private readonly string _template;

		private readonly int _dayStart;
		//private readonly int _dayLen;

		private readonly int _hourStart;
		//private readonly int _hourLen;

		private readonly int _minuteStart;
		//private readonly int _minuteLen;

		private readonly int _secondStart;
		//private readonly int _secondLen;

		private readonly int _milliStart;
		//private readonly int _milliLen;

		public FastTimeSpanParser(string template)
		{
			if (template.IsEmpty())
				throw new ArgumentNullException(nameof(template));

			_template = template;

			template = template.Replace("\\", string.Empty);

			_dayStart = template.IndexOf('d');
			_hourStart = template.IndexOf('h');
			_minuteStart = template.IndexOf('m');
			_secondStart = template.IndexOf('s');
			_milliStart = template.IndexOf('f');

			//TimeHelper.InitBounds(template, 'd', out _dayStart, out _dayLen);
			//TimeHelper.InitBounds(template, 'h', out _hourStart, out _hourLen);
			//TimeHelper.InitBounds(template, 'm', out _minuteStart, out _minuteLen);
			//TimeHelper.InitBounds(template, 's', out _secondStart, out _secondLen);
			//TimeHelper.InitBounds(template, 'f', out _milliStart, out _milliLen);
		}

		public TimeSpan Parse(string input)
		{
			try
			{
				var days = _dayStart == -1 ? 0 : (input[_dayStart] - '0') * 10 + (input[_dayStart + 1] - '0');

				var hours = _hourStart == -1 ? 0 : (input[_hourStart] - '0') * 10 + (input[_hourStart + 1] - '0');
				var minutes = _minuteStart == -1 ? 0 : (input[_minuteStart] - '0') * 10 + (input[_minuteStart + 1] - '0');
				var seconds = _secondStart == -1 ? 0 : (input[_secondStart] - '0') * 10 + (input[_secondStart + 1] - '0');

				var millis = _milliStart == -1 ? 0 : (input[_milliStart] - '0') * 100 + (input[_milliStart + 1] - '0') * 10 + (input[_milliStart + 2] - '0');

				return new TimeSpan(days, hours, minutes, seconds, millis);
			}
			catch (Exception ex)
			{
				throw new InvalidCastException("Cannot convert {0} with format {1} to {2}.".Put(input, _template, typeof(TimeSpan).Name), ex);
			}
		}
	}
}