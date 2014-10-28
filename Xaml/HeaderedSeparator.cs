﻿namespace Ecng.Xaml
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Разделитель с заголовком.
	/// </summary>
	public class HeaderedSeparator : Control
	{
		/// <summary>
		/// <see cref="DependencyProperty"/> <see cref="Header"/>.
		/// </summary>
		public static DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(string), typeof(HeaderedSeparator));

		/// <summary>
		/// Заголовок.
		/// </summary>
		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
	}
}