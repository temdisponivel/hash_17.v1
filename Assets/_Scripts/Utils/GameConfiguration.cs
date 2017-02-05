using UnityEngine;
using System.Collections;

namespace Hash17.Utils
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Hash17/GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        public string OwnedDeviceId;
        public string VoxPopuliServer;

        public string CollectionsSavePath;
        public string CollectionLoadPath;

        #region Text

        public UIFont TextFont;
        public Color TextColor;
        public int TextSize;
        public string CarrotChar;
        public int MaxEntriesCount;
        public int EntriesCountToRemoveWhenMaxed;

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