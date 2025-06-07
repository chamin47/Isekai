using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_G : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private float _bubbleDropHeight = 7.0f; // 떨어지는 시작 y
    [SerializeField] private float _bubbleDropDuration = 1.0f;
    [SerializeField] private float _bubbleSize = 1.5f;

    private List<UI_Bubble> bubbles = new List<UI_Bubble>();

    private void Start()
    {
        StartCoroutine(AttackSequnce());
    }

    private IEnumerator AttackSequnce()
    {
        for (int i = 0; i < 4; i++)
        {            
            Vector3 startPos = _player.transform.position + Vector3.up * _bubbleDropHeight;
            Vector3 endPos = _player.transform.position + Vector3.up * (2.5f - i * 0.1f + i * _bubbleSize); // 머리 위 기본 높이

            UI_Bubble ui = Managers.UI.MakeSubItem<UI_Bubble>();
            ui.transform.position = startPos;
            ui.Init("hi", 0, false);
            ui.transform.DOMoveY(endPos.y, _bubbleDropDuration)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    float yScale = _player.transform.localScale.y - 0.2f;
                    _player.transform.localScale = new Vector3(_player.transform.localScale.x, yScale, _player.transform.localScale.z);

                    bubbles.Insert(0, ui);
                    for (int j = 0; j < bubbles.Count; j++)
                    {

                        float pos = bubbles[j].transform.position.y;
                        Sequence seq = DOTween.Sequence();
                        seq.Append(bubbles[j].transform.DOMoveY(pos - 0.3f, 0.05f).SetEase(Ease.InCubic));
                        seq.Append(bubbles[j].transform.DOMoveY(pos - 0.1f, 0.05f).SetEase(Ease.OutBounce));

                    }
                }
            );

            yield return WaitForSecondsCache.Get(2f);
        }
    }
}
