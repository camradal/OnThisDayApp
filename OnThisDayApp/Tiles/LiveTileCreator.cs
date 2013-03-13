using System;
using System.Windows;
using Microsoft.Phone.Shell;
using OnThisDayApp;

public class LiveTileCreator : LiveTile
{
    public static void CreateLiveTile(string title, string content)
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
            ShellTile.Create(new Uri("/MainPage.xaml?DefaultTitle=Highlights", UriKind.Relative), data);
        }
        else
        {
            string fontSize = Application.Current.Resources["PhoneFontSizeLarge"].ToString();
            string fileNameMed = WriteTileToDisk(title, content, 336, 336, fontSize, new Thickness(19, 13, 13, 38));
            string fileNameBig = WriteTileToDisk(title, content, 691, 336, fontSize, new Thickness(19, 13, 13, 38));

            UpdateFlipTile(
                title,
                "On This Day...",
                string.Empty,
                string.Empty,
                0,
                new Uri("/icons/Application_Icon_336.png", UriKind.Relative),
                new Uri("isostore:" + fileNameMed),
                new Uri("/icons/Application_Icon_336.png", UriKind.Relative),
                new Uri("isostore:" + fileNameBig),
                new Uri("/icons/Application_Icon_691.png", UriKind.Relative));

            CreateFlipTile(
                title,
                "On This Day...",
                string.Empty,
                string.Empty,
                0,
                new Uri("/icons/Application_Icon_336.png", UriKind.Relative),
                new Uri("isostore:" + fileNameMed),
                new Uri("/icons/Application_Icon_336.png", UriKind.Relative),
                new Uri("isostore:" + fileNameBig),
                new Uri("/icons/Application_Icon_691.png", UriKind.Relative));
        }
    }

    private static void CreateFlipTile(
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

        var UpdateTileData = flipTileDataType.GetConstructor(new Type[] { }).Invoke(null);
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

        var types = new[] { typeof(Uri), typeof(ShellTileData), typeof(bool) };
        var parameters = new[] { new Uri("/MainPage.xaml?DefaultTitle=Highlights", UriKind.Relative), UpdateTileData, true };
        shellTileType.GetMethod("Create", types).Invoke(null, parameters);
    }
}