using System.Windows;
using System.Windows.Media;

namespace GW2BuildLibrary.UI.Controls
{
    /// <summary>
    /// Simple element that draws the trait choices.
    /// </summary>
    public class SlotTraitChoices : FrameworkElement
    {
        private const double traitSize = 3;
        private Pen TraitPen;

        /// <summary>
        /// The brush to use for the trait choices.
        /// </summary>
        public Brush TraitBrush
        {
            get { return (Brush)GetValue(TraitBrushProperty); }
            set { SetValue(TraitBrushProperty, value); }
        }

        public static readonly DependencyProperty TraitBrushProperty =
            DependencyProperty.Register("TraitBrush", typeof(Brush), typeof(SlotTraitChoices), new PropertyMetadata(null,
                new PropertyChangedCallback((d, e) =>
                {
                    SlotTraitChoices traitChoices = (SlotTraitChoices)d;
                    traitChoices.TraitPen = new Pen(traitChoices.TraitBrush, 1);
                    traitChoices.TraitPen.Freeze();
                    traitChoices.InvalidateVisual();
                })));

        /// <summary>
        /// The specialization slot this element is drawing traits for.
        /// </summary>
        public SpecializationSlot Slot
        {
            get { return (SpecializationSlot)GetValue(SlotProperty); }
            set { SetValue(SlotProperty, value); }
        }

        public static readonly DependencyProperty SlotProperty = DependencyProperty.Register(nameof(Slot),
            typeof(SpecializationSlot), typeof(SlotTraitChoices), new PropertyMetadata(null));

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout
        /// system. The rendering instructions for this element are not used directly when this method is invoked, and
        /// are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="dc">The drawing instructions for a specific element. This context is provided to
        /// the layout system.</param>
        protected override void OnRender(DrawingContext dc)
        {
            // No slot? Nothing to draw.
            if (Slot == null)
                return;

            dc.DrawEllipse((Slot.Trait1 == 1) ? TraitBrush : null, TraitPen,
                new Point(0, 0), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait1 == 2) ? TraitBrush : null, TraitPen,
                new Point(0, ActualHeight / 2), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait1 == 3) ? TraitBrush : null, TraitPen,
                new Point(0, ActualHeight), traitSize, traitSize);

            dc.DrawEllipse((Slot.Trait2 == 1) ? TraitBrush : null, TraitPen,
                new Point(ActualWidth / 2, 0), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait2 == 2) ? TraitBrush : null, TraitPen,
                new Point(ActualWidth / 2, ActualHeight / 2), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait2 == 3) ? TraitBrush : null, TraitPen,
                new Point(ActualWidth / 2, ActualHeight), traitSize, traitSize);

            dc.DrawEllipse((Slot.Trait3 == 1) ? TraitBrush : null, TraitPen,
                new Point(ActualWidth, 0), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait3 == 2) ? TraitBrush : null, TraitPen,
                new Point(ActualWidth, ActualHeight / 2), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait3 == 3) ? TraitBrush : null, TraitPen,
                new Point(ActualWidth, ActualHeight), traitSize, traitSize);
        }
    }
}
