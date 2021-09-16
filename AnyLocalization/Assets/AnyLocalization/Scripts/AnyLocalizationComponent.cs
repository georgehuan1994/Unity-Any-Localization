
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
            XmlDocument xmlDocument = new XmlDocument();

            string xmlStreamPath = $"{Application.streamingAssetsPath}/{StreamPath}/{Language}.xml";
            Debug.Log($"XML Stream Path: {xmlStreamPath}");
            Debug.Log("Loading Languages XML Stream....");

            if (Utility.IsAndroid())
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
                        string xmlStr = request.downloadHandler.text;

                        // Remove XML Bom 
                        string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                        if (xmlStr.StartsWith(_byteOrderMarkUtf8))
                        {
                            Debug.Log("Remove XML Bom");
                            int lastIndexOfUtf8 = _byteOrderMarkUtf8.Length;
                            xmlStr = xmlStr.Remove(0, lastIndexOfUtf8);
                        }

                        xmlDocument.LoadXml(xmlStr);
                        break;
                    }
                }
            }
            else if (Utility.IsWebGL())
            {
                Debug.Log("Is WebGL Player");
                GetXml();
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

        IEnumerator GetXml()
        {
            Debug.Log($"请求开始！");
            var request = UnityWebRequest.Get($"{Application.streamingAssetsPath}/{StreamPath}/{Language}.xml");
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                Debug.Log($"请求完成！");
                Debug.Log($"text：{request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"请求失败！");
            }
        }

        public void SetLanguage(Language language)
        {
            Language = language;
            PlayerPrefs.SetInt("Setting.Language", (int)language);
            LoadXmlStream();

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
