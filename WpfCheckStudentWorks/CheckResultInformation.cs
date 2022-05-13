using System.Collections.Generic;
using System.ComponentModel;

namespace WpfCheckStudentWorks
{
    public class CheckResultInformation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string file1;
        string text1;
        string path1;
        string file2;
        string text2;
        string path2;
        double result;
        List<string> match;
        public string File1_name
        {
            get => file1;
            set
            {
                file1 = value;
                OnPropertyChanged("File1_name");
            }
        }
        public string Text_File1
        {
            get => text1;
            set
            {
                text1 = value;
                OnPropertyChanged("Text_File1");
            }
        }
        public string Path_File1
        {
            get => path1;
            set
            {
                path1 = value;
                OnPropertyChanged("Path_File1");
            }
        }
        public string File2_name
        {
            get => file2;
            set
            {
                file2 = value;
                OnPropertyChanged("File2_name");
            }
        }
        public string Text_File2
        {
            get => text2;
            set
            {
                text2 = value;
                OnPropertyChanged("Text_File2");
            }
        }
        public string Path_File2
        {
            get => path2;
            set
            {
                path2 = value;
                OnPropertyChanged("Path_File2");
            }
        }
        public double Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }
        public List<string> Match
        {
            get => match;
            set
            {
                match = value;
                OnPropertyChanged("Match");
            }
        }
        public CheckResultInformation(string file1, string path1, string text1, string file2, string path2, string text2, double res)
        {
            File1_name = file1;
            Text_File1 = text1;
            Path_File1 = path1;
            File2_name = file2;
            Text_File2 = text2;
            Path_File2 = path2;
            Result = res;
            this.match = new List<string>();
        }
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
