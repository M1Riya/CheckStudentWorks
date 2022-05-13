using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfCheckStudentWorks
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel(new DialogWindow());
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            richTextBox1.Document.Blocks.Clear();
            richTextBox2.Document.Blocks.Clear();

            var obj = sender as System.Windows.Controls.RadioButton;
            var inf = obj.DataContext as CheckResultInformation;
            string extension1 = inf.File1_name.Substring(inf.File1_name.IndexOf('.'));
            string extension2 = inf.File2_name.Substring(inf.File2_name.IndexOf('.'));

            try
            {
                // output to first rich text box
                if (extension1 == ".docx")
                {                   
                    LoadWordToRichTextBox(richTextBox1, inf.Path_File1);
                }
                else if (extension1 == ".pdf")
                {
                    LoadPDFToRichTextBox(richTextBox1, inf.Path_File1);
                }

                // output to secon rich text box
                if (extension2 == ".docx")
                {                    
                    LoadWordToRichTextBox(richTextBox2, inf.Path_File2);
                }
                else if (extension2 == ".pdf")
                {
                    LoadPDFToRichTextBox(richTextBox2, inf.Path_File2);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Could not open the file.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            //search similar fragments
            List<string> fragments1 = MethodShingles.OutputMatchedText(inf.Text_File1,  inf.Match);
            List<string> fragments2 = MethodShingles.OutputMatchedText(inf.Text_File2, inf.Match);

           /*TextRange text = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
            text.ClearAllProperties();
            string textBoxText = text.Text;

            if (!string.IsNullOrWhiteSpace(textBoxText))
            {
                foreach (string fr in fragments1)
                {
                    TextPointer current = richTextBox1.Document.ContentStart;
                    while (current != null)
                    {
                        string parsedString = current.GetTextInRun(LogicalDirection.Forward).Replace("\n", "  ").Replace("\r", "  ");
                        if (!string.IsNullOrWhiteSpace(parsedString))
                        {
                            int index = parsedString.IndexOf(fr);
                            if (index >= 0)
                            {
                                TextPointer selectionStart = current.GetPositionAtOffset(index);
                                if (selectionStart != null)
                                {
                                    TextPointer selectionEnd = selectionStart.GetPositionAtOffset(fr.Length);
                                    TextRange selection = new TextRange(selectionStart, selectionEnd);
                                    selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));
                                }
                            }
                        }
                        current = current.GetNextContextPosition(LogicalDirection.Forward);
                    }
                }
            }*/

            //highlighting of matched fragments
            var tuple1 = new Tuple<List<string>, System.Windows.Controls.RichTextBox>(fragments1, richTextBox1);
            Thread thr1 = new Thread(HilightFragmentsRun);
            thr1.Start(tuple1);

            var tuple2 = new Tuple<List<string>, System.Windows.Controls.RichTextBox>(fragments2, richTextBox2);
            Thread thr2 = new Thread(HilightFragmentsRun);
            thr2.Start(tuple2);

            if (thr1.IsAlive) thr1.Join();

            if (thr2.IsAlive) thr2.Join();
            Mouse.OverrideCursor = Cursors.Arrow;

        }
         void HilightFragmentsRun(object inf)
        {
            var allinf = (Tuple<List<string>, System.Windows.Controls.RichTextBox>)inf;
            List<string> fragments = allinf.Item1;
            System.Windows.Controls.RichTextBox rt = allinf.Item2;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                HilightFragments(fragments, rt);
            });
        }
         static void HilightFragments(List<string> fragments, System.Windows.Controls.RichTextBox rt)
        {
            TextRange text = new TextRange(rt.Document.ContentStart, rt.Document.ContentEnd);
            text.ClearAllProperties();
            string textBoxText = text.Text;

            if (!string.IsNullOrWhiteSpace(textBoxText))
            {
                foreach (string fr in fragments)
                {
                    TextPointer current = rt.Document.ContentStart;
                    while (current != null)
                    {
                        string parsedString = current.GetTextInRun(LogicalDirection.Forward);
                        if (!string.IsNullOrWhiteSpace(parsedString))
                        {
                            int index = parsedString.IndexOf(fr);
                            if (index >= 0)
                            {
                                TextPointer selectionStart = current.GetPositionAtOffset(index);
                                if (selectionStart != null)
                                {
                                    TextPointer selectionEnd = selectionStart.GetPositionAtOffset(fr.Length);
                                    TextRange selection = new TextRange(selectionStart, selectionEnd);
                                    selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));
                                }
                            }
                        }
                        current = current.GetNextContextPosition(LogicalDirection.Forward);
                    }
                }
            }
        }
        static string ConvertPdf(string path)
        {
            byte[] pdf = File.ReadAllBytes(path);
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            f.OpenPdf(pdf);

            if (f.PageCount > 0)
            {
               // f.WordOptions.Format = SautinSoft.PdfFocus.CWordOptions.eWordDocument.Rtf;           
               //return f.ToWord();
               return f.ToText();
            }
            else
                return null;
        }
        static void LoadWordToRichTextBox(System.Windows.Controls.RichTextBox rt, string path)
        {
            object File = path;
            Microsoft.Office.Interop.Word.Application wordObject = new Microsoft.Office.Interop.Word.Application();
            object nullobject = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Application wordobject = new Microsoft.Office.Interop.Word.Application();
            wordobject.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;
            Microsoft.Office.Interop.Word._Document docs = wordObject.Documents.Open(ref File, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject, ref nullobject);
            docs.ActiveWindow.Selection.WholeStory();
            docs.ActiveWindow.Selection.Copy();
            rt.Paste();
            docs.Close(ref nullobject, ref nullobject, ref nullobject);
        }
        static void LoadPDFToRichTextBox(System.Windows.Controls.RichTextBox rt, string path)
        {
            TextRange doc = new TextRange(rt.Document.ContentStart, rt.Document.ContentEnd);
            /* byte[] word = ConvertPdf(path);

             if (word != null)
             {
                 string s = Encoding.UTF8.GetString(word);
                 using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(s)))
                 {
                     doc.Load(ms, DataFormats.Rtf);
                 }
             }*/
            string s = ConvertPdf(path);
            if (s != null)
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(s)))
                {
                    doc.Load(ms, DataFormats.Text);
                }
            }
        }
    }
}
