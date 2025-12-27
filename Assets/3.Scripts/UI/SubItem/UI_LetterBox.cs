using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LetterBox : MonoBehaviour
{
    [SerializeField] private RectTransform[] _letterBoxObject = new RectTransform[2];
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private Vector3[] _originalPositions = new Vector3[2];
    private Coroutine _moveCoroutine;

    [ContextMenu("Show")]
    public IEnumerator ShowLetterBox()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        yield return _moveCoroutine = StartCoroutine(MoveLetterBox(true));
    }
    [ContextMenu("Hide")]
    public IEnumerator HideLetterBox()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        yield return _moveCoroutine = StartCoroutine(MoveLetterBox(false));
    }

    private IEnumerator MoveLetterBox(bool show)
    {
        float elapsedTime = 0f;
        Vector3[] startPositions = new Vector3[2];
        Vector3[] targetPositions = new Vector3[2];
        for (int i = 0; i < _letterBoxObject.Length; i++)
        {
            startPositions[i] = _letterBoxObject[i].anchoredPosition;
            targetPositions[i] = show ? _originalPositions[i] : new Vector3(_originalPositions[i].x, -_originalPositions[i].y, _originalPositions[i].z);
        }
        while (elapsedTime < _moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _moveDuration);
            for (int i = 0; i < _letterBoxObject.Length; i++)
            {
                _letterBoxObject[i].anchoredPosition = Vector3.Lerp(startPositions[i], targetPositions[i], t);
            }
            yield return null;
        }

        Debug.Log("LetterBox movement completed.");

        for (int i = 0; i < _letterBoxObject.Length; i++)
        {
            _letterBoxObject[i].anchoredPosition = targetPositions[i];
        }
    }

}
