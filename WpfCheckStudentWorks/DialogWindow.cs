using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;

namespace WpfCheckStudentWorks
{
    public class DialogWindow
    {
        public string FolderPath { get; set; }
        public List<string> FilesPath { get; set; }
        public int FilterIndex { get; set; }
        public bool OpenFileDialog()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            fileDialog.Filter = "Файл в pdf|*.pdf|Файл в docx|*.docx|Все файлы (*.*)|*.*";
            FilesPath = new List<string>();
            if (fileDialog.ShowDialog() == true)
            {
                foreach (string filename in fileDialog.FileNames)
                    FilesPath.Add(Path.GetFullPath(filename));

                //FilterIndex = f.FilterIndex;
                return true;
            }
            return false;
        }
        public bool OpenFolderDialog()
        {
            var folderDialog = new VistaFolderBrowserDialog();
            folderDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
            if (folderDialog.ShowDialog() == true)
            {
                FolderPath = folderDialog.SelectedPath;         
                return true;
            }

            return false;
        }
    }
}
