using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Core.Framework.Helper;
using Core.Framework.Helper.Extention;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Helper;
using Core.Framework.Windows.Implementations.PropertyGrid.Implementation.Definitions;

namespace Core.Framework.Windows.Controls
{
    public class ColumnInfo
    {
        public double Width { get; set; }
    }
    public class CoreFlexGrid : ScrollViewer
    {
        #region Property Dependency


        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(CoreFlexGrid), new PropertyMetadata(1, RowCountCallback));

        private static void RowCountCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var form = dependencyObject as CoreFlexGrid;
            if (form != null)
            {
                form.MeasureOverride(form.LastSize);
                form.RowCount = (int)dependencyPropertyChangedEventArgs.NewValue;

                if (form.AutoClear)
                {
                    form.CurrentContainer.Children.Clear();
                    form.CreateCursor();
                    form.Elements = new CoreDictionary<string, object>();

                    return;
                }
                var temp = new CoreDictionary<string, object>();
                //                form.Elements = temp == null ? new CoreDictionary<string, object>() : new CoreDictionary<string, object>(temp);
                if (form.Elements != null)
                    foreach (var element in form.Elements)
                    {
                        var arr = element.Key.Split(new[] { '+' })[0].ToInt16();
                        if (arr < form.RowCount)
                            temp.Add(element.Key, element.Value);
                        else
                        {
                            var obj = element.Value;
                            if (obj != null)
                            {
                                var grid = (obj as UIElement).GetParentObject() as Grid;
                                if (grid != null)
                                    grid.Children.Remove(obj as UIElement);
                            }
                        }
                    }
                form.Elements = form.Elements == null ? new CoreDictionary<string, object>() : new CoreDictionary<string, object>(temp);
            }
        }



        public bool AutoClear { get; set; }


        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(CoreFlexGrid), new PropertyMetadata(1, ColumnCountPropertyChangedCallback));

        private static void ColumnCountPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var form = dependencyObject as CoreFlexGrid;
            if (form != null)
            {
                if (form.Columns == null)
                    form.Columns = new ColumnInfo[(int)e.NewValue];
                if (form.Columns.Length != (int)e.NewValue)
                {
                    var columns = form.Columns;
                    form.Columns = new ColumnInfo[(int)e.NewValue];
                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (i < form.Columns.Length)
                            form.Columns[i] = columns[i];
                    }
                }
                form.ColumnCount = (int)e.NewValue;
                form.MeasureOverride(form.LastSize);
                if (form.AutoClear)
                {

                    form.CurrentContainer.Children.Clear();
                    form.Elements = new CoreDictionary<string, object>();
                    form.CreateCursor();
                    return;
                }
                var temp = new CoreDictionary<string, object>();
                if (form.Elements != null)
                    foreach (var element in form.Elements)
                    {
                        var arr = element.Key.Split(new[] { '+' })[1].ToInt16();
                        if (arr < form.ColumnCount)
                            temp.Add(element.Key, element.Value);
                        else
                        {
                            var obj = element.Value;
                            if (obj != null)
                            {
                                var grid = (obj as UIElement).GetParentObject() as Grid;
                                if (grid != null)
                                    grid.Children.Remove(obj as UIElement);
                            }
                        }
                    }
                form.Elements = form.Elements == null ? new CoreDictionary<string, object>() : new CoreDictionary<string, object>(temp);
                //for (int i = 0; i < form.RowCount; i++)
                //{
                //    for (int j = 0; j < (int)e.NewValue; j++)
                //    {
                //        try
                //        {
                //            form.Elements[i + "+" + j] = temp[i + "+" + j];
                //        }
                //        catch (Exception exception)
                //        {
                //            Log.Error(exception);
                //        }

                //    }
                //}
            }
        }


        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(CoreFlexGrid), new PropertyMetadata(27.0));



        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(double), typeof(CoreFlexGrid), new PropertyMetadata(100.0));



        public Brush FixBrush
        {
            get { return (Brush)GetValue(FixBrushProperty); }
            set { SetValue(FixBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FixBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FixBrushProperty =
            DependencyProperty.Register("FixBrush", typeof(Brush), typeof(CoreFlexGrid), new PropertyMetadata());



        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(CoreFlexGrid), new PropertyMetadata());



        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(CoreFlexGrid), new PropertyMetadata(1.0));

        private int currentColumn;
        private int currentRow;

        #endregion

        #region Override

        List<FrameworkElement> listLoad;
        #region Overrides of ScrollViewer

        /// <summary>
        /// Measures the content of a <see cref="T:System.Windows.Controls.ScrollViewer"/> element.
        /// </summary>
        /// <returns>
        /// The computed desired limit <see cref="T:System.Windows.Size"/> of the <see cref="T:System.Windows.Controls.ScrollViewer"/> element.
        /// </returns>
        /// <param name="constraint">The upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        protected override Size MeasureOverride(Size constraint)
        {
            LastSize = constraint;
            if (CurrentContainer != null)
            {
                CurrentContainer.RowDefinitions.Clear();
                listContainer = new int[RowCount, ColumnCount];
                CurrentContainer.ColumnDefinitions.Clear();
                foreach (var frameworkElement in listLoad)
                {
                    CurrentContainer.Children.Remove(frameworkElement);
                }
                Rectangle line;
                line = new Rectangle();
                line.Fill = BorderBrush;
                listLoad.Add(line);
                line.Height = Thickness;
                line.VerticalAlignment = VerticalAlignment.Top;
                line.HorizontalAlignment = HorizontalAlignment.Stretch;
                Grid.SetColumnSpan(line, ColumnCount);
                Grid.SetRow(line, 0);
                CurrentContainer.Children.Add(line);
                line = new Rectangle();
                line.Width = Thickness;
                line.Fill = BorderBrush;
                listLoad.Add(line);
                line.VerticalAlignment = VerticalAlignment.Stretch;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetRowSpan(line, RowCount);
                Grid.SetColumn(line, 0);
                CurrentContainer.Children.Add(line);
                CurrentContainer.ColumnDefinitions.Clear();
                for (int i = 0; i < RowCount; i++)
                {
                    var rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(RowHeight, GridUnitType.Pixel);
                    CurrentContainer.RowDefinitions.Add(rowDefinition);

                }
                CurrentContainer.Width = 0;
                for (int i = 0; i < ColumnCount; i++)
                {
                    var rowDefinition = new ColumnDefinition();

                    CurrentContainer.ColumnDefinitions.Add(rowDefinition);

                    if (Columns != null && Columns.Length >= i)
                        if (Columns[i] != null)
                        {

                            rowDefinition.Width = new GridLength(Columns[i].Width, GridUnitType.Pixel);
                            CurrentContainer.Width += ColumnWidth;
                        }
                        else
                        {
                            rowDefinition.Width = new GridLength(ColumnWidth, GridUnitType.Pixel);
                            CurrentContainer.Width += ColumnWidth;
                        }
                    listLoad.Add(line);
                }
                for (int i = 0; i < RowCount; i++)
                {
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        line = new Rectangle();
                        line.Fill = BorderBrush;
                        line.Height = Thickness;
                        line.VerticalAlignment = VerticalAlignment.Bottom;
                        line.HorizontalAlignment = HorizontalAlignment.Stretch;
                        //Grid.SetColumnSpan(line, ColumnCount);
                        Grid.SetRow(line, i);
                        Grid.SetColumn(line, j);
                        var first =
                        MergeCell.FirstOrDefault(n => n.X1.ToInt16() <= i && n.X2.ToInt16() > i && n.Y1 <= j && n.Y2 >= j);// && n.Y1 >= j && n.Y2 <= j
                        //MergeCell.FirstOrDefault(n => n.Y1.ToInt16() <= j && n.Y2.ToInt16() > j);// && n.Y1 >= j && n.Y2 <= j
                        if (first == null)
                        {
                            CurrentContainer.Children.Add(line);
                            listLoad.Add(line);
                        }
                        line = new Rectangle();
                        line.Width = Thickness;
                        line.Fill = BorderBrush;
                        line.VerticalAlignment = VerticalAlignment.Stretch;
                        line.HorizontalAlignment = HorizontalAlignment.Right;
                        Grid.SetRow(line, i);
                        Grid.SetColumn(line, j);
                        first =
                        MergeCell.FirstOrDefault(n => n.X1.ToInt16() <= i && n.X2.ToInt16() > i - 1 && n.Y1 <= j && n.Y2 > j);// && n.Y1 >= j && n.Y2 <= j
                        if (first == null)
                        {
                            CurrentContainer.Children.Add(line);
                            listLoad.Add(line);
                        }
                        var item = new Canvas();
                        Grid.SetZIndex(item, -100);
                        if (i <= RowFix - 1 || j <= ColumnFix - 1)
                            item.Background = FixBrush;
                        else
                            item.Background = new SolidColorBrush(Colors.Transparent);

                        item.Margin = new Thickness(0);
                        item.VerticalAlignment = VerticalAlignment.Stretch;
                        item.HorizontalAlignment = HorizontalAlignment.Stretch;
                        Grid.SetColumn(item, j);
                        Grid.SetRow(item, i);
                        item.Tag = new Point(j, i);
                        item.MouseLeftButtonUp += (sender, e) =>
                        {
                            var canvas = sender as Canvas;
                            if (canvas == null) return;
                            if ((Point)(sender as Canvas).Tag == null) return;
                            currentColumn = ((Point)(sender as Canvas).Tag).X.ToInt16();
                            currentRow = ((Point)(sender as Canvas).Tag).Y.ToInt16();
                            if (currentRow <= RowFix - 1)
                                currentRow = RowFix;
                            if (currentColumn <= ColumnFix - 1)
                                currentColumn = ColumnFix;
                            RebindPosition();
                            if (currentObjEdit != null)
                            {
                                var current = this[currentRowEdit, currentColumnEdit];
                                if (current != null)
                                    if (!currentObjEdit.Equals(current))
                                        this[currentRowEdit, currentColumnEdit] = currentObjEdit;
                            }
                            else
                                if (this[currentRowEdit, currentColumnEdit] == null)
                                this[currentRowEdit, currentColumnEdit] = currentObjEdit;
                            IsEdit = false;

                        };
                        listLoad.Add(item);
                        CurrentContainer.Children.Add(item);
                        if (Elements != null)
                        {
                            if (i < RowCount - 1 && j < ColumnCount - 1)
                            {
                                try
                                {
                                    var value = Elements[i + "+" + j];
                                    if (Elements[i + "+" + j] != null)
                                    {
                                        if (value is UIElement)
                                        {
                                            var item2 = (UIElement)value;
                                            if (item2.GetParentObject() == null)
                                            {

                                                CurrentContainer.Children.Add(item2);
                                                Grid.SetColumn(item2, j);
                                                Grid.SetRow(item2, i);
                                                first = MergeCell.FirstOrDefault(n => n.X1.ToInt16() <= currentRow && n.X2.ToInt16() >= currentRow && n.Y1 <= currentColumn && n.Y2 >= currentColumn);
                                                Grid.SetColumn(item2, first.Y1.ToInt16());
                                                //currentX = first.X1.ToInt16();
                                                //currentY = first.Y1.ToInt16();
                                                Grid.SetRow(item2, first.X1.ToInt16());
                                                var rowSpan = (first.X2 - first.X1).ToInt16() + 1;
                                                var columnSpan = (first.Y2 - first.Y1).ToInt16() + 1;
                                                if (rowSpan == 0) rowSpan = 1;
                                                if (columnSpan == 0) columnSpan = 1;
                                                Grid.SetRowSpan(item2, rowSpan);
                                                Grid.SetColumnSpan(item2, columnSpan);
                                            }
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Log.Error(exception);
                                }
                            }
                        }
                    }
                }
            }
            return base.MeasureOverride(constraint);
        }
        private int[,] listContainer { get; set; }

        public Size LastSize { get; set; }

        #endregion

        #endregion
        public CoreFlexGrid()
        {
            CurrentContainer = new Grid();
            CurrentContainer.HorizontalAlignment = HorizontalAlignment.Left;
            CurrentContainer.Background = new SolidColorBrush(Colors.Transparent);
            CurrentContainer.MouseLeftButtonDown += CurrentContainerOnMouseLeftButtonDown;
            Content = CurrentContainer;
            ScrollChanged += OnScrollChanged;
            BorderThickness = new Thickness(1);

            Loaded += OnLoaded;
            listLoad = new List<FrameworkElement>();
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            e.Handled = true;
            if (e.OriginalSource == null)
            {

            }
        }

        private void CurrentContainerOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private ColumnInfo[] Columns;
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Elements = new CoreDictionary<string, object>();
            CreateCursor();
            PreviewKeyDown += OnKeyDown;

        }

        internal void CreateCursor()
        {
            FocusControl = new Canvas();
            FocusControl.Background = SelectedBrush;
            FocusControl.Margin = new Thickness(0);
            FocusControl.VerticalAlignment = VerticalAlignment.Stretch;
            FocusControl.HorizontalAlignment = HorizontalAlignment.Stretch;

            currentRow = 0;
            currentColumn = 0;
            RebindPosition();
            Grid.SetColumn(FocusControl, currentColumn);
            Grid.SetRow(FocusControl, currentRow);
            CurrentContainer.Children.Add(FocusControl);
        }

        #region Overrides of FrameworkElement

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus"/> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Colors.AliceBlue);
            base.OnGotFocus(e);
        }

        #region Overrides of UIElement

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
        }

        #endregion

        #region Overrides of UIElement

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.LostFocus"/> routed event by using the event data that is provided. 
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs"/> that contains event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus"/> event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Colors.Transparent);
            base.OnLostFocus(e);
        }

        #endregion

        #endregion


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Delete();
            }
            if (e.Key == Key.Escape && IsEdit)
            {
                Commit(false);
            }
            if (!IsEdit)
            {
                if (e.Key == Key.Up)
                    if (currentRow > 0)
                        currentRow--;
                if (e.Key == Key.Down)
                    if (currentRow < RowCount)
                        currentRow++;
                if (e.Key == Key.Left)
                    if (currentColumn > 0)
                        currentColumn--;
                if (e.Key == Key.Right)
                    if (currentColumn < ColumnCount)
                        currentColumn++;
                if (currentRow <= RowFix - 1)
                    currentRow = RowFix;
                if (currentColumn <= ColumnFix - 1)
                    currentColumn = ColumnFix;
                RebindPosition();
                if (e.Key == Key.Return)
                {
                    object objt;
                    Elements.TryGetValue(currentRow + "+" + currentColumn, out objt);
                    var args = new CellArgs();
                    args.Source = objt;
                    args.Row = currentRow;
                    args.Column = currentColumn;

                    if (currentObjEdit != null)
                    {
                        var current = this[currentRowEdit, currentColumnEdit];
                        if (current != null)
                            if (!currentObjEdit.Equals(current))
                                this[currentRowEdit, currentColumnEdit] = currentObjEdit;

                    }
                    currentObjEdit = objt;
                    currentRowEdit = currentRow;
                    currentColumnEdit = currentColumn;
                    OnEditCell(objt, args);
                    IsEdit = true;
                    if (!args.Handle)
                    {
                        this[currentRow, currentColumn] = args.Source;
                        currentObjEditSource = args.Source;
                    }
                }
            }


        }

        private bool isEdit;

        private bool IsEdit
        {
            get { return isEdit; }
            set
            {
                isEdit = value;

            }
        }

        //public bool IsEdit { get; set; }

        public class CellArgs : EventArgs
        {
            public object Source { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
            public bool Handle { get; set; }
            public bool IsCancle { get; set; }
            public object ObjectBefore { get; set; }
        }
        public event EditCellHandler EditCell;
        public event EventHandler<CellArgs> CommitCell;
        public event EventHandler<CellArgs> DeleteCell;
        public delegate void EditCellHandler(object sender, CellArgs e);
        public class CellChangedEvent
        {
            public int X { get; set; }
            public int Y { get; set; }
            public object SourceControl { get; set; }
        }

        public event CellChangedHendler CellChanged;
        public delegate void CellChangedHendler(CoreFlexGrid sender, CellChangedEvent e);
        private void RebindPosition()
        {
            Debug.WriteLine(currentColumn + " " + currentRow);
            Panel.SetZIndex(FocusControl, -1000);
            Grid.SetColumn(FocusControl, currentColumn);
            Grid.SetRow(FocusControl, currentRow);
            Grid.SetRowSpan(FocusControl, 1);
            Grid.SetColumnSpan(FocusControl, 1);
            var first =
                            MergeCell.FirstOrDefault(n => n.X1.ToInt16() <= currentRow && n.X2.ToInt16() >= currentRow && n.Y1 <= currentColumn && n.Y2 >= currentColumn);// && n.Y1 >= j && n.Y2 <= j
            if (first != null)
            {

                Grid.SetColumn(FocusControl, first.Y1.ToInt16());
                //currentX = first.X1.ToInt16();
                //currentY = first.Y1.ToInt16();
                Grid.SetRow(FocusControl, first.X1.ToInt16());
                var rowSpan = (first.X2 - first.X1).ToInt16() + 1;
                var columnSpan = (first.Y2 - first.Y1).ToInt16() + 1;
                if (rowSpan == 0) rowSpan = 1;
                if (columnSpan == 0) columnSpan = 1;
                Grid.SetRowSpan(FocusControl, rowSpan);
                Grid.SetColumnSpan(FocusControl, columnSpan);
            }
            try
            {
                var args = new CellChangedEvent()
                {
                    X = currentColumn,
                    Y = currentRow,
                    SourceControl = Elements[currentRow + "+" + currentColumn]
                };
                OnCellChanged(args);
                Elements[currentRow + "+" + currentColumn] = args.SourceControl;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }

        }

        private Canvas FocusControl { get; set; }



        public Grid CurrentContainer { get; set; }
        public int RowFix { get; set; }
        public int ColumnFix { get; set; }
        public int Row { get { return currentRow; } set { currentRow = value; } }
        public int Column { get { return currentColumn; } set { currentColumn = value; } }

        public ColumnInfo this[int y]
        {
            get
            {
                if (Columns.Length < y)
                    return null;
                return Columns[y];
            }
            set
            {
                if (Columns == null)
                    return;
                if (Columns.Length <= y)
                    return;
                Columns[y] = value;
                MeasureOverride(LastSize);
            }
        }
        public CoreDictionary<string, object> Elements;
        public object this[int x, int y]
        {
            get
            {
                if (x > RowCount || y > ColumnCount)
                    return null;
                return Elements[x + "+" + y];
            }
            set
            {
                int localX = x;
                int localY = y;
                if (Elements == null)
                    Elements = new CoreDictionary<string, object>();
                if (x >= RowCount || y >= ColumnCount)
                    return;
                object obj;
                if (Elements.TryGetValue(x + "+" + y, out obj))
                {
                    if (obj is UIElement)
                    {
                        var grid = (obj as UIElement).GetParentObject() as Grid;
                        if (grid != null)
                            grid.Children.Remove(obj as UIElement);
                    }
                    Elements[x + "+" + y] = value;
                }
                else
                    Elements.Add(x + "+" + y, value);
                if (value is UIElement)
                {
                    var item = (UIElement)value;
                    if (item.GetParentObject() == null)
                    {
                        CurrentContainer.Children.Add(item);
                        Grid.SetColumn(item, y);
                        Grid.SetRow(item, x);
                        var first = MergeCell.FirstOrDefault(n => n.X1.ToInt16() <= x && n.X2.ToInt16() >= x && n.Y1 <= y && n.Y2 >= y);
                        if (first != null)
                        {
                            var item2 = item;
                            Grid.SetColumn(item2, first.Y1.ToInt16());
                            //currentX = first.X1.ToInt16();
                            //currentY = first.Y1.ToInt16();
                            Grid.SetRow(item2, first.X1.ToInt16());
                            var rowSpan = (first.X2 - first.X1).ToInt16() + 1;
                            var columnSpan = (first.Y2 - first.Y1).ToInt16() + 1;
                            if (rowSpan == 0) rowSpan = 1;
                            if (columnSpan == 0) columnSpan = 1;
                            Grid.SetRowSpan(item2, rowSpan);
                            Grid.SetColumnSpan(item2, columnSpan);
                        }
                        item.MouseLeftButtonUp += (sender, e) =>
                        {

                            currentColumn = localY;
                            currentRow = localX;
                            if (currentRow <= RowFix - 1)
                                currentRow = RowFix;
                            if (currentColumn <= ColumnFix - 1)
                                currentColumn = ColumnFix;
                            RebindPosition();
                        };
                    }
                }
                else if (value == null)
                {
                    if (currentObjEditSource != null)
                    {
                        var frameworkElement = currentObjEditSource as FrameworkElement;
                        if (frameworkElement != null)
                        {
                            var panel = frameworkElement.Parent as Panel;
                            if (panel != null)
                                panel.Children.Remove(frameworkElement);
                        }
                        currentObjEditSource = null;
                    }
                }
            }
        }

        protected virtual void OnCellChanged(CellChangedEvent e)
        {
            if (CellChanged != null)
                CellChanged.Invoke(this, e);
        }

        public void Merge(int x, int y, int x1, int y1)
        {
            MergeCell.Add(new Line()
            {
                X1 = x,
                X2 = x1,
                Y1 = y,
                Y2 = y1
            });
            MeasureOverride(LastSize);
        }
        private List<Line> MergeCell = new List<Line>();
        private object currentObjEdit;
        private int currentRowEdit;
        private int currentColumnEdit;
        private object currentObjEditSource;

        protected virtual void OnEditCell(object objt, CellArgs obj)
        {
            if (EditCell != null) EditCell.Invoke(obj, obj);
        }
        protected virtual void OnDeleteCell(object objt, CellArgs obj)
        {
            if (DeleteCell != null) DeleteCell.Invoke(objt, obj);
        }
        protected virtual void OnCommitCell(object objt, CellArgs obj)
        {
            if (CommitCell != null) CommitCell.Invoke(objt, obj);
        }
        public void Delete()
        {
            object objt;
            Elements.TryGetValue(currentRow + "+" + currentColumn, out objt);
            var args = new CellArgs();
            args.Source = objt;
            args.Row = currentRow;
            args.Column = currentColumn;
            OnDeleteCell(objt, args);
            if (!args.Handle)
                this[currentRow, currentColumn] = null;
            IsEdit = false;
            Manager.Timeout(Dispatcher, () => Focus());
        }
        public void Commit(bool isCommit = true)
        {
            object objt;
            Elements.TryGetValue(currentRow + "+" + currentColumn, out objt);
            var args = new CellArgs();
            args.Source = objt;
            args.ObjectBefore = currentObjEdit;
            args.Row = currentRow;
            args.Column = currentColumn;
            args.IsCancle = !isCommit;
            OnCommitCell(objt, args);
            if (!args.Handle)
                this[currentRow, currentColumn] = args.Source;
            if (args.IsCancle)
                this[currentRow, currentColumn] = currentObjEdit;
            currentObjEdit = null;
            IsEdit = false;
            Manager.Timeout(Dispatcher, () => Focus());
        }
    }
}
