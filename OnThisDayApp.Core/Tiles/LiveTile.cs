﻿using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using Utilities;

namespace OnThisDayApp
{
    /// <summary>
    /// Class for creating and saving live tile
    /// </summary>
    public class LiveTile
    {
        private const string SharedImagePath = "/Shared/ShellContent/";

        public static void UpdateLiveTile(string title, string content)
        {
            //application tile is always the first tile, even if it is not pinned
            if (!IsTargetedVersion)
            {
                var data = GetTile(title, content);
                var tiles = ShellTile.ActiveTiles;
                foreach (var tile in tiles)
                {
                    if (tile != null)
                    {
                        tile.Update(data);
                    }
                }
            }
            else
            {
                string fontSize = Application.Current.Resources["PhoneFontSizeLarge"].ToString();
                string fileNameMed = WriteTileToDisk(title, content, 336, 336, fontSize, new Thickness(19, 13, 13, 38));
                string fileNameBig = WriteTileToDisk(title, content, 691, 336, fontSize, new Thickness(19, 13, 13, 38));

                GC.Collect();
                GC.WaitForPendingFinalizers();

                var backBackgroundImage = new Uri("", UriKind.Relative);
                var wideBackBackgroundImage = new Uri("", UriKind.Relative);
                const string backTitle = "";

                UpdateFlipTile(
                    title,
                    backTitle,
                    string.Empty,
                    string.Empty,
                    0,
                    new Uri("/icons/Application_Icon_336.png", UriKind.Relative),
                    new Uri("isostore:" + fileNameMed),
                    backBackgroundImage,
                    new Uri("isostore:" + fileNameBig),
                    wideBackBackgroundImage);
            }
        }

        protected static StandardTileData GetTile(string title, string content)
        {
            string fontSize = Application.Current.Resources["PhoneFontSizeSmall"].ToString();
            string fileName = WriteTileToDisk(title, content, 173, 173, fontSize, new Thickness(12, 6, 6, 32));
            Uri backBackgroundImage = new Uri("", UriKind.Relative);
            const string backTitle = "";
            var data = new StandardTileData()
            {
                Title = title,
                BackTitle = backTitle,
                BackgroundImage = new Uri("isostore:" + fileName),
                BackBackgroundImage = backBackgroundImage
            };
            return data;
        }

        #region Windows Phone 8 Tile

        private static readonly Version TargetedVersion = new Version(7, 10, 8858);
        private static readonly Version WP8Version = new Version(8, 0);

        public static bool IsTargetedVersion
        {
            get { return Environment.OSVersion.Version >= TargetedVersion; }
        }

        public static bool IsWP8Version
        {
            get { return Environment.OSVersion.Version >= WP8Version; }
        }

        protected static void UpdateFlipTile(
            string title,
            string backTitle,
            string backContent,
            string wideBackContent,
            int count,
            Uri smallBackgroundImage,
            Uri backgroundImage,
            Uri backBackgroundImage,
            Uri wideBackgroundImage,
            Uri wideBackBackgroundImage)
        {
            if (!IsTargetedVersion)
                return;

            Type flipTileDataType = Type.GetType("Microsoft.Phone.Shell.FlipTileData, Microsoft.Phone");
            if (flipTileDataType == null)
                return;

            Type shellTileType = Type.GetType("Microsoft.Phone.Shell.ShellTile, Microsoft.Phone");
            if (shellTileType == null)
                return;

            foreach (var tileToUpdate in ShellTile.ActiveTiles)
            {
                var UpdateTileData = flipTileDataType.GetConstructor(new Type[] {}).Invoke(null);
                SetProperty(UpdateTileData, "Title", title);
                SetProperty(UpdateTileData, "Count", count);
                SetProperty(UpdateTileData, "BackTitle", backTitle);
                SetProperty(UpdateTileData, "BackContent", backContent);
                SetProperty(UpdateTileData, "SmallBackgroundImage", smallBackgroundImage);
                SetProperty(UpdateTileData, "BackgroundImage", backgroundImage);
                SetProperty(UpdateTileData, "BackBackgroundImage", backBackgroundImage);
                SetProperty(UpdateTileData, "WideBackgroundImage", wideBackgroundImage);
                SetProperty(UpdateTileData, "WideBackBackgroundImage", wideBackBackgroundImage);
                SetProperty(UpdateTileData, "WideBackContent", wideBackContent);

                shellTileType.GetMethod("Update").Invoke(tileToUpdate, new Object[] {UpdateTileData});
            }
        }

        protected static void SetProperty(object instance, string name, object value)
        {
            var setMethod = instance.GetType().GetProperty(name).GetSetMethod();
            setMethod.Invoke(instance, new object[] {value});
        }

        #endregion

        #region Helper Methods

        protected static string WriteTileToDisk(string year, string description, int width, int height, string fontSize, Thickness margins)
        {
            var background = IsWP8Version
                ? (Brush) Application.Current.Resources["TransparentBrush"]
                : (SolidColorBrush) Application.Current.Resources["PhoneAccentBrush"];

            Grid container = new Grid()
            {
                Width = width,
                Height = height,
                Background = background
            };

            container.Children.Add(GetTextBlockToRender(description, fontSize, margins));

            // Force the container to render itself
            container.UpdateLayout();
            container.Arrange(new Rect(0, 0, width, height));

            var writeableBitmap = new WriteableBitmap(container, null);

            string ext = IsWP8Version ? ".png" : ".jpg";
            string fileName = SharedImagePath + "tile" + height + width + ext;
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = new IsolatedStorageFileStream(fileName, FileMode.Create, storage))
                {
                    if (writeableBitmap.PixelHeight > 0)
                    {
                        if (IsWP8Version)
                        {
                            writeableBitmap.WritePNG(stream);
                        }
                        else
                        {
                            writeableBitmap.SaveJpeg(stream, width, height, 0, 100);                            
                        }
                    }
                }
            }

            return fileName;
        }

        private static TextBlock GetTextBlockToRender(string description, string fontSize, Thickness margins)
        {
            // The font size, line height, and margin have all been chosen to match the shell's rendering of text on tiles
            return new TextBlock
                                {
                                    Text = description,
                                    TextWrapping = TextWrapping.Wrap,
                                    Foreground = new SolidColorBrush(Colors.White),
                                    FontSize = Double.Parse(fontSize),
                                    TextTrimming = TextTrimming.WordEllipsis,
                                    Margin = margins
                                };
        }

        #endregion
    }
}
