namespace WelwiseGamesSDK.Internal.GameSaves
{
    internal interface ISaveParser
    {
        public void DeserializeJsonToContainer(string json, DataContainer container);
        public string SerializeContainerToJson(DataContainer container);
    }
}