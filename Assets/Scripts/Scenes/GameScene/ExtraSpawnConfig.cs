using UnityEngine;

/// <summary>
/// 엑스트라 프리팹에 붙여서
/// - 좌/우 오프셋 월드값 오버라이드
/// - 지면 투영(프로젝트 투 그라운드) 오버라이드
/// - 호버(상하 바운스) 효과
/// 등을 개별 프리팹 단위로 설정.
/// </summary>
public class ExtraSpawnConfig : MonoBehaviour
{
	[Header("Offset Override (월드 단위)")]
	public bool overrideOffsets = false;
	public Vector2 offsetLeftWorld = Vector2.zero;
	public Vector2 offsetRightWorld = Vector2.zero;

	[Header("Project-To-Ground Override")]
	public bool overrideProjectToGround = false;
	public bool projectToGround = true; // overrideProjectToGround가 true일 때만 적용

	[Header("Hover (공중 바운스)")]
	public bool enableHover = false;
	public float hoverAmplitude = 0.15f;
	public float hoverSpeed = 1.2f;
}
