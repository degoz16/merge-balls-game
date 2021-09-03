using System;
using System.Collections;
using GoogleMobileAds.Api;
using Internal.Scripts.Core.BaseClasses;
using UnityEngine;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class AdMobManager : MonoBehaviourSingleton<AdMobManager> {
        private InterstitialAd _interstitialAd;
        public bool IsAdClosed { get; private set; } = true;

        protected override void SingletonAwakened() {
            Initialize();
        }

        private void Initialize() {
            MobileAds.Initialize(status => { });
            _interstitialAd = new InterstitialAd("ca-app-pub-3601458819359442/7680657930");
            _interstitialAd.OnAdClosed += OnAdClosed;
        }

        private void OnAdClosed(object sender, EventArgs args) {
            IsAdClosed = true;
        }

        private IEnumerator StartCoroutineWithTimeOut(IEnumerator routine, float seconds) {
            StartCoroutine(routine);
            yield return new WaitForSecondsRealtime(seconds);
            StopCoroutine(routine);
        }

        private IEnumerator ShowInterstitialAdCoroutine() {
            if (_interstitialAd != null) {
                AdRequest request = new AdRequest.Builder().Build();
                _interstitialAd.LoadAd(request);
                yield return new WaitUntil(_interstitialAd.IsLoaded);
                _interstitialAd.Show();
            }
        }

        public void ShowInterstitialAd() {
            if (Application.internetReachability == NetworkReachability.NotReachable ||
                SettingsManager.Instance.IsAdsRemoved) return;
            if (!IsAdClosed) return;
            IsAdClosed = false;
            StartCoroutine(StartCoroutineWithTimeOut(ShowInterstitialAdCoroutine(), 3));
        }
    }
}