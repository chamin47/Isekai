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
    [Header("Player")]
    [SerializeField] private PlayerController _player;

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

    private Coroutine _runCo;

    private DialogueContext _context;
    private DialogueCommandFactory _commandFactory;

    private void Start()
    {
        InitializeDatabase();
        InitializeHooks();
        InitializeCommandFactory();
    }

    private void InitializeDatabase()
    {
        if (_database == null)
            _database = gameObject.AddComponent<DialogueDatabaseRuntime>();
        _database.LoadAllFromResources();
    }

    private void InitializeHooks()
    {
        if (_hookProviderBehaviour == null)
            _hookProviderBehaviour = gameObject.AddComponent<AdrianIDHooks>();
        _hooks = _hookProviderBehaviour as IDialogueHookProvider;
    }

    private void InitializeCommandFactory()
    {
        _commandFactory = new DialogueCommandFactory();
    }

    private void InitializeContext()
    {
        _context = new DialogueContext
        {
            Player = _player,
            TextPresenter = _textPresenter,
            CameraService = _cameraService,
            ActorDirector = _actorDirector,
            ChoiceUI = _choiceUI,
            InputPrompt = _inputPrompt,
            Happiness = _happiness,
            LetterBox = _letterBox,
            CameraController = _cameraController,
            BranchTable = _branchTable,
            ChoiceTable = _choiceTable,
            ClickTargetTable = _clickTargetTable
        };
    }

    public void Play(string id, ActorDirectorSimple actorDirector, Action<int> endEvent = null)
    {
        if (_runCo != null)
            StopCoroutine(_runCo);

        // 배우 설정
        _actorDirector = actorDirector;

        // 필요 서비스 및 상태 초기화
        InitializeContext();

        // 종료 이벤트 설정
        _context.OnDialogueEnd = endEvent;

        // 서비스들에 배우 주입
        if (_cameraService != null && _actorDirector != null)
            _cameraService.Init(_actorDirector);
        if (_textPresenter != null && _actorDirector != null)
            _textPresenter.actor = _actorDirector;

        // 대화 시작
        _runCo = StartCoroutine(Run(id));
    }

    void Fire(IEnumerator fx)
    {
        if (fx != null)
            StartCoroutine(fx);
    }

    IEnumerator Run(string id)
    {
        yield return PrepareDialogue();

        while (!string.IsNullOrEmpty(id))
        {
            if (!_database.TryGet(id, out var data))
            {
                Debug.LogError($"Dialogue id not found: {id}");
                yield break;
            }

            Debug.Log($"Executing: {id}");

            // 실행전 실행할 로직 호출
            if (_hooks != null)
                yield return _hooks.OnPreEnter(id);

            // 명령 실행
            var command = _commandFactory.GetCommand(data.eventName?.Trim());
            yield return command.Execute(_context, data);

            id = _context.NextID;
            yield return null;
        }

        Debug.Log("Dialogue End");
    }

    private IEnumerator PrepareDialogue()
    {
        // 카메라 비활성화
        if (_cameraController != null)
            _cameraController.DisableCameraUpdate();

        // 행복 게이지 숨기기
        if (_happiness != null)
            _happiness.Disappear();

		// 레터박스 표시
		float baseH = Screen.height * 0.1f;
		float overshoot = baseH;
		float settle = baseH * 0.85f;

        UILetterboxOverlay letterbox = UILetterboxOverlay.GetOrCreate();

		yield return letterbox.OpenOvershoot(settle, overshoot, 250f);
	}
}
