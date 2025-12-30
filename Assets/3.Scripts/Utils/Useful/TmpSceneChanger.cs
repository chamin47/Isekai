using TMPro;
using UnityEngine;

public class TMPFontReplacer : MonoBehaviour
{
    public TMP_FontAsset targetFont;
    [ContextMenu("Replace TMP Font In Scene")]
    private void ReplaceFont()
    {
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>(true);

        foreach (var text in texts)
        {
            text.font = targetFont;
        }

        Debug.Log($"Replaced {texts.Length} TMP texts");
    }
}