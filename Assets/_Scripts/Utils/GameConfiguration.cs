using UnityEngine;
using System.Collections;

namespace Hash17.Utils
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Hash17/GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        public string OwnedDeviceId;
        public string VoxPopuliServer;

        #region Text

        public UIFont TextFont;
        public Color TextColor;
        public int TextSize;

        #endregion

        #region Colors

        public Color FileColor;
        public Color LockedFileColor;
        public Color SecureFileColor;
        public Color DirectoryColor;

        public Color WarningMessageColor;
        public Color ErrorMessageColor;
        public Color CommonMessageColor;

        #endregion

        #region Window

        public GameObject WindowPrefab;

        #endregion
    }

}