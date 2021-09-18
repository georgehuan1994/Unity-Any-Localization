
// Any Localization - © 2020-2021 George Huan. All rights reserved
// https://gorh.cn/any-localization/


using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
        public Language DefaultLanguage = Language.English;

        [SerializeField]
        public string StreamPath = ANL.XMLStreamingAssetsPath;

        /// <summary>
        /// Gets or sets the localized language <br/>获取或设置本地化语言 <br/>로 컬 언어 가 져 오기 또는 설정 <br/>ローカライズ言語の取得または設定
        /// </summary>
        public Language Language { get; set; }

        private string xmlStreamPath = string.Empty;

        private XmlDocument xmlDocument = new XmlDocument();

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);

            //Debug.Log($"System Language: {(Language)System.Enum.Parse(typeof(Language), Application.systemLanguage.ToString())}");

            Language = (Language)PlayerPrefs.GetInt("Setting.Language", 0);

            if (Language == Language.Unspecified)
            {
                Language = DefaultLanguage;
                Debug.LogWarning("Language Unspecified! Using Default Language!");
            }

            LoadXmlStream();
        }

        private void LoadXmlStream()
        {
            xmlStreamPath = $"{Application.streamingAssetsPath}/{StreamPath}/{Language}.xml";
            Debug.Log($"XML Stream Path: {xmlStreamPath}");
            Debug.Log("Loading Languages XML Stream....");

            if (Utility.IsWebGL())
            {
                StartCoroutine(LoadXmlStreamAsyn());
                return;
            }
            else if (Utility.IsAndroid())
            {
                var request = UnityWebRequest.Get(xmlStreamPath);
                request.SendWebRequest();
                while (true)
                {
                    if (request.result.Equals(UnityWebRequest.Result.ConnectionError))
                    {
                        Debug.LogError(request.error);
                        return;
                    }

                    if (request.isDone)
                    {
                        xmlDocument.LoadXml(Utility.RemoveXMLBom(request.downloadHandler.text));
                        break;
                    }
                }
            }
            else
            {
                if (File.Exists(xmlStreamPath)) xmlDocument.Load(xmlStreamPath);
            }

            CreateDictionary();
        }

        private IEnumerator LoadXmlStreamAsyn()
        {
            var request = UnityWebRequest.Get(xmlStreamPath);
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                Debug.Log($"Request is done！");
                xmlDocument.LoadXml(Utility.RemoveXMLBom(request.downloadHandler.text));
                CreateDictionary();
                RefreshText();
            }
            else
            {
                Debug.LogError($"Request failure！{request.error}");
            }
        }

        private void CreateDictionary()
        {
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
            RefreshText();
        }

        private void RefreshText()
        {
            Scene scene = SceneManager.GetActiveScene();
            foreach (var obj in scene.GetRootGameObjects())
            {
                obj.BroadcastMessage("ShowText", SendMessageOptions.DontRequireReceiver);
            }
        }

        public string GetString(string key)
        {
            if (strKeyValuePairs.TryGetValue(key, out string value)) return System.Text.RegularExpressions.Regex.Unescape(value);
            Debug.LogWarning("Non-existent Key:" + key);
            return $"[No Key]{key}";
        }
    }
}
