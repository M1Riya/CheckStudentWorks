

namespace WpfCheckStudentWorks
{
    public class TextInformation
    {
        string file_path;
        string text;

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
