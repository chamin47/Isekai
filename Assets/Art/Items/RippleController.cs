using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // SpriteRenderer�� �ʼ����� ���
public class RippleController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material material; // ���̴� ������Ƽ�� ������ ��Ƽ����
    private Coroutine rippleCoroutine;

    [Tooltip("������ ������ �ð�")]
    public float duration = 1.0f;
    [Tooltip("������ �ִ� ����")]
    public float maxStrength = 0.1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // �߿�: ���� ��Ƽ������ �ƴ�, �� ��������Ʈ���� ���� ��Ƽ���� �ν��Ͻ��� �����մϴ�.
        material = spriteRenderer.material;
    }

    // ��������Ʈ�� Collider2D�� �־�� OnMouseDown�� �����մϴ�.
    void OnMouseDown()
    {
        // ī�޶󿡼� ���콺 ��ġ�� ����(Ray)�� ���� �ε��� ���� Ȯ��
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            Debug.Log($"Clicked on: {hit.collider.gameObject.name} at {hit.point}");
            // ������ ���� ���� �ڷ�ƾ�� �ִٸ� ����
            if (rippleCoroutine != null)
            {
                StopCoroutine(rippleCoroutine);
            }
            // Ŭ���� ������ UV ��ǥ�� ����Ͽ� �ڷ�ƾ ����
            rippleCoroutine = StartCoroutine(AnimateRipple(hit.point));
        }
    }

    private IEnumerator AnimateRipple(Vector2 worldClickPoint)
    {
        // ���� ��ǥ�� ��������Ʈ�� ���� ��ǥ�� ��ȯ
        Vector2 localPoint = transform.InverseTransformPoint(worldClickPoint);

        Debug.Log($"Local Point: {localPoint}");

        // ���� ��ǥ�� UV ��ǥ(0~1)�� ��ȯ
        Sprite sprite = spriteRenderer.sprite;
        Vector2 uv = new Vector2(
            (localPoint.x - sprite.bounds.min.x) / sprite.bounds.size.x,
            (localPoint.y - sprite.bounds.min.y) / sprite.bounds.size.y
        );

        // ���̴��� ���� �߽��� ����
        material.SetVector("_RippleCenter", uv);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration; // 0���� 1�� ����

            // �ð��� ������ ���� ������ 0���� �����ϵ��� ���� (�ε巯�� ����)
            float currentStrength = Mathf.Lerp(maxStrength, 0f, progress);

            material.SetFloat("_RippleStrength", currentStrength);
            material.SetFloat("_EffectTime", elapsedTime); // ��� �ð��� ���̴��� ����

            yield return null; // ���� �����ӱ��� ���
        }

        // ȿ���� ������ ������ 0���� Ȯ���ϰ� ����
        material.SetFloat("_RippleStrength", 0f);
    }
}