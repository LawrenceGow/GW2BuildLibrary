using GW2BuildLibrary.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GW2BuildLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Reference to the <see cref="GW2BuildLibrary.BuildLibrary"/> instance.
        /// </summary>
        private BuildLibrary BuildLibrary
        { get { return App.BuildLibrary; } }

        /// <summary>
        /// Initialises a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            if (BuildLibrary != null)
            {
                if (BuildLibrary.Settings.ProfessionFilter != Profession.None)
                {
                    ProfessionFilter = BuildLibrary.Settings.ProfessionFilter;
                    // TODO Hide filter buttons
                }

                if (BuildLibrary.Settings.OverlayMode)
                {
                    InOverlayMode = true;
                    WindowStyle = WindowStyle.None;
                    AllowsTransparency = true;
                    Topmost = true;
                    Opacity = 0.9d;
                }
            }

            SyncModels();
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call
        /// <see cref="FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            WindowState = BuildLibrary.WindowState;
            Width = BuildLibrary.Width;
            Height = BuildLibrary.Height;
            Left = BuildLibrary.Left;
            Top = BuildLibrary.Top;

            base.OnApplyTemplate();

            BuildTemplateItems.ItemCountChanged += BuildTemplateItems_ItemCountChanged;
        }

        /// <summary>
        /// Raises the System.Windows.Window.Closed event.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            BuildLibrary?.UpdateWindowStateForSaving(WindowState, RenderSize, Left, Top);

            base.OnClosed(e);
        }

        /// <summary>
        /// Handles the ItemCountChanged from <see cref="BuildTemplateItems"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BuildTemplateItems_ItemCountChanged(object sender, EventArgs e)
        {
            SyncModels();
        }

        #region Dependency Properties

        /// <summary>
        /// The build template view models.
        /// </summary>
        public ObservableCollection<BuildTemplateViewModel> BuildTemplateModels
        {
            get { return (ObservableCollection<BuildTemplateViewModel>)GetValue(BuildTemplateModelsProperty); }
            set { SetValue(BuildTemplateModelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BuildTemplateModels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BuildTemplateModelsProperty =
            DependencyProperty.Register("BuildTemplateModels", typeof(ObservableCollection<BuildTemplateViewModel>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<BuildTemplateViewModel>()));

        /// <summary>
        /// Gets or sets the profession filter.
        /// </summary>
        public Profession ProfessionFilter
        {
            get { return (Profession)GetValue(ProfessionFilterProperty); }
            set { SetValue(ProfessionFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProfessionFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProfessionFilterProperty =
            DependencyProperty.Register("ProfessionFilter", typeof(Profession), typeof(MainWindow), new PropertyMetadata(Profession.None));

        /// <summary>
        /// Gets whether the library is in overlay mode or not.
        /// </summary>
        public bool InOverlayMode
        {
            get { return (bool)GetValue(InOverlayModeProperty); }
            set { SetValue(InOverlayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InOverlayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InOverlayModeProperty =
            DependencyProperty.Register("InOverlayMode", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        #endregion

        #region Commands

        /// <summary>
        /// Stores a build template if the selected slot is empty, otherwise the stored build will be placed
        /// into the clipboard.
        /// </summary>
        public static RoutedCommand StoreOrRecallBuildTemplate = new RoutedCommand();

        private void StoreOrRecallBuildTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is BuildTemplateViewModel model)
            {
                if (model.BuildTemplate == null)
                {
                    BuildLibrary.CreateBuildTemplate(model.Page, model.Index, Clipboard.GetText());
                    SyncModels();
                }
                else
                {
                    Clipboard.SetText(model.BuildTemplate.BuildData);
                }

                // If we are in quick mode then we exit after this interaction
                if (BuildLibrary.Settings.QuickMode)
                    Application.Current.Shutdown(0);
            }
            else
            {
                Debug.Fail("Unexpected model type.");
            }
        }

        /// <summary>
        /// Clears the build template out of the slot.
        /// </summary>
        public static RoutedCommand ClearBuildTemplate = new RoutedCommand();

        private void ClearBuildTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.Assert(e.Parameter is BuildTemplateViewModel);
            BuildTemplateViewModel model = (BuildTemplateViewModel)e.Parameter;
            if (model.BuildTemplate != null)
            {
                BuildLibrary.DeleteBuildTemplate(model.Page, model.Index);
                SyncModels();
            }
        }

        private BuildTemplateViewModel renameTarget = null;

        /// <summary>
        /// Enters rename mode, targeting the selected slot.
        /// </summary>
        public static RoutedCommand EnterRenameMode = new RoutedCommand();

        private void EnterRenameMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.Assert(e.Parameter is BuildTemplateViewModel);
            BuildTemplateViewModel model = (BuildTemplateViewModel)e.Parameter;
            if (model.BuildTemplate != null)
            {
                PART_RenameInputDialog.Visibility = Visibility.Visible;
                PART_RenameTextInput.Text = model.Name;
                PART_RenameTextInput.Focus();
                PART_RenameTextInput.Select(0, model.Name.Length);
                renameTarget = model;
            }
        }

        /// <summary>
        /// Exits rename mode for the targeted slot, and pushes the name to the underlying <see cref="BuildTemplate"/>.
        /// </summary>
        public static RoutedCommand ExitRenameMode = new RoutedCommand();

        private void ExitRenameMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (renameTarget != null && bool.TryParse(e.Parameter as string, out bool setName))
            {
                PART_RenameInputDialog.Visibility = Visibility.Collapsed;
                if (setName)
                    BuildLibrary.SetBuildTemplateName(renameTarget.Page, renameTarget.Index, PART_RenameTextInput.Text);
                PART_RenameTextInput.Clear();
                renameTarget = null;
            }
        }

        /// <summary>
        /// Toggles the profession filter.
        /// </summary>
        public static RoutedCommand ToggleFilter = new RoutedCommand();

        private void ToggleFilter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.Assert(e.Parameter is Profession);
            Profession filter = (Profession)e.Parameter;
            ProfessionFilter = ProfessionFilter == filter ? Profession.None : filter;
            SyncModels();
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        public static RoutedCommand CloseApplication = new RoutedCommand();

        private void CloseApplication_Executed(object sender, ExecutedRoutedEventArgs e) =>
            Close();

        #endregion

        /// <summary>
        /// Synchronises the view models for the build templates with the templates in the library.
        /// </summary>
        private void SyncModels()
        {
            Debug.Assert(BuildLibrary != null, "No BuildLibrary instance was set.");
            if (BuildLibrary == null)
                return;

            if (ProfessionFilter == Profession.None)
            {
                for (int i = 0; i < BuildTemplateItems.ItemCount; i++)
                {
                    BuildTemplateViewModel model;
                    if (BuildTemplateModels.Count <= i)
                    {
                        model = new BuildTemplateViewModel();
                        BuildTemplateModels.Add(model);
                    }
                    else
                        model = BuildTemplateModels[i];

                    BuildTemplate build = BuildLibrary.GetBuildTemplate(CurrentPage - 1, i);
                    model.BuildTemplate = build;
                    model.Page = build?.Page ?? CurrentPage - 1;
                    model.Index = build?.Index ?? i;
                }
            }
            else
            {
                // If a filter is applied then get all templates and only create models for those that match
                List<BuildTemplate> buildTemplates = BuildLibrary.GetAllBuildTemplates(ProfessionFilter).OrderBy(b => b.Id).ToList();
                int visibleCount = 0;
                for (int i = BuildTemplateItems.ItemCount * (CurrentPage - 1); i < buildTemplates.Count && i < (BuildTemplateItems.ItemCount * CurrentPage); i++)
                {
                    BuildTemplate build = buildTemplates[i];

                    BuildTemplateViewModel model;
                    if (BuildTemplateModels.Count <= i)
                    {
                        model = new BuildTemplateViewModel();
                        BuildTemplateModels.Add(model);
                    }
                    else
                    {
                        model = BuildTemplateModels[i];
                    }

                    model.BuildTemplate = build;
                    model.Page = build.Page;
                    model.Index = build.Index;
                    visibleCount++;
                }

                while (BuildTemplateModels.Count > visibleCount)
                {
                    BuildTemplateModels[BuildTemplateModels.Count - 1].Dispose();
                    BuildTemplateModels.RemoveAt(BuildTemplateModels.Count - 1);
                }
            }
        }
    }
}
