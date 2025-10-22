using System.Collections;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
	[Header("Entry")]
	public string startID;

	[Header("Data")]
	public DialogueDatabaseRuntime database;   // 자동 로딩
	public BranchTable branchTable;
	public ChoiceTable choiceTable;

	[Header("Services")]
	public DialogueTextPresenter textPresenter;
	public CameraService cameraService;
	public SimpleChoiceUI choiceUI;
	public SimpleInputPrompt inputPrompt;
	public KeywordBranchResolver branchResolver;
	public ActorDirectorStub actorDirector; // 선택

	Coroutine runCo;

	void Awake()
	{
		if (!database) database = gameObject.AddComponent<DialogueDatabaseRuntime>();
		database.LoadAllFromResources();

		if (textPresenter && actorDirector && textPresenter.actor == null)
			textPresenter.actor = actorDirector;
	}

	public void Play(string id)
	{
		if (runCo != null) StopCoroutine(runCo);
		runCo = StartCoroutine(Run(id));
	}
	public void Play() => Play(startID);

	IEnumerator Start()
	{
		if (!string.IsNullOrEmpty(startID)) Play(startID);
		yield break;
	}

	void Fire(IEnumerator fx)
	{
		if (fx != null) StartCoroutine(fx); // 병렬 실행, 여기서는 대기하지 않음
	}

	IEnumerator Run(string id)
	{
		while (!string.IsNullOrEmpty(id))
		{
			if (!database.TryGet(id, out var row))
			{
				Debug.LogError($"Dialogue id not found: {id}");
				yield break;
			}

			var evt = row.eventName?.Trim();
			var param = row.eventParam?.Trim();

			switch (evt)
			{
				default:
					// (이벤트가 비어있으면 ShowText 취급)
					goto case "ShowText";

				case "ShowText":
					{
						yield return textPresenter.ShowText(row.speaker, row.script, row.animName);
						id = row.nextID;
						break;
					}

				case "WaitTimer":
					{
						var t = ParamParser.ToFloat(param, 0f);
						if (t > 0) yield return new WaitForSeconds(t);
						id = row.nextID;
						break;
					}

				case "CameraZoomIn":
					{
						// 예: "2.5,0.8" -> (목표스케일, 지속시간)
						var (target, dur) = ParamParser.Floats2(param, 1f, 0.5f);
						if (cameraService) Fire(cameraService.ZoomTo(target, dur));

						// 텍스트가 있으면 정상적으로 출력/클릭 대기 (그 사이 카메라 연출은 병렬로 진행)
						if (!string.IsNullOrWhiteSpace(row.script))
							yield return textPresenter.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "CameraZoomOut":
					{
						var (target, dur) = ParamParser.Floats2(param, 1f, 0.5f);
						if (cameraService) Fire(cameraService.ZoomOutTo(target, dur));

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return textPresenter.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "CameraShake":
					{
						var (mag, dur) = ParamParser.Floats2(param, 0.2f, 0.4f);
						if (cameraService) Fire(cameraService.Shake(mag, dur));

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return textPresenter.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "PlayAnim":
					{
						// eventParam에는 지속시간만: "0.6"
						var (dur, _) = ParamParser.Floats2(row.eventParam, 0.5f, 0f);
						if (actorDirector) Fire(actorDirector.PlayOnce(row.speaker, row.animName, dur));

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return textPresenter.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "JUMP":
					{
						id = row.nextID;
						break;
					}

				case "EndScript":
					{
						Debug.Log("Dialogue ended.");
						yield break;
					}

				case "ShowChoice":
					{
						// EventParam = ChoiceID
						var choice = choiceTable ? choiceTable.Get(param) : null;
						if (choice == null)
						{
							Debug.LogError($"Choice not found: {param}");
							id = row.nextID; // fallback
							break;
						}

						int sel = -1;
						yield return choiceUI.ShowChoices(choice, i => sel = i);
						if (0 <= sel && sel < choice.options.Count)
							id = choice.options[sel].nextID;
						else
							id = row.nextID; // 혹시 모를 fallback
						break;
					}

				case "WaitForInput":
					{
						// EventParam = BranchID
						string userText = "";
						yield return inputPrompt.Prompt(row.script, s => userText = s);
						var type = branchResolver ? branchResolver.Classify(userText) : "Ambiguous";
						var next = branchTable ? branchTable.Resolve(param, type) : null;

						id = next;
						break;
					}
			}

			// 안전장치: 무한루프 방지
			yield return null;
		}
	}
}
