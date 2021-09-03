﻿//------------------------------------------------------------
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
        public static Dictionary<string, string> strKeyValuePairs = new Dictionary<string, string>();

        [SerializeField]
        public bool EditorMode = false;

        [SerializeField] 
        public Language EditorLanguage = Language.English;

        [SerializeField]
        public Language DefaultLanguage = Language.English;

        [SerializeField]
        public string StreamPath = ANL.XMLStreamingAssetsPath;

        [SerializeField]
        public GameObject UICanvas = null;

        /// <summary>
        /// Gets or sets the localized language <br/>获取或设置本地化语言 <br/>로 컬 언어 가 져 오기 또는 설정 <br/>ローカライズ言語の取得または設定
        /// </summary>
        public Language Language { get; set; }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);

            DefaultLanguage = (Language)System.Enum.Parse(typeof(Language), Application.systemLanguage.ToString());
            Debug.Log($"System Language: {DefaultLanguage}");

            if (EditorMode)
            {
                Language = EditorLanguage;
            }
            else
            {
                Language = (Language)PlayerPrefs.GetInt("Setting.Language", 0);
            }

            if (Language == Language.Unspecified)
            {
                Language = DefaultLanguage;
                Debug.LogWarning("Language Unspecified! Using Default Language!");
            }

            LoadXmlStream();
        }

        private void LoadXmlStream()
        {
            XmlDocument xmlDocument = new XmlDocument();

            string xmlStreamPath = $"{Application.streamingAssetsPath}/{StreamPath}/{Language}.xml";
            Debug.Log($"XML Stream Path: {xmlStreamPath}");
            Debug.Log("Loading Languages XML Stream....");

            if (Utility.IsAndroid())
            {
                int frame = 0;
                UnityWebRequest request = new UnityWebRequest(xmlStreamPath);
                while (!request.isDone)
                {
                    if (++frame >= 500) return;
                }
                xmlDocument.Load(request.downloadHandler.text);
            }
            else
            {
                if (File.Exists(xmlStreamPath)) xmlDocument.Load(xmlStreamPath);
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

            strKeyValuePairs = new Dictionary<string, string>();

            foreach (XmlNode item in xmlNodeList)
            {
                string key = item.Attributes["Key"].Value;
                string value = item.Attributes["Value"].Value;
                strKeyValuePairs.Add(key, value);
            }
            Debug.Log("Load Simple Languages XML Succeed.");
        }

        public void SetLanguage(Language language)
        {
            Language = language;
            PlayerPrefs.SetInt("Setting.Language", (int)language);
            LoadXmlStream();
            UICanvas.BroadcastMessage("ShowText");
        }

        public string GetString(string key)
        {
            if (strKeyValuePairs.TryGetValue(key, out string value)) return System.Text.RegularExpressions.Regex.Unescape(value);
            Debug.LogWarning("Non-existent Key:" + key);
            return $"[No Key]{key}";
        }
    }
}
