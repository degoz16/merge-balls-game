using System;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

namespace Internal.Scripts.Game.Managers.Implementations {
    public static class CloudSaveManager {
        private static ISavedGameClient _savedGameClient;
        private static ISavedGameMetadata _currentMetadata;
        private static DateTime _startDateTime;

        public static void Initialize() {
            _savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            _startDateTime = DateTime.Now;
        }

        private static void OpenSaveData(string fileName,
            Action<SavedGameRequestStatus, ISavedGameMetadata> onDataOpen) {
            if (!GpgsManager.IsAuthenticated) {
                onDataOpen(SavedGameRequestStatus.AuthenticationError, null);
                return;
            }
            _savedGameClient.OpenWithAutomaticConflictResolution(fileName,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                onDataOpen);
        }

        private static void ReadSaveData(string fileName, Action<SavedGameRequestStatus, byte[]> onDataRead) {
            if (!GpgsManager.IsAuthenticated) {
                onDataRead(SavedGameRequestStatus.AuthenticationError, null);
                return;
            }
            OpenSaveData(fileName, (status, metadata) => {
                if (status == SavedGameRequestStatus.Success) {
                    _savedGameClient.ReadBinaryData(metadata, onDataRead);
                    _currentMetadata = metadata;
                }
            });
        }

        public static void SaveData(string fileName, string stringData) {
            if (!GpgsManager.IsAuthenticated || stringData == null || stringData.Length == 0)
                return;
            var data = Encoding.ASCII.GetBytes(stringData);
            TimeSpan currentSpan = DateTime.Now - _startDateTime;
            Action onDataWrite = () => {
                TimeSpan totalPlayTime = _currentMetadata.TotalTimePlayed + currentSpan;
                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
                    .WithUpdatedDescription("Saved game at " + DateTime.Now)
                    .WithUpdatedPlayedTime(totalPlayTime);
                SavedGameMetadataUpdate updatedMetadata = builder.Build();
                _savedGameClient.CommitUpdate(_currentMetadata,
                    updatedMetadata,
                    data,
                    (status, metadata) => _currentMetadata = metadata);
                _startDateTime = DateTime.Now;
            };
            if (_currentMetadata == null) {
                OpenSaveData(fileName, (status, metadata) => {
                    if (status == SavedGameRequestStatus.Success) {
                        _currentMetadata = metadata;
                        onDataWrite();
                    }
                });
                return;
            }
            onDataWrite();
        }

        public static void LoadData(string filename, Action<string> onDataLoaded) {
            ReadSaveData(filename, (status, bytes) => {
                onDataLoaded(status != SavedGameRequestStatus.Success ? null : Encoding.ASCII.GetString(bytes));
            });
        }
    }
}