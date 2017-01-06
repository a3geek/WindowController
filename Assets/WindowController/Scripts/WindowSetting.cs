using UnityEngine;
using System.Collections;
using System;

namespace WindowController
{
    /// <summary>
    /// コマンドライン引数でWindowを設定する
    /// </summary>
    [AddComponentMenu("Window Controller/Window Setting")]
    public class WindowSetting : MonoBehaviour
    {
        /// <summary>
        /// コマンドライン引数のキー情報
        /// </summary>
        [Serializable]
        public class CommandLineKeys
        {
            /// <summary>X座標のキー</summary>
            public string posX = "--pos-x";
            /// <summary>Y座標のキー</summary>
            public string posY = "--pos-y";
            /// <summary>トップモストのキー</summary>
            public string topmost = "--topmost";
        }

        /// <summary>
        /// トップ処理の種類
        /// </summary>
        [Serializable]
        public enum MostType
        {
            Top = 1, Untop
        }

        /// <summary>
        /// トップ処理の繰り返しに関するパラメーター
        /// </summary>
        [Serializable]
        public class LoopSettingMost
        {
            /// <summary>有効化</summary>
            public bool Validity = true;
            /// <summary>処理同士の間隔</summary>
            public float Interval = 1.0f;
            /// <summary>トップ処理の種類</summary>
            public MostType MostType = MostType.Top;
        }

        /// <summary>コマンドライン引数のキー情報</summary>
        public CommandLineKeys keys = new CommandLineKeys();
        /// <summary>トップ処理の繰り返しのパラメーター</summary>
        public LoopSettingMost LoopMost = new LoopSettingMost();

        /// <summary>繰り返し処理用のタイマー</summary>
        private float timer = 0f;


        void Awake()
        {
#if !UNITY_EDITOR
            this.Setting();
#endif
        }

        void Update()
        {
            if(this.LoopMost.Validity == false)
            {
                return;
            }

            this.timer += Time.deltaTime;
            if(this.timer > this.LoopMost.Interval)
            {
                this.SetMost();
                this.timer = 0f;
            }
        }

        /// <summary>
        /// 引数を取得してWindowを設定する
        /// </summary>
        private void Setting()
        {
            if(Application.isEditor || Screen.fullScreen || Application.platform != RuntimePlatform.WindowsPlayer)
            {
                return;
            }

            int x = 0, y = 0;
            var moveX = CommandLineHelper.GetIntValue(this.keys.posX, out x);
            var moveY = CommandLineHelper.GetIntValue(this.keys.posY, out y);
            if(moveX || moveY)
            {
                WindowController.TryMoveWindow(x, y, 1000, 60);
            }

            var topmost = false;
            if(CommandLineHelper.GetBoolValue(this.keys.topmost, out topmost) && topmost)
            {
                WindowController.SetToTopMost();
            }
            else
            {
                this.LoopMost.Validity = false;
            }
        }

        /// <summary>
        /// トップ処理をする
        /// </summary>
        private void SetMost()
        {
            if(this.LoopMost.MostType == MostType.Top)
            {
                WindowController.SetToTopMost();
            }
            else if(this.LoopMost.MostType == MostType.Untop)
            {
                WindowController.SetToUntopMost();
            }
        }
    }
}
