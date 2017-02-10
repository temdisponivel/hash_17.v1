using UnityEngine;
using System.Collections;

namespace Hash17.Utils
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Hash17/GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        public string CollectionsSavePath;
        public string CollectionLoadPath;

        public string CampaignSavePath;

        public int GameStartCampaignItemReward;

        #region Text

        public UIFont TextFont;
        public Color TextColor;
        public int TextSize;
        public string CarrotChar;
        public int MaxEntriesCount;
        public int EntriesCountToRemoveWhenMaxed;
        public string CharToConsiderSystemVariable;
        public char CharToConsiderTime;

        #endregion

        #region Colors

        public Color FileColor;
        public Color LockedFileColor;
        public Color SecureFileColor;
        public Color DirectoryColor;

        public Color ProgramColor;

        public Color DeviceIdColor;
        public Color UserNameColor;

        public Color WarningMessageColor;
        public Color ErrorMessageColor;
        public Color CommonMessageColor;

        #endregion

        #region Window

        public GameObject WindowPrefab;

        #endregion
    }

}