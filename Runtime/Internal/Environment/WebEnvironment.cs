using System;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class WebEnvironment : IEnvironment
    {
        public Guid PlayerId { get; }
        public bool UseMetaverse { get; }

        public WebEnvironment(bool useMetaverse)
        {
            var id = CookieHandler.LoadData(WelwiseSDK.IdKey);
            if (string.IsNullOrEmpty(id))
            {
                PlayerId = Guid.NewGuid();
                CookieHandler.SaveData(WelwiseSDK.IdKey, PlayerId.ToString());
            }
            else
            {
                PlayerId = Guid.Parse(id);
            }
            UseMetaverse = useMetaverse;
        }
    }
}