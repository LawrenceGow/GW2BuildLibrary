using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GW2BuildLibrary
{
    public class BuildTemplateItemsControl : ItemsControl
    {
        /// <summary>
        /// Gets or sets the number of rows we are displaying the templates on.
        /// </summary>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(BuildTemplateItemsControl), new PropertyMetadata(16));

        /// <summary>
        /// Gets or sets the number of columns we are displaying the templates on.
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(BuildTemplateItemsControl), new PropertyMetadata(2));

        /// <summary>
        /// Gets or sets the target item height.
        /// </summary>
        public double TargetItemHeight
        {
            get { return (double)GetValue(TargetItemHeightProperty); }
            set { SetValue(TargetItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetItemHeightProperty =
            DependencyProperty.Register("TargetItemHeight", typeof(double), typeof(BuildTemplateItemsControl), new PropertyMetadata(30d));

        /// <summary>
        /// Gets or sets the target item width.
        /// </summary>
        public double TargetItemWidth
        {
            get { return (double)GetValue(TargetItemWidthProperty); }
            set { SetValue(TargetItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetItemWidthProperty =
            DependencyProperty.Register("TargetItemWidth", typeof(double), typeof(BuildTemplateItemsControl), new PropertyMetadata(260d));

        /// <summary>
        /// Gets the maximum number of items that currently can be displayed.
        /// </summary>
        public int ItemCount
        { get; set; } = 32;

        public EventHandler ItemCountChanged;

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>The size of the control, up to the maximum specified by constraint.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            // Calculate the optimal number of rows & columns to show as many templates as possible.
            Columns = (int)(constraint.Width / TargetItemWidth + 0.5d);
            Rows = (int)(constraint.Height / TargetItemHeight + 0.5d);
            int count = Rows * Columns;
            if (count != ItemCount)
            {
                ItemCount = count;
                ItemCountChanged?.Invoke(this, null);
            }
            return base.MeasureOverride(constraint);
        }
    }
}
