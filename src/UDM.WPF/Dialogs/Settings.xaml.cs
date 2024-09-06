using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using UDM.Core.ViewModels;
using UDM.Model.MainModelSpace;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;

namespace UDM.WPF.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings
    {
        private readonly SettingsViewModel _dataContext = new();
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dataContext.ShowMsg = App.ShowMessage;
            _dataContext.InvalidPathMsg = FindResource("MsgInvalidLogPath").ToString();

            DataContext = _dataContext;

            foreach (var settingName in MainModel.SettingsStorage.GetSettingsNames())
            {
                var setting = MainModel.SettingsStorage.Get(settingName);
                var stack = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(10, 10, 10, 0)
                };

                if (setting.GetSettingType() == typeof(bool))
                {
                    var checkbox = new CheckBox()
                    {
                        Content = new TextBlock()
                        {
                            Style = (Style)FindResource("ThemedTextBlock"),
                            FontSize = 12,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };
                    checkbox.SetResourceReference(ContentProperty, setting.DisplayName);
                    var binding = new Binding
                    {
                        Source = _dataContext,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath(_dataContext.GetPropertyNameBySettingName(settingName))
                    };
                    BindingOperations.SetBinding(checkbox, ToggleButton.IsCheckedProperty, binding);
                    stack.Children.Add(checkbox);
                }

                if (setting.GetSettingType() == typeof(string) && setting.IsUsingPossibleValues())
                {
                    var comboBox = new ComboBox
                    {
                        Style = (Style)FindResource("ThemedComboBox"),
                        Width = 90,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var title = new TextBlock
                    {
                        Style = (Style)FindResource("ThemedTextBlock"),
                        Margin = new Thickness(0, 0, 15, 0),
                        FontSize = 12,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    title.SetResourceReference(TextBlock.TextProperty, setting.DisplayName);

                    foreach (var possibleValue in setting.GetPossibleValues() ?? [])
                    {
                        comboBox.Items.Add(new TextBlock()
                        {
                            Text = possibleValue + " ",
                            Style = (Style)FindResource("ThemedTextBlock"),
                            Margin = new Thickness(2),
                            TextWrapping = TextWrapping.Wrap,
                            MinWidth = 52
                        });
                    }

                    var binding = new Binding
                    {
                        Source = _dataContext,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath(_dataContext.GetPropertyNameBySettingName(settingName))
                    };
                    BindingOperations.SetBinding(comboBox, Selector.SelectedIndexProperty, binding);

                    stack.Children.Add(title);
                    stack.Children.Add(comboBox);
                }

                if (setting.GetSettingType() == typeof(string) && !setting.IsUsingPossibleValues())
                {
                    var title = new TextBlock
                    {
                        Style = (Style)FindResource("ThemedTextBlock"),
                        Margin = new Thickness(0, 0, 15, 0),
                        FontSize = 12,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    title.SetResourceReference(TextBlock.TextProperty, setting.DisplayName);
                    stack.Children.Add(title);

                    var textBox = new TextBox()
                    {
                        Style = (Style)FindResource("ThemedTextBox"),
                        FontSize = 12,
                        Width = 300,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var binding = new Binding
                    {
                        Source = _dataContext,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath(_dataContext.GetPropertyNameBySettingName(settingName))
                    };
                    BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
                    stack.Margin = new Thickness(10, 10, 10, 5);
                    stack.Children.Add(textBox);
                }

                MainStackPanel.Children.Add(stack);
            }
        }
    }
}
