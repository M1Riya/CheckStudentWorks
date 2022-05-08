using BitMiracle.Docotic.Pdf;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace WpfCheckStudentWorks
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        CheckResultInformation selectedResult; 
        BaseCommand openCommand;
        BaseCommand openCommandFolder;
        BaseCommand runCommand;
        DialogWindow dialogW;
        List<TextInformation> allText = new List<TextInformation>();
        string selectedPercent;
        public CheckResultInformation SelectedResult
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
        public ObservableCollection<CheckResultInformation> checkStudWorkAllInf { get; set; }
        public ViewModel(DialogWindow d)
        {
            dialogW = d;
            checkStudWorkAllInf = new ObservableCollection<CheckResultInformation>();

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
                            string extension = currentFile.Substring(currentFile.IndexOf('.'));

                            if (extension == ".docx")
                            { text = OpenWordprocessingDocumentReadonly(currentFile); }
                            else if (extension == ".pdf")
                            { text = OpenPdfMethod(currentFile); }

                            TextInformation inf = new TextInformation();
                            inf.Text = text;
                            inf.FileName = Path.GetFileName(currentFile);
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
                            string extension = currentFile.Substring(currentFile.IndexOf('.'));

                            if (extension == ".docx")
                            { text = OpenWordprocessingDocumentReadonly(currentFile); }
                            else if (extension == ".pdf")
                            { text = OpenPdfMethod(currentFile); }

                            TextInformation inf = new TextInformation();
                            inf.Text = text;
                            inf.FileName = Path.GetFileName(currentFile);
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
            using (var pdf = new PdfDocument(filepath))
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
                    if (allText != null)
                    {
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
                                    CheckResultInformation res = new CheckResultInformation(allText[i].FileName, allText[i].FilePath, allText[i].Text, 
                                                                    allText[j].FileName, allText[j].FilePath, allText[j].Text, checkResult);
                                    checkStudWorkAllInf.Add(res);
                                }
                            }
                    }

                }));
            }
        }


    }
}
