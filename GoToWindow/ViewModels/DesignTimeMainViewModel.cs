﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GoToWindow.Extensibility;
using GoToWindow.Extensibility.ViewModel;

namespace GoToWindow.ViewModels
{
	public class DesignTimeMainViewModel : MainViewModel
	{
		public DesignTimeMainViewModel()
		{
			IsEmpty = false;
			AvailableWindowWidth = 640;
			AvailableWindowHeight = 240;
			SearchText = "User Query...";
			var icon = ConvertFromIcon(Properties.Resources.AppIcon);
			Windows = new CollectionViewSource
			{
				Source = new List<ISearchResult>
				{
					new DesignTimeSearchResult(icon, "process", "Window Title"),
					new DesignTimeSearchResult(icon, "very long process name", "Very very long window title that should end up with ellipsis because it is so very long"),
					new DesignTimeSearchResult(icon, "filler", "Some Window Title"),
					new DesignTimeSearchResult(icon, "filler", "Some Window Title"),
					new DesignTimeSearchResult(icon, "filler", "Some Window Title"),
					new DesignTimeSearchResult(icon, "filler", "Some Window Title"),
					new DesignTimeSearchResult(icon, "filler", "Some Window Title"),
					new DesignTimeSearchResult(icon, "filler", "Some Window Title"),
					new DesignTimeSearchResult(icon, "filler", "Some Window Title"),
					new DesignTimeSearchResult(icon, "filler", "Some Window Title")
				}
			};
		}

		private static BitmapFrame ConvertFromIcon(Icon icon)
		{
			var memoryStream = new MemoryStream();
			icon.Save(memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);
			return BitmapFrame.Create(memoryStream);
		}  
	}
}