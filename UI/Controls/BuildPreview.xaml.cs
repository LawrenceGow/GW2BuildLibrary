using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GW2BuildLibrary.UI.Controls
{
    /// <summary>
    /// Interaction logic for BuildPreviews.
    /// </summary>
    public partial class BuildPreview : UserControl
    {
        public BuildPreview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The first specialization slot.
        /// </summary>
        public SpecializationSlot Slot1
        {
            get { return (SpecializationSlot)GetValue(Slot1Property); }
            set { SetValue(Slot1Property, value); }
        }

        public static readonly DependencyProperty Slot1Property =
            DependencyProperty.Register("Slot1", typeof(SpecializationSlot), typeof(BuildPreview), new PropertyMetadata(null));

        /// <summary>
        /// The second specialization slot.
        /// </summary>
        public SpecializationSlot Slot2
        {
            get { return (SpecializationSlot)GetValue(Slot2Property); }
            set { SetValue(Slot2Property, value); }
        }

        public static readonly DependencyProperty Slot2Property =
            DependencyProperty.Register("Slot2", typeof(SpecializationSlot), typeof(BuildPreview), new PropertyMetadata(null));

        /// <summary>
        /// The third specialization slot.
        /// </summary>
        public SpecializationSlot Slot3
        {
            get { return (SpecializationSlot)GetValue(Slot3Property); }
            set { SetValue(Slot3Property, value); }
        }

        public static readonly DependencyProperty Slot3Property =
            DependencyProperty.Register("Slot3", typeof(SpecializationSlot), typeof(BuildPreview), new PropertyMetadata(null));
    }
}
