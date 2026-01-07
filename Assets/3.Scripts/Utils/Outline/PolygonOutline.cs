using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[DisallowMultipleComponent]
public class PolygonOutline : MonoBehaviour
{
	[Header("Build")]
	[SerializeField] private bool _autoBuildOnAwake = true;
	[SerializeField] private bool _hideOnAwake = true;

	[Header("Main Line (얇은 선)")]
	[SerializeField] private float _width = 0.035f;
	[SerializeField] private Color _color = Color.white;
	[SerializeField] private Material _material;           // 비우면 URP Unlit로 자동 생성
	[SerializeField] private int _orderOffset = +1;        // 스프라이트보다 위(앞)로 올리려면 +값

	[Header("Common")]
	[SerializeField] private LineAlignment _alignment = LineAlignment.View; // 화면 기준 정렬
	[SerializeField] private int _cornerVerts = 2;
	[SerializeField] private int _capVerts = 2;

	private PolygonCollider2D _poly;
	private SpriteRenderer _sprite;
	private readonly List<LineRenderer> _lines = new();

	// 동적으로 만들어 쓰는 머티리얼(직접 지정 안 했을 때)
	private Material _runtimeMat;

	void Awake()
	{
		_poly = GetComponent<PolygonCollider2D>();
		_sprite = GetComponent<SpriteRenderer>();

		if (_autoBuildOnAwake)
		{
			Build();
			if (_hideOnAwake) Hide();
		}
	}

	[ContextMenu("Rebuild Now")]
	public void Rebuild()
	{
		Clear();
		Build();
	}

	public void Build()
	{
		Clear();
		if (_poly == null || _poly.pathCount == 0)
			return;

		// 머티리얼 확보 (없으면 URP/Unlit 생성)
		Material useMat = _material;
		if (useMat == null)
		{
			if (_runtimeMat == null)
			{
				var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");

				if (shader == null) shader = Shader.Find("Sprites/Default"); // 폴백
				_runtimeMat = new Material(shader);
				_runtimeMat.color = Color.white;
			}
			useMat = _runtimeMat;
		}

		for (int p = 0; p < _poly.pathCount; ++p)
		{
			Vector2[] pts2 = _poly.GetPath(p);
			if (pts2 == null || pts2.Length < 2) continue;

			// 경로 루트 GO
			var pathRoot = new GameObject($"OutlinePath_{p}");
			pathRoot.transform.SetParent(transform, false);

			// 루프 닫기
			var pts3 = new Vector3[pts2.Length + 1];
			for (int i = 0; i < pts2.Length; i++) pts3[i] = pts2[i];
			pts3[pts2.Length] = pts2[0];

			// 라인 생성
			var lr = CreateLine(pathRoot, pts3, _width, _color, useMat);
			ApplySorting(lr, _orderOffset);
			_lines.Add(lr);
		}
	}

	private LineRenderer CreateLine(GameObject parent, Vector3[] points, float width, Color color, Material mat)
	{
		var go = new GameObject("Line");
		go.transform.SetParent(parent.transform, false);

		var lr = go.AddComponent<LineRenderer>();
		lr.useWorldSpace = false;
		lr.loop = true;
		lr.positionCount = points.Length;
		lr.SetPositions(points);

		lr.widthMultiplier = width;
		lr.startColor = color;
		lr.endColor = color;

		lr.numCornerVertices = _cornerVerts;
		lr.numCapVertices = _capVerts;
		lr.alignment = _alignment;
		lr.textureMode = LineTextureMode.Stretch;

#if UNITY_2022_2_OR_NEWER
		lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lr.receiveShadows = false;
#endif
		lr.material = mat;
		return lr;
	}

	private void ApplySorting(LineRenderer lr, int orderOffset)
	{
		if (_sprite != null)
		{
			lr.sortingLayerID = _sprite.sortingLayerID;
			lr.sortingOrder = _sprite.sortingOrder + orderOffset;
		}
	}

	public void Show() { SetEnable(true); }
	public void Hide() { SetEnable(false); }

	private void SetEnable(bool on)
	{
		foreach (var lr in _lines)
			if (lr) lr.enabled = on;
	}

	public void Clear()
	{
		for (int i = transform.childCount - 1; i >= 0; --i)
		{
			var t = transform.GetChild(i);
			if (t && t.name.StartsWith("OutlinePath_"))
				DestroyImmediate(t.gameObject);
		}
		_lines.Clear();
	}

	void OnDestroy()
	{
		Clear();
		if (_runtimeMat != null)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) DestroyImmediate(_runtimeMat);
			else Destroy(_runtimeMat);
#else
            Destroy(_runtimeMat);
#endif
			_runtimeMat = null;
		}
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		foreach (var lr in _lines)
		{
			if (!lr) continue;
			lr.widthMultiplier = _width;
			lr.startColor = lr.endColor = _color;
		}
	}
#endif
}