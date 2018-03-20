// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR || UNITY_STANDALONE
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
#endif

namespace PixelCrushers
{

    /// <summary>
    /// Implements SavedGameDataStorer using local disk files.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class DiskSavedGameDataStorer : SavedGameDataStorer
    {

#if UNITY_EDITOR || UNITY_STANDALONE

        [Tooltip("Encrypt saved game files.")]
        public bool encrypt = true;

        [Tooltip("If encrypting, use this password.")]
        public string encryptionPassword = "My Password";

        [Tooltip("Log debug info.")]
        [SerializeField]
        private bool m_debug;

        private class SavedGameInfo
        {
            public string sceneName;

            public SavedGameInfo(string sceneName)
            {
                this.sceneName = sceneName;
            }
        }

        private List<SavedGameInfo> m_savedGameInfo = new List<SavedGameInfo>();

        public bool debug
        {
            get { return m_debug && Debug.isDebugBuild; }
        }

        public void Start()
        {
            LoadSavedGameInfoFromFile();
        }

        public string GetSaveGameFilename(int slotNumber)
        {
            return Application.persistentDataPath + "/save_" + slotNumber + ".dat";
        }

        public string GetSavedGameInfoFilename()
        {
            return Application.persistentDataPath + "/saveinfo.dat";
        }

        public void LoadSavedGameInfoFromFile()
        {
            var filename = GetSavedGameInfoFilename();
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename)) return;
            if (debug) Debug.Log("Save System: DiskSavedGameDataStorer loading " + filename);
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    m_savedGameInfo.Clear();
                    int safeguard = 0;
                    while (!streamReader.EndOfStream && safeguard < 999)
                    {
                        var sceneName = streamReader.ReadLine().Replace("<cr>", "\n");
                        m_savedGameInfo.Add(new SavedGameInfo(sceneName));
                        safeguard++;
                    }
                }
            }
            catch (System.Exception)
            {
                Debug.Log("Save System: DiskSavedGameDataStorer - Error reading file: " + filename);
            }
        }

        public void UpdateSavedGameInfoToFile(int slotNumber, SavedGameData savedGameData)
        {
            var slotIndex = slotNumber - 1;
            for (int i = m_savedGameInfo.Count; i <= slotIndex; i++)
            {
                m_savedGameInfo.Add(new SavedGameInfo(string.Empty));
            }
            m_savedGameInfo[slotIndex].sceneName = (savedGameData != null) ? savedGameData.sceneName : string.Empty;
            var filename = GetSavedGameInfoFilename();
            if (debug) Debug.Log("Save System: DiskSavedGameDataStorer updating " + filename);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename))
                {
                    for (int i = 0; i < m_savedGameInfo.Count; i++)
                    {
                        streamWriter.WriteLine(m_savedGameInfo[i].sceneName.Replace("\n", "<cr>"));
                    }
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("Save System: DiskSavedGameDataStorer - Can't create file: " + filename);
            }
        }

        public override bool HasDataInSlot(int slotNumber)
        {
            var slotIndex = slotNumber - 1;
            return 0 <= slotIndex && slotIndex < m_savedGameInfo.Count && !string.IsNullOrEmpty(m_savedGameInfo[slotIndex].sceneName);
        }

        public override void StoreSavedGameData(int slotNumber, SavedGameData savedGameData)
        {
            var s = SaveSystem.Serialize(savedGameData);
            if (debug) Debug.Log("Save System: DiskSavedGameDataStorer - Saving " + GetSaveGameFilename(slotNumber) + ": " + s);
            WriteStringToFile(GetSaveGameFilename(slotNumber), encrypt ? Encrypt(s, encryptionPassword) : s);
            UpdateSavedGameInfoToFile(slotNumber, savedGameData);
        }

        public override SavedGameData RetrieveSavedGameData(int slotNumber)
        {
            var s = ReadStringFromFile(GetSaveGameFilename(slotNumber));
            if (encrypt)
            {
                string plainText;
                s = TryDecrypt(s, encryptionPassword, out plainText) ? plainText : string.Empty;
            }
            if (debug) Debug.Log("Save System: DiskSavedGameDataStorer - Loading " + GetSaveGameFilename(slotNumber) + ": " + s);
            return SaveSystem.Deserialize<SavedGameData>(s);
        }

        public override void DeleteSavedGameData(int slotNumber)
        {
            try
            {
                var filename = GetSaveGameFilename(slotNumber);
                if (File.Exists(filename)) File.Delete(filename);
            }
            catch (System.Exception)
            {
            }
            UpdateSavedGameInfoToFile(slotNumber, null);
        }

        public static void WriteStringToFile(string filename, string data)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename))
                {
                    streamWriter.WriteLine(data);
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("Save System: Can't create file: " + filename);
            }
        }

        public static string ReadStringFromFile(string filename)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (System.Exception)
            {
                Debug.Log("Save System: Error reading file: " + filename);
                return string.Empty;
            }
        }

        // From: https://developingsoftware.com/how-to-securely-store-data-in-unity-player-preferences

        const int Iterations = 1000;

        public string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(password)) return string.Empty;

            // create instance of the DES crypto provider
            var des = new DESCryptoServiceProvider();

            // generate a random IV will be used a salt value for generating key
            des.GenerateIV();

            // use derive bytes to generate a key from the password and IV
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, des.IV, Iterations);

            // generate a key from the password provided
            byte[] key = rfc2898DeriveBytes.GetBytes(8);

            // encrypt the plainText
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(key, des.IV), CryptoStreamMode.Write))
            {
                // write the salt first not encrypted
                memoryStream.Write(des.IV, 0, des.IV.Length);

                // convert the plain text string into a byte array
                byte[] bytes = Encoding.UTF8.GetBytes(plainText);

                // write the bytes into the crypto stream so that they are encrypted bytes
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public bool TryDecrypt(string cipherText, string password, out string plainText)
        {
            // its pointless trying to decrypt if the cipher text
            // or password has not been supplied
            if (string.IsNullOrEmpty(cipherText) ||
                string.IsNullOrEmpty(password))
            {
                plainText = string.Empty;
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
                    var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, iv, Iterations);

                    byte[] key = rfc2898DeriveBytes.GetBytes(8);

                    using (var cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        plainText = streamReader.ReadToEnd();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Dialogue System Menus: Can't decrypt data: + " + ex.Message);
                plainText = string.Empty;
                return false;
            }
        }

#else
        void Start()
        {
            Debug.LogError("DiskSavedGameDataStorer is currently only supported in Standalone (desktop) builds.");
        }

        public override bool HasDataInSlot(int slotNumber)
        {
            Debug.LogError("DiskSavedGameDataStorer is currently only supported in Standalone (desktop) builds.");
            return false;
        }

        public override SavedGameData RetrieveSavedGameData(int slotNumber)
        {
            Debug.LogError("DiskSavedGameDataStorer is currently only supported in Standalone (desktop) builds.");
            return null;
        }

        public override void StoreSavedGameData(int slotNumber, SavedGameData savedGameData)
        {
            Debug.LogError("DiskSavedGameDataStorer is currently only supported in Standalone (desktop) builds.");
        }

        public override void DeleteSavedGameData(int slotNumber)
        {
            Debug.LogError("DiskSavedGameDataStorer is currently only supported in Standalone (desktop) builds.");
        }

#endif

    }

}
