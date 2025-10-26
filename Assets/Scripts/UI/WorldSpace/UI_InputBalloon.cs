using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InputBalloon : UI_Base
{
    [Header("Refs")]
    [SerializeField] RectTransform root;     // 말풍선 루트(배경 포함)
    [SerializeField] CanvasGroup cg;
    [SerializeField] TMP_Text promptLabel;
    [SerializeField] TMP_InputField inputField;

    [Header("Layout")]
    [SerializeField] Vector2 screenOffset = new Vector2(0, 80f);
    [SerializeField] bool rebuildWhileTyping = true;

    [Header("Behavior")]
    [SerializeField] float fadeIn = 0.15f;
    [SerializeField] float fadeOut = 0.12f;
    [SerializeField] float caretBlinkRate = 0.6f;

    Transform _anchor;
    private const int _charLimit = 15;

    public void Init(Transform anchor)
    {
        _anchor = anchor;

        if (cg != null)
            cg.alpha = 0f;

        // TMP 세팅
        if (inputField)
        {
            inputField.characterLimit = _charLimit;
            inputField.text = "";
            inputField.caretBlinkRate = caretBlinkRate;
            // 포커스 주기
            StartCoroutine(ActivateNextFrame());
        }
    }

    IEnumerator ActivateNextFrame()
    {
        yield return null;
        inputField?.ActivateInputField();
        inputField?.Select();
    }

    void LateUpdate()
    {
        if (_anchor == null)
            return;

        var cam = Camera.main;
        if (cam == null)
            return;

        // 픽셀 오프셋 → 월드 오프셋
        var baseWorld = cam.ScreenToWorldPoint(Vector3.zero);
        var offWorld = cam.ScreenToWorldPoint(new Vector3(screenOffset.x, screenOffset.y, 0f)) - baseWorld;

        var pos = _anchor.position + offWorld;
        pos.z = _anchor.position.z; // 2D 고정
        root.position = pos;

        if (rebuildWhileTyping && root)
            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    public IEnumerator CoPrompt(string prompt, Action<string> onDone)
    {
        if (promptLabel) promptLabel.text = prompt ?? "";

        // 페이드 인
        if (cg != null)
            yield return cg.FadeCanvas(1f, fadeIn);

        string result = null;

        // Enter 키
        while (result == null)
        {
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                result = inputField.text;
                break;
            }
            yield return null;
        }

        // 페이드 아웃
        if (cg != null)
            yield return cg.FadeCanvas(0f, fadeOut);

        onDone?.Invoke(result ?? "");
    }

    public override void Init() {  }
}
