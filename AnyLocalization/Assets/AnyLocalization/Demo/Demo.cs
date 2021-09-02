using UnityEngine;
using UnityEngine.UI;

using AnyLocalization;

public class Demo : MonoBehaviour
{
    public Text Text_SimpleLanguage = null;

    public void OnTestButtonClick()
    {
        Text_SimpleLanguage.text = ANL.GetString("Game.Name");
    }
}


