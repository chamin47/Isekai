using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_GuideBookHUD : UI_Base
{
	[Header("표시(페이지)")]
	[SerializeField] private Image _pageImage;                // 현재/다음 페이지 표시용(뒤쪽)
	[SerializeField] private bool _setNativeSize = false;     // 원본크기 적용 여부

	[Header("페이지 탐색 버튼")]
	[SerializeField] private Button _prevButton;
	[SerializeField] private Button _nextButton;

	[Header("리소스 로딩(Resources)")]
	[SerializeField] private string _resourcesRoot = "GuideBook"; // Resources/GuideBook/{WorldType}/
	[SerializeField] private int _startIndex = 0;

	[Header("책갈피")]
	[SerializeField] private Button _bookmarkButton;          // 좌측 책갈피 버튼
	[SerializeField] private CanvasGroup _bookmarkGroup;      // (튜토리얼 블링크용)

	[Header("책 패널(열고 닫기)")]
	[SerializeField] private RectTransform _bookPanel;        // 책 패널 루트
	[SerializeField] private float _slideTime = 0.35f;        // 왼→오 슬라이드 시간
	[SerializeField] private float _closedPosX = -900f;       // 닫힘 X
	[SerializeField] private float _openedPosX = 0f;          // 열림 X

	[Header("딤/차단")]
	[SerializeField] private Image _dim;                      // 전체 화면 딤
	[SerializeField] private float _dimAlpha = 0.78f;          // 열렸을 때 알파
	[SerializeField] private Button _outsideCloseButton;      // 책 바깥 클릭 → 닫기

	[Header("책장 넘김(조각 페이드)")]
	[SerializeField, Tooltip("세로 조각 개수")]
	private int _sliceCount = 5;
	[SerializeField, Tooltip("조각 하나가 1→0으로 페이드되는 시간(초)")]
	private float _sliceFadeTime = 0.08f;

	[SerializeField] private GameObject _newBadge;

	// 내부 상태
	private PlayerController _playerController;
	private readonly List<Sprite> _pages = new();
	private int _index;
	private bool _isTransitioning;
	private bool _isOpen;
	private bool _tutorialMode = false; 

	public override void Init()
	{
		// Canvas 세팅(카메라 할당)
		var canvas = GetComponent<Canvas>();
		if (canvas != null && canvas.worldCamera == null && Camera.main != null)
			canvas.worldCamera = Camera.main;

		// 버튼 바인딩
		if (_bookmarkButton) _bookmarkButton.onClick.AddListener(OnClickBookmark);
		if (_outsideCloseButton) _outsideCloseButton.onClick.AddListener(TryCloseByOutside);
		if (_prevButton) _prevButton.onClick.AddListener(OnClickPrev);
		if (_nextButton) _nextButton.onClick.AddListener(OnClickNext);

		// 초기 화면 상태
		if (_dim)
		{
			var c = _dim.color; c.a = 0; _dim.color = c;
			_dim.raycastTarget = false;
		}

		// 책 패널 닫힌 위치
		if (_bookPanel)
		{
			var p = _bookPanel.anchoredPosition;
			p.x = _closedPosX;
			_bookPanel.anchoredPosition = p;
		}
		_isOpen = false;
		_isTransitioning = false;

		_playerController = FindAnyObjectByType<PlayerController>();

		// 페이지 로딩
		LoadPagesForCurrentWorld();
		if (_pages.Count > 0)
		{
			_index = Mathf.Clamp(_startIndex, 0, _pages.Count - 1);
			ApplyPageImmediate(_index);
		}

		// 시작은 닫혀 있으니 네비 비활성(시각/입력 모두)
		SetNavInteractable(false);

		SetNewBadge();

		if (Managers.World.CurrentWorldType == WorldType.Vinter)
		{
			_tutorialMode = true;
			StartCoroutine(CoTutorialGate());
		}
	}

	private void OnDestroy()
	{
		if (_bookmarkButton) _bookmarkButton.onClick.RemoveListener(OnClickBookmark);
		if (_outsideCloseButton) _outsideCloseButton.onClick.RemoveListener(TryCloseByOutside);
		if (_prevButton) _prevButton.onClick.RemoveListener(OnClickPrev);
		if (_nextButton) _nextButton.onClick.RemoveListener(OnClickNext);
	}

	private void LoadPagesForCurrentWorld()
	{
		_pages.Clear();
		string world = Managers.World.CurrentWorldType.ToString();
		string path = $"{_resourcesRoot}/{world}"; // Resources/GuideBook/{World}/
		var loaded = Resources.LoadAll<Sprite>(path);
		if (loaded != null && loaded.Length > 0)
			_pages.AddRange(loaded.OrderBy(s => s.name, System.StringComparer.OrdinalIgnoreCase));
		else
			Debug.LogWarning($"[UI_GuideBookHUD] Resources/{path} 스프라이트 없음.");
	}

	private void ApplyPageImmediate(int idx)
	{
		if (!ValidIndex(idx) || _pageImage == null) return;
		_pageImage.sprite = _pages[idx];
		if (_setNativeSize) _pageImage.SetNativeSize();
		var c = _pageImage.color; c.a = 1f; _pageImage.color = c; // 보정
	}

	private bool ValidIndex(int i) => (_pages != null && i >= 0 && i < _pages.Count);

	private IEnumerator CoTutorialGate()
	{
		// 화면 어둡게 + 모든 상호작용 차단, 오직 책갈피만
		if (_dim)
		{
			yield return FadeImageAlpha(_dim, _dim.color.a, _dimAlpha, 0.75f, unscaled: true);
			_dim.raycastTarget = true;
		}
		// 책갈피 깜빡임 시작
		StartCoroutine(CoBlinkBookmarkCG());

		// 게임 정지
		Time.timeScale = 0f;
		_playerController.canMove = false;
		yield break;
	}

	private IEnumerator CoBlinkBookmarkCG(
	float minAlpha = 0.12f,
	float maxAlpha = 1f,
	float speed = 2f,
	float lowHold = 0f,
	float highHold = 0f
)
	{
		if (_bookmarkGroup == null) yield break;

		_bookmarkGroup.ignoreParentGroups = true;
		_bookmarkGroup.blocksRaycasts = true;
		_bookmarkGroup.interactable = true;

		float t = 0f;
		bool hasPlayedAtMax = false; // 최대 알파에서 사운드 재생 여부
		const float eps = 0.02f;     // 임계값 여유

		while (_tutorialMode)
		{
			t += Time.unscaledDeltaTime;

			float p = Mathf.PingPong(t * speed, 1f);
			float a01 = Mathf.SmoothStep(0f, 1f, p);
			float alpha = Mathf.Lerp(minAlpha, maxAlpha, a01);
			_bookmarkGroup.alpha = alpha;

			if (alpha >= maxAlpha - eps)
			{
				if (!hasPlayedAtMax)
				{
					Managers.Sound.Play("ding-126626", Sound.Effect);
					hasPlayedAtMax = true; // 이번 사이클에서 재생 완료
				}
				yield return new WaitForSecondsRealtime(highHold);
			}
			else if (alpha <= minAlpha + eps)
			{
				hasPlayedAtMax = false; // 사이클 리셋 → 다음 최대에서 재생 가능
				yield return new WaitForSecondsRealtime(lowHold);
			}
			else
			{
				yield return null;
			}
		}

		_bookmarkGroup.alpha = 1f;
	}

	private void OnClickBookmark()
	{
		if (_isTransitioning) return;

		if (_isOpen) CloseBook();
		else OpenBook();
	}

	private void TryCloseByOutside()
	{
		if (_isTransitioning || !_isOpen) return;
		if (_tutorialMode) return; // 튜토리얼 중 외부 닫기 금지
		CloseBook();
	}

	private void OpenBook()
	{
		if (_isTransitioning || _isOpen) return;
		StartCoroutine(CoOpenBook());
	}

	private void CloseBook()
	{
		if (_isTransitioning || !_isOpen) return;
		StartCoroutine(CoCloseBook());
	}

	private IEnumerator CoOpenBook()
	{
		_isTransitioning = true;

		if (_dim)
		{
			_dim.raycastTarget = true;
			yield return FadeImageAlpha(_dim, _dim.color.a, _dimAlpha, 0.2f, unscaled: true);
		}
		if (_bookPanel)
			yield return MoveAnchoredX(_bookPanel, _closedPosX, _openedPosX, _slideTime, unscaled: true);

		_isOpen = true;
		_isTransitioning = false;

		_newBadge.SetActive(false);

		// 열렸으니 네비 버튼 상태 갱신(여기서 켜짐)
		UpdateNavInteractable();

		Time.timeScale = 0f;
		_playerController.canMove = false;

		if (_tutorialMode)
		{
			_tutorialMode = false;
		}
	}

	private IEnumerator CoCloseBook()
	{
		_isTransitioning = true;

		if (_bookPanel)
			yield return MoveAnchoredX(_bookPanel, _openedPosX, _closedPosX, _slideTime, unscaled: true);

		if (_dim)
		{
			yield return FadeImageAlpha(_dim, _dim.color.a, 0f, 0.2f, unscaled: true);
			_dim.raycastTarget = false;
		}

		_isOpen = false;
		_isTransitioning = false;

		// 닫히면 네비 버튼 비활성 유지(시각/입력 모두 Off)
		SetNavInteractable(false);

		Time.timeScale = 1f;
		_playerController.canMove = true;
	}

	private void OnClickPrev()
	{
		if (_isTransitioning || !_isOpen) return;
		if (!ValidIndex(_index - 1)) return;
		StartCoroutine(CoSliceTurnPage(_index - 1, dir: +1));
	}

	private void OnClickNext()
	{
		if (_isTransitioning || !_isOpen) return;
		if (!ValidIndex(_index + 1)) return;
		StartCoroutine(CoSliceTurnPage(_index + 1, dir: -1));
	}

	/// <summary>
	/// A→B→C→D→E 순차 페이드(Next, dir=+1) / 역순(Prev, dir=-1)
	/// </summary>
	private IEnumerator CoSliceTurnPage(int nextIndex, int dir)
	{
		_isTransitioning = true;
		SetNavInteractable(false); // 전환 동안 비활성(시각적으로도 끔)

		// 뒤쪽 페이지를 "다음 페이지"로 먼저 교체
		Sprite cur = _pages[_index];
		Sprite nxt = _pages[nextIndex];

		_pageImage.sprite = nxt;
		if (_setNativeSize) _pageImage.SetNativeSize();
		var bc = _pageImage.color; bc.a = 1f; _pageImage.color = bc;

		// 앞쪽 오버레이 조각 컨테이너 생성 (현재 페이지를 띄워서 페이드로 지움)
		var slicer = BuildSlicerOverlay(cur, _sliceCount);

		// 순차 페이드
		if (dir >= 0)
		{
			// Next: A→B→C→D→E
			for (int i = 0; i < slicer.slices.Count; i++)
				yield return FadeImageAlpha(slicer.slices[i], 1f, 0f, _sliceFadeTime, unscaled: true);
		}
		else
		{
			// Prev: E→D→C→B→A
			for (int i = slicer.slices.Count - 1; i >= 0; i--)
				yield return FadeImageAlpha(slicer.slices[i], 1f, 0f, _sliceFadeTime, unscaled: true);
		}

		Destroy(slicer.root);
		_index = nextIndex;

		_isTransitioning = false;
		UpdateNavInteractable();    

		yield break;
	}

	/// <summary>
	/// 현재 페이지를 세로 N등분으로 마스크해 올려놓는 오버레이를 만든다.
	/// 뒤에는 _pageImage(=다음 페이지)가 깔려 있어야 함.
	/// </summary>
	private (GameObject root, List<Image> slices) BuildSlicerOverlay(Sprite sprite, int count)
	{
		var list = new List<Image>();
		var root = new GameObject("PageSlicerOverlay", typeof(RectTransform));
		var rootRT = (RectTransform)root.transform;
		rootRT.SetParent(_pageImage.transform.parent, false);
		root.transform.SetSiblingIndex(_pageImage.transform.GetSiblingIndex() + 1);
		CopyRect(_pageImage.rectTransform, rootRT);

		float fullW = rootRT.rect.width;
		float fullH = rootRT.rect.height;
		float sliceW = Mathf.Max(1f, fullW / Mathf.Max(1, count));

		for (int i = 0; i < count; i++)
		{
			var sliceRoot = new GameObject($"Slice_{i}", typeof(RectTransform), typeof(RectMask2D));
			var sliceRT = (RectTransform)sliceRoot.transform;
			sliceRT.SetParent(rootRT, false);

			sliceRT.anchorMin = new Vector2((float)i / count, 0f);
			sliceRT.anchorMax = new Vector2((float)(i + 1) / count, 1f);
			sliceRT.offsetMin = Vector2.zero;
			sliceRT.offsetMax = Vector2.zero;
			sliceRT.pivot = new Vector2(0.5f, 0.5f);

			var imgGO = new GameObject("Img", typeof(RectTransform), typeof(Image));
			var imgRT = (RectTransform)imgGO.transform;
			imgRT.SetParent(sliceRT, false);

			imgRT.anchorMin = new Vector2(0f, 0.5f);
			imgRT.anchorMax = new Vector2(0f, 0.5f);
			imgRT.pivot = new Vector2(0f, 0.5f);
			imgRT.sizeDelta = new Vector2(fullW, fullH);
			imgRT.anchoredPosition = new Vector2(-i * sliceW, 0f);

			var img = imgGO.GetComponent<Image>();
			img.sprite = sprite;
			img.type = Image.Type.Simple;
			img.preserveAspect = false;
			img.raycastTarget = false;
			var c = img.color; c.a = 1f; img.color = c;

			list.Add(img);
		}

		return (root, list);
	}

	private void CopyRect(RectTransform src, RectTransform dst)
	{
		dst.anchorMin = src.anchorMin;
		dst.anchorMax = src.anchorMax;
		dst.pivot = src.pivot;
		dst.sizeDelta = src.sizeDelta;
		dst.anchoredPosition = src.anchoredPosition;
		dst.localScale = src.localScale;
		dst.localRotation = src.localRotation;
	}

	private void UpdateNavInteractable()
	{
		bool hasPages = _pages.Count > 0;
		bool on = hasPages && _isOpen && !_isTransitioning;

		// 경계 체크도 포함 (첫/마지막 페이지에서 해당 버튼만 꺼짐)
		if (_prevButton) _prevButton.interactable = on && _index > 0;
		if (_nextButton) _nextButton.interactable = on && _index < _pages.Count - 1;
	}

	private void SetNavInteractable(bool on)
	{
		if (_prevButton) _prevButton.interactable = on;
		if (_nextButton) _nextButton.interactable = on;
	}

	private IEnumerator FadeImageAlpha(Image img, float from, float to, float time, bool unscaled)
	{
		if (img == null) yield break;

		Color c = img.color; c.a = from; img.color = c;
		if (time <= 0f) { c.a = to; img.color = c; yield break; }

		float t = 0f;
		while (t < time)
		{
			t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
			c.a = Mathf.Lerp(from, to, t / time);
			img.color = c;
			yield return null;
		}
		c.a = to; img.color = c;
	}

	private IEnumerator MoveAnchoredX(RectTransform rt, float fromX, float toX, float time, bool unscaled)
	{
		if (!rt) yield break;
		Vector2 p = rt.anchoredPosition; p.x = fromX; rt.anchoredPosition = p;

		if (time <= 0f) { p.x = toX; rt.anchoredPosition = p; yield break; }

		float t = 0f;
		while (t < time)
		{
			t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
			p.x = Mathf.Lerp(fromX, toX, Mathf.SmoothStep(0f, 1f, t / time));
			rt.anchoredPosition = p;
			yield return null;
		}
		p.x = toX; rt.anchoredPosition = p;
	}

	private void SetNewBadge()
	{
		WorldType world = Managers.World.CurrentWorldType;

		switch (world)
		{
			case WorldType.Vinter:
				_newBadge.SetActive(true);
				break;
			case WorldType.Chaumm:
				_newBadge.SetActive(true);
				break;
			case WorldType.Gang:
				_newBadge.SetActive(true);
				break;
			case WorldType.Pelmanus:
				_newBadge.SetActive(false);
				break;
		}
	}
}
