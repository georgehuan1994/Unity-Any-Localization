
// Any Localization - © 2020-2021 George Huan. All rights reserved
// https://gorh.cn/any-localization/


using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace AnyLocalization
{
    [CustomEditor(typeof(AnyLocalizationComponent))]
    public class AnyLocalizationInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            AnyLocalizationComponent SLC = (AnyLocalizationComponent)base.target;
            SLC.DefaultLanguage = (Language)EditorGUILayout.EnumPopup("Default Language", SLC.DefaultLanguage);
            SLC.StreamPath = EditorGUILayout.TextField("Stream Path:        StreamingAssets/", SLC.StreamPath);
        }
    }
}