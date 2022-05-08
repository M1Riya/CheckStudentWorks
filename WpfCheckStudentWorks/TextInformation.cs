using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCheckStudentWorks
{
    public class TextInformation
    {
        string file_name;
        string file_path;
        string text;

        public string FileName
        {
            get => file_name; 
            set => file_name = value; 
        }
        public string FilePath
        {
            get =>  file_path; 
            set => file_path = value; 
        }
        public string Text
        {
            get => text; 
            set => text = value; 
        }
    }
}
