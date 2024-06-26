﻿namespace Ecng.Common
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;

	#endregion

	public static class Enumerator
	{
		public static Type GetEnumBaseType<T>()
		{
			return typeof(T).GetEnumBaseType();
		}

		public static Type GetEnumBaseType(this Type enumType)
		{
			return Enum.GetUnderlyingType(enumType);
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string GetName<T>(T value)
		{
			return value.To<Enum>().GetName();
		}

		public static string GetName(this Enum value)
		{
			return Enum.GetName(value.GetType(), value);
		}

		public static IEnumerable<T> GetValues<T>()
		{
			return typeof(T).GetValues().Cast<T>();
		}

		public static IEnumerable<T> ExcludeObsolete<T>(this IEnumerable<T> values)
		{
			return values.Where(v => v.GetAttributeOfType<ObsoleteAttribute>() is null);
		}

		public static IEnumerable<object> GetValues(this Type enumType)
		{
			return Enum.GetValues(enumType).Cast<object>();
		}

		public static IEnumerable<string> GetNames<T>()
		{
			return typeof(T).GetNames();
		}

		public static IEnumerable<string> GetNames(this Type enumType)
		{
			return Enum.GetNames(enumType);
		}

		public static bool IsDefined<T>(this T enumValue)
		{
			return Enum.IsDefined(typeof(T), enumValue);
		}

		public static bool IsFlags(this Type enumType)
			=> enumType.GetAttribute<FlagsAttribute>() is not null;

		public static IEnumerable<object> SplitMask2(this object maskedValue)
		{
			if (maskedValue is null)
				throw new ArgumentNullException(nameof(maskedValue));

			return maskedValue.GetType().GetValues().Where(v => HasFlags(maskedValue, v));
		}

		public static IEnumerable<T> SplitMask<T>(this T maskedValue)
		{
			return GetValues<T>().Where(v => HasFlags(maskedValue, v));
		}

		public static T JoinMask<T>()
		{
			return GetValues<T>().JoinMask();
		}

		public static T JoinMask<T>(this IEnumerable<T> values)
		{
			if (values is null)
				throw new ArgumentNullException(nameof(values));

			return values.Aggregate(default(T), (current, t) => (current.To<long>() | t.To<long>()).To<T>());
		}

		public static T Remove<T>(T enumSource, T enumPart)
		{
			return enumSource.To<Enum>().Remove(enumPart);
		}

		public static T Remove<T>(this Enum enumSource, T enumPart)
		{
			if (enumSource.GetType() != typeof(T))
				throw new ArgumentException(nameof(enumPart));

			return (enumSource.To<long>() & ~enumPart.To<long>()).To<T>();
		}

		[Obsolete("Use HasFlags method.")]
		public static bool Contains<T>(T enumSource, T enumPart)
		{
			return HasFlags(enumSource, enumPart);
		}

		public static bool HasFlags<T>(T enumSource, T enumPart)
		{
			return enumSource.To<Enum>().HasFlag(enumPart.To<Enum>());
		}

		[Obsolete("Use Enum.HasFlag method.")]
		public static bool Contains(this Enum enumSource, Enum enumPart)
		{
			return enumSource.HasFlag(enumPart);
		}

		public static bool TryParse<T>(this string str, out T value, bool ignoreCase = true)
			where T : struct
		{
			return Enum.TryParse(str, ignoreCase, out value);
		}

		//
		// https://stackoverflow.com/a/9276348
		//

		/// <summary>
		/// Gets an attribute on an enum field value
		/// </summary>
		/// <typeparam name="TAttribute">The type of the attribute you want to retrieve</typeparam>
		/// <param name="enumVal">The enum value</param>
		/// <returns>The attribute of type <typeparam name="TAttribute" /> that exists on the enum value</returns>
		public static TAttribute GetAttributeOfType<TAttribute>(this object enumVal)
			where TAttribute : Attribute
		{
			if (enumVal is null)
				throw new ArgumentNullException(nameof(enumVal));

			var memInfo = enumVal.GetType().GetMember(enumVal.ToString());
			return memInfo[0].GetAttribute<TAttribute>(false);
		}

		public static bool IsEnumBrowsable(this object enumVal)
			=> enumVal.GetAttributeOfType<BrowsableAttribute>()?.Browsable ?? true;

		public static IEnumerable<T> ExcludeNonBrowsable<T>(this IEnumerable<T> values)
			=> values.Where(v => v.IsEnumBrowsable());
	}
}