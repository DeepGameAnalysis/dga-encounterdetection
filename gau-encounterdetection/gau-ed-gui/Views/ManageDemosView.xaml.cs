﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Views
{
    /// <summary>
    /// Interaction logic for ManageDemosView.xaml
    /// </summary>
    public partial class ManageDemosView : Page
    {
        public ManageDemosView()
        {
            InitializeComponent();
        }

        private void onChooseClick(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "CSGO Demofile (.json)|*.json"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filenames_box.Text = String.Join("", dlg.FileNames);

                List<DemoListEntry> items = new List<DemoListEntry>();
                //items.AddRange(demofile_list.ItemsSource);
                foreach (string dem in dlg.FileNames)
                {
                    items.Add(new DemoListEntry() { FileName = System.IO.Path.GetFileName(dem), FilePath = dem });
                }

                demofile_list.ItemsSource = items;
            }
        }

        
    }

    public class DemoListEntry
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

    }
}
