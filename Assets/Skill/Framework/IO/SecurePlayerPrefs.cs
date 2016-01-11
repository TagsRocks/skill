using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
namespace Skill.Framework.IO
{
    public static class SecurePlayerPrefs
    {
        public static string Encrypt(string text, string password)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // create instance of the DES crypto provider
            var des = new DESCryptoServiceProvider();

            // generate a random IV will be used a salt value for generating key
            des.GenerateIV();

            // use derive bytes to generate a key from the password and IV
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, des.IV, 1000);

            // generate a key from the password provided
            byte[] key = rfc2898DeriveBytes.GetBytes(8);

            // encrypt the plainText
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(key, des.IV), CryptoStreamMode.Write))
            {
                // write the salt first not encrypted
                memoryStream.Write(des.IV, 0, des.IV.Length);

                // convert the plain text string into a byte array
                byte[] bytes = Encoding.UTF8.GetBytes(text);

                // write the bytes into the crypto stream so that they are encrypted bytes
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public static bool TryDecrypt(string cipherText, string password, out string text)
        {
            // its pointless trying to decrypt if the cipher text
            // or password has not been supplied
            if (string.IsNullOrEmpty(cipherText))
            {
                text = "";
                return false;
            }

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (var memoryStream = new MemoryStream(cipherBytes))
                {
                    // create instance of the DES crypto provider
                    var des = new DESCryptoServiceProvider();

                    // get the IV
                    byte[] iv = new byte[8];
                    memoryStream.Read(iv, 0, iv.Length);

                    // use derive bytes to generate key from password and IV
                    var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, iv, 1000);

                    byte[] key = rfc2898DeriveBytes.GetBytes(8);

                    using (var cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        text = streamReader.ReadToEnd();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception
                Console.WriteLine(ex);

                text = "";
                return false;
            }
        }

        public static void SetString(string key, string value, string password)
        {
            string hashedKey = Utility.GenerateMD5(key);
            string encryptedValue = Encrypt(value, password);
            PlayerPrefs.SetString(hashedKey, encryptedValue);

            string checksum = Utility.GenerateMD5(encryptedValue);
            PlayerPrefs.SetString(GetCheckSumKey(hashedKey), checksum);

            PlayerPrefs.Save();
        }

        public static string GetString(string key, string password)
        {
            string hashedKey = Utility.GenerateMD5(key);
            HasBeenEdited = false;
            if (PlayerPrefs.HasKey(hashedKey))
            {
                string encryptedValue = PlayerPrefs.GetString(hashedKey);

                string checkSumKey = GetCheckSumKey(hashedKey);
                if (!PlayerPrefs.HasKey(checkSumKey))
                    HasBeenEdited = true;
                else
                {
                    string checksumSaved = PlayerPrefs.GetString(checkSumKey);
                    string checksumReal = Utility.GenerateMD5(encryptedValue);
                    HasBeenEdited = !checksumSaved.Equals(checksumReal);
                }

                string decryptedValue;
                TryDecrypt(encryptedValue, password, out decryptedValue);
                return decryptedValue;
            }
            else
            {
                HasBeenEdited = true;
                return "";
            }
        }

        public static string GetString(string key, string defaultValue, string password)
        {
            if (HasKey(key))
            {
                return GetString(key, password);
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool HasKey(string key)
        {
            string hashedKey = Utility.GenerateMD5(key);
            bool hasKey = PlayerPrefs.HasKey(hashedKey);
            return hasKey;
        }

        private static string GetCheckSumKey(string key) { return "CHECKSUM" + key; }

        /// <summary> true if last loaded string modified </summary>
        public static bool HasBeenEdited { get; private set; }
    }
}