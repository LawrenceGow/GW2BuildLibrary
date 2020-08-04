using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace GW2BuildLibrary.UI.ViewModels
{
    /// <summary>
    /// View model class for <see cref="GW2BuildLibrary.BuildTemplate"/>s.
    /// </summary>
    public class BuildTemplateViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Dispatcher Dispatcher;

        /// <summary>
        /// Initialises a new instance of the <see cref="BuildTemplateViewModel"/> class.
        /// </summary>
        public BuildTemplateViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// The index of the model.
        /// </summary>
        public int Index = -1;

        private string name = "Empty";

        /// <summary>
        /// Gets or sets the models name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private Profession profession = Profession.None;

        /// <summary>
        /// Gets or sets the models profession.
        /// </summary>
        public Profession Profession
        {
            get { return profession; }
            set
            {
                if (profession != value)
                {
                    profession = value;
                    OnPropertyChanged("Profession");
                }
            }
        }

        private BuildTemplate buildTemplate = null;

        /// <summary>
        /// Gets or set the <see cref="GW2BuildLibrary.BuildTemplate"/> we are modelling.
        /// </summary>
        public BuildTemplate BuildTemplate
        {
            get { return buildTemplate; }
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
            }
            else
            {
                Name = "Empty";
                Profession = Profession.None;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked when a visual property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
