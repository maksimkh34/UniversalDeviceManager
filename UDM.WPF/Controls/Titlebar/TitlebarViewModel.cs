using System.Windows;
using System.Windows.Input;
using UDM.Core.ViewModels;
using UDM.Model;

namespace UDM.WPF.Controls.Titlebar
{
    internal class TitlebarViewModel : BaseViewModel
    {
        #region Properties

        private int _defaultHeight;
        private int _defaultWidth;

        #endregion

        #region Commands

        public ICommand CloseTitlebarCommand { get; }
        public ICommand ResizeTitlebarCommand { get; }
        public ICommand MinimizeTitlebarCommand { get; }

        private readonly Window _parentWindow;

        #endregion

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

        #endregion

        #region Constructor

        public TitlebarViewModel(Window parentWindow, int defaultWidth, int defaultHeight)
        {
            _parentWindow = parentWindow;

            _defaultWidth = defaultWidth;
            _defaultHeight = defaultHeight;

            CloseTitlebarCommand = new DelegateCommand(CloseWindow, CoreCommands.DefaultCanExecute);
            ResizeTitlebarCommand = new DelegateCommand(ResizeWindow, CoreCommands.DefaultCanExecute);
            MinimizeTitlebarCommand = new DelegateCommand(MinimizeWindow, CoreCommands.DefaultCanExecute);
        }

        #endregion
    }
}
 