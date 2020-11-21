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
        private Brush DimTraitBrush;

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
                    Color traitColour = ((SolidColorBrush)traitChoices.TraitBrush).Color;
                    double s = 0.6d;
                    traitChoices.DimTraitBrush = new SolidColorBrush(Color.FromRgb((byte)(traitColour.R * s),
                        (byte)(traitColour.G * s), (byte)(traitColour.B * s)));
                    traitChoices.DimTraitBrush.Freeze();
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

            // Draw line between 1&2
            if (Slot.Trait1 > 0 && Slot.Trait2 > 0)
            {
                dc.DrawLine(TraitPen,
                    GetPositionForTrait(1, Slot.Trait1),
                    GetPositionForTrait(2, Slot.Trait2));
            }

            // Draw line between 2&3
            if (Slot.Trait2 > 0 && Slot.Trait3 > 0)
            {
                dc.DrawLine(TraitPen,
                    GetPositionForTrait(2, Slot.Trait2),
                    GetPositionForTrait(3, Slot.Trait3));
            }

            dc.DrawEllipse((Slot.Trait1 == 1) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(1, 1), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait1 == 2) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(1, 2), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait1 == 3) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(1, 3), traitSize, traitSize);

            dc.DrawEllipse((Slot.Trait2 == 1) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(2, 1), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait2 == 2) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(2, 2), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait2 == 3) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(2, 3), traitSize, traitSize);

            dc.DrawEllipse((Slot.Trait3 == 1) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(3, 1), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait3 == 2) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(3, 2), traitSize, traitSize);
            dc.DrawEllipse((Slot.Trait3 == 3) ? DimTraitBrush : null, TraitPen,
                GetPositionForTrait(3, 3), traitSize, traitSize);
        }

        /// <summary>
        /// Gets the position for the specified trait.
        /// </summary>
        /// <param name="traitStage">The stage the trait is in.</param>
        /// <param name="traitChoice">The trait choice number.</param>
        /// <returns></returns>
        private Point GetPositionForTrait(in byte traitStage, in byte traitChoice)
        {
            double x = 0, y = 0;
            switch (traitStage)
            {
                case 1:
                    x = 0;
                    break;

                case 2:
                    x = ActualWidth / 2;
                    break;

                case 3:
                    x = ActualWidth;
                    break;
            }

            switch (traitChoice)
            {
                case 1:
                    y = 0;
                    break;

                case 2:
                    y = ActualHeight / 2;
                    break;

                case 3:
                    y = ActualHeight;
                    break;
            }

            return new Point(x, y);
        }
    }
}
