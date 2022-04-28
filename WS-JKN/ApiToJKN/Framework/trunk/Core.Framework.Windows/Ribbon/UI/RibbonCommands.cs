#region Using directives

using System.Windows.Input;

#endregion

namespace Core.Framework.Windows.Ribbon.UI {
    public static class RibbonCommands {
        public static RoutedUICommand OpenAppMenu =
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.OpenAppMenuCommandTooltip, Core.Framework.Windows.Properties.Resources.OpenAppMenuCommandName, typeof(Ribbon));

        public static RoutedUICommand CloseAppMenu =
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.CloseAppMenuCommandTooltip, Core.Framework.Windows.Properties.Resources.CloseAppMenuCommandName, typeof(Ribbon));

        public static RoutedUICommand BlendInRibbon =
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.BlendInRibbonCommandTooltip, Core.Framework.Windows.Properties.Resources.BlendInRibbonCommandName, typeof(RibbonWindow));

        public static RoutedUICommand OpenRibbonOptions =
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.OpenRibbonOptionsCommandTooltip, Core.Framework.Windows.Properties.Resources.OpenRibbonOptionsCommandName, typeof(RibbonWindow));

        public static RoutedUICommand AddQuickAccess =
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.AddQuickAccessCommandTooltip, Core.Framework.Windows.Properties.Resources.AddQuickAccessCommandName, typeof(RibbonWindow));

        public static RoutedUICommand RemoveQuickAccess =
            new RoutedUICommand(Core.Framework.Windows.Properties.Resources.RemoveQuickAccessCommandTooltip, Core.Framework.Windows.Properties.Resources.RemoveQuickAccessCommandName, typeof(RibbonWindow));
    }
}