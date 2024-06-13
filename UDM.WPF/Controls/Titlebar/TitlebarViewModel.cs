using System.Windows;
using System.Windows.Input;
using UDM.Core.ViewModels;
using UDM.Model;

namespace UDM.WPF.Controls.Titlebar
{
    internal class TitlebarViewModel : BaseViewModel
    {
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

        }

        public void MinimizeWindow(object param)
        {

        }



        #endregion

        #region Constructor

        public TitlebarViewModel(Window parentWindow)
        {
            _parentWindow = parentWindow;

            CloseTitlebarCommand = new DelegateCommand(CloseWindow, CoreCommands.DefaultCanExecute);
            ResizeTitlebarCommand = new DelegateCommand(ResizeWindow, CoreCommands.DefaultCanExecute);
            MinimizeTitlebarCommand = new DelegateCommand(MinimizeWindow, CoreCommands.DefaultCanExecute);
        }

        #endregion
    }
}
