/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1823:AvoidUnusedPrivateFields",
  Scope = "member",
  Target = "_XceedVersionInfoCommon.Build" )]

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1823:AvoidUnusedPrivateFields",
  Scope = "member",
  Target = "_XceedVersionInfo.CurrentAssemblyPackUri" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1020:AvoidNamespacesWithFewTypes", 
  Scope = "namespace", 
  Target = "XamlGeneratedNamespace" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2209:AssembliesShouldDeclareMinimumSecurity" )]

[assembly: SuppressMessage(
  "Microsoft.Design",
  "CA1020:AvoidNamespacesWithFewTypes",
  Scope = "namespace",
  Target = "Core.Framework.Windows.Datagrid.ValidationRules" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2233:OperationsShouldNotOverflow", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.DataGridCollectionView.System.Collections.ICollection.CopyTo(System.Array,System.Int32):System.Void", 
  MessageId = "index+1" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1020:AvoidNamespacesWithFewTypes", 
  Scope = "namespace", 
  Target = "Xceed.Utils.Wpf.Markup" )]

[assembly: SuppressMessage( 
  "Microsoft.Security", 
  "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.Print.DataGridPaginator.InitializeSettings(System.Printing.PrintQueue,System.Printing.PrintTicket):System.Void",
  Justification = "A permission demand for FullTrust has been added to InitializeSettings()." )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.DataGridControl.System.Windows.Documents.IDocumentPaginatorSource.DocumentPaginator" )]

[assembly: SuppressMessage(
  "Microsoft.Design",
  "CA1043:UseIntegralOrStringArgumentForIndexers",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.CellCollection.Item[Core.Framework.Windows.Datagrid.Column]" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA1801:ReviewUnusedParameters",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DataGridCollectionView..ctor(System.Type)", MessageId = "itemType" )]

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1805:DoNotInitializeUnnecessarily",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DetailConfiguration..ctor(Core.Framework.Windows.Datagrid.DataGridContext)" )]

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1800:DoNotCastUnnecessarily",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.GroupByControl.Xceed.Utils.Wpf.DragDrop.IDropTarget.CanDropElement(System.Windows.UIElement):System.Boolean" )]

[assembly: SuppressMessage(
  "Microsoft.Performance",
  "CA1800:DoNotCastUnnecessarily",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.GroupByItem.Xceed.Utils.Wpf.DragDrop.IDropTarget.CanDropElement(System.Windows.UIElement):System.Boolean" )]

[assembly: SuppressMessage(
  "Microsoft.Design",
  "CA1011:ConsiderPassingBaseTypesAsParameters",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.Views.Theme.IsViewSupported(System.Type,System.Type):System.Boolean" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.GroupLevelIndicatorPane.System.Windows.IWeakEventListener.ReceiveWeakEvent(System.Type,System.Object,System.EventArgs):System.Boolean" )]

[assembly: SuppressMessage( 
  "Microsoft.Design", 
  "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.HierarchicalGroupLevelIndicatorPane.System.Windows.IWeakEventListener.ReceiveWeakEvent(System.Type,System.Object,System.EventArgs):System.Boolean" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA1801:ReviewUnusedParameters", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.DetailConfiguration.AddDefaultHeadersFooters(System.Collections.ObjectModel.ObservableCollection`1<System.Windows.DataTemplate>,System.Collections.ObjectModel.ObservableCollection`1<System.Windows.DataTemplate>):System.Void", MessageId = "footersCollection" )]

#region CA2214:DoNotCallOverridableMethodsInConstructors

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.Cell..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.CellEditor..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.Column..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.Column..ctor(System.String,System.Object,System.Windows.Data.BindingBase)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.ColumnManagerCell..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.ColumnManagerRow..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DataCell..ctor(System.String,System.Object)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DataGridControl..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DropMarkAdorner..ctor(System.Windows.UIElement,System.Windows.Media.Pen,Core.Framework.Windows.Datagrid.DropMarkOrientation)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.Views.SynchronizedScrollViewer..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.Views.ViewBase..ctor()" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.GroupHeaderControl..ctor(Core.Framework.Windows.Datagrid.Group)" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.Views.FixedCellPanel..ctor()" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.Views.ScrollingCellsDecorator..ctor(Core.Framework.Windows.Datagrid.Views.FixedCellPanel)" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.Print.DataGridPageControl..ctor(Core.Framework.Windows.Datagrid.DataGridControl)" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.Views.DataGridScrollViewer..ctor()" )]

[assembly: SuppressMessage( 
  "Microsoft.Usage", 
  "CA2214:DoNotCallOverridableMethodsInConstructors", 
  Scope = "member", 
  Target = "Core.Framework.Windows.Datagrid.GroupConfiguration..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DataGridContext..ctor(Core.Framework.Windows.Datagrid.DataGridContext,Core.Framework.Windows.Datagrid.DataGridControl,System.Object,System.Windows.Data.CollectionView,Core.Framework.Windows.Datagrid.DetailConfiguration)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DetailConfiguration..ctor(System.Boolean)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DetailConfiguration..ctor(Core.Framework.Windows.Datagrid.DataGridContext)" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.SaveRestoreStateVisitor..ctor()" )]

[assembly: SuppressMessage(
  "Microsoft.Usage",
  "CA2214:DoNotCallOverridableMethodsInConstructors",
  Scope = "member",
  Target = "Core.Framework.Windows.Datagrid.DefaultDetailConfiguration..ctor()" )]

#endregion CA2214:DoNotCallOverridableMethodsInConstructors
