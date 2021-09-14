
// Any Localization - © 2020-2021 George Huan. All rights reserved
// https://gorh.cn/any-localization/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;


namespace AnyLocalization
{
    public class LanguageEditorWindow : EditorWindow
    {
        static Dictionary<Language, bool> languageStates = new Dictionary<Language, bool>();

        public static void Init()
        {
            EditorWindow languageEditorWindow = GetWindow(typeof(LanguageEditorWindow), false, "Languages", true);
            languageEditorWindow.maxSize = languageEditorWindow.minSize = new Vector2(600, 500);

            languageStates = new Dictionary<Language, bool>();
            foreach (Language lang in Enum.GetValues(typeof(Language)))
            {
                languageStates.Add(lang, AnyLocalizationEditorWindow.languages.Exists(x => x == lang));
            }
        }

        private void OnGUI()
        {
            for (int i = 1; i < languageStates.Count; i++)
            {
                var key = languageStates.ElementAt(i).Key;
                var value = languageStates.ElementAt(i).Value;

                var btn_rect = new Rect((i - 1) % 4 * 150 + 2, 32 * (float)Math.Floor((i - 1) / 4f) + 2, 150, 32);
                var toggle_rect = new Rect((i - 1) % 4 * 150 + 7, 32 * (float)Math.Floor((i - 1) / 4f) + 2, 150, 32);

                EditorGUI.BeginDisabledGroup(AnyLocalizationEditorWindow.languages.Exists(x => x == key));
                if (GUI.Button(btn_rect, "")) languageStates[key] = !value;
                EditorGUI.ToggleLeft(toggle_rect, key.ToString(), value);
                EditorGUI.EndDisabledGroup();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply", GUILayout.Height(30)))
            {
                ChangeLanguages();
                Close();
                AnyLocalizationEditorWindow.LoadXmlFiles();
            }
        }

        private void ChangeLanguages()
        {
            var langs = new List<Language>();
            foreach (var item in languageStates)
            {
                if (item.Value)
                {
                    if (AnyLocalizationEditorWindow.languages.Exists(x => x == item.Key)) continue;
                    else CreateXmlFile(item.Key);
                }
            }
        }

        /// <summary>
        /// 创建XML
        /// </summary>
        private static void CreateXmlFile(Language lang)
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlDeclaration dec = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDocument.AppendChild(dec);

            XmlElement dictionaries = xmlDocument.CreateElement("Dictionaries");
            xmlDocument.AppendChild(dictionaries);

            XmlElement dictionary = xmlDocument.CreateElement("Dictionary");
            dictionaries.AppendChild(dictionary);

            dictionary.SetAttribute("Language", lang.ToString());

            foreach (var key in AnyLocalizationEditorWindow.strKeyValuePairs.Keys)
            {
                XmlElement element = xmlDocument.CreateElement("String");
                element.SetAttribute("Key", key);
                element.SetAttribute("Value", string.Empty);
                dictionary.AppendChild(element);
            }

            if (AnyLocalizationEditorWindow.strKeyValuePairs.Count == 0)
            {
                XmlElement element = xmlDocument.CreateElement("String");
                element.SetAttribute("Key", "Example.Text");
                element.SetAttribute("Value", "This is an <b>Any Localization</b> example text.");
                dictionary.AppendChild(element);
            }

            var xmlPath = $"{AnyLocalizationEditorWindow.XMLDictionariesPath}/{lang}.xml";
            xmlDocument.Save(xmlPath);
            AssetDatabase.Refresh();
        }
    }
}

