using System.Collections.Generic;
using System;

namespace WindowController
{
    /// <summary>
    /// コマンドラウン引数を管理
    /// </summary>
    public static class CommandLineHelper
    {
        /// <summary>拡張引数のキープレフィックス</summary>
        private const string KEY_PREFIX = "--";

        /// <summary>コマンドライン引数</summary>
        private static Dictionary<string, string> Arguments = null;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        static CommandLineHelper()
        {
            if(Arguments != null)
            {
                return;
            }

            var key = string.Empty;
            var args = Environment.GetCommandLineArgs();
            Arguments = new Dictionary<string, string>();

            foreach(var arg in args)
            {
                if(arg.StartsWith(KEY_PREFIX))
                {
                    key = arg;
                }
                else if(string.IsNullOrEmpty(key) == false)
                {
                    Arguments[key] = arg;
                    key = string.Empty;
                }
            }
        }

        /// <summary>
        /// 引数で渡されたキーのリストを取得する
        /// </summary>
        /// <returns>キーリスト</returns>
        public static List<string> GetKeys()
        {
            return new List<string>(Arguments.Keys);
        }

        /// <summary>
        /// 引数で渡されたstring型の値を取得する
        /// </summary>
        /// <param name="key">引数値のキー</param>
        /// <returns>引数値 (取得に失敗したらnullを返します)</returns>
        public static string GetStringValue(string key)
        {
            if(Arguments == null || Arguments.ContainsKey(key) == false)
            {
                return null;
            }

            return Arguments[key];
        }

        /// <summary>
        /// 引数で渡されたfloat型の値を取得する
        /// </summary>
        /// <param name="key">引数値のキー</param>
        /// <param name="value">引数値を代入する変数</param>
        /// <returns>取得に成功したかどうか</returns>
        public static bool GetFloatValue(string key, out float value)
        {
            var str = GetStringValue(key);
            if(str == null)
            {
                value = default(float);
                return false;
            }

            return float.TryParse(str, out value);
        }

        /// <summary>
        /// 引数で渡されたint型の値を取得する
        /// </summary>
        /// <param name="key">引数値のキー</param>
        /// <param name="value">引数値を代入する変数</param>
        /// <returns>取得に成功したかどうか</returns>
        public static bool GetIntValue(string key, out int value)
        {
            var str = GetStringValue(key);
            if(str == null)
            {
                value = default(int);
                return false;
            }

            return int.TryParse(str, out value);
        }

        /// <summary>
        /// 引数で渡されたbool型の値を取得する
        /// </summary>
        /// <param name="key">引数値のキー</param>
        /// <param name="value">引数値を代入する変数</param>
        /// <returns>取得に成功したかどうか</returns>
        public static bool GetBoolValue(string key, out bool value)
        {
            var integer = 0;
            if(GetIntValue(key, out integer))
            {
                value = integer != 0;
                return true;
            }

            value = default(bool);
            return false;
        }
    }
}
