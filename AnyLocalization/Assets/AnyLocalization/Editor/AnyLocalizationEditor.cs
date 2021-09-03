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
    [CustomEditor(typeof(AnyLocalizationComponent))]
    public class SimpleLanguagesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            AnyLocalizationComponent SLC = (AnyLocalizationComponent)base.target;

            SLC.EditorMode = EditorGUILayout.Toggle("Editor Mode", SLC.EditorMode);
            EditorGUI.BeginDisabledGroup(!SLC.EditorMode);
            SLC.EditorLanguage = (Language)EditorGUILayout.EnumPopup("Editor Language", SLC.EditorLanguage);
            EditorGUI.EndDisabledGroup();
            SLC.DefaultLanguage = (Language)EditorGUILayout.EnumPopup("Default Language", SLC.DefaultLanguage);
            SLC.StreamPath = EditorGUILayout.TextField("Stream Path:        StreamingAssets/", SLC.StreamPath);
            SLC.UICanvas = (GameObject)EditorGUILayout.ObjectField("UI Root", SLC.UICanvas, typeof(GameObject), true);
        }
    }
}