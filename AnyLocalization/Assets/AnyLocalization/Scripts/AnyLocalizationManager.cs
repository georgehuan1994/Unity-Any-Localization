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
    }
}

