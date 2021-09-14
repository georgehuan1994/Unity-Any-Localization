
// Any Localization - © 2020-2021 George Huan. All rights reserved
// https://gorh.cn/any-localization/


using System;
using System.Text;
using UnityEngine;

namespace AnyLocalization
{
    public static partial class Utility
    {
        [ThreadStatic]
        private static StringBuilder s_StringBuilder = null;

        public static bool IsEditor()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                return true;
            }
            return false;
        }

        public static bool IsPC()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                return true;
            }
            return false;
        }

        public static bool IsIOS()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return true;
            }
            return false;
        }

        public static bool IsAndroid()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return true;
            }
            return false;
        }

        public static bool IsWebGL()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return true;
            }
            return false;
        }

        public static string Format(string format, object arg0)
        {
            if (format == null)
            {
                throw new SystemException("Format is invalid.");
            }

            CheckCachedStringBuilder();
            s_StringBuilder.Length = 0;
            s_StringBuilder.AppendFormat(format, arg0);
            return s_StringBuilder.ToString();
        }

        
        public static string Format(string format, object arg0, object arg1)
        {
            if (format == null)
            {
                throw new SystemException("Format is invalid.");
            }

            CheckCachedStringBuilder();
            s_StringBuilder.Length = 0;
            s_StringBuilder.AppendFormat(format, arg0, arg1);
            return s_StringBuilder.ToString();
        }

        
        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            if (format == null)
            {
                throw new SystemException("Format is invalid.");
            }

            CheckCachedStringBuilder();
            s_StringBuilder.Length = 0;
            s_StringBuilder.AppendFormat(format, arg0, arg1, arg2);
            return s_StringBuilder.ToString();
        }

        public static string Format(string format, params object[] args)
        {
            if (format == null)
            {
                throw new SystemException("Format is invalid.");
            }

            if (args == null)
            {
                throw new SystemException("Args is invalid.");
            }

            CheckCachedStringBuilder();
            s_StringBuilder.Length = 0;
            s_StringBuilder.AppendFormat(format, args);
            return s_StringBuilder.ToString();
        }

        private static void CheckCachedStringBuilder()
        {
            if (s_StringBuilder == null)
            {
                s_StringBuilder = new StringBuilder(1024);
            }
        }

    }
}

