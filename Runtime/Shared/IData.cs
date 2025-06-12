namespace WelwiseGamesSDK.Shared
{
    /// <summary>
    /// Provides persistent data storage capabilities.
    /// Implement this interface for platform-agnostic data persistence.
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Stores a string value with the specified key.
        /// </summary>
        /// <param name="key">Unique identifier for the data</param>
        /// <param name="value">Value to store</param>
        public void SetString(string key, string value);
        
        /// <summary>
        /// Retrieves a string value by key.
        /// </summary>
        /// <param name="key">Key of the stored data</param>
        /// <param name="defaultValue">Value to return if key doesn't exist</param>
        /// <returns>Stored value or default if not found</returns>
        public string GetString(string key, string defaultValue = "");
        
        public void SetInt(string key, int value);
        /// <summary>
        /// Retrieves a string value by key.
        /// </summary>
        /// <param name="key">Key of the stored data</param>
        /// <param name="defaultValue">Value to return if key doesn't exist</param>
        /// <returns>Stored value or default if not found</returns>
        public int GetInt(string key, int defaultValue = 0);
        
        /// <summary>
        /// Stores a float value with the specified key.
        /// </summary>
        /// <param name="key">Unique identifier for the data</param>
        /// <param name="value">Value to store</param>
        public void SetFloat(string key, float value);
        /// <summary>
        /// Retrieves a float value by key.
        /// </summary>
        /// <param name="key">Key of the stored data</param>
        /// <param name="defaultValue">Value to return if key doesn't exist</param>
        /// <returns>Stored value or default if not found</returns>
        public float GetFloat(string key, float defaultValue = 0);
        
        /// <summary>
        /// Stores a bool value with the specified key.
        /// </summary>
        /// <param name="key">Unique identifier for the data</param>
        /// <param name="value">Value to store</param>
        public void SetBool(string key, bool value);
        /// <summary>
        /// Retrieves a bool value by key.
        /// </summary>
        /// <param name="key">Key of the stored data</param>
        /// <param name="defaultValue">Value to return if key doesn't exist</param>
        /// <returns>Stored value or default if not found</returns>
        public bool GetBool(string key, bool defaultValue = false);
        
        /// <summary>
        /// Checks if a specific key exists in storage.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if key exists, otherwise false</returns>
        public bool HasKey(string key);
    }
}