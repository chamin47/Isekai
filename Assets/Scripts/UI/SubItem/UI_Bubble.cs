﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Bubble : UI_Base
{
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private Image _bubbleImage;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] private float _widthPadding = 0.1f;
    [SerializeField] private float _heightPadding = 0.1f;
    
    [SerializeField] private float _textSize = 0.3f;
    [SerializeField] private float _dropSpeed = 2.0f;
    private int _score;
    public string testText = "평범하기 짝이 없으면서 어딜 나서려고 하는 거야?";

    public event Action OnCollisionEvent;

    public float test;

    public void Init(string text, int score, bool drop = true)
    {
        FixBubbleSize(text, score);
        if(drop)
            StartCoroutine(CoDropDown());
    }

    [ContextMenu("Test Init")]
    private void FixBubbleSize(string text, int score)
    {
        _text.text = text;
        _text.fontSize = _textSize;

        _score = score;

        float length = _text.preferredWidth;
        float hight = _text.preferredHeight;

        Vector2 preferredSize = new Vector2(length + _widthPadding, hight + _heightPadding);
        _bubbleImage.rectTransform.sizeDelta = preferredSize;

        _boxCollider.size = preferredSize - new Vector2(0.1f, 0.1f);
    }

    [Tooltip("떨어지는 변수")]

    public float dropTime = 1f;      // 떨어지는 시간
    public float bounceHeight = 0.8f;    // 튀는 높이
    public int bounceCount = 4;        // 튕길 횟수
    public float bounceDuration = 0.3f;// 한 번 튀는 시간
    public float fadeOutTime = 1f;     // 서서히 사라지는 시간
    public float bounceDecrease = 0.6f; // 튕길 때마다 감소하는 높이

    IEnumerator CoDropDown()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, -4.5f, startPos.z);

        while (elapsedTime < dropTime)
        {
            elapsedTime += Time.deltaTime * test;
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / dropTime);
            yield return null;
        }

        transform.position = targetPos; 

        Managers.Sound.Play("s1_say_impact3", Sound.Effect);
        StartCoroutine(BounceEffect());
    }

    IEnumerator BounceEffect()
    {
        for (int i = 0; i < bounceCount; i++)
        {
            float elapsedTime = 0f;
            Vector3 startPos = transform.position;
            Vector3 peakPos = startPos + new Vector3(0, bounceHeight, 0);

            // 위로 이동
            while (elapsedTime < bounceDuration / 2)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, peakPos, elapsedTime / (bounceDuration / 2));
                yield return null;
            }

            transform.position = peakPos;

            elapsedTime = 0f;

            // 아래로 이동
            while (elapsedTime < bounceDuration / 2)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(peakPos, startPos, elapsedTime / (bounceDuration / 2));
                yield return null;
            }

            transform.position = startPos; // 정확한 위치 보정
            bounceHeight *= bounceDecrease; // 튕길 때마다 높이 감소
        }

        // 4️⃣ 1초 대기 후 서서히 사라지기
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color originalColor = _bubbleImage.color;

        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            _bubbleImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (elapsedTime / fadeOutTime));
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1 - (elapsedTime / fadeOutTime));
            yield return null;
        }

        OnCollisionEvent?.Invoke();
        Destroy(gameObject); // 완전히 사라지면 오브젝트 삭제
    }

    public IEnumerator FadeOutImage()
    {
        float elapsedTime = 0f;
        Color originalColor = _bubbleImage.color;
        Color originalTextColor = _text.color;
        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            _bubbleImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (elapsedTime / fadeOutTime));
            _text.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 1 - (elapsedTime / fadeOutTime));
            yield return null;
        }

        Destroy(gameObject);
    }

    bool isFading = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFading) return;

        if (other.CompareTag("Player"))
        {
            isFading = true;
            Managers.Sound.Play("s1_say_impact3", Sound.Effect);
            Managers.Happy.AddHappiness(_score);
            Camera.main.GetComponent<CameraShake>().Shake();
        }        
    }    


    public override void Init()
    {
       
    }
}
