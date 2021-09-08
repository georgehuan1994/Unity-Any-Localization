
// Any Localization - © 2020-2021 George Huan. All rights reserved
// http://gorh.cn/any-localization/


using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnyLocalization
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Any Localization/Auto Localization")]
    public class AutoLocalization : MonoBehaviour
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
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                return;
            }

            if (TryGetComponent(out TextMeshProUGUI textMeshProUGUI))
            {
                textMeshProUGUI.text = path;
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                return;
            }

            if (TryGetComponent(out TextMeshPro textMeshPro))
            {
                textMeshPro.text = path;
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                return;
            }
        }
    }
}