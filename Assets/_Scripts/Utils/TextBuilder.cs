using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hash17.Utils
{
    public static class TextBuilder
    {
        public static string WarningText(string text)
        {
            return BuildText(text, Alias.GameConfig.WarningMessageColor);
        }

        public static string ErrorText(string text)
        {
            return BuildText(text, Alias.GameConfig.ErrorMessageColor);
        }

        public static string MessageText(string text)
        {
            return BuildText(text, Alias.GameConfig.CommonMessageColor);
        }

        public static string BuildText(string text, Color color)
        {
            return string.Format("[{1}]{0}[-]", text, color.ToRGBHex());
        }

        public static string ToRGBHex(this Color color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}", ToByte(color.r), ToByte(color.g), ToByte(color.b));
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
    }
}
