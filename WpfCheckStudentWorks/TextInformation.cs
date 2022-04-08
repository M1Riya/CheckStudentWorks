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
        string text;

        public string FileName
        {
            get { return file_name; }
            set { file_name = value; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }
}
