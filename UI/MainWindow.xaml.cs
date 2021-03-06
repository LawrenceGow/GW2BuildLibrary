﻿using GW2BuildLibrary.UI.ViewModels;
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
        #region Fields

        private BuildTemplateViewModel renameTarget = null;

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="BuildTemplateModels"/>.
        /// </summary>
        public static readonly DependencyProperty BuildTemplateModelsProperty =
            DependencyProperty.Register("BuildTemplateModels", typeof(ObservableCollection<BuildTemplateViewModel>),
                typeof(MainWindow), new PropertyMetadata(new ObservableCollection<BuildTemplateViewModel>()));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="CurrentPage"/>.
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="InOverlayMode"/>.
        /// </summary>
        public static readonly DependencyProperty InOverlayModeProperty =
            DependencyProperty.Register("InOverlayMode", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        /// <summary>
        /// Backing <see cref="DependencyProperty"/> for <see cref="ProfessionFilter"/>.
        /// </summary>
        public static readonly DependencyProperty ProfessionFilterProperty =
            DependencyProperty.Register("ProfessionFilter", typeof(Profession), typeof(MainWindow),
                new PropertyMetadata(Profession.None));

        /// <summary>
        /// Clears the build template out of the slot.
        /// </summary>
        public static RoutedCommand ClearBuildTemplate = new RoutedCommand();

        /// <summary>
        /// Closes the application.
        /// </summary>
        public static RoutedCommand CloseApplication = new RoutedCommand();

        /// <summary>
        /// Enters rename mode, targeting the selected slot.
        /// </summary>
        public static RoutedCommand EnterRenameMode = new RoutedCommand();

        /// <summary>
        /// Exits rename mode for the targeted slot, and pushes the name to the underlying <see cref="BuildTemplate"/>.
        /// </summary>
        public static RoutedCommand ExitRenameMode = new RoutedCommand();

        /// <summary>
        /// Moves to the next page.
        /// </summary>
        public static RoutedCommand NextPage = new RoutedCommand();

        /// <summary>
        /// Moves to the previous page.
        /// </summary>
        public static RoutedCommand PrevPage = new RoutedCommand();

        /// <summary>
        /// Stores a build template if the selected slot is empty, otherwise the stored build will be placed into the clipboard.
        /// </summary>
        public static RoutedCommand StoreOrRecallBuildTemplate = new RoutedCommand();

        /// <summary>
        /// Toggles the profession filter.
        /// </summary>
        public static RoutedCommand ToggleFilter = new RoutedCommand();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            if (BuildLibrary != null)
            {
                if (BuildLibrary.Settings.ProfessionFilter != Profession.None)
                    ProfessionFilter = BuildLibrary.Settings.ProfessionFilter;

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

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Reference to the <see cref="GW2BuildLibrary.BuildLibrary"/> instance.
        /// </summary>
        private BuildLibrary BuildLibrary
        { get { return App.BuildLibrary; } }

        /// <summary>
        /// The build template view models.
        /// </summary>
        public ObservableCollection<BuildTemplateViewModel> BuildTemplateModels
        {
            get { return (ObservableCollection<BuildTemplateViewModel>)GetValue(BuildTemplateModelsProperty); }
            set { SetValue(BuildTemplateModelsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        /// <summary>
        /// Gets whether the library is in overlay mode or not.
        /// </summary>
        public bool InOverlayMode
        {
            get { return (bool)GetValue(InOverlayModeProperty); }
            set { SetValue(InOverlayModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the profession filter.
        /// </summary>
        public Profession ProfessionFilter
        {
            get { return (Profession)GetValue(ProfessionFilterProperty); }
            set { SetValue(ProfessionFilterProperty, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the ItemCountChanged event from <see cref="BuildTemplateItems"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BuildTemplateItems_ItemCountChanged(object sender, EventArgs e)
        {
            SyncModels();
        }

        /// <summary>
        /// Logic for the <see cref="ClearBuildTemplate"/> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The execution arguments.</param>
        private void ClearBuildTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.Assert(e.Parameter is BuildTemplateViewModel);
            BuildTemplateViewModel model = (BuildTemplateViewModel)e.Parameter;
            if (model.BuildTemplate != null)
            {
                BuildLibrary.DeleteBuildTemplate(model.Index);
                SyncModels();
            }
        }

        /// <summary>
        /// Logic for the <see cref="CloseApplication"/> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The execution arguments.</param>
        private void CloseApplication_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        /// <summary>
        /// Logic for the <see cref="EnterRenameMode"/> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The execution arguments.</param>
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
        /// Logic for the <see cref="ExitRenameMode"/> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The execution arguments.</param>
        private void ExitRenameMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (renameTarget != null && bool.TryParse(e.Parameter as string, out bool setName))
            {
                PART_RenameInputDialog.Visibility = Visibility.Collapsed;
                if (setName)
                    BuildLibrary.SetBuildTemplateName(renameTarget.Index, PART_RenameTextInput.Text);
                PART_RenameTextInput.Clear();
                renameTarget = null;
            }
        }

        /// <summary>
        /// Logic for the <see cref="NextPage"/> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The execution arguments.</param>
        private void NextPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentPage < 255)
            {
                CurrentPage++;
                SyncModels();
            }
        }

        /// <summary>
        /// Logic for the <see cref="PrevPage"/> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The execution arguments.</param>
        private void PrevPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                SyncModels();
            }
        }

        /// <summary>
        /// Logic for the <see cref="StoreOrRecallBuildTemplate"/> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The execution arguments.</param>
        private void StoreOrRecallBuildTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is BuildTemplateViewModel model)
            {
                if (model.BuildTemplate == null)
                {
                    BuildLibrary.CreateBuildTemplate(model.Index, Clipboard.GetText());
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
        /// Synchronises the view models for the build templates with the templates in the library.
        /// </summary>
        private void SyncModels()
        {
            Debug.Assert(BuildLibrary != null, "No BuildLibrary instance was set.");
            if (BuildLibrary == null)
                return;

            // Ensure there is a model for every slot
            while (BuildTemplateModels.Count < BuildTemplateItems.ItemCount)
            {
                BuildTemplateModels.Add(new BuildTemplateViewModel());
            }
            // And no more
            while (BuildTemplateModels.Count > BuildTemplateItems.ItemCount)
            {
                BuildTemplateModels.RemoveAt(BuildTemplateModels.Count - 1);
            }

            int pageOffset = BuildTemplateItems.ItemCount * (CurrentPage - 1);
            if (ProfessionFilter == Profession.None)
            {
                /* No filter applied so just show the builds as they are
                 * All models are used and empty ones allow for storing more builds
                 */
                for (int modelIndex = 0; modelIndex < BuildTemplateItems.ItemCount; modelIndex++)
                {
                    BuildTemplateViewModel model = BuildTemplateModels[modelIndex];
                    BuildTemplate build = BuildLibrary.GetBuildTemplate(modelIndex + pageOffset);
                    model.BuildTemplate = build;
                    model.Index = build?.Index ?? (modelIndex + pageOffset);
                    model.IsHidden = false;
                }
            }
            else
            {
                // If a filter is applied then get all templates and only create models for those that match
                List<BuildTemplate> buildTemplates = BuildLibrary.GetAllBuildTemplates(ProfessionFilter)
                    .OrderBy(b => b.Index).ToList();

                int modelIndex;
                for (modelIndex = 0; modelIndex < BuildTemplateItems.ItemCount
                    && (modelIndex + pageOffset) < buildTemplates.Count; modelIndex++)
                {
                    BuildTemplateViewModel model = BuildTemplateModels[modelIndex];
                    BuildTemplate build = buildTemplates[modelIndex + pageOffset];
                    model.BuildTemplate = build;
                    model.Index = build.Index;
                    model.IsHidden = false;
                }

                // Ensure there is an empty slot after the filtered results if there is room
                if (modelIndex < BuildTemplateModels.Count)
                {
                    BuildTemplateViewModel model = BuildTemplateModels[modelIndex];
                    model.BuildTemplate = null;
                    model.Index = BuildLibrary.GetNextFreeIndex();
                    model.IsHidden = false;
                    modelIndex++;
                }

                // Instead of deleting models, tell them to hide
                for (; modelIndex < BuildTemplateItems.ItemCount; modelIndex++)
                {
                    BuildTemplateViewModel model = BuildTemplateModels[modelIndex];
                    model.IsHidden = true;
                    model.BuildTemplate = null;
                }
            }
        }

        /// <summary>
        /// Logic for the <see cref="ToggleFilter"/> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The execution arguments.</param>
        private void ToggleFilter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.Assert(e.Parameter is Profession);
            Profession filter = (Profession)e.Parameter;
            ProfessionFilter = ProfessionFilter == filter ? Profession.None : filter;
            SyncModels();
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
        /// Is invoked whenever application code or internal processes call <see cref="FrameworkElement.ApplyTemplate"/>.
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

        #endregion Methods
    }
}