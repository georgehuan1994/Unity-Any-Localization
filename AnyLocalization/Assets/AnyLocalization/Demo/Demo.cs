using UnityEngine;
using UnityEngine.UI;

using AnyLocalization;

public class Demo : MonoBehaviour
{
    private void Start()
    {
        SetDropdownValue();
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
                value = 0;
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
        if (Utility.IsEditor())
        {
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
        Application.Quit();
    }
}


