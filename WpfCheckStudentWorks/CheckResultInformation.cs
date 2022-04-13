using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WpfCheckStudentWorks
{
    public class CheckResultInformation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string file1;
        string text1;
        string file2;
        string text2;
        double result;
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
        public double Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }
        public CheckResultInformation(string file1, string text1, string file2, string text2, double res)
        {
            File1_name = file1;
            Text_File1 = text1;
            File2_name = file2;
            Text_File2 = text2;
            Result = res;
        }
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
