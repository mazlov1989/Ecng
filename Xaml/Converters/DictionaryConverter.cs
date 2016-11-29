﻿namespace Ecng.Xaml.Converters
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	using Ecng.Common;
	using Ecng.Reflection;

	public class DictionaryConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var dictionary = (IDictionary)value;

			var type = dictionary?.GetType().GetGenericType(typeof(IDictionary<,>));

			if (type == null)
				return DependencyProperty.UnsetValue;

			var key = parameter.To(type.GetGenericArguments()[0]);

			if (!dictionary.Contains(key))
				return DependencyProperty.UnsetValue;

			return dictionary[key];
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}