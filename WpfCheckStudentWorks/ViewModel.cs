﻿using BitMiracle.Docotic.Pdf;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace WpfCheckStudentWorks
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        BaseCommand openCommand;
        BaseCommand openCommandFolder;
        BaseCommand runCommand;
        DialogWindow dialogW;
        List<TextInformation> allText = new List<TextInformation>();
        public ObservableCollection<CheckResultInformation> checkStudWorkAllInf { get; set; }
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public ViewModel(DialogWindow d)
        {
            dialogW = d;
            checkStudWorkAllInf = new ObservableCollection<CheckResultInformation>();

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

                            allText.Add(inf);
                        }

                    }
                }));
            }
        }

        public BaseCommand RunCommand
        {
            get
            {
                return runCommand ?? (runCommand = new BaseCommand(obj =>
                {
                    if (allText != null)
                    {                       
                        MethodShingles.ShinglCreate(allText);
                        checkStudWorkAllInf.Clear();
                        for (int i = 0; i < allText.Count - 1; i++)
                            for (int j = i + 1; j < allText.Count; j++)
                            {
                                double checkResult = MethodShingles.CheckSumCompair(i, j);
                                if (checkResult > 2)                    //разместить выбор порога для пользолвателя рядом с OPEN !!!!!
                                {
                                    CheckResultInformation res = new CheckResultInformation(allText[i].FileName, allText[i].Text, allText[j].FileName, allText[j].Text, checkResult);
                                    checkStudWorkAllInf.Add(res);
                                }
                            }
                    }
                   
                }));
            }
        }

        static string OpenWordprocessingDocumentReadonly(string filepath)        //работа с docx
        {
            // Open a WordprocessingDocument based on a filepath.
            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Open(filepath, false))
            {
                // Assign a reference to the existing document body.  
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
    }
}
