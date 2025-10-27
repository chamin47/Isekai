using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 월드 스페이스 입력 말풍선.
/// TMP_InputField를 표시해 사용자의 텍스트 입력을 수집하고 페이드/위치 갱신을 수행한다.
/// </summary>
public class UI_InputBalloon : UI_Base
{   
    [SerializeField] private RectTransform root;     // 말풍선 루트(배경 포함)
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private TMP_Text promptLabel;
    [SerializeField] private TMP_InputField inputField;

    [Header("Layout")]
    [SerializeField] private Vector2 screenOffset = new Vector2(0, 80f);
    [SerializeField] private bool rebuildWhileTyping = true;

    [Header("Behavior")]
    [SerializeField] private float fadeIn = 0.15f;
    [SerializeField] private float fadeOut = 0.12f;
    [SerializeField] private float caretBlinkRate = 0.6f;

	private Transform _anchor;
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

	private IEnumerator ActivateNextFrame()
    {
        while (true)
        {
			yield return null;
			inputField?.ActivateInputField();
			inputField?.Select();
		}        
    }

	private void LateUpdate()
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
        Destroy(gameObject);
    }

    public override void Init() {  }
}
