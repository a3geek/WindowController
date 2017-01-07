using UnityEngine;
using System;

namespace WindowController
{
    /// <summary>
    /// トップ処理の種類
    /// </summary>
    [Serializable]
    public enum MostType
    {
        Top = 1, Untop
    }

    /// <summary>
    /// コマンドライン引数でWindowを設定する
    /// </summary>
    [AddComponentMenu("Window Controller/Window Controller")]
    public class WindowController : MonoBehaviour
    {
        #region "Singleton"
        /// <summary>シングルトンインスタンス</summary>
        public static WindowController Instance {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<WindowController>();
                }

                return instance;
            }
        }
        /// <summary>インスタンス</summary>
        private static WindowController instance = null;
        #endregion

        /// <summary>動作環境の確認</summary>
        public bool IsValidity
        {
            get { return !(Application.isEditor || Screen.fullScreen || Application.platform != RuntimePlatform.WindowsPlayer); }
        }
        /// <summary>一定間隔毎のトップ化処理の有効化</summary>
        public bool LoopMostValidity
        {
            get { return this.loopMost.Validity; }
            set { this.loopMost.Validity = value; }
        }
        /// <summary>トップ化処理の種類</summary>
        public MostType MostType
        {
            get { return this.loopMost.MostType; }
            set { this.loopMost.MostType = value; }
        }
        
        /// <summary>コマンドライン引数のキー情報</summary>
        [SerializeField]
        private CommandLineKeys keys = new CommandLineKeys();
        /// <summary>トップ処理の繰り返しのパラメーター</summary>
        [SerializeField]
        private LoopSettingMost loopMost = new LoopSettingMost();

        /// <summary>繰り返し処理用のタイマー</summary>
        private float timer = 0f;


        void Awake()
        {
            instance = this;
            if(this.IsValidity)
            {
                this.Setting();
            }
        }

        void Update()
        {
            if(this.IsValidity == false || this.loopMost.Validity == false)
            {
                return;
            }

            this.timer += Time.deltaTime;
            if(this.timer > this.loopMost.Interval)
            {
                this.timer = 0f;
                this.SetMost();
            }
        }

        /// <summary>
        /// 引数を取得してWindowを設定する
        /// </summary>
        private void Setting()
        {
            if(this.IsValidity == false)
            {
                return;
            }

            int x = 0, y = 0;
            var moveX = CommandLineHelper.GetIntValue(this.keys.posX, out x);
            var moveY = CommandLineHelper.GetIntValue(this.keys.posY, out y);
            if(moveX || moveY)
            {
                WindowSetting.TryMoveWindow(x, y, 1000, 60);
            }

            var topmost = false;
            if(CommandLineHelper.GetBoolValue(this.keys.topmost, out topmost) && topmost)
            {
                WindowSetting.SetToTopMost();
            }
            else
            {
                this.loopMost.Validity = false;
            }
        }

        /// <summary>
        /// トップ化処理をする
        /// </summary>
        private void SetMost()
        {
            if(this.loopMost.MostType == MostType.Top)
            {
                WindowSetting.SetToTopMost();
            }
            else if(this.loopMost.MostType == MostType.Untop)
            {
                WindowSetting.SetToUntopMost();
            }
        }
        
        /// <summary>
        /// トップ化処理の繰り返しに関するパラメーター
        /// </summary>
        [Serializable]
        private class LoopSettingMost
        {
            /// <summary>有効化</summary>
            public bool Validity = true;
            /// <summary>処理同士の間隔</summary>
            public float Interval = 1.0f;
            /// <summary>トップ化処理の種類</summary>
            public MostType MostType = MostType.Top;
        }

        /// <summary>
        /// コマンドライン引数のキー情報
        /// </summary>
        [Serializable]
        private class CommandLineKeys
        {
            /// <summary>X座標のキー</summary>
            public string posX = "--pos-x";
            /// <summary>Y座標のキー</summary>
            public string posY = "--pos-y";
            /// <summary>トップモストのキー</summary>
            public string topmost = "--topmost";
        }
    }
}
