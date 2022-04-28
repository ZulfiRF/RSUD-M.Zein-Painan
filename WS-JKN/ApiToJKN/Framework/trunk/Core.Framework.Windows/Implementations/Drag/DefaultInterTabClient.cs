using System;
using System.Linq;
using System.Windows;
using Core.Framework.Windows.Extensions;
using Core.Framework.Windows.Implementations.Drag.Core;

namespace Core.Framework.Windows.Implementations.Drag
{
    public class DefaultInterTabClient : IInterTabClient
    {        
        public virtual INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            if (source == null) throw new ArgumentNullException("source");
            var sourceWindow = Window.GetWindow(source);
            if (sourceWindow == null) throw new ApplicationException("Unable to ascrtain source window.");
            var newWindow = (Window)Activator.CreateInstance(sourceWindow.GetType());

            var newTabablzControl = newWindow.LogicalTreeDepthFirstTraversal().OfType<TabablzControl>().FirstOrDefault();
            if (newTabablzControl == null) throw new ApplicationException("Unable to ascrtain tab control.");

            newTabablzControl.Items.Clear();

            return new NewTabHost<Window>(newWindow, newTabablzControl);            
        }

        public virtual TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}