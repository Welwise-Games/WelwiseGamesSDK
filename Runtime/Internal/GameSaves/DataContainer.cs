using System.Collections.Generic;

namespace WelwiseGamesSDK.Internal.GameSaves
{
    internal sealed class DataContainer
    {
        public readonly Dictionary<string, float> Floats = new ();
        public readonly Dictionary<string, string> Strings = new ();
        public readonly Dictionary<string, int > Ints = new ();
        public readonly Dictionary<string, bool> Booleans = new ();
        public string PlayerName { get;  set; } = null;

        public void Clear()
        {
            Floats.Clear();
            Strings.Clear();
            Ints.Clear();
            Booleans.Clear();
            PlayerName = null;
        }
    }
}