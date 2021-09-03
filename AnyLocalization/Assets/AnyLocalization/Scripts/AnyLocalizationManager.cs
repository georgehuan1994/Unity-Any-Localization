//------------------------------------------------------------
// Any Localization
// Author: George Huan
// Date: 2020-11-12
// Homepage: 
// Feedback: 
//------------------------------------------------------------

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

