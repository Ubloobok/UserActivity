using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UserActivity.Viewer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			GlobalDispatcher.UnhandledException += OnDispatcherUnhandledException;
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			GlobalDispatcher.BeginInvoke(new Action<Exception>(error =>
			{
				MessageBox.Show(error.Message, "Ошибка");
			}), e.Exception);
		}

		/// <summary>
		/// Gets current threads dispatcher for application.
		/// </summary>
		public static Dispatcher GlobalDispatcher
		{
			get { return App.Current.Dispatcher; }
		}
	}
}
