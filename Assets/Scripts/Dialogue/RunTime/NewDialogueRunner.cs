using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 대화 실행기.
/// DialogueData의 event/script/nextID를 해석하여 텍스트/카메라/애니/선택지/입력/을 실행한다.
/// </summary>
public class NewDialogueRunner : MonoBehaviour
{
    [Header("Entry")]
    [SerializeField] private string _startID;

    [Header("Data")]
    [SerializeField] private DialogueDatabaseRuntime _database;
    [SerializeField] private BranchTable _branchTable;
    [SerializeField] private ChoiceTable _choiceTable;
    [SerializeField] private ClickTargetTable _clickTargetTable;

    [Header("Services")]
    [SerializeField] private DialogueTextPresenter _textPresenter;
    [SerializeField] private CameraService _cameraService;
    [SerializeField] private SimpleChoiceUI _choiceUI;
    [SerializeField] private SimpleInputPrompt _inputPrompt;
    [SerializeField] private ActorDirectorSimple _actorDirector;
    [SerializeField] private HappinessHUD _happiness;
    [SerializeField] private UI_LetterBox _letterBox;
    [SerializeField] private CameraController _cameraController;

    [Header("Hooks")]
    [SerializeField] private MonoBehaviour _hookProviderBehaviour;
    private IDialogueHookProvider _hooks;

    private Coroutine runCo;

    [Header("Events")]
    private Action OnDialogueEnd;
    void Awake()
    {
        if (_database == null)
            _database = gameObject.AddComponent<DialogueDatabaseRuntime>();

        _database.LoadAllFromResources();

        if (_database == null)
            _hookProviderBehaviour = gameObject.AddComponent<AdrianIDHooks>();

        _hooks = _hookProviderBehaviour as IDialogueHookProvider;
    }

    public void Play(string id, ActorDirectorSimple actorDirector, Action endEvent = null)
    {
        if (runCo != null)
            StopCoroutine(runCo);

        OnDialogueEnd += endEvent;
        _actorDirector = actorDirector;
        if (_cameraService != null && _actorDirector != null)
            _cameraService.Init(_actorDirector);
        if (_textPresenter != null && _actorDirector != null)
            _textPresenter.actor = _actorDirector;

        runCo = StartCoroutine(Run(id));
    }

    void Fire(IEnumerator fx)
    {
        if (fx != null)
            StartCoroutine(fx);
    }

    IEnumerator Run(string id)
    {
        if (_cameraController != null)
            _cameraController.DisableCameraUpdate();
        if (_happiness != null)
            _happiness.gameObject.SetActive(false);
        _letterBox.gameObject.SetActive(true);
        yield return _letterBox.ShowLetterBox();
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
                    {
                        id = row.nextID;
                        break;
                    }

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
                case "StopPatrol":
                    {
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
                case "CameraZoomIn":
                    {
                        var (scale, dur, anchor) = ParamParser.Zoom3(param, 1f, 0.5f, null);
                        if (_cameraService != null)
                            Fire(_cameraService.ZoomTo(scale, dur, anchor));

                        if (!string.IsNullOrWhiteSpace(row.animName) && _actorDirector != null)
                            Fire(_actorDirector.PlayAnim(row.speaker, row.animName));

                        if (!string.IsNullOrWhiteSpace(row.script))
                            yield return _textPresenter?.ShowText(row.speaker, row.script, row.animName); 

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
                        OnDialogueEnd?.Invoke();
                        OnDialogueEnd = null;

                        _happiness.gameObject.SetActive(true);
                        if (_cameraController != null)
                            _cameraController.EnableCameraUpdate();
                        yield return _letterBox.HideLetterBox();
                        yield break;
                    }
                case "WaitClick":
                    {
                        string eventParam = row.eventParam?.Trim().ToLower();
                        int sel = -1;
                        if (eventParam == "click_flower")
                        {
                            var ui = Managers.UI.ShowPopupUI<UI_Click_Flower>();
                            yield return ui.ClickFlower((i) => sel = i);
                        }
                        else if(eventParam == "click_lady")
                        {
                            var ladiesController = _actorDirector.GetComponent<LadiesController>();
                            yield return ladiesController.ClickLady((i) => sel = i);
                        }

                        yield return null;
                        var choice = _clickTargetTable ? _clickTargetTable.Get(param) : null;
                        if(choice == null)
                        {
                            Debug.LogError($"Click Target not found: {param}");
                            id = row.nextID; // fallback
                            break;
                        }
                        Debug.Log(sel);
                        id = choice.options[sel].nextID;
                        break;
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

                        var player = FindAnyObjectByType<PlayerController>();

                        bool isLeft = player.transform.position.x < _actorDirector.transform.position.x;
                        if (!isLeft)
                            _choiceUI.transform.position = player.transform.position + new Vector3(0, 1.0f, 0) + new Vector3(1.9f, 0, 0);
                        else
                            _choiceUI.transform.position = player.transform.position + new Vector3(0, 1.0f, 0) + new Vector3(-1.9f, 0, 0);

                        int sel = -1;
                        yield return _choiceUI?.ShowChoices(choice, i => sel = i);
                        if (0 <= sel && sel < choice.options.Count)
                            id = choice.options[sel].nextID;
                        else
                            id = row.nextID;

                        (_textPresenter as DialogueTextPresenter)?.ClearAllStacked();
                        break;
                    }
                case "ModifyHappyGauge":
                    {
                        float amount = ParamParser.ToFloat(param, 0f);
                        if (_happiness != null)
                        {
                            _happiness.gameObject.SetActive(true);
                            _happiness.ChangeHappiness(amount);
                            Managers.Sound.Play("happiness_gauge", Sound.Effect);
                        }

                        id = row.nextID;
                        break;
                    }

                case "WaitForInput":
                    {
                        // EventParam = BranchID
                        string userText = "";
                        yield return _inputPrompt?.Prompt(row.script, s => userText = s);
                        var next = _branchTable ? _branchTable.Resolve(param, "Default") : null;
                        (_textPresenter as DialogueTextPresenter)?.ClearAllStacked();
                        break;
                    }
                // 플레이어가 특정 시간동안 멈춰있다면 성공 아니면 실패
                case "CheckPlayerCondition":
                    {
                        var player = FindAnyObjectByType<PlayerController>();
                        player.canMove = true;

                        Debug.Log("CheckPlayerCondition started");
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
                        string branchType = success ? "Success" : "Fail";
                        var next = _branchTable ? _branchTable.Resolve(param, branchType) : null;

                        Debug.Log(next);
                        id = next;
                        break;
                    }
                    // 특정 UI를 보여준다.
                case "ShowUI":
                    {
                        var (name, dur) = ParamParser.UI(param);
                        if(name == "portrait")
                        {
                            var ui = Managers.UI.ShowPopupUI<UI_Portrait>();
                            yield return ui.Disapear(dur);
                        }

                        id = row.nextID;
                        break;
                    }
            }
            yield return null;
        }
    }
}
