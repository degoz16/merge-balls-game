using System;
using System.Collections;
using System.IO;
using Internal.Scripts.Core.BaseClasses;
using Internal.Scripts.Core.Utils.Coroutines;
using Internal.Scripts.Game.DataModels;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class SaveLoadManager : MonoBehaviourSingleton<SaveLoadManager> {
        private static string _path;
        private const string GameDataName = "save.json";
        private const string SettingsDataName = "settings.json";
        private static string _fullPathSettings;
        private static string _fullPathFieldSave;
        private GameFieldData _gameFieldData;
        private SettingsData _settingsData;
        private bool _isGameLoaded;
        private bool _isSettingsLoaded;
        private bool _isCloudChecked;

        protected override void SingletonAwakened() {
            _path = Application.persistentDataPath;
            _fullPathSettings = Path.Combine(_path, SettingsDataName);
            _fullPathFieldSave = Path.Combine(_path, GameDataName);
        }

        private IEnumerator TryCloudLoadCoroutine(string filename, Action<string> onDataLoaded) {
            yield return new WaitUntil(() => GpgsManager.IsAuthenticated);
            CloudSaveManager.LoadData(filename, onDataLoaded);
        }

        private void TryCloudLoad(string filename, Action<string> onDataLoaded) {
            CoroutineTask task = new CoroutineTask(TryCloudLoadCoroutine(filename, onDataLoaded), this);
            if (Application.internetReachability != NetworkReachability.NotReachable)
                task.StartWithTimeout(120f, () => onDataLoaded(null));
            else {
                onDataLoaded(null);
            }
        }
        
        public void LoadGameFieldData(Action<GameFieldData> onDataLoaded) {
            if (!File.Exists(_fullPathFieldSave)) {
                _gameFieldData = new GameFieldData();
            } else if (_gameFieldData == null) {
                var encryptedGameData =
                    JsonUtility.FromJson<EncryptedGameData<GameFieldData>>(File.ReadAllText(_fullPathFieldSave));
                _gameFieldData = encryptedGameData.GetGameSaveData();
            }
            
            if (!_isGameLoaded) {
                TryCloudLoad("highscore", cloudSaveString => {
                    if (GpgsManager.IsAuthenticated)
                        _isCloudChecked = true;
                    if (cloudSaveString == null) return;
                    var cloudSave = long.Parse(cloudSaveString);
                    if (cloudSave > GlobalStateManager.Instance.HighScore) {
                        GlobalStateManager.Instance.HighScore = cloudSave;
                    }
                    _isCloudChecked = true;
                });
            }
            _isGameLoaded = true;
            onDataLoaded(_gameFieldData);
        }

        private IEnumerator SaveGameDataCoroutine(bool afterLoad) {
            if (afterLoad && !_isGameLoaded) yield break;
            _gameFieldData = new GameFieldData();
            var encryptedGameData = new EncryptedGameData<GameFieldData>(_gameFieldData);
            var json = JsonUtility.ToJson(encryptedGameData);
            File.WriteAllText(_fullPathFieldSave, json);
            if (_isCloudChecked) {
                CloudSaveManager.SaveData("highscore", _gameFieldData.highScore.ToString());
            }
        }
        
        public void SaveGameData(bool afterLoad = true) {
            StartCoroutine(SaveGameDataCoroutine(afterLoad));
        }
        
        public void LoadSettingsData(Action<SettingsData> onDataLoaded) {
            if (!File.Exists(_fullPathSettings)) {
                _settingsData = new SettingsData();
            } else {
                var encryptedGameData =
                    JsonUtility.FromJson<EncryptedGameData<SettingsData>>(File.ReadAllText(_fullPathSettings));
                _settingsData = encryptedGameData.GetGameSaveData();
            }
            
            _isSettingsLoaded = true;
            onDataLoaded(_settingsData);
        }
        
        private IEnumerator SaveSettingsCoroutine(bool afterLoad) {
            if (afterLoad && !_isSettingsLoaded) yield break;
            _settingsData = new SettingsData();
            var encryptedGameData = new EncryptedGameData<SettingsData>(_settingsData);
            var json = JsonUtility.ToJson(encryptedGameData);
            File.WriteAllText(_fullPathSettings, json);
        }
        
        public void SaveSettingsData(bool afterLoad = true) {
            StartCoroutine(SaveSettingsCoroutine(afterLoad));
        }
    }
}