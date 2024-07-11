using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для PreDIL.xaml
    /// </summary>
    public partial class PreDIL : Window
    {
        private PreDILViewModel? _dataContext;

        public PreDIL()
        {
            InitializeComponent();
        }

        private void PreDIL_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dataContext = new PreDILViewModel();
        }

        private void Menu_Save_Click(object sender, RoutedEventArgs e)
        {
            MainModel.CurrentScriptCode = CodeTextBox.Text;
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save DIL Script",
                DefaultExt = "dil",
                Filter = "DIL Scripts (*.dil)|*.dil|All files (*.*)|*.*",
                CheckFileExists = true,
                CheckPathExists = true
            };
            dialog.ShowDialog();
            File.WriteAllLines(dialog.FileName, new List<string> {MainModel.CurrentScriptCode});
        }
    }
}
