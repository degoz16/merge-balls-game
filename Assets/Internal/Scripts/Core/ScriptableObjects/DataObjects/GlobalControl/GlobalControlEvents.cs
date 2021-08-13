using System;

namespace Internal.Scripts.Core.ScriptableObjects.DataObjects.GlobalControl {
    public class StartEventArgs : EventArgs {
        public bool IsNewGame { get; set; } = false;
    }
    
    public partial class GlobalControl {
        public static event EventHandler GamePausedEvent = delegate {  };
        public static event EventHandler GameOverEvent = delegate {  };
        public static event EventHandler GameUnpausedEvent = delegate {  };
        public static event EventHandler<StartEventArgs> GameStartEvent = delegate {  };
    }
}