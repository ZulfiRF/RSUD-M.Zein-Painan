#region Using directives

using System.Windows.Input;

#endregion

namespace Core.Framework.Windows.Ribbon.UI {
    public static class WindowCommands {
        public static RoutedUICommand Minimize = 
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.MinimizeCommandTooltip, Core.Framework.Windows.Properties.Resources.MinimizeCommandName, typeof(RibbonWindow));

        public static RoutedUICommand Maximize = 
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.MaximizeCommandTooltip, Core.Framework.Windows.Properties.Resources.MaximizeCommandName, typeof(RibbonWindow));

        public static RoutedUICommand RestoreDown = 
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.RestoreDownCommandTooltip, Core.Framework.Windows.Properties.Resources.RestoreDownCommandName, typeof(RibbonWindow));
    }
}