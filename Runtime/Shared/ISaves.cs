namespace WelwiseGamesSDK.Shared
{
    public interface ISaves
    {
        public string GetPlayerName();
        public void SetPlayerName(string name);
        
        public void SetString(string key, string value);
        public string GetString(string key, string defaultValue);
        
        public void SetInt(string key, int value);
        public int GetInt(string key, int defaultValue);
        
        public void SetFloat(string key, float value);
        public float GetFloat(string key, float defaultValue);
        
        public void SetBool(string key, bool value);
        public bool GetBool(string key, bool defaultValue);
    }
}