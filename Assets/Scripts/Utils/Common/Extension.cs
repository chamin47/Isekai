using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class WaitForSecondsCache
{
    private static Dictionary<float, WaitForSeconds> cache = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds Get(float time)
    {
        if(cache != null && cache.Count > 100)
        {
            cache.Clear();
        }                 
        
        if (!cache.TryGetValue(time, out var wait))
        {
            wait = new WaitForSeconds(time);
            cache[time] = wait;
        }
        return wait;
    }

    public static void Clear()
    {
        cache.Clear();
    }
}


public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Util.GetOrAddComponent<T>(go);
	}

    #region ListUtill
    /// <summary>
    /// 리스트를 랜덤으로 섞는다
    /// </summary>
    public static void Shuffle<T>(this List<T> list)
	{
		System.Random rand = new System.Random();

		for(int i = list.Count - 1; i > 0; i--)
		{
			int randIdx = rand.Next(i + 1);
			(list[i], list[randIdx]) = (list[randIdx], list[i]);
        }
	}

	public static List<T> GetRandomN<T>(this List<T> list, int n)
	{
		List<T> result = new List<T>(list);
        result.Shuffle();
        return result.GetRange(0, n);
    }
    #endregion

    #region TextMeshPro
    /// <summary>
    /// <.*?> : <로 시작하고 >로 끝나는 모든 문자열 삭제
    /// </summary>
    public static string RemoveRichTextTags(this string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    /// <summary>
    /// text를 마스킹된 문자로 변경한다
    /// </summary>
    public static string GetRandomMaskedText(int length, in string maskCharacters = "#*@$%&!")
    {
        StringBuilder randomText = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            randomText.Append(maskCharacters[UnityEngine.Random.Range(0, maskCharacters.Length)]);
        }
        return randomText.ToString();
    }

    /// <summary>
    /// text를 마스킹된 문자로 변경한다
    /// </summary>
    /// <param name="text"></param>

    public static string GetRandomMaskedText(this string text, in string maskCharacters = "#*@$%&!")
    {
        return GetRandomMaskedText(text.Length, maskCharacters);
    }

    /// <summary>
    /// text에서 랜덤으로 n개 만큼 마스킹된 문자로 변경한다
    /// </summary>
    public static string GetNRandomMaskedText(this string text, int n, in string maskCharacters = "#*@$%&!")
    {
        StringBuilder stringBuilder = new StringBuilder(text);
        List<int> randomIndices = Enumerable.Range(0, text.Length).ToList();
        randomIndices.Shuffle();

        for(int i = 0; i < n; i++)
        {
            int index = randomIndices[i];
            stringBuilder[index] = maskCharacters[UnityEngine.Random.Range(0, maskCharacters.Length)];
        }
        return stringBuilder.ToString();
    }

    //Rich Text 태그를 포함한 문자열을 타이핑 효과로 출력하는 코루틴
    public static IEnumerator CoTypeEffectWithRichText(this TMP_Text textComponent, string content, float typingSpeed, float waitTime = 0.5f)
    {
        textComponent.text = ""; // 초기화

        int stringIndex = 0;

        WaitForSeconds typeInterval = WaitForSecondsCache.Get(typingSpeed);

        while (stringIndex < content.Length)
        {
            char c = content[stringIndex];

            if (c == '<')
            {
                int closeIndex = content.IndexOf('>', stringIndex);

                if (closeIndex == -1){ // 태그가 아닌경우 연속 태그를 고려하면 수정해 줘야되긴한다
                    textComponent.text += c;
                }
                else
                {
                    textComponent.text += content.Substring(stringIndex, closeIndex - stringIndex + 1);
                    stringIndex = closeIndex; // 태그 끝까지 건너뛰기
                }
            }
            else
            {
                textComponent.text += c;
            }

            stringIndex++;
            yield return typeInterval;
        }

        yield return WaitForSecondsCache.Get(waitTime); // 효과 마무리 시간
    }

    public static IEnumerator CoTypeEffectWithRichText(this TMP_Text textComponent, string content, float typingSpeed, Action soundAction, float waitTime = 0.5f)
    {
        int typingCount = 0;
        textComponent.text = ""; // 초기화

        int stringIndex = 0;

        WaitForSeconds typeInterval = WaitForSecondsCache.Get(typingSpeed);

        while (stringIndex < content.Length)
        {
            char c = content[stringIndex];

            if (c == '<') // Rich Text 태그 시작
            {
                int closeIndex = content.IndexOf('>', stringIndex);
                if (closeIndex == -1) // 태그가 정상적으로 닫히지 않음
                {
                    textComponent.text += c;
                }
                else
                {
                    textComponent.text += content.Substring(stringIndex, closeIndex - stringIndex + 1);
                    stringIndex = closeIndex; // 태그 끝까지 건너뛰기
                }
            }
            else
            {
                textComponent.text += c;
            }

            if (c != ' ' || c != '\n')
            {
                typingCount += 1;
                if (typingCount % 2 == 0)
                {
                    soundAction?.Invoke();
                }
            }

            stringIndex++;
            yield return typeInterval;
        }

        yield return WaitForSecondsCache.Get(waitTime); // 효과 마무리 시간
    }


    public static IEnumerator CoBlinkText(this TMP_Text text, int blinkCount, float blinkDuration, Action soundAction)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            soundAction?.Invoke();

            yield return CoFadeIn(text, blinkDuration / 2);
            yield return CoFadeOut(text, blinkDuration / 2);
        }
    }
    public static IEnumerator CoTypingEffect(this TMP_Text text, string message, float textSpeed,Action soundAction)
    {
        int typingCount = 0;

        text.text = "";
        foreach (char c in message)
        {
            text.text += c;
            if (c != ' ' && c != '\n')
            {
                typingCount += 1;
                if (typingCount % 2 == 0)
                {
                    soundAction?.Invoke();
                }
            }
            yield return WaitForSecondsCache.Get(textSpeed); // 타자 치는 속도 조절 가능
        }
    }

    public static IEnumerator CoTypingEffectPerChar(this TMP_Text text, string message, float textSpeed, bool playSound, Action soundAction, bool spaceSkip = false, bool resetText = true)
    {
        int typingCount = 0;
        if(resetText)
            text.text = "";
        foreach (char c in message)
        {
            text.text += c;
            if (playSound && (c != ' ' && c != '\n'))
            {
                typingCount += 1;
                soundAction?.Invoke();
            }
            if (spaceSkip && c == ' ')
            {
                continue;
            }
            yield return WaitForSecondsCache.Get(textSpeed); // 타자 치는 속도 조절 가능
        }
    }

    public static IEnumerator CoTypingEffect(this TMP_Text text, string message, float typingSpeed, bool spaceSkip = false)
    {
        text.text = "";
        foreach (char letter in message)
        {
            text.text += letter;
            if (spaceSkip && letter == ' ')
            {
                continue;
            }
            yield return WaitForSecondsCache.Get(typingSpeed);
        }
    }
	#endregion

	#region TextAnimator
	/// <summary>
	/// Text Animator 전용 리치태그 제거기 '{}' 와 '<>'를 포함한 태그 안의 모든 글자를 제거한다.
	/// </summary>
	public static string RemoveRichTags(this string input)
	{
		if (string.IsNullOrEmpty(input))
			return string.Empty;

		System.Text.StringBuilder sb = new();
		bool insideCurly = false;
		bool insideAngle = false;

		foreach (char c in input)
		{
			if (c == '{') { insideCurly = true; continue; }
			if (c == '}') { insideCurly = false; continue; }
			if (c == '<') { insideAngle = true; continue; }
			if (c == '>') { insideAngle = false; continue; }

			// 태그 영역이 아닐 때만 문자 추가
			if (!insideCurly && !insideAngle)
				sb.Append(c);
		}

		return sb.ToString().Trim();
	}

	public static string KeepOnlySizeTag(this string input)
	{
		if (string.IsNullOrEmpty(input))
			return string.Empty;

		System.Text.StringBuilder sb = new();
		System.Text.StringBuilder temp = new();
		bool insideCurly = false;
		bool insideAngle = false;

		foreach (char c in input)
		{
			if (c == '{') { insideCurly = true; continue; }
			if (c == '}') { insideCurly = false; continue; }

			if (insideCurly)
				continue;

			if (c == '<')
			{
				insideAngle = true;
				temp.Clear();
				continue;
			}

			if (c == '>')
			{
				insideAngle = false;

				string tag = temp.ToString();

				// "<size=60%>" 형태만 살린다.
				if (tag.StartsWith("size=") && tag.EndsWith("%"))
				{
					sb.Append('<').Append(tag).Append('>');
				}

				continue;
			}

			if (insideAngle)
			{
				temp.Append(c);
				continue;
			}

			sb.Append(c);
		}

		return sb.ToString().Trim();
	}
	#endregion

	/// <summary>
	/// 알파값을 감소시킨다
	/// </summary>
	public static IEnumerator CoFadeIn(this Graphic graphic, float fadeTime, float waitBefore = 0f, float waitAfter = 0f)
    {
        if (waitBefore > 0f) yield return WaitForSecondsCache.Get(waitBefore);

        Color color = graphic.color;
        float startAlpha = 1;
        float targetAlpha = 0;
        float elapsedTime = 0;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            graphic.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        graphic.color = color;

        if (waitAfter > 0f) yield return WaitForSecondsCache.Get(waitAfter);
    }

    /// <summary>
    /// 알파값을 증가시킨다
    /// </summary>
    public static IEnumerator CoFadeOut(this Graphic graphic, float fadeTime, float waitBefore = 0f, float waitAfter = 0f)
    {
        if (waitBefore > 0f) yield return WaitForSecondsCache.Get(waitBefore);

        Color color = graphic.color;
        float startAlpha = 0;
        float targetAlpha = 1;
        float elapsedTime = 0;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            graphic.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        graphic.color = color;

        if (waitAfter > 0f) yield return WaitForSecondsCache.Get(waitAfter);
    }

    public static IEnumerator CoFadeIn(this SpriteRenderer sprite, float fadeTime, float waitBefore = 0f, float waitAfter = 0f)
    {
        if (waitBefore > 0f) yield return WaitForSecondsCache.Get(waitBefore);

        Color color = sprite.color;
        float startAlpha = 1;
        float targetAlpha = 0;
        float elapsedTime = 0;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            sprite.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        sprite.color = color;

        if (waitAfter > 0f) yield return WaitForSecondsCache.Get(waitAfter);
    }
    public static IEnumerator CoFadeOut(this SpriteRenderer sprite, float fadeTime, float waitBefore = 0f, float waitAfter = 0f)
    {
        if (waitBefore > 0f) yield return WaitForSecondsCache.Get(waitBefore);

        Color color = sprite.color;
        float startAlpha = 0;
        float targetAlpha = 1;
        float elapsedTime = 0;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            sprite.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        sprite.color = color;

        if (waitAfter > 0f) yield return WaitForSecondsCache.Get(waitAfter);
    }

    public static IEnumerator CoFadeIn(this CanvasGroup canvasGroup, float fadeTime, float waitBefore = 0f, float waitAfter = 0f)
    {
        if (waitBefore > 0f) yield return WaitForSecondsCache.Get(waitBefore);
        float startAlpha = 1;
        float targetAlpha = 0;
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        if (waitAfter > 0f) yield return WaitForSecondsCache.Get(waitAfter);
    }

    public static IEnumerator CoFadeOut(this CanvasGroup canvasGroup, float fadeTime, float waitBefore = 0f, float waitAfter = 0f)
    {
        if (waitBefore > 0f) yield return WaitForSecondsCache.Get(waitBefore);
        float startAlpha = 0;
        float targetAlpha = 1;
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        if (waitAfter > 0f) yield return WaitForSecondsCache.Get(waitAfter);
    }

    public static IEnumerator CoFillImage(this Image image, float targetFill, float duration)
    {
        float startFill = image.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime / duration);
            yield return null;
        }

        image.fillAmount = targetFill;
    }

	public static IEnumerator FadeCanvas(this CanvasGroup cg, float to, float time)
	{
		float from = cg.alpha;
		float t = 0f;
		while (t < time)
		{
			t += Time.deltaTime;
			cg.alpha = Mathf.Lerp(from, to, t / time);
			yield return null;
		}
		cg.alpha = to;
	}
}
