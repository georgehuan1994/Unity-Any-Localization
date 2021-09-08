//------------------------------------------------------------
// Any Localization
// Author: George Huan
// Date: 2020-11-12
// Homepage: 
// Feedback: 
//------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace AnyLocalization
{
    [CustomEditor(typeof(AutoLocalization))]
    public class LocalizationInspector : Editor
    {
        private SerializedProperty key;
        private string searchText = string.Empty;
        private Vector2 scrollViewPos;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("KeySearch");
                    searchText = EditorGUILayout.TextField(searchText);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.Space();

                scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, GUILayout.MaxHeight(110));
                {
                    foreach (var item in AnyLocalizationEditorWindow.strKeyValuePairs)
                    {
                        if (KVQuery(item))
                        {
                            Rect rect = EditorGUILayout.BeginHorizontal("button");
                            if (GUI.Button(rect, GUIContent.none))
                            {
                                searchText = key.stringValue = item.Key.ToString();
                                EditorGUI.FocusTextInControl(null);
                            }
                            EditorGUILayout.LabelField(item.Key.ToString());
                            item.Value.TryGetValue(Language.ChineseSimplified, out string btnValue);
                            EditorGUILayout.LabelField(btnValue);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }



        void OnEnable()
        {
            key = serializedObject.FindProperty("m_Key");
            AnyLocalizationEditorWindow.LoadXmlFiles();
            searchText = key.stringValue;
        }


        /// <summary>
        /// 绘制搜索框
        /// </summary>
        /// <param name="text"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private string DrawSearchField(string text, params GUILayoutOption[] options)
        {
            MethodInfo info = typeof(EditorGUILayout).GetMethod("ToolbarSearchField", BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string), typeof(GUILayoutOption[]) }, null);
            if (info != null)
            {
                text = (string)info.Invoke(null, new object[] { text, options });
            }
            return text;
        }

        /// <summary>
        /// 键值对查询
        /// </summary>
        /// <param name="kv"></param>
        /// <returns></returns>
        private bool KVQuery(KeyValuePair<string, Dictionary<Language, string>> kv)
        {
            if (searchText == string.Empty) return false;

            if (kv.Key.ToLower().Contains(searchText.ToLower())) return true;

            for (int i = 0; i < AnyLocalizationEditorWindow.languages.Count; i++)
            {
                kv.Value.TryGetValue(AnyLocalizationEditorWindow.languages[i], out string v);
                if (v.Contains(searchText)) return true;
            }
            return false;
        }
    }
}