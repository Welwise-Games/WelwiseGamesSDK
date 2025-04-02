using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Analytics.SessionTracker
{
    public class DebugGameSessionTracker : IGameSessionTracker
    {
        public void SessionStarted() => Debug.Log($"[{nameof(DebugGameSessionTracker)}] Session Started call");
        public void SessionEnded() => Debug.Log($"[{nameof(DebugGameSessionTracker)}] Session Ended call");
        public void Send() => Debug.Log($"[{nameof(DebugGameSessionTracker)}] Send call");
    }
}