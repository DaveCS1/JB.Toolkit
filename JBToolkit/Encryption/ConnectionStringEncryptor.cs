using System;
using System.Collections.Generic;
using System.Linq;

namespace JBToolkit.Encryption
{
    /// <summary>
    /// Very simple connection string encryptor. Useful to hide the connection string from ordinary service and not have any invalid XML characters, while also being sharable
    /// or to be easily decrypted if needs be
    /// </summary>
    public class ConnectionStringEncryptor
    {
        // for encrypted connection strings

        private const string m_decryptionDic = "¬ @	ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_!\"#$%&'()*+,-./`abcdefghijklmno0123456789:;<=>?pqrstuvwxyz{|}~£";
        private const string m_encryptionDic = "35FDFB6FEBDBCBBBAB9B8B7B6B5B4B3B2B1B0BFAEADACABAAA9A8A7A6A5A4A3A2A1A0AEDDDCDBDAD9D8D7D6D5D4D3D2D1D0DF9E9D9C9B9A999897969594939291909FCECDCCCBCAC9C8C7C6C5C4C3C2C1C0CF8E8D8C8B8A898887868584838281808C5";

        /// <summary>
        /// Generates a 3D list to match between encrypted character and actual string
        /// </summary>
        /// <returns>Dictionary list</returns>
        private static List<List<string>> GetEncryptionDictionary()
        {
            var encryptionDictionary = new List<List<string>>();

            var dDic = new List<string>();
            dDic.AddRange(m_decryptionDic.Select(c => c.ToString()));

            var eDic = new List<string>();

            for (int i = 0; i < m_encryptionDic.Length; i += 2)
                eDic.Add(Convert.ToString(m_encryptionDic[i]) + Convert.ToString(m_encryptionDic[i + 1]));

            encryptionDictionary.Add(dDic);
            encryptionDictionary.Add(eDic);

            return encryptionDictionary;
        }

        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="input">String to be encrypted</param>
        /// <returns>Encrypted string</returns>
        public static string EncryptString(string input)
        {
            var currentChar = string.Empty;

            try
            {
                var encryptionDictionary = GetEncryptionDictionary();
                var encrypedString = string.Empty;

                for (int i = 0; i < input.Length; i++)
                {
                    currentChar = input[i].ToString();

                    int eIndex = encryptionDictionary[0].IndexOf(input[i].ToString());
                    encrypedString += encryptionDictionary[1][eIndex];
                }

                return encrypedString;
            }
            catch (Exception err)
            {
                throw new ApplicationException(string.Format("Error 'Connection String Encyrpt': Cannot encrypt: '{0}'. Message: {1} ", currentChar, err.Message));
            }
        }

        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="encryptedInput">An encrypted string</param>
        /// <returns>Encrypted string</returns>
        public static string DecryptString(string encryptedInput)
        {
            var currentEbase = string.Empty;

            try
            {
                var encryptionDictionary = GetEncryptionDictionary();
                var decryptedString = string.Empty;

                var eBase = new List<string>();

                for (int i = 0; i < encryptedInput.Length; i += 2)
                    eBase.Add(Convert.ToString(encryptedInput[i]) + Convert.ToString(encryptedInput[i + 1]));

                for (int i = 0; i < eBase.Count; i++)
                {
                    currentEbase = eBase[i];

                    int dIndex = encryptionDictionary[1].IndexOf(eBase[i]);
                    decryptedString += encryptionDictionary[0][dIndex];
                }

                return decryptedString;
            }
            catch (Exception err)
            {
                throw new ApplicationException(string.Format("Error 'Connection String Decrypt': Cannot decrypt: '{0}'. Message: {1} ", currentEbase, err.Message));
            }
        }

        /// <summary>
        /// Tests whether a string is already encrypted or not. Quite simply, it runs it through the encrytion process. If it works it wasn't encrypted
        /// </summary>
        /// <param name="input">String to test if it's encrypted or not</param>
        /// <returns>True is encrypted, false otherwise</returns>
        public static bool IsEncrypted(string input)
        {
            try
            {
                string s = DecryptString(input);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
