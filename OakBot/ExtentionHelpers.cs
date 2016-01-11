using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Diagnostics;

namespace OakBotExtentions
{
    public static class ExtentionHelpers
    {

        #region RichTextBox Extention: Colored RichTextBox.AppendText
        // Append a text to the RichTexBox object and color this appended text.
        // Accepts either a Brush or Color object where Brush doesn't have to be converted.
        //
        // Named colors: Brushes.ColorName
        // Color value: Color.FromArgb(a, r, g, b)
        public static void AppendText(this RichTextBox box, string text, Brush brush)
        {
            try
            {
                TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
                tr.Text = text;

                tr.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            }

            catch (Exception ex)
            {
                Trace.WriteLine("Ex AppendText: " + ex.ToString());
            }
        }

        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            try
            { 
                TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
                tr.Text = text;

                Brush brush = new SolidColorBrush(color);
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            }

            catch (Exception ex)
            {
                Trace.WriteLine("Ex AppendText: " + ex.ToString());
            }
        }
        #endregion

    }
}