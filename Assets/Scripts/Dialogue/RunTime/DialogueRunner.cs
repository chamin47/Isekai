using System.Collections;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
	[Header("Entry")]
	public string startID;

	[Header("Data")]
	public DialogueDatabaseRuntime database;   // ???? ????
	public BranchTable branchTable;
	public ChoiceTable choiceTable;

	[Header("Services")]
	public DialogueTextPresenter textPresenter;
	public CameraService cameraService;
	public SimpleChoiceUI choiceUI;
	public SimpleInputPrompt inputPrompt;
	public KeywordBranchResolver branchResolver;
	public ActorDirectorSimple actorDirector; // ????

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
			StartCoroutine(fx); // ???? ????, ???????? ???????? ????
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
					// (???????? ?????????? ShowText ????)
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
							Fire(cameraService.ZoomTo(scale, dur, anchor)); // ????

						// AnimName?? ?????? ???? ?????? ???????? ???? ???? ????
						if (!string.IsNullOrWhiteSpace(row.animName) && actorDirector != null)
							Fire(actorDirector.PlayOnce(row.speaker, row.animName)); // ????/???? ??????

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return textPresenter?.ShowText(row.speaker, row.script, row.animName); // ???? ?? ???????? ????

						id = row.nextID;
						break;
					}

				case "CameraZoomOut":
					{
						var (scale, dur, anchor) = ParamParser.Zoom3(param, 1f, 0.5f, null);
						if (cameraService != null)
							Fire(cameraService?.ZoomOutTo(scale, dur, anchor)); // ????

						if (!string.IsNullOrWhiteSpace(row.animName) && actorDirector != null)
							Fire(actorDirector.PlayOnce(row.speaker, row.animName)); // ????

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
						// null ????: null???? ???? ?????? ????
						float? dur = ParamParser.NullableFloat(row.eventParam); // null ???? ????
						IEnumerator co = null;
						if (actorDirector != null)
							co = actorDirector.PlayOnce(row.speaker, row.animName, dur); // ???????? dur null???? ???????? ????

						if (string.IsNullOrWhiteSpace(row.script) || row.script == "null")
						{
							// ???? ?????? ???? ?????? ?????????? ????????
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

						FindAnyObjectByType<PlayerController>().canMove = true;

						var letterbox = UILetterboxOverlay.GetOrCreate();

						float baseH = Screen.height * 0.1f;  // ???? ???? ????
						float overshoot = baseH;
						float settle = baseH * 0.85f;        // 10 ?? 7 ???????? 70%

						yield return letterbox.CloseOvershoot(settle, overshoot, 170f);
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
							id = row.nextID; // ???? ???? fallback
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

			// ????????: ???????? ????
			yield return null;
		}
	}
}
