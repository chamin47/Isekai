using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 미니게임(말풍선) 스폰 시 세계별 엑스트라를 즉시 생성하고,
/// 말풍선의 실제 렌더 크기에 따라 "좌/우 가장자리 + 패딩 + 추가 오프셋(X,Y)" 지점에
/// 항상 정렬되도록 매 프레임 보정한다.
/// ExtraSpawnConfig를 프리팹에 붙이면 그 설정(오프셋/지면/페이싱/호버)을 개별로 오버라이드 가능.
/// </summary>
[DisallowMultipleComponent]
public class IsekaiExtraSpawner : MonoBehaviour
{
	[Tooltip("말풍선 Image(RectTransform). FixBubbleSize가 반영되는 그 RectTransform")]
	[SerializeField] private RectTransform _bubbleRect;
	[SerializeField] private Canvas _bubbleCanvas; // 말풍선이 속한 Canvas(비워도 자동)

	[Header("정렬/여백/오프셋 (월드 단위)")]
	[Tooltip("말풍선 가장자리에서 엑스트라까지 기본 패딩(좌/우 공통, X+)")]
	[SerializeField] private float _edgePaddingWorld = 0.3f;

	[Tooltip("왼쪽 꼬리일 때 추가 미세 오프셋 (월드 단위, X/Y)")]
	[SerializeField] private Vector2 _extraOffsetLeftWorld = Vector2.zero;

	[Tooltip("오른쪽 꼬리일 때 추가 미세 오프셋 (월드 단위, X/Y)")]
	[SerializeField] private Vector2 _extraOffsetRightWorld = Vector2.zero;

	[Header("지면 붙이기")]
	[SerializeField] private bool _projectToGround = true;
	[SerializeField] private LayerMask _groundMask;
	[SerializeField] private float _rayUp = 5f;
	[SerializeField] private float _rayDown = 20f;
	[SerializeField] private float _groundYOffset = 0f;
	[SerializeField] private float _worldZ = 0f; // 2D면 0

	[Header("사라질 때 페이드")]
	[SerializeField, Range(0.05f, 3f)] private float _fadeOutDuration = 0.6f;

	[Header("세계별 프리팹")]
	[SerializeField] private List<GameObject> _gangPrefabs;     // bear/bird/deer/rabbit
	[SerializeField] private List<GameObject> _chaummPrefabs;   // hand out_1 / loving
	[SerializeField] private List<GameObject> _vinterPrefabs;   // Cheering_1 / Cheering_2 / honor
	[SerializeField] private List<GameObject> _pelmanusPrefabs; // praying_sit / priaying_stand

	[Header("방향 설정 (전역 기본값)")]
	[Tooltip("엑스트라가 '항상 말풍선을 바라보게' (좌우 반전)")]
	[SerializeField] private bool _flipTowardBubble = true;
	[Tooltip("프리팹 기본 아트가 '오른쪽'을 바라보는 경우 체크, '왼쪽'이면 해제")]
	[SerializeField] private bool _prefabFacesRightByDefault = true;

	private UI_MiniGame _miniGame;
	private GameObject _spawned;
	private bool _isFading;
	private Camera _uiCam;

	private bool _ovrOffsets = false;
	private Vector2 _ovrOffsetLeft;
	private Vector2 _ovrOffsetRight;

	private bool _ovrProject = false;
	private bool _ovrProjectToGround = true;

	private bool _ovrFaceDefault = false;
	private bool _ovrFacesRightDefault = true;

	private bool _hover = false;
	private float _hoverAmp = 0.15f;
	private float _hoverSpeed = 1.2f;
	private float _hoverPhase = 0f;

	private void Awake()
	{
		_miniGame = GetComponent<UI_MiniGame>();
		if (!_bubbleRect) TryAutoFindBubbleRect();
		if (!_bubbleCanvas && _bubbleRect) _bubbleCanvas = _bubbleRect.GetComponentInParent<Canvas>();
		_uiCam = ResolveUICamera();
	}

	private void OnEnable()
	{
		if (_miniGame != null)
		{
			_miniGame.onSpawned += OnMiniGameSpawned;
			_miniGame.onMiniGameSucced += HandleSuccess;
			_miniGame.onMiniGameDestroyed += InstantCleanup;
		}
	}

