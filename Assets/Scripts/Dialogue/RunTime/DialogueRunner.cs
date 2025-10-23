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
	public ActorDirectorSimple actorDirector; // 선택

	[Header("Hooks")]
	[SerializeField] MonoBehaviour hookProviderBehaviour;
	IDialogueHookProvider _hooks;

	Coroutine runCo;

	void Awake()
	{
		if (database == null) 
			database = gameObject.AddComponent<DialogueDatabaseRuntime>();

		database.LoadAllFromResources();

		if (textPresenter != null && actorDirector != null && textPresenter.actor == null)
			textPresenter.actor = actorDirector;

		_hooks = hookProviderBehaviour as IDialogueHookProvider;
	}

	public void Play(string id)
	{
		if (runCo != null) 
			StopCoroutine(runCo);

		runCo = StartCoroutine(Run(id));
	}
	public void Play()
	{
		Play(startID);
	}
	IEnumerator Start()
	{
		if (!string.IsNullOrEmpty(startID)) 
			Play(startID);

		yield break;
	}

	void Fire(IEnumerator fx)
	{
		if (fx != null) 
			StartCoroutine(fx); // 병렬 실행, 여기서는 대기하지 않음
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

			Debug.Log(id);

			if (_hooks != null)
				yield return _hooks.OnPreEnter(id);

			var evt = row.eventName?.Trim();
			var param = row.eventParam?.Trim();

			switch (evt)
			{
				default:
					// (이벤트가 비어있으면 ShowText 취급)
					goto case "ShowText";

				case "ShowText":
					{
						yield return textPresenter?.ShowText(row.speaker, row.script, row.animName);
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
						var (scale, dur, anchor) = ParamParser.Zoom3(param, 1f, 0.5f, null);
						if (cameraService != null)
							Fire(cameraService.ZoomTo(scale, dur, anchor)); // 병렬

						// AnimName이 있으면 대사 유무와 관계없이 애니 동시 재생
						if (!string.IsNullOrWhiteSpace(row.animName) && actorDirector != null)
							Fire(actorDirector.PlayOnce(row.speaker, row.animName)); // 루프/포즈 유지형

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return textPresenter?.ShowText(row.speaker, row.script, row.animName); // 병렬 중 텍스트만 대기

						id = row.nextID;
						break;
					}

				case "CameraZoomOut":
					{
						var (scale, dur, anchor) = ParamParser.Zoom3(param, 1f, 0.5f, null);
						if (cameraService != null)
							Fire(cameraService?.ZoomOutTo(scale, dur, anchor)); // 병렬

						if (!string.IsNullOrWhiteSpace(row.animName) && actorDirector != null)
							Fire(actorDirector.PlayOnce(row.speaker, row.animName)); // 병렬

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return textPresenter?.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "CameraShake":
					{
						var (mag, dur) = ParamParser.Floats2(param, 0.2f, 0.4f);
						if (cameraService != null)
							Fire(cameraService?.Shake(mag, dur));

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return textPresenter?.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "PlayAnim":
					{
						Debug.Log("PlayAnim");
						// null 허용: null이면 클립 길이로 대기
						float? dur = ParamParser.NullableFloat(row.eventParam); // null 또는 숫자
						IEnumerator co = null;
						if (actorDirector != null)
							co = actorDirector.PlayOnce(row.speaker, row.animName, dur); // 내부에서 dur null이면 클립길이 사용

						if (string.IsNullOrWhiteSpace(row.script) || row.script == "null")
						{
							// 대사 없으면 애니 끝까지 기다렸다가 다음으로
							if (co != null) 
								yield return co;
						}

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
						yield return choiceUI?.ShowChoices(choice, i => sel = i);
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
						yield return inputPrompt?.Prompt(row.script, s => userText = s);
						var type = branchResolver ? branchResolver?.Classify(userText) : "Ambiguous";
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
