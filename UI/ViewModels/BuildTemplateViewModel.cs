using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace GW2BuildLibrary.UI.ViewModels
{
    /// <summary>
    /// View model class for <see cref="GW2BuildLibrary.BuildTemplate"/> s.
    /// </summary>
    public class BuildTemplateViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private readonly Dispatcher Dispatcher;

        private BuildTemplate buildTemplate = null;

        private bool disposedValue = false;

        private bool isEmpty = true;

        private bool isHidden = false;

        private string name = "Empty";

        private Profession profession = Profession.None;

        private SpecializationSlot slot1 = null;

        private SpecializationSlot slot2 = null;

        private SpecializationSlot slot3 = null;

        /// <summary>
        /// The index of the model.
        /// </summary>
        public int Index = -1;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="BuildTemplateViewModel"/> class.
        /// </summary>
        public BuildTemplateViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or set the <see cref="GW2BuildLibrary.BuildTemplate"/> we are modelling.
        /// </summary>
        public BuildTemplate BuildTemplate
        {
            get
            {
                return buildTemplate;
            }
            set
            {
                if (buildTemplate != null)
                {
                    buildTemplate.Updated -= BuildTemplate_Updated;
                }
                buildTemplate = value;
                if (buildTemplate != null)
                {
                    buildTemplate.Updated += BuildTemplate_Updated;
                }
                FillProperties();
            }
        }

        /// <summary>
        /// Whether or not this model represents an empty slot.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }
            set
            {
                if (isEmpty != value)
                {
                    isEmpty = value;
                    OnPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        /// <summary>
        /// Whether or not model should be hidden from view.
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return isHidden;
            }
            set
            {
                if (isHidden != value)
                {
                    isHidden = value;
                    OnPropertyChanged(nameof(IsHidden));
                }
            }
        }

        /// <summary>
        /// Gets or sets the models name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the models profession.
        /// </summary>
        public Profession Profession
        {
            get
            {
                return profession;
            }
            set
            {
                if (profession != value)
                {
                    profession = value;
                    OnPropertyChanged(nameof(Profession));
                }
            }
        }

        /// <summary>
        /// The first specialization slot.
        /// </summary>
        public SpecializationSlot Slot1
        {
            get
            {
                return slot1;
            }
            set
            {
                if (slot1 != value)
                {
                    slot1 = value;
                    OnPropertyChanged(nameof(Slot1));
                }
            }
        }

        /// <summary>
        /// The second specialization slot.
        /// </summary>
        public SpecializationSlot Slot2
        {
            get
            {
                return slot2;
            }
            set
            {
                if (slot2 != value)
                {
                    slot2 = value;
                    OnPropertyChanged(nameof(Slot2));
                }
            }
        }

        /// <summary>
        /// The third specialization slot.
        /// </summary>
        public SpecializationSlot Slot3
        {
            get
            {
                return slot3;
            }
            set
            {
                if (slot3 != value)
                {
                    slot3 = value;
                    OnPropertyChanged(nameof(Slot3));
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the Updated event from <see cref="BuildTemplate"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BuildTemplate_Updated(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(FillProperties));
        }

        /// <summary>
        /// Fills the visual properties of the view model.
        /// </summary>
        private void FillProperties()
        {
            if (BuildTemplate != null)
            {
                if (string.IsNullOrEmpty(BuildTemplate.Name))
                    Name = $"{BuildTemplate.Profession} Build";
                else
                    Name = BuildTemplate.Name;
                Profession = BuildTemplate.Profession;
                Slot1 = BuildTemplate.Slot1;
                Slot2 = BuildTemplate.Slot2;
                Slot3 = BuildTemplate.Slot3;
                IsEmpty = false;
            }
            else
            {
                Name = string.Empty;
                Profession = Profession.None;
                Slot1 = null;
                Slot2 = null;
                Slot3 = null;
                IsEmpty = true;
            }
        }

        /// <summary>
        /// Invoked when a visual property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // To detect redundant calls

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>True</c> if disposing managed resources, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    BuildTemplate = null;
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion Methods
    }
}