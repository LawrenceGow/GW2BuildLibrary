﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace GW2BuildLibrary.UI.Controls
{
    /// <summary>
    /// An <see cref="ItemsControl"/> that keeps track of the number of possible items it can hold based on a target
    /// item width and height.
    /// </summary>
    public class BuildTemplateItemsControl : ItemsControl
    {
        #region Fields

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="Columns"/>.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int), typeof(BuildTemplateItemsControl),
                new PropertyMetadata(2));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="Rows"/>.
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(nameof(Rows), typeof(int), typeof(BuildTemplateItemsControl),
                new PropertyMetadata(16));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="TargetItemHeight"/>.
        /// </summary>
        public static readonly DependencyProperty TargetItemHeightProperty =
            DependencyProperty.Register(nameof(TargetItemHeight), typeof(double), typeof(BuildTemplateItemsControl),
                new PropertyMetadata(30d));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="TargetItemWidth"/>.
        /// </summary>
        public static readonly DependencyProperty TargetItemWidthProperty =
            DependencyProperty.Register(nameof(TargetItemWidth), typeof(double), typeof(BuildTemplateItemsControl),
                new PropertyMetadata(260d));

        /// <summary>
        /// Event for when the number of possible items in the control has changed.
        /// </summary>
        public EventHandler ItemCountChanged;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the number of columns we are displaying the templates on.
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Gets the maximum number of items that currently can be displayed.
        /// </summary>
        public int ItemCount
        { get; set; } = 32;

        /// <summary>
        /// Gets or sets the number of rows we are displaying the templates on.
        /// </summary>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the target item height.
        /// </summary>
        public double TargetItemHeight
        {
            get { return (double)GetValue(TargetItemHeightProperty); }
            set { SetValue(TargetItemHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the target item width.
        /// </summary>
        public double TargetItemWidth
        {
            get { return (double)GetValue(TargetItemWidthProperty); }
            set { SetValue(TargetItemWidthProperty, value); }
        }

        #endregion Properties

        #region Methods

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

        #endregion Methods
    }
}