using System.Collections.Generic;

namespace WpfCheckStudentWorks
{
    public class CheckSumInformation        //сделаьть наследование от TextInformation?
    {
        
        string text;
        List<string> hash_shingle;
        List<string> origin_shingle;

        public string Text
        {
            get => text; 
            set => text = value; 
        }

        public List<string> HashShingle
        {
            get => hash_shingle; 
            set => hash_shingle = value; 
        }

        public List<string> OriginShingle
        {
            get => origin_shingle; 
            set => origin_shingle = value; 
        }

        public CheckSumInformation()
        {
            this.hash_shingle = new List<string>();
            this.origin_shingle = new List<string>();
        }
    }
}
