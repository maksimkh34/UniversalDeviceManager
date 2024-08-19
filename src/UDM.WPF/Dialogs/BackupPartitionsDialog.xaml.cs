﻿using System;
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
using System.Windows.Shapes;
using UDM.Core.ViewModels;
using UDM.Model;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для BackupPartitionsDialog.xaml
    /// </summary>
    public partial class BackupPartitionsDialog : Window
    {
        BackupPartitionsViewModel _viewModel = new();
        public BackupPartitionsDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(_viewModel.BeforeSelectPartitions.Count == 0)
            {
                App.ShowMessage("Select ADB -> Update partitions");
                Close();
            }

            DataContext = _viewModel;
        }
    }
}