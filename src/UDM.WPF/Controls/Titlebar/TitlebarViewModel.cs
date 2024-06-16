using System.Windows;
using System.Windows.Input;
using UDM.Core.ViewModels;
using UDM.Model.Commands;

namespace UDM.WPF.Controls.Titlebar
{
    internal class TitlebarViewModel : BaseViewModel
    {
        #region Properties

        private readonly int _defaultHeight;
        private readonly int _defaultWidth;

        #endregion Properties

        #region Commands

        public ICommand CloseTitlebarCommand { get; }
        public ICommand ResizeTitlebarCommand { get; }
        public ICommand MinimizeTitlebarCommand { get; }

        private readonly Window _parentWindow;

        #endregion Commands

        #region Command Functions

        public void CloseWindow(object param)
        {
            _parentWindow.Close();
        }

        public void ResizeWindow(object param)
        {
            _parentWindow.Width = _defaultWidth;
            _parentWindow.Height = _defaultHeight;
        }

        public void MinimizeWindow(object param)
        {
            _parentWindow.WindowState = WindowState.Minimized;
        }

        #endregion Command Functions

        #region Constructor

        public TitlebarViewModel(Window parentWindow, int defaultWidth, int defaultHeight)
        {
            _parentWindow = parentWindow;

            _defaultWidth = defaultWidth;
            _defaultHeight = defaultHeight;

            CloseTitlebarCommand = new DelegateCommand(CloseWindow, DelegateCommand.DefaultCanExecute);
            ResizeTitlebarCommand = new DelegateCommand(ResizeWindow, DelegateCommand.DefaultCanExecute);
            MinimizeTitlebarCommand = new DelegateCommand(MinimizeWindow, DelegateCommand.DefaultCanExecute);
        }

        #endregion Constructor
    }
}