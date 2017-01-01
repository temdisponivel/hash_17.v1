using FH.DataRetrieving;
using UnityEngine;

namespace Hash17.Utils
{
    [CreateAssetMenu()]
    public class Hash17DataRetriever : DataRetrieverBase
    {
        #region Programs

        public string ProgramsSpreadSheetId;

        #endregion

        #region Devices

        public string DevicesSpreadSheetId;

        #endregion

        #region Text assets

        public string TextAssetsSpreadSheetId;

        #endregion
    }
}