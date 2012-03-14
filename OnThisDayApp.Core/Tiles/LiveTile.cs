using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;

namespace OnThisDayApp
{
    /// <summary>
    /// Class for creating and saving live tile
    /// </summary>
    public static class LiveTile
    {
        private const int TileSize = 173;
        private const string SharedImagePath = "/Shared/ShellContent/";

        private static IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

        public static void UpdateLiveTile(string title, string content)
        {
            // application tile is always the first tile, even if it is not pinned
            ShellTile tile = ShellTile.ActiveTiles.First();

            if (tile != null)
            {
                string fileName = WriteTileToDisk(title, content);
                StandardTileData data = new StandardTileData()
                {
                    Title = title,
                    BackTitle = "On This Day...",
                    BackgroundImage = new Uri("isostore:" + fileName),
                    BackBackgroundImage = new Uri("/icons/Application_Icon_173.png", UriKind.Relative)
                };
                tile.Update(data);
            }
        }

        #region Helper Methods

        private static string WriteTileToDisk(string year, string description)
        {
            Grid container = new Grid()
            //Grid container = new Grid()
            {
                Width = TileSize,
                Height = TileSize,
                Background = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"],
                //Background = (Brush)Application.Current.Resources["TransparentBrush"]
            };

            container.Children.Add(GetTextBlockToRender(description));

            // Force the container to render itself
            container.UpdateLayout();
            container.Arrange(new Rect(0, 0, TileSize, TileSize));

            WriteableBitmap wb = new WriteableBitmap(container, null);

            string fileName = SharedImagePath + "tile.jpg";
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Create, storage))
            {
                if (wb.PixelHeight > 0)
                {
                    wb.SaveJpeg(stream, TileSize, TileSize, 0, 100);
                }
            }

            return fileName;
        }

        private static TextBlock GetTextBlockToRender(string description)
        {
            // The font size, line height, and margin have all been chosen to match the shell's rendering of text on tiles
            return new TextBlock
                                {
                                    Text = description,
                                    TextWrapping = TextWrapping.Wrap,
                                    Foreground = new SolidColorBrush(Colors.White),
                                    FontSize = Double.Parse(Application.Current.Resources["PhoneFontSizeSmall"].ToString()),
                                    TextTrimming = TextTrimming.WordEllipsis,
                                    Margin = new Thickness(12, 6, 6, 32), // TODO: 12, 10, 8, 34
                                };
        }

        #endregion
    }
}
