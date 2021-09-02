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

            SLC.EditorLanguage = (Language)EditorGUILayout.EnumPopup("Editor Language", SLC.EditorLanguage);
            SLC.StreamPath = EditorGUILayout.TextField("Stream Path:        StreamingAssets/", SLC.StreamPath);
        }
    }
}