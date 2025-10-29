using UnityEngine;

public class CameraColliderBounds : MonoBehaviour, IRuntimeCamera
{
    [Tooltip("경계로 사용할 콜라이더 (씬에 있는 BoxCollider2D 오브젝트 권장)")]
    public Collider2D boundingShape;

    private Camera _cam;
    private Bounds _mapBounds;

    public bool UpdateCamera { get; set; } = true;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        if (!_cam.orthographic)
        {
            Debug.LogWarning("CameraColliderBounds 스크립트는 Orthographic 카메라에서만 정확하게 동작합니다.");
        }
    }

    private void Start()
    {
        _mapBounds = boundingShape.bounds;
    }

    void LateUpdate()
    {
        if(UpdateCamera == false)
            return;

        // 경계 콜라이더가 할당되지 않았으면 아무것도 하지 않음
        if (boundingShape == null || !_cam.orthographic)
        {
            return;
        }

        // 2. 줌을 고려하여 카메라 뷰의 절반 크기 계산 (이전과 동일)
        float camHalfHeight = _cam.orthographicSize;
        float camHalfWidth = camHalfHeight * _cam.aspect;

        // 3. 카메라 '중앙'이 이동할 수 있는 실제 경계 계산
        float clampedMinX = _mapBounds.min.x + camHalfWidth;
        float clampedMaxX = _mapBounds.max.x - camHalfWidth;
        float clampedMinY = _mapBounds.min.y + camHalfHeight;
        float clampedMaxY = _mapBounds.max.y - camHalfHeight;

        // 4. (예외 처리) 맵이 카메라보다 좁거나 낮은 경우 (이전과 동일)
        if (clampedMinX > clampedMaxX)
        {
            clampedMinX = _mapBounds.center.x;
            clampedMaxX = clampedMinX;
        }
        if (clampedMinY > clampedMaxY)
        {
            clampedMinY = _mapBounds.center.y;
            clampedMaxY = clampedMinY;
        }

        // 5. 카메라 위치를 경계 안에 가두기 (이전과 동일)
        Vector3 camPos = transform.position;
        camPos.x = Mathf.Clamp(camPos.x, clampedMinX, clampedMaxX);
        camPos.y = Mathf.Clamp(camPos.y, clampedMinY, clampedMaxY);
        transform.position = camPos;
    }
}