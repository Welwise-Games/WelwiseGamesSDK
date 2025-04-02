namespace WelwiseGamesSDK.Shared
{
    public interface IGameSessionTracker
    {
        public void SessionStarted();
        public void SessionEnded();
        public void Send();
    }
}