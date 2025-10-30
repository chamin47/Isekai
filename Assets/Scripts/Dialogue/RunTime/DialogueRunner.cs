using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// 대화 실행기.
/// DialogueData의 event/script/nextID를 해석하여 텍스트/카메라/애니/선택지/입력/을 실행한다.
/// </summary>
public class DialogueRunner : MonoBehaviour
{
	[Header("Entry")]
	[SerializeField] private string _startID;

	[Header("Data")]
	[SerializeField] private DialogueDatabaseRuntime _database;
	[SerializeField] private BranchTable _branchTable;
	[SerializeField] private ChoiceTable _choiceTable;

	[Header("Services")]
	[SerializeField] private DialogueTextPresenter _textPresenter;
	[SerializeField] private CameraService _cameraService;
	[SerializeField] private SimpleChoiceUI _choiceUI;
	[SerializeField] private SimpleInputPrompt _inputPrompt;
	[SerializeField] private KeywordBranchResolver _branchResolver;
	[SerializeField] private ActorDirectorSimple _actorDirector; 

	[Header("Hooks")]
	[SerializeField] private MonoBehaviour _hookProviderBehaviour;
	private IDialogueHookProvider _hooks;

	private Coroutine runCo;

	void Awake()
	{
		if (_database == null) 
			_database = gameObject.AddComponent<DialogueDatabaseRuntime>();

		_database.LoadAllFromResources();

		if (_textPresenter != null && _actorDirector != null && _textPresenter.actor == null)
			_textPresenter.actor = _actorDirector;

		_hooks = _hookProviderBehaviour as IDialogueHookProvider;
	}

	public void Play(string id)
	{
		if (runCo != null) 
			StopCoroutine(runCo);

		runCo = StartCoroutine(Run(id));
	}
	public void Play()
	{
		Play(_startID);
	}
	IEnumerator Start()
	{
		if (!string.IsNullOrEmpty(_startID)) 
			Play(_startID);

		yield break;
	}

	void Fire(IEnumerator fx)
	{
		if (fx != null) 
			StartCoroutine(fx); 
	}

