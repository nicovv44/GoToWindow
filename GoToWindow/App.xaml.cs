﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using GoToWindow.Commands;
using Hardcodet.Wpf.TaskbarNotification;
using log4net;

namespace GoToWindow
{
	public partial class App : Application
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(App).Assembly, "GoToWindow");

		private IGoToWindowContext _context;
		private Mutex _mutex;
		private TaskbarIcon _trayIcon;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			bool mutexCreated;
			_mutex = new Mutex(true, "GoToWindow", out mutexCreated);
			if (!mutexCreated)
			{
				Log.Warn("Application already running. Shutting down.");
				Current.Shutdown(1);
				return;
			}

			_context = new GoToWindowContext();
			
			_trayIcon = new TaskbarIcon
			{
				Icon = GoToWindow.Properties.Resources.AppIcon,
				ToolTipText = "Go To Window",
				DoubleClickCommand = new OpenMainWindowCommand(_context),
				ContextMenu = CreateContextMenu()
			};

			_context.EnableAltTabHook(
				GoToWindow.Properties.Settings.Default.HookAltTab,
				GoToWindow.Properties.Settings.Default.ShortcutPressesBeforeOpen
				);

			_context.Init();

			Log.Info("Application started.");

			ShowStartupTooltip();
		}

		private void ShowStartupTooltip()
		{
			var shortcutPressesBeforeOpen = GoToWindow.Properties.Settings.Default.ShortcutPressesBeforeOpen;
			var openShortcutDescription = shortcutPressesBeforeOpen == 1
				? "Alt + Tab"
				: "Alt + Tab + Tab";

			_trayIcon.ShowBalloonTip(
				"Go To Window",
				string.Format("Press {0} and start typing to search through your opened windows.", openShortcutDescription),
				BalloonIcon.Info);
		}

		private ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			var showMenu = new MenuItem { Header = "_Show", Command = new OpenMainWindowCommand(_context) };
			contextMenu.Items.Add(showMenu);

			var settingsMenu = new MenuItem { Header = "S_ettings", Command = new ShowSettingsCommand(_context) };
			contextMenu.Items.Add(settingsMenu);

			contextMenu.Items.Add(new Separator());

			var exitMenuItem = new MenuItem { Header = "E_xit", Command = new ExitCommand() };
			contextMenu.Items.Add(exitMenuItem);

			return contextMenu;
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (_context != null)
			{
				_context.Dispose();
			}

			if (_mutex != null)
			{
				_mutex.Dispose();
			}

			if (_trayIcon != null)
			{
				_trayIcon.Dispose();
			}

			Log.Info("Application exited.");
		}

		private void Application_Activated(object sender, EventArgs e)
		{
			Log.Debug("Application activated.");
		}

		private void Application_Deactivated(object sender, EventArgs e)
		{
			Log.Debug("Application deactivated.");

			_context.Hide();
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			Log.Fatal(e.Exception);
		}
	}
}
