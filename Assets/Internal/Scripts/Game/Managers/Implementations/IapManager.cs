using Internal.Scripts.Core.BaseClasses;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Internal.Scripts.Game.Managers.Implementations {
    public class IapManager : MonoBehaviourSingleton<IapManager>, IStoreListener {
        private IStoreController _storeController;

        public static void Init() {
            Instance.InternalInit();
        }

        private void InternalInit() {
            
        }
        
        protected override void SingletonStarted() {
            
        }

        public void OnInitializeFailed(InitializationFailureReason error) {
            
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) { }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            _storeController = controller;
            Debug.Log("SDADASDA");
        }

        public bool CheckIsAdRemoved() {
            Product product = _storeController.products.WithID("pool_noads");
            return false;
        }
    }
}