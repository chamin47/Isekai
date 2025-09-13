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

    /// <summary>
    /// �⺻���� Ŭ�� �̺�Ʈ�� �߰��մϴ�.
    /// </summary>
	public static void BindEvent(this GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
	{
		UI_Base.BindEvent(go, action, type);
	}

	public static bool IsValid(this GameObject go)
	{
		return go != null && go.activeSelf;
	}

    #region ListUtill
    /// <summary>
    /// ����Ʈ�� �������� ���´�
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
    /// <.*?> : <�� �����ϰ� >�� ������ ��� ���ڿ� ����
    /// </summary>
    public static string RemoveRichTextTags(this string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    /// <summary>
    /// text�� ����ŷ�� ���ڷ� �����Ѵ�
    /// </summary>
    public static string GetRandomMaskedText(int length, string maskCharacters = "#*@$%&!")
    {
        StringBuilder randomText = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            randomText.Append(maskCharacters[UnityEngine.Random.Range(0, maskCharacters.Length)]);
        }
        return randomText.ToString();
    }

    /// <summary>
    /// text�� ����ŷ�� ���ڷ� �����Ѵ�
    /// </summary>
    /// <param name="text"></param>

    public static string GetRandomMaskedText(this string text, string maskCharacters = "#*@$%&!")
    {
        return GetRandomMaskedText(text.Length, maskCharacters);
    }

    /// <summary>
    /// text���� �������� n�� ��ŭ ����ŷ�� ���ڷ� �����Ѵ�
    /// </summary>
    public static string GetNRandomMaskedText(this string text, int n, string maskCharacters = "#*@$%&!")
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

    //Rich Text �±׸� ������ ���ڿ��� Ÿ���� ȿ���� ����ϴ� �ڷ�ƾ
    public static IEnumerator CoTypeEffectWithRichText(this TMP_Text textComponent, string content, float typingSpeed)
    {
        textComponent.text = ""; // �ʱ�ȭ

        int stringIndex = 0;
        while (stringIndex < content.Length)
        {
            char c = content[stringIndex];

            if (c == '<') // Rich Text �±� ����
            {
                int closeIndex = content.IndexOf('>', stringIndex);
                if (closeIndex == -1) // �±װ� ���������� ������ ����
                {
                    textComponent.text += c;
                }
                else
                {
                    textComponent.text += content.Substring(stringIndex, closeIndex - stringIndex + 1);
                    stringIndex = closeIndex; // �±� ������ �ǳʶٱ�
                }
            }
            else
            {
                textComponent.text += c;
            }

            stringIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(0.5f); // ȿ�� ������ �ð�
    }

    public static IEnumerator CoTypeEffectWithRichText(this TMP_Text textComponent, string content, float typingSpeed, string soundKey = null)
    {
        int typingCount = 0;
        textComponent.text = ""; // �ʱ�ȭ

        int stringIndex = 0;
        while (stringIndex < content.Length)
        {
            char c = content[stringIndex];

            if (c == '<') // Rich Text �±� ����
            {
                int closeIndex = content.IndexOf('>', stringIndex);
                if (closeIndex == -1) // �±װ� ���������� ������ ����
                {
                    textComponent.text += c;
                }
                else
                {
                    textComponent.text += content.Substring(stringIndex, closeIndex - stringIndex + 1);
                    stringIndex = closeIndex; // �±� ������ �ǳʶٱ�
                }
            }
            else
            {
                textComponent.text += c;
            }

            if (soundKey != null && (c != ' ' || c != '\n'))
            {
                typingCount += 1;
                if (typingCount % 2 == 0)
                {
                    Managers.Sound.Play(soundKey, Sound.Effect);
                }
            }

            stringIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(0.5f); // ȿ�� ������ �ð�
    }


    public static IEnumerator CoBlinkText(this TMP_Text text, int blinkCount, float blinkDuration, string soundKey = "")
    {
        for (int i = 0; i < blinkCount; i++)
        {
            if(soundKey != "")
            {
                Managers.Sound.Play(soundKey, Sound.Effect);
            }
            yield return CoFadeIn(text, blinkDuration / 2);
            yield return CoFadeOut(text, blinkDuration / 2);
        }
    }
    public static IEnumerator CoTypingEffect(this TMP_Text text, string message, float textSpeed, bool playSound, string soundKey)
    {
        int typingCount = 0;

        text.text = "";
        foreach (char c in message)
        {
            text.text += c;
            if (playSound && (c != ' ' && c != '\n'))
            {
                typingCount += 1;
                if (typingCount % 2 == 0)
                {
                    Managers.Sound.Play(soundKey, Sound.Effect);
                }
            }
            yield return WaitForSecondsCache.Get(textSpeed); // Ÿ�� ġ�� �ӵ� ���� ����
        }
    }

    public static IEnumerator CoTypingEffectPerChar(this TMP_Text text, string message, float textSpeed, bool playSound, string soundKey, bool spaceSkip = false, bool resetText = true)
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
                Managers.Sound.Play(soundKey, Sound.Effect);
                
            }
            if (spaceSkip && c == ' ')
            {
                continue;
            }
            yield return WaitForSecondsCache.Get(textSpeed); // Ÿ�� ġ�� �ӵ� ���� ����
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



    /// <summary>
    /// ���İ��� ���ҽ�Ų��
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
    /// ���İ��� ������Ų��
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
