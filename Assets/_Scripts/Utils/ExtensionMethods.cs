using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using DarkTonic.MasterAudio;
using Hash17.MockSystem;

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

            return String.Format("This file is encrypted. The following is its data encrypted: \n {0}", BitConverter.ToString(Encoding.ASCII.GetBytes(text)));


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

        #region Highlight

        public static string HighlightTerms(this string text, params string[] terms)
        {
            var builder = new StringBuilder(text);
            for (int i = 0; i < terms.Length; i++)
            {
                builder.Replace(terms[i], string.Format("[b][i]{0}[/i][/b]", terms[i])); ;
            }
            return builder.ToString();
        }

        #endregion

        #region Input

        public static string ClearInput(this string text)
        {
            return text.Trim().Replace("\n", string.Empty);
        }

        #endregion

        #region System Variable

        public static string HandleSystemVariables(this string text)
        {
            if (!text.Contains(Alias.Config.CharToConsiderSystemVariable))
                return text;

            var toReplace = new Dictionary<string, string>();
            var occurencies = text.MultipleIndexOf(Alias.Config.CharToConsiderSystemVariable, StringComparison.OrdinalIgnoreCase);
            for (int i = 0; i < occurencies.Count - 1; i++, i++)
            {
                var startIndex = occurencies[i];
                var endIndex = occurencies[i + 1];

                var variable = text.Substring(startIndex + 1, endIndex - startIndex - 1);

                if (Alias.SysVariables.ContainsKey(variable))
                {
                    string value;
                    if (Alias.SysVariables.TryGetValue(variable, out value))
                        toReplace[variable] = value.ColorizeSystemVariable(variable);
                }
            }

            if (toReplace.Count > 0)
            {
                var builder = new StringBuilder(text);
                foreach (var entry in toReplace)
                {
                    builder.Replace("{0}{1}{0}".InLineFormat(Alias.Config.CharToConsiderSystemVariable, entry.Key), entry.Value);
                }
                text = builder.ToString();
            }

            return text;
        }

        public static string ColorizeSystemVariable(this string text, string type)
        {
            switch (type)
            {
                case SystemVariables.USERNAME:
                    return TextBuilder.BuildText(text, Alias.Config.UserNameColor);
            }

            return text;
        }

        #endregion

        #region Color

        public static string Colorize(this string text, Color color)
        {
            return string.Format("[{1}]{0}[-]", text, color.ToRGBHex());
        }

        #endregion

        #region Timing

        public static List<Tuple<string, float>> GetStringAndTime(this string text)
        {
            const int lookingForTime = 0;
            const int readingTime = 1;


            var currentState = lookingForTime;
            var builder = new StringBuilder();
            var currentTuple = new Tuple<string, float>();
            var result = new List<Tuple<string, float>>();
            for (int i = 0; i < text.Length; i++)
            {
                var currentChar = text[i];

                if (currentState == lookingForTime)
                {
                    if (currentChar == Alias.Config.CharToConsiderTime)
                    {
                        currentTuple.Key = builder.ToString();
                        builder.Remove(0, builder.Length);
                        currentState = readingTime;
                    }
                    else
                    {
                        builder.Append(currentChar);
                    }
                }
                else if (currentState == readingTime)
                {
                    if (currentChar == Alias.Config.CharToConsiderTime)
                    {
                        float time;
                        if (float.TryParse(builder.ToString(), out time))
                        {
                            currentState = lookingForTime;
                            builder.Remove(0, builder.Length);
                            currentTuple.Value = time;
                            result.Add(currentTuple);

                            currentTuple = new Tuple<string, float>();
                        }
                        else
                        {
                            Debug.LogError("INVALID TIMED STRING AT {0}".InLineFormat(i));
                        }
                    }
                    else
                    {
                        builder.Append(currentChar);
                    }
                }
            }

            result.Add(new Tuple<string, float>()
            {
                Key = builder.ToString(),
                Value = 0,
            });

            return result;
        }

        #endregion

        #endregion

        #region NGUI

        #region Label

        public static void SetupWithHash17Settings(this UILabel label)
        {
            label.fontSize = Alias.Config.TextSize;
            label.color = Alias.Config.TextColor;
            label.bitmapFont = Alias.Config.TextFont;
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
            widget.width = (int)size.x;
            widget.height = (int)size.y;
        }

        #endregion

        #endregion
    }
}