using System.Windows;

namespace UDM.WPF.Controls.Titlebar
{
    public partial class Titlebar
    {
        #region Constructor

        public Titlebar()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        private Window? _parentWindow;

        #endregion Properties

        #region Triggers

        private void Border_MouseDown_Trigger(object sender, RoutedEventArgs e)
        {
            try { _parentWindow?.DragMove(); } catch (InvalidOperationException) { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this);
            DataContext = new TitlebarViewModel(_parentWindow!, DefaultWidth, DefaultHeight);

            if (!CanMinimize)
            {
                MinimizeButton.Visibility = Visibility.Collapsed;
            }

            if (!CanRestoreSize)
            {
                RestoreSizeButton.Visibility = Visibility.Collapsed;
            }
        }

        #endregion Triggers

        #region DependencyProperties

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public int DefaultWidth
        {
            get => (int)GetValue(DefaultWidthProperty);
            set => SetValue(DefaultWidthProperty, value);
        }

        public int DefaultHeight
        {
            get => (int)GetValue(DefaultHeightProperty);
            set => SetValue(DefaultHeightProperty, value);
        }

        public bool CanMinimize
        {
            get => (bool)GetValue(CanMinimizeProperty);
            set => SetValue(CanMinimizeProperty, value);
        }

        public bool CanRestoreSize
        {
            get => (bool)GetValue(CanRestoreSizeProperty);
            set => SetValue(CanRestoreSizeProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(Titlebar), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty DefaultWidthProperty =
            DependencyProperty.Register(nameof(DefaultWidth), typeof(int), typeof(Titlebar), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty DefaultHeightProperty =
            DependencyProperty.Register(nameof(DefaultHeight), typeof(int), typeof(Titlebar), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.Register(nameof(CanMinimize), typeof(bool), typeof(Titlebar), new PropertyMetadata(true));

        public static readonly DependencyProperty CanRestoreSizeProperty =
            DependencyProperty.Register(nameof(CanRestoreSize), typeof(bool), typeof(Titlebar), new PropertyMetadata(true));

        #endregion DependencyProperties
    }
}