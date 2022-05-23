using System.IO;
using System.Windows;

namespace WpfCheckStudentWorks
{
    /// <summary>
    /// Interaction logic for SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        public string NewPath { get; set; }
        public SubWindow(string path)
        {
            InitializeComponent();
            DataContext = new ViewModel(new DialogWindow());            
            Closing += ViewModel.OnWindowClosing;

            NewPath = Path.GetDirectoryName(path) + "\\1" + Path.GetFileName(path);
            Browser.Address = "file:///" + NewPath;          
        }
    }
}
