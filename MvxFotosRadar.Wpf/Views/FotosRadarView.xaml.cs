using Microsoft.Win32;
using MvvmCross.Platforms.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MvxFotosRadar.Core.ViewModels;

namespace MvxFotosRadar.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para FotosRadarView.xaml
    /// </summary>
    public partial class FotosRadarView : MvxWpfView
    {
        public FotosRadarView()
        {
            InitializeComponent();
        }

        private void btnOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Carpeta";
            folderBrowser.ShowDialog();
            string selectedFilePath = folderBrowser.FileName;
            string folderPath = Path.GetDirectoryName(selectedFilePath);
            this.tbRutaSeleccionada.Text = folderPath;
            

        }
    }
}