	IEnumerator Run(string id)
	{
        while (!string.IsNullOrEmpty(id))
		{
			if (!_database.TryGet(id, out var row))
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
					goto case "ShowText";

				case "ShowText":
					{
						if (!string.IsNullOrEmpty(row.script))
							yield return _textPresenter?.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "ShowTextStacked":
					{
						if (!string.IsNullOrEmpty(row.script))
							yield return (_textPresenter as DialogueTextPresenter)
								?.ShowTextStacked(row.speaker, row.script, row.animName);

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
						if (_cameraService != null)
							Fire(_cameraService.ZoomTo(scale, dur, anchor)); 

						if (!string.IsNullOrWhiteSpace(row.animName) && _actorDirector != null)
							Fire(_actorDirector.PlayAnim(row.speaker, row.animName)); 

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return _textPresenter?.ShowText(row.speaker, row.script, row.animName); // ???? ?? ???????? ????

						id = row.nextID;
						break;
					}
				case "SCameraZoomIn":
					{
                        var (scale, dur, anchor) = ParamParser.Zoom3(param, 1f, 0.5f, null);
                        if (_cameraService != null)
                            Fire(_cameraService.ZoomTo(scale, dur, anchor));

                        if (!string.IsNullOrWhiteSpace(row.animName) && _actorDirector != null)
                            Fire(_actorDirector.PlayAnim(row.speaker, row.animName));

                        if (!string.IsNullOrWhiteSpace(row.script))
                            yield return _textPresenter?.ShowTextStacked(row.speaker, row.script, row.animName); // ???? ?? ???????? ????

                        id = row.nextID;
                        break;
                    }


                case "CameraZoomOut":
					{
						var (scale, dur, anchor) = ParamParser.Zoom3(param, 1f, 0.5f, null);
						if (_cameraService != null)
							Fire(_cameraService?.ZoomOutTo(scale, dur, anchor)); 

						if (!string.IsNullOrWhiteSpace(row.animName) && _actorDirector != null)
							Fire(_actorDirector.PlayAnim(row.speaker, row.animName)); 

						if (!string.IsNullOrWhiteSpace(row.script) && row.script != "null")
							yield return _textPresenter?.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "CameraShake":
					{
						var (mag, dur) = ParamParser.Floats2(param, 0.2f, 0.4f);
						if (_cameraService != null)
							Fire(_cameraService?.Shake(mag, dur));

						if (!string.IsNullOrWhiteSpace(row.script))
							yield return _textPresenter?.ShowText(row.speaker, row.script, row.animName);

						id = row.nextID;
						break;
					}

				case "PlayAnim":
					{
						Debug.Log("PlayAnim");
						float? dur = ParamParser.NullableFloat(row.eventParam); 
						IEnumerator co = null;
						if (_actorDirector != null)
							co = _actorDirector.PlayAnim(row.speaker, row.animName, dur); 

						if (string.IsNullOrWhiteSpace(row.script) || row.script == "null")
						{
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

						//var letterbox = UILetterboxOverlay.GetOrCreate();

						//float baseH = Screen.height * 0.1f;  
						//float overshoot = baseH;
						//float settle = baseH * 0.85f;        

						//yield return letterbox.CloseOvershoot(settle, overshoot, 170f);
						yield break;
					}

				case "ShowChoice":
					{
						// EventParam = ChoiceID
						var choice = _choiceTable ? _choiceTable.Get(param) : null;
						if (choice == null)
						{
							Debug.LogError($"Choice not found: {param}");
							id = row.nextID; // fallback
							break;
						}

						int sel = -1;
						yield return _choiceUI?.ShowChoices(choice, i => sel = i);
						if (0 <= sel && sel < choice.options.Count)
							id = choice.options[sel].nextID;
						else
							id = row.nextID; 
						break;
					}

				case "WaitForInput":
					{
						// EventParam = BranchID
						string userText = "";
						yield return _inputPrompt?.Prompt(row.script, s => userText = s);

                        if (!string.IsNullOrWhiteSpace(userText))
                        {
                            float sentimentScore = 0f;

                            (_textPresenter as DialogueTextPresenter)?.ClearAllStacked();

							var ballon = _textPresenter.ShowTextTemp("사서", "...", "");

                            // Task를 코루틴으로 래핑해서 대기
                            yield return CoroutineUtil.RunTask(
                                _secretSentimentAnalyer.AnalyzeText(userText, "ko"),
                                result => sentimentScore = result
                            );

							yield return new WaitForSeconds(1f);

                            Destroy(ballon.gameObject);

                            string branchType;
							if(sentimentScore == SecretSentimentAnalyer.WARNING_SCORE)
							{
								branchType = "Ambiguous";
                            }
                            else if (sentimentScore > 0.6f) branchType = "Positive";
                            else if (sentimentScore < -0.6f) branchType = "Negative";
                            else branchType = "Ambiguous";

                            var next = _branchTable ? _branchTable.Resolve(param, branchType) : null;

                            id = next;
                        }
                        else
                        {
                            var type = _branchResolver ? _branchResolver?.Classify(userText) : "Ambiguous";
                            var next = _branchTable ? _branchTable.Resolve(param, type) : null;

                            (_textPresenter as DialogueTextPresenter)?.ClearAllStacked();

                            id = next;
                        }
						break;
					}
					// 플레이어가 특정 시간동안 멈춰있다면 성공 아니면 실패
				case "CheckPlayerCondition":
					{
						var player = FindAnyObjectByType<PlayerController>();
						player.canMove = true;

						// 5초동안 a나 d키를 안누르면 성공
						float waitTime = 5f;
						float elapsed = 0f;
						bool success = true;
						while (elapsed < waitTime)
                        {
							if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                            {
								success = false;
								break;
                            }
							elapsed += Time.deltaTime;
							yield return null;
                        }

						player.canMove = false;
						string branchType = success ? "Positive" : "Negative";
                        var next = _branchTable ? _branchTable.Resolve(param, branchType) : null;

						id = next;
						break;
                    }

            }

			yield return null;
		}
    }

	private SecretSentimentAnalyer _secretSentimentAnalyer = new SecretSentimentAnalyer();
}

public static class CoroutineUtil
{
    public static IEnumerator RunTask<T>(Task<T> task, Action<T> onCompleted)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.Exception != null)
            Debug.LogError(task.Exception);
        else
            onCompleted?.Invoke(task.Result);
    }
}