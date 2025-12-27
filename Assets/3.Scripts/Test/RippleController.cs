using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // SpriteRenderer가 필수임을 명시
public class RippleController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material material; // 셰이더 프로퍼티를 제어할 머티리얼
    private Coroutine rippleCoroutine;

    [Tooltip("물결이 퍼지는 시간")]
    public float duration = 1.0f;
    [Tooltip("물결의 최대 강도")]
    public float maxStrength = 0.1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 중요: 공유 머티리얼이 아닌, 이 스프라이트만의 고유 머티리얼 인스턴스를 생성합니다.
        material = spriteRenderer.material;
    }

    // 스프라이트에 Collider2D가 있어야 OnMouseDown이 동작합니다.
    void OnMouseDown()
    {
        // 카메라에서 마우스 위치로 레이(Ray)를 쏴서 부딪힌 지점 확인
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            Debug.Log($"Clicked on: {hit.collider.gameObject.name} at {hit.point}");
            // 이전에 실행 중인 코루틴이 있다면 중지
            if (rippleCoroutine != null)
            {
                StopCoroutine(rippleCoroutine);
            }
            // 클릭한 지점의 UV 좌표를 계산하여 코루틴 시작
            rippleCoroutine = StartCoroutine(AnimateRipple(hit.point));
        }
    }

    private IEnumerator AnimateRipple(Vector2 worldClickPoint)
    {
        // 월드 좌표를 스프라이트의 로컬 좌표로 변환
        Vector2 localPoint = transform.InverseTransformPoint(worldClickPoint);

        Debug.Log($"Local Point: {localPoint}");

        // 로컬 좌표를 UV 좌표(0~1)로 변환
        Sprite sprite = spriteRenderer.sprite;
        Vector2 uv = new Vector2(
            (localPoint.x - sprite.bounds.min.x) / sprite.bounds.size.x,
            (localPoint.y - sprite.bounds.min.y) / sprite.bounds.size.y
        );

        // 셰이더에 물결 중심점 설정
        material.SetVector("_RippleCenter", uv);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration; // 0에서 1로 진행

            // 시간이 지남에 따라 강도가 0으로 수렴하도록 설정 (부드러운 감소)
            float currentStrength = Mathf.Lerp(maxStrength, 0f, progress);

            material.SetFloat("_RippleStrength", currentStrength);
            material.SetFloat("_EffectTime", elapsedTime); // 경과 시간을 셰이더로 전달

            yield return null; // 다음 프레임까지 대기
        }

        // 효과가 끝나면 강도를 0으로 확실하게 설정
        material.SetFloat("_RippleStrength", 0f);
    }
}