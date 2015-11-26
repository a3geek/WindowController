using UnityEngine;
using System.Collections;
using System;


namespace WindowController {
    public class WindowSetting : MonoBehaviour {
        [Serializable]
        public class CommandLineKeys {
            public string posX = "--pos-x";
            public string posY = "--pos-y";
            public string topmost = "--topmost";
        }
        
        public CommandLineKeys keys = new CommandLineKeys();


        void Awake() {
#if !UNITY_EDITOR
            this.Setting();
#endif
        }

        private void Setting() {
            if(Application.isEditor || !Screen.fullScreen) { return; }

            int x = 0, y = 0;
            CommandLineHelper.GetIntValue(this.keys.posX, out x);
            CommandLineHelper.GetIntValue(this.keys.posY, out y);
            WindowController.TryMoveWindow(x, y, 1000, 60);

            var topmost = false;
            if(CommandLineHelper.GetBoolValue(this.keys.topmost, out topmost) && topmost) {
                WindowController.SetToTopMost();
            }
        }
    }
}
