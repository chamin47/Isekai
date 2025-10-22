using System;
using System.Collections;
using UnityEngine;

public interface ITextPresenter
{
	// animName은 대사 중 ‘포즈/감정 유지’ 용(있으면 적용)
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
	// 말하는 순간 포즈/표정 적용(있으면)
	void SetPose(string speaker, string animName);
	// PlayAnim 이벤트로 N초짜리 단발 동작
	IEnumerator PlayOnce(string speaker, string animName, float durationSec);
}
