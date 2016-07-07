﻿namespace Ecng.Xaml.DevExp.Database
{
	using System.Windows;

	using Ecng.Data;
	using Ecng.Localization;

	public partial class DatabaseConnectionCreateWindow
	{
		public DatabaseConnectionCreateWindow()
		{
			InitializeComponent();

			Title = Title.Translate();
			CheckCtrl.Content = ((string)CheckCtrl.Content).Translate();
			Ok.Content = ((string)Ok.Content).Translate();

			Connection = new DatabaseConnectionPair
			{
				Provider = new SqlServerDatabaseProvider()
			};
		}

		public DatabaseConnectionPair Connection
		{
			get { return SettingsGrid.Connection; }
			set { SettingsGrid.Connection = value; }
		}

		private void TestCtrl_Click(object sender, RoutedEventArgs e)
		{
			Ok.IsEnabled = Connection.Test(this);
		}
	}
}