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
        string file2;
        double result;
        public string File1
        {
            get => file1;
            set
            {
                file1 = value;
                OnPropertyChanged("File1");
            }
        }
        public string File2
        {
            get => file2;
            set
            {
                file2 = value;
                OnPropertyChanged("File2");
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
        public CheckResultInformation(string file1, string file2, double res)
        {
            File1 = file1;
            File2 = file2;
            Result = res;

        }
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
