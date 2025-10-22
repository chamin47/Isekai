using System;
using System.Collections;
using UnityEngine;

public interface ITextPresenter
{
	// animName�� ��� �� ������/���� ������ ��(������ ����)
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
	// ���õ� index �ݹ�
	IEnumerator ShowChoices(ChoiceTable.ChoiceRow row, Action<int> onSelected);
}

public interface IInputPrompt
{
	// �ؽ�Ʈ �Է��� �޾� �ݹ�
	IEnumerator Prompt(string prompt, Action<string> onDone);
}

public interface IBranchResolver
{
	// �Է� �ؽ�Ʈ �� InputType(Positive/Ambiguous/Negative ��)���� �з�
	string Classify(string userInput);
}

public interface IActorDirector
{
	// ���ϴ� ���� ����/ǥ�� ����(������)
	void SetPose(string speaker, string animName);
	// PlayAnim �̺�Ʈ�� N��¥�� �ܹ� ����
	IEnumerator PlayOnce(string speaker, string animName, float durationSec);
}
