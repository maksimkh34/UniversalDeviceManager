namespace UDM.Model
{
    /// <summary>
    /// Основные команды для всего приложения. 
    /// </summary>
    public static class CoreCommands
    {
        /// <summary>
        /// Стандартная Predicate для новых DelegateCommand
        /// </summary>
        public static bool DefaultCanExecute(object param) => true;
    }
}
