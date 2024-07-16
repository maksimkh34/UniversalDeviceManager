using System.IO;
using System.Windows;
using UDM.Core.ViewModels;
using UDM.Model;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для PreDIL.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class PreDIL
    {
        private PreDILViewModel? _dataContext;

        public PreDIL()
        {
            InitializeComponent();
        }

        private void PreDIL_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dataContext = new PreDILViewModel(Close);
            DataContext = _dataContext;
        }

        private void Menu_Save_Click(object sender, RoutedEventArgs e)
        {
            MainModel.CurrentScriptCode = CodeTextBox.Text;
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save DIL Script",
                DefaultExt = "dil",
                Filter = "DIL Scripts (*.dil)|*.dil|All files (*.*)|*.*",
                CheckFileExists = false,
                CheckPathExists = false
            };
            dialog.ShowDialog();
            try
            {
                File.WriteAllLines(dialog.FileName, new List<string> { MainModel.CurrentScriptCode });
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
