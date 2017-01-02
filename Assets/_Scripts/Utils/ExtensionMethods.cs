using System;
using UnityEngine;
using System.Collections;

namespace Hash17.Utils
{
    public static class ExtensionMethods
    {
        public static string Encrypt(this string text, string password)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Cannot encrypt using an empty key. Please supply an encryption key.");
            }

            System.Security.Cryptography.CspParameters cspp = new System.Security.Cryptography.CspParameters();
            cspp.KeyContainerName = password;

            System.Security.Cryptography.RSACryptoServiceProvider rsa =
                new System.Security.Cryptography.RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            byte[] bytes = rsa.Encrypt(System.Text.UTF8Encoding.UTF8.GetBytes(text), true);

            return BitConverter.ToString(bytes);
        }

        public static string Decrypt(this string text, string password)
        {
            string result = null;

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Cannot decrypt using an empty key. Please supply a decryption key.");
            }

            try
            {
                System.Security.Cryptography.CspParameters cspp = new System.Security.Cryptography.CspParameters();
                cspp.KeyContainerName = password;

                System.Security.Cryptography.RSACryptoServiceProvider rsa =
                    new System.Security.Cryptography.RSACryptoServiceProvider(cspp);
                rsa.PersistKeyInCsp = true;

                string[] decryptArray = text.Split(new string[] {"-"}, StringSplitOptions.None);
                byte[] decryptByteArray = Array.ConvertAll<string, byte>(decryptArray,
                    (s => Convert.ToByte(byte.Parse(s, System.Globalization.NumberStyles.HexNumber))));


                byte[] bytes = rsa.Decrypt(decryptByteArray, true);

                result = System.Text.UTF8Encoding.UTF8.GetString(bytes);

            }
            finally
            {
                // no need for further processing
            }

            return result;
        }
    }
}