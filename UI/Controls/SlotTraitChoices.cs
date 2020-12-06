using System.Windows;
using System.Windows.Media;

namespace GW2BuildLibrary.UI.Controls
{
    /// <summary>
    /// Simple element that draws the trait choices.
    /// </summary>
    public class SlotTraitChoices : FrameworkElement
    {
        #region Fields

        /// <summary>
        /// The radius of the dots used for the trait choices.
        /// </summary>
        private const double traitSize = 3;

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="Slot"/>.
        /// </summary>
        public static readonly DependencyProperty SlotProperty = DependencyProperty.Register(nameof(Slot),
                    typeof(SpecializationSlot), typeof(SlotTraitChoices), new PropertyMetadata(null));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="TraitFillBrush"/>.
        /// </summary>
        public static readonly DependencyProperty TraitFillBrushProperty =
                    DependencyProperty.Register(nameof(TraitFillBrush), typeof(Brush), typeof(SlotTraitChoices),
                        new PropertyMetadata(null, new PropertyChangedCallback(
                            (d, e) => ((SlotTraitChoices)d).InvalidateVisual())));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="TraitPen"/>.
        /// </summary>
        public static readonly DependencyProperty TraitPenProperty =
                    DependencyProperty.Register(nameof(TraitPen), typeof(Pen), typeof(SlotTraitChoices),
                        new PropertyMetadata(null, new PropertyChangedCallback(
                            (d, e) => ((SlotTraitChoices)d).InvalidateVisual())));

        #endregion Fields

        #region Properties

        /// <summary>
        /// The specialization slot this element is drawing traits for.
        /// </summary>
        public SpecializationSlot Slot
        {
            get { return (SpecializationSlot)GetValue(SlotProperty); }
            set { SetValue(SlotProperty, value); }
        }

        /// <summary>
        /// The brush to fill the trait choices with.
        /// </summary>
        public Brush TraitFillBrush
        {
            get { return (Brush)GetValue(TraitFillBrushProperty); }
            set { SetValue(TraitFillBrushProperty, value); }
        }

        /// <summary>
        /// The pen to use for the traits.
        /// </summary>
        public Pen TraitPen
        {
            get { return (Pen)GetValue(TraitPenProperty); }
            set { SetValue(TraitPenProperty, value); }
        }

        #endregion Properties

        #region Methods

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

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout
        /// system. The rendering instructions for this element are not used directly when this method is invoked, and
        /// are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="dc">
        /// The drawing instructions for a specific element. This context is provided to the layout system.
        /// </param>
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

            // Draw all the trait choices
            for (byte traitTier = 0; traitTier < 3; traitTier++)
            {
                for (byte traitChoice = 1; traitChoice <= 3; traitChoice++)
                {
                    dc.DrawEllipse((Slot.Traits[traitTier] == traitChoice) ? TraitFillBrush : null, TraitPen,
                        GetPositionForTrait((byte)(traitTier + 1), traitChoice), traitSize, traitSize);
                }
            }
        }

        #endregion Methods
    }
}