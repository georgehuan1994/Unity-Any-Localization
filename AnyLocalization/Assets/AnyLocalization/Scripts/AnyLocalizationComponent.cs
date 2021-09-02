//------------------------------------------------------------
// Any Localization
// Author: George Huan
// Date: 2020-11-12
// Homepage: 
// Feedback: 
//------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace AnyLocalization
{
    /// <summary>
    /// Any Localization Mono Component
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Any Localization/Any Localization Component")]
    public sealed class AnyLocalizationComponent : MonoBehaviour
    {
        public static AnyLocalizationComponent Instance { get; set; }

        [SerializeField] 
        public Language EditorLanguage = Language.English;
        public static Language SystemLanguage = Language.English;
        public static Language UserSpecifiedLanguage = Language.Unspecified;

        [SerializeField]
        public string StreamPath = ANL.XMLStreamingAssetsPath;

        public static Dictionary<string, string> strKeyValuePairs = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the localized language <br/>获取或设置本地化语言 <br/>로 컬 언어 가 져 오기 또는 설정 <br/>ローカライズ言語の取得または設定
        /// </summary>
        public Language Language { get; set; }


        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);

            Language = Language.English;
            SystemLanguage = (Language)System.Enum.Parse(typeof(Language), Application.systemLanguage.ToString());

            if (Utility.IsEditor())
            {
                Language = EditorLanguage;
            }
            else
            {
                Language = SystemLanguage;
            }

            LoadData();
        }

        public void LoadData()
        {
            if (Language == Language.Unspecified)
            {
                Debug.LogError("Language Unspecified!");
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();

            string xmlStreamingAssetsPath = $"{Application.streamingAssetsPath}/{StreamPath}/{Language}.xml";
            Debug.Log($"XML Path: {xmlStreamingAssetsPath}");
            Debug.Log("Loading Languages XML....");

            if (Utility.IsEditor() || Utility.IsPC() || Utility.IsIOS())
            {
                if (File.Exists(xmlStreamingAssetsPath)) xmlDocument.Load(xmlStreamingAssetsPath);
            }
            else
            {
                int frame = 0;
                UnityWebRequest request = new UnityWebRequest(xmlStreamingAssetsPath);
                while (!request.isDone)
                {
                    if (++frame >= 500) return;
                }
                xmlDocument.Load(request.downloadHandler.text);
            }

            XmlNodeList xmlNodeList = null;

            try
            {
                xmlNodeList = xmlDocument.SelectSingleNode("Dictionaries/Dictionary").ChildNodes;
            }
            catch
            {
                throw new System.Exception($"Non-existent Language:{Language}"); 
            }
            
            foreach (XmlNode item in xmlNodeList)
            {
                string key = item.Attributes["Key"].Value;
                string value = item.Attributes["Value"].Value;
                strKeyValuePairs.Add(key, value);
            }
            Debug.Log("Load Simple Languages XML Succeed.");

        }

        public string GetString(string key)
        {
            if (strKeyValuePairs.TryGetValue(key, out string value)) return System.Text.RegularExpressions.Regex.Unescape(value);
            Debug.LogError("Non-existent Key:" + key);
            return $"[No Key]{key}";
        }
    }
}
