using System.Windows;
using System.Windows.Controls;

namespace GW2BuildLibrary.UI.Controls
{
    /// <summary>
    /// Interaction logic for BuildPreviews.
    /// </summary>
    public partial class BuildPreview : UserControl
    {
        #region Fields

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="Slot1"/>.
        /// </summary>
        public static readonly DependencyProperty Slot1Property =
            DependencyProperty.Register(nameof(Slot1), typeof(SpecializationSlot), typeof(BuildPreview), new PropertyMetadata(null));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="Slot2"/>.
        /// </summary>
        public static readonly DependencyProperty Slot2Property =
            DependencyProperty.Register(nameof(Slot2), typeof(SpecializationSlot), typeof(BuildPreview), new PropertyMetadata(null));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="Slot3"/>.
        /// </summary>
        public static readonly DependencyProperty Slot3Property =
            DependencyProperty.Register(nameof(Slot3), typeof(SpecializationSlot), typeof(BuildPreview), new PropertyMetadata(null));

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="BuildPreview"/> instance.
        /// </summary>
        public BuildPreview()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The first specialization slot.
        /// </summary>
        public SpecializationSlot Slot1
        {
            get { return (SpecializationSlot)GetValue(Slot1Property); }
            set { SetValue(Slot1Property, value); }
        }

        /// <summary>
        /// The second specialization slot.
        /// </summary>
        public SpecializationSlot Slot2
        {
            get { return (SpecializationSlot)GetValue(Slot2Property); }
            set { SetValue(Slot2Property, value); }
        }

        /// <summary>
        /// The third specialization slot.
        /// </summary>
        public SpecializationSlot Slot3
        {
            get { return (SpecializationSlot)GetValue(Slot3Property); }
            set { SetValue(Slot3Property, value); }
        }

        #endregion Properties
    }
}