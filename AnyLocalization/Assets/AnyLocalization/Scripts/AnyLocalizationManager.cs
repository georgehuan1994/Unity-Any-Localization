
// Any Localization - © 2020-2021 George Huan. All rights reserved
// http://gorh.cn/any-localization/


using System;

namespace AnyLocalization
{
    public static class ANL
    {
        public const string XMLStreamingAssetsPath = "AnyLocalization/XML";

        public static string GetString(string key)
        {
            if (AnyLocalizationComponent.Instance == null)
            {
                return null;
            }
            return AnyLocalizationComponent.Instance.GetString(key);
        }

        public static string GetString(string key, object arg0)
        {
            string value = GetString(key);

            try
            {
                return Utility.Format(value, arg0);
            }
            catch (Exception exception)
            {
                return Utility.Format("[Error]{0},{1},{2},{3}", key, value, arg0, exception.ToString());
            }
        }

        public static string GetString(string key, object arg0, object arg1)
        {
            string value = GetString(key);

            try
            {
                return Utility.Format(value, arg0, arg1);
            }
            catch (Exception exception)
            {
                return Utility.Format("[Error]{0},{1},{2},{3},{4}", key, value, arg0, arg1, exception.ToString());
            }
        }

        public static string GetString(string key, object arg0, object arg1, object arg2)
        {
            string value = GetString(key);

            try
            {
                return Utility.Format(value, arg0, arg1, arg2);
            }
            catch (Exception exception)
            {
                return Utility.Format("[Error]{0},{1},{2},{3},{4},{5}", key, value, arg0, arg1, arg2, exception.ToString());
            }
        }

        public static string GetString(string key, params object[] args)
        {
            string value = GetString(key);

            try
            {
                return Utility.Format(value, args);
            }
            catch (Exception exception)
            {
                string errorString = Utility.Format("[Error]{0},{1}", key, value);
                if (args != null)
                {
                    foreach (object arg in args)
                    {
                        errorString += "," + arg.ToString();
                    }
                }

                errorString += "," + exception.ToString();
                return errorString;
            }
        }

        public static Language Language 
        { 
            get 
            { 
                return AnyLocalizationComponent.Instance.Language; 
            } 
            set 
            { 
                AnyLocalizationComponent.Instance.Language = value; 
            }
        }

        public static Language DefaultLanguage
        {
            get
            {
                return AnyLocalizationComponent.Instance.DefaultLanguage;
            }
        }

        public static void SetLanguage(Language language)
        {
            AnyLocalizationComponent.Instance.SetLanguage(language);
        }
    }
}

