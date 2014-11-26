using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;

namespace AppStudio.Controls.Html2Xaml
{
    /// <summary>
    /// Usage: 
    /// 1) In a XAML file, declare the above namespace, e.g.:
    ///    xmlns:h2xaml="using:Html2Xaml"
    ///     
    /// 2) In RichTextBlock controls, set or databind the Html property, e.g.:
    ///    <RichTextBlock h2xaml:Properties.Html="{Binding ...}"/>
    ///    or
    ///    <RichTextBlock>
    ///     <h2xaml:Properties.Html>
    ///         <![CDATA[
    ///             <p>This is a list:</p>
    ///             <ul>
    ///                 <li>Item 1</li>
    ///                 <li>Item 2</li>
    ///                 <li>Item 3</li>
    ///             </ul>
    ///         ]]>
    ///     </h2xaml:Properties.Html>
    /// </RichTextBlock>
    /// </summary>
    public class Properties : DependencyObject
    {
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(Properties), new PropertyMetadata(null, HtmlChanged));

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        private static async void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextBlock richText = d as RichTextBlock;
            string html = e.NewValue as string;

            if (richText != null && !string.IsNullOrEmpty(html))
            {
                try
                {
                    string xaml = await Html2Xaml.ConvertToXaml(html);
                    ChangeRichTextBlockContents(richText, xaml);
                }
                catch (Exception ex)
                {
                    AppLogs.WriteError("Html2Xaml.HtmlChanged", ex);
                    try
                    {
                        ChangeRichTextBlockContents(richText, GetErrorXaml(ex, html));
                    }
                    catch
                    {
                        AppLogs.WriteError("Html2Xaml.HtmlChanged", ex);
                    }
                }
            }
        }

        private static void ChangeRichTextBlockContents(RichTextBlock richText, string xamlContents)
        {
            richText.Blocks.Clear();
            System.Diagnostics.Debug.WriteLine(xamlContents);
            RichTextBlock newRichText = (RichTextBlock)XamlReader.Load(xamlContents);
            for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
            {
                Block b = newRichText.Blocks[i];
                newRichText.Blocks.RemoveAt(i);
                richText.Blocks.Insert(0, b);
            }
        }

        private static string GetErrorXaml(Exception ex, string html)
        {
            string localizedError = null;
            string localizedSourceHtml = null;
            try
            {
                localizedError = string.Format(GetStringForResource("Html2XamlError/Text"), ex.Message);
                localizedSourceHtml = GetStringForResource("Html2XamlSourceHtml/Text");
            }
            catch
            {
                // Do nothing.
            }

            if (string.IsNullOrEmpty(localizedError))
            {
                localizedError = string.Format("An exception occurred while converting HTML to XAML: {0}", ex.Message);
            }
            if (string.IsNullOrEmpty(localizedSourceHtml))
            {
                localizedSourceHtml = "Source HTML:";
            }

            return string.Format(
                    @"<RichTextBlock xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">"
                    + @"<Paragraph>{0}</Paragraph><Paragraph /><Paragraph>{1}</Paragraph><Paragraph>{2}</Paragraph></RichTextBlock>",
                    localizedError,
                    localizedSourceHtml,
                    EncodeXml(html));
        }

        private static string EncodeXml(string xml)
        {
            return xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        private static string GetStringForResource(string key)
        {
            try
            {
                ResourceLoader rl = ResourceLoader.GetForViewIndependentUse();
                return rl.GetString(key);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
