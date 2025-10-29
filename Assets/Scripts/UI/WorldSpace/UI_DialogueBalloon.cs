using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Febucci.UI.Core;

/// <summary>
/// 월드 스페이스 대화 말풍선.
/// 대사타이핑 연출/스킵/클릭 대기와 페이드 인·아웃, 대사 스택 배치용 추가 오프셋을 관리한다.
/// </summary>
public class UI_DialogueBalloon : UI_Base
{
	[SerializeField] private RectTransform _root;   // 말풍선 루트
	[SerializeField] private TMP_Text _label;
	[SerializeField] private CanvasGroup _cg;
	[SerializeField] private Vector2 _screenOffset = new Vector2(0, 80f);

	[SerializeField] private TAnimCore _textAnimator;       
	[SerializeField] private TypewriterCore _typewriter;

	private Transform _anchor;

	private float _extraOffsetY = 0f;

	public void AddStackOffset(float dy) => _extraOffsetY += dy;

	public void Init(Transform anchor)
	{
		_anchor = anchor;

		_cg.alpha = 0f;
	}

	public void Appear(string text)
	{
		if (_typewriter.isShowingText == false)
			return;

		this.gameObject.SetActive(true);
        _cg.alpha = 1f;
		_typewriter.ShowText(text);
    }

	private Coroutine _fadeCoroutine;
	public void AppearAndFade(string text)
	{
		if (this.gameObject.activeInHierarchy)
			return;

        this.gameObject.SetActive(true);
        _cg.alpha = 1f;
		_typewriter.ShowText(text);

        if (_fadeCoroutine != null)
			StopCoroutine(_fadeCoroutine);
		_fadeCoroutine = StartCoroutine(CoFade());
    }

	private IEnumerator CoFade()
	{
		float elapsed = 0f;
		float duration = 2f;
		float waitTime = 2f;
		yield return new WaitForSeconds(waitTime);

		while(elapsed < duration)
		{
			_cg.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
			elapsed += Time.deltaTime;
        }

		_cg.alpha = 0f;
		this.gameObject.SetActive(false);
    }

	public void Disappear()
	{
		_cg.alpha = 0f;
		this.gameObject.SetActive(false);
    }

    private void LateUpdate()
	{
		if (_anchor == null) 
			return;
		var cam = Camera.main; 
		if (cam == null) 
			return;

		var baseWorld = cam.ScreenToWorldPoint(Vector3.zero);
		var offWorld = cam.ScreenToWorldPoint(
			new Vector3(_screenOffset.x, _screenOffset.y + _extraOffsetY, 0f)) - baseWorld;

		var pos = _anchor.position + offWorld;
		pos.z = _anchor.position.z; // 2D면 z 고정
		_root.position = pos;
	}

	public IEnumerator CoPresent(string text, float typeSpeed = 0.03f)
	{
		// 페이드 인
		yield return _cg.FadeCanvas(1f, 0.15f);


		if (_typewriter && _textAnimator)
		{
			this.gameObject.SetActive(true);
            _typewriter.ShowText(text); // Start Mode : OnShowText로 설정해둬야 됨.


			// 진행 중 클릭/스페이스로 스킵
			while (_typewriter.isShowingText)
			{		
				// 입력 감지
				if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
				{
					// 마우스 클릭 시에만 사운드 재생
					if (Input.GetMouseButtonDown(0))
						Managers.Sound.Play("click_down", Sound.Effect);

					_typewriter.SkipTypewriter(); // 즉시 전부 표시
				}

				yield return null;
			}

			// 루프 종료 후 한 프레임 동안 Up 감지
			if (Input.GetMouseButton(0))
			{
				yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
				Managers.Sound.Play("click_up", Sound.Effect);
			}
		}

		// 전체 출력 후 클릭/스페이스 대기
		while (true)
		{
			// 입력 감지 (Space 또는 Mouse)
			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
			{
				// 사운드는 마우스 클릭일 때만
				if (Input.GetMouseButtonDown(0))
				{
					Managers.Sound.Play("click_down", Sound.Effect);
					yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
					Managers.Sound.Play("click_up", Sound.Effect);
				}
				break;
			}

			yield return null;
		}

		//// 페이드 아웃
		//yield return _cg.FadeCanvas(0f, 0.12f);
		Destroy(gameObject);
	}

	public IEnumerator CoPresentStacked(string text, float typeSpeed = 0.03f)
	{
		yield return _cg.FadeCanvas(1f, 0.15f);

		if (_typewriter && _textAnimator)
		{
			_typewriter.ShowText(text);
			while (_typewriter.isShowingText)
			{
				if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
					_typewriter.SkipTypewriter();
				yield return null;
			}
		}

		// 전체 출력 후 클릭/Space 대기
		while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space))
			yield return null;

		// 여기서 종료: 파괴하지 않고 남겨둠
	}

	public IEnumerator FadeOutAndDestroy(float duration = 0.12f)
	{
		if (_cg != null) 
			yield return _cg.FadeCanvas(0f, duration);
		Destroy(gameObject);
	}

	public override void Init() { }
}
