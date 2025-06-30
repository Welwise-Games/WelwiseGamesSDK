namespace WelwiseGamesSDK.Shared.Types
{
    /// <summary>
    /// Represents the type of device running the application.
    /// Used to distinguish between different form factors and input methods.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// Mobile phone device (typically touchscreen with phone capabilities).
        /// Includes smartphones and small-screen handheld devices.
        /// </summary>
        Mobile,

        /// <summary>
        /// Desktop or laptop computer.
        /// Characterized by mouse/keyboard input and larger screens.
        /// </summary>
        Desktop,

        /// <summary>
        /// Tablet device (touchscreen interface larger than mobile phones).
        /// Includes iPads, Android tablets, and similar medium-sized touch devices.
        /// </summary>
        Tablet,
    }
}