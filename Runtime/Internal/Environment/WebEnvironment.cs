using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class WebEnvironment : IEnvironment
    {
        private const string IdKey = "WS_PLAYER_ID";

        public Guid PlayerId { get; }
        public bool UseMetaverse { get; }

        public WebEnvironment(bool useMetaverse)
        {
            var id = CookieHandler.LoadData(IdKey);
            if (string.IsNullOrEmpty(id))
            {
                PlayerId = Guid.NewGuid();
                CookieHandler.SaveData(IdKey, PlayerId.ToString());
            }
            else
            {
                PlayerId = Guid.Parse(id);
            }
            UseMetaverse = useMetaverse;
        }
    }
}