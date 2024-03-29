using System;
using System.Collections.Generic;

namespace Core.Framework.Windows.Implementations.Drag.Dockablz
{
    public class LayoutAccessor
    {
        private readonly Layout _layout;
        private readonly BranchAccessor _branchAccessor;
        private readonly TabablzControl _tabablzControl;

        public LayoutAccessor(Layout layout)
        {
            if (layout == null) throw new ArgumentNullException("layout");
            
            _layout = layout;

            var branch = Layout.Content as Branch;
            if (branch != null)
                _branchAccessor = new BranchAccessor(branch);
            else            
                _tabablzControl = Layout.Content as TabablzControl;            
        }

        public Layout Layout
        {
            get { return _layout; }
        }

        public IEnumerable<DragablzItem> FloatingItems
        {
            get { return _layout.FloatingDragablzItems(); }
        }

        public BranchAccessor BranchAccessor
        {
            get { return _branchAccessor; }
        }

        public TabablzControl TabablzControl
        {
            get { return _tabablzControl; }
        }

        /// <summary>
        /// Visits the content of the layout, according to its content type.  No more than one of the provided <see cref="Action"/>
        /// callbacks will be called.  
        /// </summary>        
        public LayoutAccessor Visit(
            Action<BranchAccessor> branchVisitor = null,
            Action<TabablzControl> tabablzControlVisitor = null,
            Action<object> contentVisitor = null)
        {
            if (_branchAccessor != null)
            {
                if (branchVisitor != null)
                    branchVisitor(_branchAccessor);

                return this;
            }

            if (_tabablzControl != null)
            {
                if (tabablzControlVisitor != null)
                    tabablzControlVisitor(_tabablzControl);

                return this;
            }

            if (_layout.Content != null && contentVisitor != null)
                contentVisitor(_layout.Content);

            return this;
        }
    }
}