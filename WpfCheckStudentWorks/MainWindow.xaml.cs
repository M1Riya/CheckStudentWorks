using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfCheckStudentWorks
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel(new DialogWindow());
        }
        private void LoadClick(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            richTextBox1.Document.Blocks.Clear();
            richTextBox2.Document.Blocks.Clear();

            var obj = sender as System.Windows.Controls.RadioButton;
            var inf = obj.DataContext as ModelResultInformation;
            string extension1 = Path.GetExtension(inf.Path_File1);
            string extension2 = Path.GetExtension(inf.Path_File2);
           
            //search similar fragments
            List<string> fragments1 = MethodShingles.OutputMatchedText(inf.Text_File1, inf.Match);
            List<string> fragments2 = MethodShingles.OutputMatchedText(inf.Text_File2, inf.Match);

           
            try
            {
                // output to secon rich text box
                Thread thr1 = new Thread(_ =>
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {                                          
                    if (extension1 == ".docx")
                    {
                        ViewModel.LoadWordToRichTextBox(richTextBox1, inf.Path_File1);
                            ViewModel.HighlightRichText(fragments1, richTextBox1);
                    }
                    else if (extension1 == ".pdf")
                    {
                        SubWindow subWindow = new SubWindow(inf.Path_File1);
                        subWindow.Show();
                        ViewModel.HighlightTextPdf(inf.Path_File1, fragments1);
                    }
                    });
                });
                thr1.Start();
                // output to second rich text box
                Thread thr2 = new Thread(_ =>
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (extension2 == ".docx")
                        {
                            ViewModel.LoadWordToRichTextBox(richTextBox2, inf.Path_File2);
                            ViewModel.HighlightRichText(fragments2, richTextBox2);
                        }
                        else if (extension2 == ".pdf")
                        {
                            SubWindow subWindow = new SubWindow(inf.Path_File2);
                            subWindow.Show();
                            ViewModel.HighlightTextPdf(inf.Path_File2, fragments2);
                        }
                    });
                });
                thr2.Start();              

                if (thr1.IsAlive) thr1.Join();
                if (thr2.IsAlive) thr2.Join();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Debug.WriteLine($"Время выполнения: {ts.TotalMilliseconds}");

            Mouse.OverrideCursor = Cursors.Arrow;
        }      
    }
}
