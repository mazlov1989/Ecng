namespace Ecng.ComponentModel
{
	using System;
	using System.ComponentModel;

	using Ecng.Common;

	public interface IRange
	{
		bool HasMinValue { get; }
		bool HasMaxValue { get; }

		object Min { get; set; }
		object Max { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class Range<T> : Equatable<Range<T>>, IConvertible, IRange
		where T : IComparable<T>
	{
		/// <summary>
		/// Initializes the <see cref="Range{T}"/> class.
		/// </summary>
		static Range()
		{
			MinValue = default;
			MaxValue = default;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Range{T}"/> class.
		/// </summary>
		public Range()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Range{T}"/> class.
		/// </summary>
		/// <param name="min">The min.</param>
		/// <param name="max">The max.</param>
		public Range(T min, T max)
		{
			Init(min, max);
		}

		/// <summary>
		/// Represents the smallest possible value of a T.
		/// </summary>
		public static readonly T MinValue;

		/// <summary>
		/// Represents the largest possible value of a T.
		/// </summary>
		public static readonly T MaxValue;

		#region HasMinValue

		/// <summary>
		/// Gets a value indicating whether this instance has min value.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has min value; otherwise, <c>false</c>.
		/// </value>
		[Browsable(false)]
		public bool HasMinValue => _min.HasValue;

		#endregion

		#region HasMaxValue

		/// <summary>
		/// Gets a value indicating whether this instance has max value.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has max value; otherwise, <c>false</c>.
		/// </value>
		[Browsable(false)]
		public bool HasMaxValue => _max.HasValue;

		#endregion

		#region Length

		private static IOperator<T> _operator;

		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <value>The length.</value>
		[Browsable(false)]
		public T Length
		{
			get
			{
				if (!HasMinValue || !HasMaxValue)
					return MaxValue;
				else
				{
					if (_operator is null)
						_operator = OperatorRegistry.GetOperator<T>();

					return _operator.Subtract(Max, Min);
				}
			}
		}

		#endregion

		#region Min

		//private bool _isMinInit;
		private readonly NullableEx<T> _min = new();

		/// <summary>
		/// Gets or sets the min value.
		/// </summary>
		/// <value>The min value.</value>
		//[Field("Min", Order = 0)]
		public T Min
		{
			get => _min.Value;
			set
			{
				if (_max.HasValue)
					ValidateBounds(value, Max);

				_min.Value = value;
				//_isMinInit = true;
			}
		}

		#endregion

		#region Max

		//private bool _isMaxInit;
		private readonly NullableEx<T> _max = new();

		/// <summary>
		/// Gets or sets the max value.
		/// </summary>
		/// <value>The max value.</value>
		//[Field("Max", Order = 1)]
		public T Max
		{
			get => _max.Value;
			set
			{
				if (_min.HasValue)
					ValidateBounds(Min, value);

				_max.Value = value;
				//_isMaxInit = true;
			}
		}

		#endregion

		#region Parse

		public static explicit operator Range<T>(string str)
		{
			return Parse(str);
		}

		/// <summary>
		/// Parses the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static Range<T> Parse(string value)
		{
			if (value.IsEmpty())
				throw new ArgumentNullException(nameof(value));

			if (value.Length < 3)
				throw new ArgumentOutOfRangeException(nameof(value));

			value = value.Substring(1, value.Length - 2);

			value = value.Remove("Min:");

			var part1 = value.Substring(0, value.IndexOf("Max:") - 1);
			var part2 = value.Substring(value.IndexOf("Max:") + 4);

			var range = new Range<T>();

			if (!part1.IsEmpty() && part1 != "null")
				range.Min = part1.To<T>();
			
			if (!part2.IsEmpty() && part2 != "null")
				range.Max = part2.To<T>();
			
			return range;
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override int GetHashCode()
		{
			return (HasMinValue ? Min.GetHashCode() : 0) ^ (HasMaxValue ? Max.GetHashCode() : 0);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return "{{Min:{0} Max:{1}}}".Put(HasMinValue ? Min.ToString() : "null", HasMaxValue ? Max.ToString() : "null");
		}

		#endregion

		#region Equatable<Range<T>> Members

		/// <summary>
		/// Called when [equals].
		/// </summary>
		/// <param name="other">The value.</param>
		/// <returns></returns>
		protected override bool OnEquals(Range<T> other)
		{
			return _min == other._min && _max == other._max;
		}

		#endregion

		object IRange.Min
		{
			get => HasMinValue ? Min : null;
			set => Min = (T)value;
		}

		object IRange.Max
		{
			get => HasMaxValue ? Max : null;
			set => Max = (T)value;
		}

		public override Range<T> Clone()
		{
			return new Range<T>(Min, Max);
		}

		public bool Contains(Range<T> range)
		{
			if (range is null)
				throw new ArgumentNullException(nameof(range));

			return Contains(range.Min) && Contains(range.Max);
		}

		public Range<T> Intersect(Range<T> range)
		{
			if (range is null)
				throw new ArgumentNullException(nameof(range));

			if (Contains(range))
				return range.Clone();
			else if (range.Contains(this))
				return Clone();
			else
			{
				var containsMin = Contains(range.Min);
				var containsMax = Contains(range.Max);

				if (containsMin)
					return new Range<T>(range.Min, Max);
				else if (containsMax)
					return new Range<T>(Min, range.Max);
				else
					return null;
			}
		}

		public Range<T> SubRange(T min, T max)
		{
			if (!Contains(min))
				throw new ArgumentException("Not in range.", nameof(min));

			if (!Contains(max))
				throw new ArgumentException("Not in range.", nameof(max));

			return new(min, max);
		}

		/// <summary>
		/// Determines whether [contains] [the specified value].
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(T value)
		{
			if (_min.HasValue && Min.CompareTo(value) > 0)
				return false;
			else if (_max.HasValue && Max.CompareTo(value) < 0)
				return false;
			else
				return true;
		}

		private void Init(T min, T max)
		{
			ValidateBounds(min, max);

			_min.Value = min;
			_max.Value = max;
		}

		private static void ValidateBounds(T min, T max)
		{
			if (min.CompareTo(max) > 0)
				throw new ArgumentOutOfRangeException(nameof(min), $"Min value {min} is more than max value {max}.");
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(string))
				return ToString();

			throw new InvalidCastException();
		}
	}
}