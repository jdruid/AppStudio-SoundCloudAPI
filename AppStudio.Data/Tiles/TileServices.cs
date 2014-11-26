using System;
using System.Linq;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace AppStudio.Services
{
    public class TileServices
    {
        static public void CreateFlipTile(string title, string content, string squareImage, string wideImage)
        {
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.EnableNotificationQueue(true);
            tileUpdater.Clear();

            var squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText02);
            SetTileImages(squareTileXml, squareImage);
            SetTileTexts(squareTileXml, title, content);
            tileUpdater.Update(new TileNotification(squareTileXml));

            var wideTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150PeekImage01);
            SetTileImages(wideTileXml, wideImage);
            SetTileTexts(wideTileXml, title, content);
            tileUpdater.Update(new TileNotification(wideTileXml));
        }

        static public void CreateCycleTile(params string[] images)
        {
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.EnableNotificationQueue(true);
            tileUpdater.Clear();

            foreach (var image in images)
            {
                var squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);
                SetTileImages(squareTileXml, image);
                tileUpdater.Update(new TileNotification(squareTileXml));

                var wideTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Image);
                SetTileImages(wideTileXml, image);
                tileUpdater.Update(new TileNotification(wideTileXml));
            }
        }

        static public void CreateIconicTile(string content1, string content2, string content3)
        {
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.EnableNotificationQueue(true);
            tileUpdater.Clear();

            var squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text03);
            SetTileTexts(squareTileXml, content1, content2, content3);
            tileUpdater.Update(new TileNotification(squareTileXml));

            var wideTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text05);
            SetTileTexts(wideTileXml, content1, content2, content3);
            tileUpdater.Update(new TileNotification(wideTileXml));
        }

        static private void SetTileImages(XmlDocument xmlDocument, params string[] images)
        {
            if (images != null)
            {
                try
                {
                    var imageElements = xmlDocument.GetElementsByTagName("image").ToArray();
                    for (int n = 0; n < images.Length; n++)
                    {
                        var imageElement = imageElements[n] as XmlElement;
                        if (images[n].StartsWith("ms-appdata:", StringComparison.OrdinalIgnoreCase))
                        {
                            imageElement.SetAttribute("src", images[n]);
                        }
                        else
                        {
                            imageElement.SetAttribute("src", String.Format("ms-appx:///Assets/{0}", images[n]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppLogs.WriteError("TileServices.SetTileImages", ex);
                }
            }
        }

        static private void SetTileTexts(XmlDocument xmlDocument, params string[] texts)
        {
            if (texts != null)
            {
                try
                {
                    var textElements = xmlDocument.GetElementsByTagName("text").ToArray();
                    for (int n = 0; n < texts.Length; n++)
                    {
                        var textElement = textElements[n] as XmlElement;
                        textElement.InnerText = texts[n];
                    }
                }
                catch (Exception ex)
                {
                    AppLogs.WriteError("TileServices.SetTileTexts", ex);
                }
            }
        }
    }
}
