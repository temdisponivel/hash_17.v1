using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;

namespace Hash17.Utils
{
    public static class ExtensionMethods
    {
        #region String

        #region Encrypting

        public static string Encrypt(this string text, string password)
        {
            if (String.IsNullOrEmpty(text))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Cannot encrypt using an empty key. Please supply an encryption key.");
            }

            return String.Format("Encrypted data: \n {0}", BitConverter.ToString(Encoding.ASCII.GetBytes(text)));


            CspParameters cspp = new CspParameters();
            cspp.KeyContainerName = password;

            RSACryptoServiceProvider rsa =
                new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            byte[] bytes = rsa.Encrypt(UTF8Encoding.UTF8.GetBytes(text), true);

            return BitConverter.ToString(bytes);
        }

        public static string Decrypt(this string text, string password)
        {
            string result = null;

            if (String.IsNullOrEmpty(text))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Cannot decrypt using an empty key. Please supply a decryption key.");
            }

            try
            {
                CspParameters cspp = new CspParameters();
                cspp.KeyContainerName = password;

                RSACryptoServiceProvider rsa =
                    new RSACryptoServiceProvider(cspp);
                rsa.PersistKeyInCsp = true;

                string[] decryptArray = text.Split(new string[] { "-" }, StringSplitOptions.None);
                byte[] decryptByteArray = Array.ConvertAll<string, byte>(decryptArray,
                    (s => Convert.ToByte(Byte.Parse(s, NumberStyles.HexNumber))));


                byte[] bytes = rsa.Decrypt(decryptByteArray, true);

                result = UTF8Encoding.UTF8.GetString(bytes);

            }
            finally
            {
                // no need for further processing
            }

            return result;
        }

        #endregion

        #region Index of

        public static List<int> MultipleIndexOf(this string text, string toFind, StringComparison comparsion)
        {
            var result = new List<int>();
            int index = 0;
            while (true)
            {
                var indexOf = text.IndexOf(toFind, index, comparsion);
                if (indexOf != -1)
                    result.Add(indexOf);
                else
                    break;
                index = indexOf + 1;
            }
            return result;
        }

        #endregion

        #region Sub string

        public static string SubString(this string text, int quantBeforeGarantee, int quantAfterGarantee, params string[] garanteeInResult)
        {
            StringBuilder result = new StringBuilder();

            // look for the query inside text
            Dictionary<string, List<int>> indexOfs = new Dictionary<string, List<int>>();
            for (int i = 0; i < garanteeInResult.Length; i++)
            {
                var current = garanteeInResult[i];

                if (String.IsNullOrEmpty(current))
                    continue;
                
                indexOfs[current] = text.MultipleIndexOf(current, StringComparison.OrdinalIgnoreCase);
            }

            // order the indexes so we have always accending indexes
            var orderedIndexes = indexOfs.OrderBy(entry =>
            {
                return entry.Value;
            }).ToList();
            
            var maxIndexReached = 0;
            for (int i = 0; i < orderedIndexes.Count; i++)
            {
                var currentGaranteed = orderedIndexes[i];
                var indexes = currentGaranteed.Value;
                for (int j = 0; j < indexes.Count; j++)
                {
                    var index = indexes[j];

                    // get index to start substring - clampded to first character
                    var indexToStart = Mathf.Max(0, index - quantBeforeGarantee);

                    // get quantity of the substring
                    var quantity = quantBeforeGarantee + quantAfterGarantee + currentGaranteed.Key.Length;

                    // if we already added this index
                    if (indexToStart <= maxIndexReached)
                    {
                        // if this substring ends inside what we've already added, continue
                        if (indexToStart + quantity <= maxIndexReached)
                            continue;

                        // get quantity left from start index and the max index we have added to result
                        quantity = (indexToStart + quantity) - maxIndexReached;
                        indexToStart = maxIndexReached;
                    }

                    // clamp quantity to always be inside text
                    var endIndex = indexToStart + quantity;
                    if (endIndex > text.Length)
                    {
                        quantity = text.Length - indexToStart;
                    }

                    // add dots before or after if necessary
                    var before = (indexToStart > 0 ? "..." : "");
                    var after = (endIndex < text.Length ? "..." : "");
                    result.Append(String.Format(before + "{0}" + after, text.Substring(indexToStart, quantity)));
                    result.Append(";");
                    maxIndexReached = endIndex;
                }
            }

            return result.ToString();
        }

        public static string InLineFormat(this string format, params object[] args)
        {
            return String.Format(format, args);
        }

        #endregion

        #endregion

        #region NGUI

        #region Label

        public static void SetupWithHash17Settings(this UILabel label)
        {
            label.fontSize = Alias.GameConfig.TextSize;
            label.color = Alias.GameConfig.TextColor;
            label.bitmapFont = Alias.GameConfig.TextFont;
        }

        #endregion

        #region Anchor Point

        public static UIRect.AnchorPoint Clone(this UIRect.AnchorPoint anchor)
        {
            var result = new UIRect.AnchorPoint();
            result.target = anchor.target;
            result.relative = anchor.relative;
            result.absolute = anchor.absolute;
            return result;
        }

        #endregion

        #region Widget

        public static Vector2 Size(this UIWidget widget)
        {
            return new Vector2(widget.width, widget.height);
        }

        public static void Size(this UIWidget widget, Vector2 size)
        {
            widget.width = (int) size.x;
            widget.height = (int) size.y;
        }

        #endregion

        #endregion
    }
}