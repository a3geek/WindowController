using UnityEngine;
using System.Collections;
using System;

namespace WindowController {
    [AddComponentMenu("Window Controller/Window Setting")]
    public class WindowSetting : MonoBehaviour {
        [Serializable]
        public enum MostType {
            Top = 1, Untop
        }

        [Serializable]
        public class CommandLineKeys {
            public string posX = "--pos-x";
            public string posY = "--pos-y";
            public string topmost = "--topmost";
        }

        [Serializable]
        public class LoopSettingMost {
            [HideInInspector]
            public bool Validity = true;
            public float Interval = 1.0f;
            public MostType MostType = MostType.Top;
        }
        
        public CommandLineKeys keys = new CommandLineKeys();
        public LoopSettingMost LoopMost = new LoopSettingMost();

        private float timer = 0f;
        private Coroutine coroutine = null;


        void Awake() {
#if !UNITY_EDITOR
            this.Setting();
#endif
        }

        void Update() {
            if(Input.GetKeyDown(KeyCode.Space)) {
                if(this.LoopMost.MostType == MostType.Top) {
                    this.LoopMost.MostType = MostType.Untop;
                }
                else if(this.LoopMost.MostType == MostType.Untop) {
                    this.LoopMost.MostType = MostType.Top;
                }
            }

            if(this.LoopMost.Validity == false) {
                return;
            }

            this.timer += Time.deltaTime;
            if(this.timer > this.LoopMost.Interval) {
                this.SetMost();
            }
        }

        void OnDestroy() {
            if(this.coroutine != null) {
                StopCoroutine(this.coroutine);
            }
        }

        private void Setting() {
            if(Application.isEditor || Screen.fullScreen || !(Application.platform == RuntimePlatform.WindowsPlayer)) { return; }

            int x = 0, y = 0;
            if(CommandLineHelper.GetIntValue(this.keys.posX, out x) || CommandLineHelper.GetIntValue(this.keys.posY, out y)) {
                WindowController.TryMoveWindow(x, y, 1000, 60);
            }

            var topmost = false;
            if(CommandLineHelper.GetBoolValue(this.keys.topmost, out topmost) && topmost) {
                WindowController.SetToTopMost();
            }
            else {
                this.LoopMost.Validity = false;
            }
        }

        private void SetMost() {
            if(this.LoopMost.MostType == MostType.Top) {
                WindowController.SetToTopMost();
            }
            else if(this.LoopMost.MostType == MostType.Untop) {
                WindowController.SetToUntopMost();
            }
        }
    }
}
