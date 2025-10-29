using UnityEngine;

public class CameraColliderBounds : MonoBehaviour
{
    [Tooltip("���� ����� �ݶ��̴� (���� �ִ� BoxCollider2D ������Ʈ ����)")]
    public Collider2D boundingShape;

    private Camera _cam;
    private Bounds _mapBounds;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        if (!_cam.orthographic)
        {
            Debug.LogWarning("CameraColliderBounds ��ũ��Ʈ�� Orthographic ī�޶󿡼��� ��Ȯ�ϰ� �����մϴ�.");
        }
    }

    private void Start()
    {
        _mapBounds = boundingShape.bounds;
    }

    public void UpdateCamera()
    {
        // ��� �ݶ��̴��� �Ҵ���� �ʾ����� �ƹ��͵� ���� ����
        if (boundingShape == null || !_cam.orthographic)
        {
            return;
        }

        // 2. ���� ����Ͽ� ī�޶� ���� ���� ũ�� ��� (������ ����)
        float camHalfHeight = _cam.orthographicSize;
        float camHalfWidth = camHalfHeight * _cam.aspect;

        // 3. ī�޶� '�߾�'�� �̵��� �� �ִ� ���� ��� ���
        float clampedMinX = _mapBounds.min.x + camHalfWidth;
        float clampedMaxX = _mapBounds.max.x - camHalfWidth;
        float clampedMinY = _mapBounds.min.y + camHalfHeight;
        float clampedMaxY = _mapBounds.max.y - camHalfHeight;

        // 4. (���� ó��) ���� ī�޶󺸴� ���ų� ���� ��� (������ ����)
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

        // 5. ī�޶� ��ġ�� ��� �ȿ� ���α� (������ ����)
        Vector3 camPos = transform.position;
        camPos.x = Mathf.Clamp(camPos.x, clampedMinX, clampedMaxX);
        camPos.y = Mathf.Clamp(camPos.y, clampedMinY, clampedMaxY);
        transform.position = camPos;
    }
}