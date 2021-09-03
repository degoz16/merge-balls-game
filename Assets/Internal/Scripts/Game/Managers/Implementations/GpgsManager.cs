using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Implementations {
    public static class GpgsManager {
        public static bool IsInitialized { get; private set; }
        public static bool IsAuthenticated => 
            PlayGamesPlatform.Instance != null && PlayGamesPlatform.Instance.IsAuthenticated();

        public static void Initialize() {
            var config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames()
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = false;
            PlayGamesPlatform.Activate();
            IsInitialized = true;
        }

        public static void Authenticate(Action<bool> onAuth) {
            Social.localUser.Authenticate(onAuth);
        }

        public static void LogOut() {
            PlayGamesPlatform.Instance.SignOut();
            IsInitialized = false;
        }
    }
}