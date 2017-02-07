using Hash17.Utils;
using UnityEngine;

namespace Hash17.Data
{
    public class DataHolder : Singleton<DataHolder>
    {
        #region Properties

        #region References

        public GameConfiguration GameConfiguration;
        public TextAsset ProgramsSerializedData;
        public TextAsset DevicesSerializedData;
        public TextAsset CampaignItemsSerializedData;

        #endregion

        #endregion

        #region Bake

        public void Bake()
        {
            GameConfiguration = Resources.LoadAll<GameConfiguration>("")[0];
            ProgramsSerializedData = Resources.Load<TextAsset>(Alias.Config.CollectionLoadPath + "ProgramCollectionData");
            DevicesSerializedData = Resources.Load<TextAsset>(Alias.Config.CollectionLoadPath + "DeviceCollectionData");
            CampaignItemsSerializedData = Resources.Load<TextAsset>(Alias.Config.CollectionLoadPath + "CampaignItemsData");
        }

        #endregion
    }
}