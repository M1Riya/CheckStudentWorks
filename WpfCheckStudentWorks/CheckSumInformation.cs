using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCheckStudentWorks
{
    public class CheckSumInformation        //сделаьть наследование от TextInformation?
    {
        string file_name;
        string text;
        List<string> hash_shingle;
        List<string> origin_shingle;

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

        public List<string> HashShingle
        {
            get { return hash_shingle; }
            set { hash_shingle = value; }
        }

        public List<string> OriginShingle
        {
            get { return origin_shingle; }
            set { origin_shingle = value; }
        }

        public CheckSumInformation()
        {
            this.hash_shingle = new List<string>();
            this.origin_shingle = new List<string>();
        }
    }
}
