using Microsoft.Phone.Shell;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OnThisDayApp
{
    /// <summary>
    /// Class for creating and saving live tile
    /// </summary>
    public static class LiveTile
    {
        private const int TileSize = 173;
        private const string SharedImagePath = "/Shared/ShellContent/";

        public static void UpdateLiveTile(string title, string content)
        {
            // application tile is always the first tile, even if it is not pinned
            var tiles = ShellTile.ActiveTiles;
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    var data = GetTile(title, content);
                    tile.Update(data);
                }
            }
        }

        public static StandardTileData GetTile(string title, string content)
        {
            string fileName = WriteTileToDisk(title, content);
            var data = new StandardTileData()
            {
                Title = title,
                BackTitle = "On This Day...",
                BackgroundImage = new Uri("isostore:" + fileName),
                BackBackgroundImage = new Uri("/icons/Application_Icon_173.png", UriKind.Relative)
            };
            return data;
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

            WriteableBitmap writeableBitmap = new WriteableBitmap(container, null);

            string fileName = SharedImagePath + "tile.jpg";
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Create, storage))
                {
                    if (writeableBitmap.PixelHeight > 0)
                    {
                        writeableBitmap.SaveJpeg(stream, TileSize, TileSize, 0, 100);
                    }
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
