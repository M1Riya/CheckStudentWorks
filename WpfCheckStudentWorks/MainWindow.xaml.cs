﻿using System.Windows;


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

    }
}
