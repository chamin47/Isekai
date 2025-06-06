using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantController : MonoBehaviour
{
    public enum GiantState
    {
        Idle,
        Walk,
        Attack,
    }

    private GiantState _currentState = GiantState.Idle;

    [SerializeField] private float _moveSpeed = 2.0f;
    [SerializeField] private PlayerController _player;
    [SerializeField] private SpriteRenderer _playerBackground;
    [SerializeField] private Animator _ani;
    private Coroutine _walkCoroutine;

    private bool _canMove = false;
    public void ChangeState(GiantState state)
    {
        if(_currentState == state)
            return;

        _currentState = state;

        switch (state)
        {
            case GiantState.Idle:
                _canMove = false;
                break;
            case GiantState.Walk:
                _canMove = true;
                break;
            case GiantState.Attack:
                _canMove = false;
                if(_walkCoroutine != null)
                {
                    StopCoroutine(_walkCoroutine);
                    _walkCoroutine = null;
                }
                StartCoroutine(AttackSequnce());
                break;
            default:
                Debug.LogWarning("Unknown giant state: " + state);
                break;
        }

        ChangeAnimateState();
    }

    private List<string> dialogues = new List<string>
    {
        "�װ� ���� ���Ҵ�����, �� �Ƿ��� �ƴϾ�.",
        "�� ���� ����� ���� ������ �ϰھ�?",
        "�װ� ������ �� �� �� �ִ� �ž�. �װ� Ư���� ���ϴ� �� �ƴ���.",
        "�����ε� �װ� �� ���� �� ������ ������ �� �𸣰ھ�.",
        "�� �ᱹ ȥ�� �����ž�."
    };

    [SerializeField] private float _bubbleDropHeight = 9.0f; // �������� ���� y
    [SerializeField] private float _bubbleDropDuration = 0.4f;
    [SerializeField] private float _bubbleSize = 1.22f;
    [SerializeField] private Transform _playerEndingPosition;
    private IEnumerator AttackSequnce()
    {
        _player.canMove = false;
        _player.SetLook(1);

        _player.transform.DOJump(_playerEndingPosition.position, 2f, 1, 1f);

        yield return WaitForSecondsCache.Get(2f); //��� ���

        List<UI_Bubble> bubbles = new List<UI_Bubble>();
        for (int i = 0; i < dialogues.Count; i++)
        {
            Vector3 startPos = _player.transform.position + Vector3.up * _bubbleDropHeight;
            Vector3 endPos = _player.transform.position + Vector3.up * (2.5f - i * 0.1f + i * _bubbleSize); // �Ӹ� �� �⺻ ����

            UI_Bubble ui = Managers.UI.MakeSubItem<UI_Bubble>();
            ui.transform.position = startPos;
            ui.Init(dialogues[i], 0, false);
            ui.transform.DOMoveY(endPos.y, _bubbleDropDuration)
                .SetEase(Ease.Linear).OnComplete(() =>
                {   
                    _player.GetComponent<PlayerEmotion>().Frusted(i);
                    Camera.main.GetComponent<CameraShake>().Shake();
                    
                    bubbles.Insert(0, ui);
                    for (int j = 0; j < bubbles.Count; j++)
                    {

                        float pos = bubbles[j].transform.position.y;
                        Sequence seq = DOTween.Sequence();
                        seq.Append(bubbles[j].transform.DOMoveY(pos - 0.3f, 0.05f).SetEase(Ease.InCubic));
                        seq.Append(bubbles[j].transform.DOMoveY(pos - 0.1f, 0.05f).SetEase(Ease.OutBounce));
                    }
                }
            );

            yield return WaitForSecondsCache.Get(3f);
        }

        yield return StartCoroutine(FadeAllUI(bubbles));

        // �����ϴ� �ִϸ��̼� ������ ����
        //RealGameScene scene = Managers.Scene.CurrentScene as GangRealWorld;

        StartCoroutine(Managers.Sound.FadeOutBGM(2f));

        yield return WaitForSecondsCache.Get(3f);

        Managers.World.MoveNextWorld();

        Managers.Scene.LoadScene(Scene.LibraryScene);
    }

    private IEnumerator FadeAllUI(List<UI_Bubble> bubbles)
    {
        List<IEnumerator> fadeOuts = new List<IEnumerator>();
        foreach (var bubble in bubbles)
        {
            fadeOuts.Add(bubble.FadeOutImage());
        }
        
        fadeOuts.Add(_playerBackground.CoFadeOut(2f));

        List<bool> isDone = new List<bool>(new bool[fadeOuts.Count]);

        for (int i = 0; i < fadeOuts.Count; i++)
        {
            int idx = i;
            StartCoroutine(FadeWrapper(fadeOuts[i], () => isDone[idx] = true));
        }

        while (!isDone.TrueForAll(x => x))
            yield return null;
    }

    private IEnumerator FadeWrapper(IEnumerator routine, Action onComplete)
    {
        yield return StartCoroutine(routine);
        onComplete?.Invoke();
    }

   
    public void OnGiantWalk()
    {
        Camera.main.GetComponent<CameraShake>().Shake();
    }

    private void ChangeAnimateState()
    {
        switch (_currentState)
        {
            case GiantState.Idle:
                _ani.SetBool("Walk", false);
                break;
            case GiantState.Walk:
                _ani.SetBool("Walk", true);
                break;
            case GiantState.Attack:
                _ani.SetBool("Walk", false);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (_canMove)
        {
            this.transform.position += Vector3.left * Time.deltaTime * _moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GiantAttack"))
        {
            Debug.Log("Giant Attack Triggered");
            ChangeState(GiantState.Attack);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player collided with Giant");
            collision.collider.GetComponent<MovementRigidbody2D>().Knockback(Vector2.left, 10f, 0.3f);
        }
    }

    [SerializeField] SpriteRenderer _giantSpriteRenderer;
    [SerializeField] PolygonCollider2D _giantCollider;


    private void LateUpdate()
    {
        if (_giantSpriteRenderer == null || _giantCollider == null)
            return;

        UpdateColliderToCurrentSprite();
    }

    void UpdateColliderToCurrentSprite()
    {
        Sprite sprite = _giantSpriteRenderer.sprite;
        if (sprite == null || _giantCollider == null)
            return;

        int physicsShapesCount = sprite.GetPhysicsShapeCount();

        _giantCollider.pathCount = physicsShapesCount;

        for (int i = 0; i < physicsShapesCount; i++)
        {
            var shape = new System.Collections.Generic.List<Vector2>();
            sprite.GetPhysicsShape(i, shape);
            _giantCollider.SetPath(i, shape.ToArray());
        }
    }
}
