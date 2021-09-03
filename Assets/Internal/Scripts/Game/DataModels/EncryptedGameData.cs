using System;
using Internal.Scripts.Core.Utils.Cryptography;
using UnityEngine;

namespace Internal.Scripts.Game.DataModels {
    [Serializable]
    public class EncryptedGameData<T> where T : new() {
        [NonSerialized]
        private static readonly string Key = "(wl^sAzWP|~A4YK`}&v&Z*&J1:.2Ad]@Kmy|4V6nfXJqov]bN0eazyd/:d;iUbI";

        public string hash;
        public Cryptography.AesEncryptedText encryptedData;

        public EncryptedGameData(T gameSaveData) {
            var json = JsonUtility.ToJson(gameSaveData);
            hash = Cryptography.GetMd5Hash(json);
            encryptedData = Cryptography.Encrypt(json, Key);
        }

        public T GetGameSaveData() {
            string json = Cryptography.Decrypt(encryptedData, Key);
            // Debug.Log(json);
            return !hash.Equals(Cryptography.GetMd5Hash(json)) ? 
                new T() : JsonUtility.FromJson<T>(json);
        }
    }
}