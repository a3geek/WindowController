using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace WindowController
{
    // HWND ... ハンドルWindow(Windowハンドルを表す)
    using Hwnd = System.IntPtr;

    /// <summary>
    /// win32APIによってWindowの設定を変更する
    /// </summary>
    public static class WindowSetting
    {
        /// <summary>
        /// <see cref="GetWindowRect"/>用のWindowスクリーン座標取得用構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>左辺X座標</summary>
            public int Left;
            /// <summary>上辺Y座標</summary>
            public int Top;
            /// <summary>右辺X座標</summary>
            public int Right;
            /// <summary>下辺Y座標</summary>
            public int Bottom;
        }

        /// <summary>現在のWindowサイズを維持する</summary>
        private const int SwpNoSize = 0x0001;
        /// <summary>現在のWindow位置を維持する</summary>
        private const int SwpNoMove = 0x0002;
        /// <summary>最前面設定</summary>
        private const int HwndTopmost = -1;
        /// <summary>非最前面設定</summary>
        private const int HwndNoTopmost = -2;

        /// <summary>繰り返し処理用タイマー</summary>
        private static Timer Timer = null;
        /// <summary>タイマーの繰り返し回数</summary>
        private static int TimerRepeatCount = 0;
        /// <summary>Windowの座標</summary>
        private static Vector2 WindowPos = Vector2.zero;


        /// <summary>
        /// Windowを移動させる
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        public static void MoveWindow(int x, int y)
        {
            Hwnd hwnd = GetCurrentWindow();
            RECT rc;

            GetWindowRect(hwnd, out rc);
            MoveWindow(hwnd, x, y, rc.Right - rc.Left, rc.Bottom - rc.Top, false);
        }

        /// <summary>
        /// リピート設定しつつWindowを移動させる
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="interval">移動間隔</param>
        /// <param name="repeatCount">繰り返し回数</param>
        public static void TryMoveWindow(int x, int y, float interval, int repeatCount = -1)
        {
            if(Timer == null)
            {
                Timer = new Timer();
                Timer.AutoReset = true;
                Timer.Elapsed += OnTimerElapsed;
            }

            WindowPos.Set(x, y);
            TimerRepeatCount = repeatCount < 0 ? -1 : repeatCount; // -1:forever
            Timer.Interval = interval;
            Timer.Enabled = true;
        }

        /// <summary>
        /// <see cref="Timer"/>によって呼び出されるハンドラ
        /// </summary>
        /// <param name="sender">送信元情報</param>
        /// <param name="e">ハンドラ情報</param>
        static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            int x = (int)WindowPos.x, y = (int)WindowPos.y;
            MoveWindow(x, y);

            RECT rc;
            GetWindowRect(GetCurrentWindow(), out rc);
            if(rc.Left == x && rc.Top == y)
            {
                Timer.Enabled = false;
                return;
            }

            if(TimerRepeatCount != -1 && --TimerRepeatCount < 0)
            {
                Timer.Enabled = false;
                return;
            }
        }

        /// <summary>
        /// 現在選択されているWindowのProcessIDを取得する
        /// </summary>
        /// <returns>ProcessID</returns>
        public static int GetProcessId()
        {
            return Process.GetCurrentProcess().Id;
        }

        /// <summary>
        /// 表示状態の全てのWindowを取得する
        /// </summary>
        /// <returns>アクティブ状態の全てのWindow</returns>
        public static Dictionary<Hwnd, string> GetWindows()
        {
            var shellWindow = GetShellWindow();
            var windows = new Dictionary<Hwnd, string>();

            EnumWindows((hwnd, lParam) =>
            {
                var shell = hwnd == shellWindow;
                var visible = IsWindowVisible(hwnd);
                var length = GetWindowTextLength(hwnd);
                var builder = new StringBuilder(length);

                GetWindowText(hwnd, builder, length + 1);
                if(shell || !visible || length == 0)
                {
                    return true;
                }

                windows[hwnd] = builder.ToString();
                return true;
            }, 0);

            return windows;
        }

        /// <summary>
        /// <paramref name="processId"/>からWindowを取得する
        /// </summary>
        /// <param name="processId">プロセスIDを</param>
        /// <returns>Windowハンドラ</returns>
        public static Hwnd GetWindow(int processId)
        {
            var pair = GetWindows().Select(x =>
            {
                long pid = 0;
                GetWindowThreadProcessId(x.Key, out pid);

                return new KeyValuePair<Hwnd, long>(x.Key, pid);
            }).FirstOrDefault(x => x.Value == processId);

            return pair.Key;
        }

        /// <summary>
        /// 現在選択されているWindowを取得する
        /// </summary>
        /// <returns>Windowハンドラ</returns>
        public static Hwnd GetCurrentWindow()
        {
            return GetWindow(GetProcessId());
        }

        /// <summary>
        /// Windowを最前面設定する
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <returns>設定に成功したかどうか</returns>
        public static bool SetToTopMost(Hwnd hwnd)
        {
            return SetWindowPos(hwnd, HwndTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize);
        }

        /// <summary>
        /// Windowを最前面設定する
        /// </summary>
        /// <returns>設定に成功したかどうか</returns>
        public static bool SetToTopMost()
        {
            return SetToTopMost(GetCurrentWindow());
        }

        /// <summary>
        /// Windowを非最前面設定する
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <returns>設定に成功したかどうか</returns>
        public static bool SetToUntopMost(Hwnd hwnd)
        {
            return SetWindowPos(hwnd, HwndNoTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize);
        }

        /// <summary>
        /// Windowを非最前面設定する
        /// </summary>
        /// <returns>設定に成功したかどうか</returns>
        public static bool SetToUntopMost()
        {
            return SetToUntopMost(GetCurrentWindow());
        }

        /// <summary>
        /// <see cref="EnumWindows(EnumWindowsProc, int)"/>のコールバック用デリゲート
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private delegate bool EnumWindowsProc(Hwnd hwnd, int lParam);

        #region "win32 APIs"
        /// <summary>
        /// 指定されたWindowの位置とサイズを変更する
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="nWidth">幅</param>
        /// <param name="nHeight">高さ</param>
        /// <param name="bRepaint">再描画オプション</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool MoveWindow(Hwnd hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// 呼び出し側のスレッドのメッセージキューに関連付けられているアクティブWindowのWindowハンドルを取得する
        /// </summary>
        /// <returns>Windowハンドラ</returns>
        [DllImport("user32.dll")]
        private static extern Hwnd GetActiveWindow();

        /// <summary>
        /// <paramref name="hwnd"/>Windowの左上端と右下端の座標をスクリーン座標で取得する
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <param name="lpRect">Windowの座標値</param>
        /// <returns>取得に成功したかどうか</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(Hwnd hwnd, out RECT lpRect);

        /// <summary>
        /// 画面上の全てのトップレベルWindowを列挙する
        /// </summary>
        /// <param name="enumFunc">コールバック関数</param>
        /// <param name="lParam">アプリケーション定義の値</param>
        /// <returns>全てのトップレベルWindow</returns>
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        /// <summary>
        /// <paramref name="hwnd"/>Windowのタイトルバーのテキストをバッファへコピーする
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <param name="lpString">テキストバッファ</param>
        /// <param name="nMaxCount">コピーする最大文字数</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetWindowText(Hwnd hwnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// <paramref name="hwnd"/>Windowのタイトルバーテキストの文字数を返す
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <returns>テキストの文字数</returns>
        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(Hwnd hwnd);

        /// <summary>
        /// <paramref name="hwnd"/>Windowの表示状態を調べる
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <returns>表示中かどうか</returns>
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(Hwnd hwnd);

        /// <summary>
        /// シェルデスクトップWindowのWindowハンドラを取得する
        /// </summary>
        /// <returns>Windowハンドラ</returns>
        [DllImport("user32.dll")]
        private static extern Hwnd GetShellWindow();

        /// <summary>
        /// <paramref name="hwnd"/>Windowを作成したスレッドIDとプロセスIDを取得する
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <param name="lpdwProcessId">プロセスIDを</param>
        /// <returns>Windowを作成したスレッドID</returns>
        [DllImport("user32.dll")]
        private static extern long GetWindowThreadProcessId(Hwnd hwnd, out long lpdwProcessId);

        /// <summary>
        /// Windowのサイズ、位置、Zオーダーを変更する
        /// </summary>
        /// <param name="hwnd">Windowハンドラ</param>
        /// <param name="hwndInsertAfter">配置順序のハンドル</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="cx">幅</param>
        /// <param name="cy">高さ</param>
        /// <param name="uFlags">Windowの位置オプション</param>
        /// <returns>変更に成功したかどうか</returns>
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(Hwnd hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);
        #endregion
    }
}
