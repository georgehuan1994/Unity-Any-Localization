//------------------------------------------------------------
// Simple Languages
// Author: George Huan
// Date: 2020-11-12
// Homepage: 
// Feedback: 
//------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnyLocalization
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Any Localization/Localization")]
    public class Localization : MonoBehaviour
    {
        [SerializeField] public string m_Key = string.Empty;

        private void Start()
        {
            ShowText();
        }

        private void OnEnable()
        {
            ShowText();
        }

        public void ShowText()
        {
            if (m_Key.Equals(string.Empty))
            {
                return;
            }

            string path = ANL.GetString(m_Key);

            if (TryGetComponent(out Text text))
            {
                text.text = path;
                return;
            }

            if (TryGetComponent(out TextMeshProUGUI textMeshProUGUI))
            {
                textMeshProUGUI.text = path;
                return;
            }
        }
    }
}