
// Any Localization - © 2020-2021 George Huan. All rights reserved
// https://gorh.cn/any-localization/


using UnityEngine;
using UnityEngine.UI;

using AnyLocalization;

public class Demo : MonoBehaviour
{
    [SerializeField] private Text m_SeeOnlineDocument = null;

    private void Start()
    {
        SetDropdownValue();
        RefreshCustomText();
    }

    public void SetDropdownValue()
    {
        var value = 0;

        switch (ANL.Language)
        {
            case Language.ChineseSimplified:
                value = 1;
                break;
            case Language.ChineseTraditional:
                value = 2;
                break;
            case Language.English:
                value = 3;
                break;
            case Language.Korean:
                value = 4;
                break;
            case Language.Japanese:
                value = 5;
                break;
            default:
                break;
        }

        transform.Find("UIForm_Menu/Dropdown_ChangeLanguage").GetComponent<Dropdown>().value = value;
    }

    public void ChangeLanguage(int value)
    {
        switch (value)
        {
            case 1:
                ANL.SetLanguage(Language.ChineseSimplified);
                break;
            case 2:
                ANL.SetLanguage(Language.ChineseTraditional);
                break;
            case 3:
                ANL.SetLanguage(Language.English);
                break;
            case 4:
                ANL.SetLanguage(Language.Korean);
                break;
            case 5:
                ANL.SetLanguage(Language.Japanese);
                break;
            default:
                ANL.SetLanguage(ANL.DefaultLanguage);
                break;
        }
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void ShowText()
    {
        RefreshCustomText();
    }

    private void RefreshCustomText()
    {
        m_SeeOnlineDocument.text = ANL.GetString("AnyLocalization.OnlineDocument", ANL.GetString("AnyLocalization.Website"));
    }
}


