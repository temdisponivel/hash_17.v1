using FH.DataRetrieving;
using UnityEngine;

namespace Hash17.Utils
{
    [CreateAssetMenu(fileName = "Hash17DataRetriever", menuName = "Hash17/Data retriever")]
    public class Hash17DataRetriever : DataRetrieverBase
    {
        #region ProgramsByCommand

        public string ProgramsSpreadSheetId;

        #endregion

        #region Devices

        public string DevicesSpreadSheetId;

        #endregion

        #region Text assets

        public string TextAssetsSpreadSheetId;

        #endregion

        #region Campaign

        public string CampaignItemsSpreedSheetId;
        public string DefaultCampaignInfoSpreedSheetId;

        #endregion
    }
}