namespace WelwiseGamesSDK.Shared
{
    public interface IAnalytics
    {
        public void SendEvent(string eventName);
        public void SendEvent(string eventName, string data);

        public void GameIsReady();
        public void GameplayStart();
        public void GameplayEnd();
    }
}