	private void OnDisable()
	{
		if (_miniGame != null)
		{
			_miniGame.onSpawned -= OnMiniGameSpawned;
			_miniGame.onMiniGameSucced -= HandleSuccess;
			_miniGame.onMiniGameDestroyed -= InstantCleanup;
		}
		InstantCleanup();
	}

	private void LateUpdate()
	{
		if (!_spawned || !_bubbleRect) return;

		bool tailLeft = GetTailIsLeft();
		Vector3 pos = ComputeWorldEdgePosition(tailLeft);

		// Hover(공중 바운스)
		if (_hover)
		{
			_hoverPhase += Time.deltaTime * _hoverSpeed;
			pos.y += Mathf.Sin(_hoverPhase) * _hoverAmp;
		}

		_spawned.transform.position = new Vector3(pos.x, pos.y, _worldZ);
	}

	private void OnMiniGameSpawned()
	{
		if (_spawned) return;

		if (!_bubbleRect)
		{
			TryAutoFindBubbleRect();
			if (!_bubbleRect)
			{
				Debug.LogWarning("[IsekaiExtraSpawner] Bubble Rect를 찾지 못했습니다.");
				return;
			}
		}

		if (!_bubbleCanvas) _bubbleCanvas = _bubbleRect.GetComponentInParent<Canvas>();
		_uiCam = ResolveUICamera();

		var world = Managers.World.CurrentWorldType;
		GameObject prefab = PickRandomPrefab(world);
		if (!prefab)
		{
			Debug.LogWarning("[IsekaiExtraSpawner] 스폰할 프리팹이 없습니다.");
			return;
		}

		_spawned = Instantiate(prefab);

		// 프리팹 내 ExtraSpawnConfig 확인(오버라이드 흡수)
		ReadOverridesFromPrefab(_spawned);

		// 최초 위치 & 방향 세팅
		bool tailLeft = GetTailIsLeft();
		_spawned.transform.position = ComputeWorldEdgePosition(tailLeft);
		if (_flipTowardBubble) SetFacing(_spawned, tailLeft);
	}

	private void ReadOverridesFromPrefab(GameObject go)
	{
		// 기본값 리셋
		_ovrOffsets = false;
		_ovrProject = false;
		_ovrFaceDefault = false;
		_hover = false;
		_hoverPhase = 0f;

		var cfg = go.GetComponentInChildren<ExtraSpawnConfig>(true);
		if (!cfg) return;

		// offsets
		if (cfg.overrideOffsets)
		{
			_ovrOffsets = true;
			_ovrOffsetLeft = cfg.offsetLeftWorld;
			_ovrOffsetRight = cfg.offsetRightWorld;
		}

		// project-to-ground
		if (cfg.overrideProjectToGround)
		{
			_ovrProject = true;
			_ovrProjectToGround = cfg.projectToGround;
		}

		// hover
		if (cfg.enableHover)
		{
			_hover = true;
			_hoverAmp = Mathf.Max(0f, cfg.hoverAmplitude);
			_hoverSpeed = Mathf.Max(0f, cfg.hoverSpeed);
		}
	}

	private void HandleSuccess()
	{
		if (!_spawned) return;

		// 엑스트라 자신에게 페이드-파괴를 맡긴다 (미니게임 오브젝트가 꺼져도 계속 동작)
		var fader = _spawned.GetComponent<FadeOutAndDestroy>();
		if (!fader) fader = _spawned.AddComponent<FadeOutAndDestroy>();
		fader.Play(_fadeOutDuration);

		// 스포너는 참조만 끊어둠
		_spawned = null;
	}


	private void InstantCleanup()
	{
		if (_spawned != null)
		{
			Destroy(_spawned);
			_spawned = null;
			_isFading = false;
		}
	}

