namespace GW2BuildLibrary
{
    /// <summary>
    /// Hold all the settings for the <see cref="BuildLibrary"/>.
    /// </summary>
    public class BuildLibrarySettings
    {
        /// <summary>
        /// Whether or not the library is in overlay mode.
        /// </summary>
        public bool OverlayMode { get; set; } = false;

        /// <summary>
        /// Whether or not the library is in full screen mode.
        /// </summary>
        public bool FullScreenMode { get; set; } = false;

        /// <summary>
        /// Whether or not the library is in quick mode.
        /// </summary>
        public bool QuickMode { get; set; } = false;

        /// <summary>
        /// Whether or not the library will save the window state.
        /// </summary>
        public bool SaveWindowState { get; set; } = true;

        /// <summary>
        /// The library profession filter.
        /// </summary>
        public Profession ProfessionFilter { get; set; } = Profession.None;
    }
}
