using System.Windows;

namespace UDM.WPF.Controls.Titlebar
{
    public partial class Titlebar
    {
        Window? _parentWindow;
        public Titlebar()
        {
            InitializeComponent();
        }

        private void Border_MouseDown_Trigger(object sender, RoutedEventArgs e) => _parentWindow?.DragMove();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this);
            DataContext = new TitlebarViewModel(_parentWindow!);
        }

        #region DependencyProperties

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public int DefaultWidth
        {
            get => (int)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public int DefaultHeight
        {
            get => (int)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public bool CanMinimize
        {
            get => (bool)GetValue(CanMinimizeProperty);
            set => SetValue(CanMinimizeProperty, value);
        }



        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(Titlebar), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty DefaultWidthProperty =
            DependencyProperty.Register(nameof(DefaultWidth), typeof(int), typeof(Titlebar), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty DefaultHeightProperty =
            DependencyProperty.Register(nameof(DefaultHeight), typeof(int), typeof(Titlebar), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty CanMinimizeProperty =
                    DependencyProperty.Register(nameof(CanMinimize), typeof(bool), typeof(Titlebar), new PropertyMetadata(default(bool)));


        #endregion
    }
}