	private Vector3 ComputeWorldEdgePosition(bool tailLeft)
	{
		// 1) 말풍선 사각형의 화면 좌표(픽셀) 구하기
		Vector3[] worldCorners = new Vector3[4];
		_bubbleRect.GetWorldCorners(worldCorners);

		var cam = _uiCam ?? Camera.main;
		Vector3 c0 = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[0]); // BL
		Vector3 c2 = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[2]); // TR

		float widthPx = Mathf.Abs(c2.x - c0.x);
		Vector2 centerPx = new((c0.x + c2.x) * 0.5f, (c0.y + c2.y) * 0.5f);

		// 2) 가장자리 기준 월드 위치
		float dir = tailLeft ? -1f : +1f;
		float edgeX_Px = centerPx.x + dir * (widthPx * 0.5f);
		Vector3 edgeWorld = ScreenToWorldAtPlaneZ(new Vector3(edgeX_Px, centerPx.y, 0f), cam, _worldZ);

		// 3) 패딩 + 추가 오프셋(X,Y)
		Vector2 baseExtra = tailLeft ? _extraOffsetLeftWorld : _extraOffsetRightWorld;
		Vector2 instExtra = tailLeft ? _ovrOffsetLeft : _ovrOffsetRight;
		Vector2 finalExtra = _ovrOffsets ? instExtra : baseExtra;

		edgeWorld += new Vector3(dir * _edgePaddingWorld + finalExtra.x, finalExtra.y, 0f);

		// 4) 지면 투영(전역 또는 오버라이드)
		bool useGround = _ovrProject ? _ovrProjectToGround : _projectToGround;
		if (useGround)
		{
			Vector3 origin = new(edgeWorld.x, edgeWorld.y + _rayUp, _worldZ);
			var hit = Physics2D.Raycast(origin, Vector2.down, _rayUp + _rayDown, _groundMask);
			if (hit.collider) edgeWorld.y = hit.point.y + _groundYOffset;
		}

		edgeWorld.z = _worldZ;
		return edgeWorld;
	}

	private Vector3 ScreenToWorldAtPlaneZ(Vector3 screen, Camera cam, float worldZ)
	{
		if (cam == null) cam = Camera.main;
		if (cam.orthographic)
		{
			Vector3 w = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(worldZ - cam.transform.position.z)));
			w.z = worldZ;
			return w;
		}
		else
		{
			Ray r = cam.ScreenPointToRay(new Vector3(screen.x, screen.y, 0f));
			float t = (worldZ - r.origin.z) / r.direction.z;
			return r.origin + r.direction * t;
		}
	}

	private bool GetTailIsLeft()
	{
		if (_miniGame != null)
			return _miniGame.TailIsLeft;
		return true;
	}

	private GameObject PickRandomPrefab(WorldType world)
	{
		List<GameObject> pool = null;
		switch (world)
		{
			case WorldType.Gang: pool = _gangPrefabs; break;
			case WorldType.Chaumm: pool = _chaummPrefabs; break;
			case WorldType.Vinter: pool = _vinterPrefabs; break;
			case WorldType.Pelmanus: pool = _pelmanusPrefabs; break;
		}
		if (pool == null || pool.Count == 0) return null;
		return pool[Random.Range(0, pool.Count)];
	}

	// 엑스트라가 말풍선을 바라보게(좌/우) — flipX만 사용 (부모 스케일 영향 없음)
	private void SetFacing(GameObject root, bool tailLeft)
	{
		// 말풍선을 '바라보게' → 왼쪽 꼬리면 엑스트라는 '왼쪽'을, 오른쪽 꼬리면 '오른쪽'을 보게
		bool wantRight = !tailLeft; // tailLeft=true → 왼쪽, false → 오른쪽
		bool baseRight = _ovrFaceDefault ? _ovrFacesRightDefault : _prefabFacesRightByDefault;
		bool flip = (baseRight != wantRight);

		var srs = root.GetComponentsInChildren<SpriteRenderer>(true);
		for (int i = 0; i < srs.Length; i++)
			srs[i].flipX = flip;
	}

	// 말풍선을 자동 탐색(가장 큰 Image를 버블로 판단)
	private void TryAutoFindBubbleRect()
	{
		Image biggest = null;
		float best = -1f;
		var imgs = GetComponentsInChildren<Image>(true);
		foreach (var img in imgs)
		{
			if (!img.rectTransform) continue;
			var size = img.rectTransform.rect;
			float area = Mathf.Abs(size.width * size.height);
			if (area > best)
			{
				best = area;
				biggest = img;
			}
		}
		if (biggest) _bubbleRect = biggest.rectTransform;
	}

	private Camera ResolveUICamera()
	{
		if (_bubbleCanvas)
		{
			if (_bubbleCanvas.renderMode == RenderMode.ScreenSpaceCamera)
				return _bubbleCanvas.worldCamera ? _bubbleCanvas.worldCamera : Camera.main;
		}
		return Camera.main;
	}
}
