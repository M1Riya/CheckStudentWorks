using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Spire.Pdf;
using Spire.Pdf.General.Find;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfCheckStudentWorks
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ModelResultInformation selectedResult;
        string selectedPercent;
        BaseCommand openCommand;
        BaseCommand openCommandFolder;
        BaseCommand runCommand;
        DialogWindow dialogW;
        List<TextInformation> allText = new List<TextInformation>();
        public ModelResultInformation SelectedResult
        {
            get => selectedResult;
            set
            {
                selectedResult = value;
                OnPropertyChanged("SelectedResult");
            }
        }
        public string SelectedPercent
        {
            get => selectedPercent;
            set
            {
                selectedPercent = value;
                OnPropertyChanged("SelectedPercent");
            }
        }
        public ObservableCollection<ModelResultInformation> checkStudWorkAllInf { get; set; }
        public ViewModel(DialogWindow d)
        {
            dialogW = d;
            checkStudWorkAllInf = new ObservableCollection<ModelResultInformation>();
        }
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public BaseCommand OpenCommandFolder
        {
            get
            {
                return openCommandFolder ?? (openCommandFolder = new BaseCommand(obj =>
                {
                    if (dialogW.OpenFolderDialog() == true)
                    {
                        allText.Clear();
                        var txtFiles = Directory.EnumerateFiles(dialogW.FolderPath);
                        foreach (string currentFile in txtFiles)
                        {
                            string text = "";
                            string extension = Path.GetExtension(currentFile);
                            TextInformation inf = new TextInformation();

                            if (extension == ".docx")
                            { text = OpenWordprocessingDocumentReadonly(currentFile); }
                            else if (extension == ".pdf")
                            { text = OpenPdfMethod(currentFile); }
                            else continue;

                            inf.Text = text;
                            inf.FilePath = Path.GetFullPath(currentFile);

                            allText.Add(inf);
                        }
                    }
                }));
            }
        }

        public BaseCommand OpenCommandFiles
        {
            get
            {
                return openCommand ?? (openCommand = new BaseCommand(obj =>
                {
                    if (dialogW.OpenFileDialog() == true)
                    {
                        allText.Clear();
                        foreach (string currentFile in dialogW.FilesPath)
                        {
                            string text = "";
                            string extension = Path.GetExtension(currentFile);
                            TextInformation inf = new TextInformation();

                            if (extension == ".docx")
                            { text = OpenWordprocessingDocumentReadonly(currentFile); }
                            else if (extension == ".pdf")
                            { text = OpenPdfMethod(currentFile); }
                            else continue;

                            inf.Text = text;
                            inf.FilePath = Path.GetFullPath(currentFile);

                            allText.Add(inf);
                        }
                    }
                }));
            }
        }
        static string OpenWordprocessingDocumentReadonly(string filepath)        //работа с docx
        {
            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Open(filepath, false))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;
                return body.InnerText.ToString();
            }
            return "-1";
        }
        static string OpenPdfMethod(string filepath)     //работа с pdf
        {
            using (var pdf = new BitMiracle.Docotic.Pdf.PdfDocument(filepath))
            {
                string documentText = pdf.GetText();
                return documentText;
            }
            return "-1";
        }
        public BaseCommand RunCommand
        {
            get
            {
                return runCommand ?? (runCommand = new BaseCommand(obj =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    if (allText != null)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        checkStudWorkAllInf.Clear();
                        MethodShingles.ShinglCreate(allText);
                        int _percent = 0;
                        if (SelectedPercent != null)
                        {
                            string percent = SelectedPercent.Split(' ')[1];
                            _percent = Convert.ToInt32(percent.Substring(0, percent.IndexOf('%')));
                        }

                        for (int i = 0; i < allText.Count - 1; i++)
                            for (int j = i + 1; j < allText.Count; j++)
                            {
                                double checkResult = MethodShingles.CheckSumCompair(i, j);

                                if (checkResult > _percent)
                                {
                                    ModelResultInformation res = new ModelResultInformation(allText[i].FilePath, allText[i].Text,
                                                                     allText[j].FilePath, allText[j].Text, checkResult);
                                    res.Match.AddRange(MethodShingles.SearchMatch(i, j).ToArray());
                                    checkStudWorkAllInf.Add(res);
                                }
                            }
                        Mouse.OverrideCursor = Cursors.Arrow;
                    }
                    sw.Stop();
                    TimeSpan ts = sw.Elapsed;
                    Debug.WriteLine($"Время выполнения: {ts.TotalMilliseconds}");

                }));
            }
        }

        public static void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var obj = sender as SubWindow;

            string path = obj.NewPath;

            if (path != null)
                File.Delete(path);
        }
        public static void HighlightTextPdf(string path, List<string> fragments)
        {
            Spire.Pdf.PdfDocument pdf = new Spire.Pdf.PdfDocument(path);
          
            foreach (PdfPageBase page in pdf.Pages)
            {
                foreach (string fr in fragments)
                {
                    PdfTextFindCollection result = page.FindText(fr);

                    foreach (PdfTextFind find in result.Finds)
                    {

                        find.ApplyRecoverString(fr, System.Drawing.Color.Yellow, true);
                    }
                }
            }
            pdf.SaveToFile(Path.GetDirectoryName(path) + "\\1" + Path.GetFileName(path));
        }
        public static void HighlightRichText(List<string> fragments, System.Windows.Controls.RichTextBox rt)
        {
            TextRange text = new TextRange(rt.Document.ContentStart, rt.Document.ContentEnd);
            text.ClearAllProperties();
            string textBoxText = text.Text;

            if (!string.IsNullOrWhiteSpace(textBoxText))
            {
                foreach (string fr in fragments)
                {
                    TextPointer current = rt.Document.ContentStart;
                    while (true)
                    {
                        var searchRange = new TextRange(current, rt.Document.ContentEnd);
                        TextRange foundRange = searchRange.FindText(fr);
                        if (foundRange == null)
                            break;
                        foundRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));

                        current = foundRange.End;
                    }
                    rt.Focus();
                }
            }
        }
        public static void LoadWordToRichTextBox(System.Windows.Controls.RichTextBox rt, string path)
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
    }
}

