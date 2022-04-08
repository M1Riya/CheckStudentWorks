using Ookii.Dialogs.Wpf;


namespace WpfCheckStudentWorks
{
    public class DialogWindow
    {
        public string FolderPath { get; set; }
        public int FilterIndex { get; set; }
        public bool OpenDialog()
        {
            var f = new VistaFolderBrowserDialog();
            f.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            if (f.ShowDialog() == true)
            {
                FolderPath = f.SelectedPath;         
                return true;
            }
            

         //OpenFileDialog f = new OpenFileDialog();
            /* f.InitialDirectory = @"C:\Users";
             f.Filter = "Файл в docx|*.docx|Файл в doc|*.doc|Файл в pdf|*.pdf";
             if (f.ShowDialog() == true)
             {
                 FilePath = f.FileName;
                 FilterIndex = f.FilterIndex;
                 return true;
             }*/

            return false;
        }
    }
}
