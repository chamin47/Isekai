using System;
using System.Collections;
using UnityEngine;

public interface ITextPresenter
{
	// animName은 대사 중 애니메이션 출력용
	IEnumerator ShowText(string speaker, string text, string animName);
}

public interface ICameraService
{
	IEnumerator ZoomTo(float targetScale, float durationSec);
	IEnumerator ZoomOutTo(float targetScale, float durationSec);
	IEnumerator Shake(float magnitude, float durationSec);
}

public interface IChoiceUI
{
	// 선택된 index 콜백
	IEnumerator ShowChoices(ChoiceTable.ChoiceRow row, Action<int> onSelected);
}

public interface IInputPrompt
{
	// 텍스트 입력을 받아 콜백
	IEnumerator Prompt(string prompt, Action<string> onDone);
}

public interface IBranchResolver
{
	// 입력 텍스트 → InputType(Positive/Ambiguous/Negative 등)으로 분류
	string Classify(string userInput);
}

public interface IActorDirector
{
	// 에디터에서 클립의 Loop Time 체크 여부로 루프 or 한번 재생 결정
	IEnumerator PlayAnim(string speaker, string animName, float? durationSec = null);
}

public interface IDialogueHookProvider
{
	IEnumerator OnPreEnter(string id);
}

public interface ISpeakerAnchorResolver
{
	Transform ResolveAnchor(string speaker);
}
