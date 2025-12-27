using UnityEngine;

public enum GroundMode
{
	Inherit,      // 전역(_projectToGround / _groundYOffset / 전역 offset) 그대로 사용
	BubbleSpace,  // 지면 스냅 안 함, 버블 기준 X/Y 오프셋 사용
	SnapToGround  // 지면 스냅, 개별 groundYOffset 사용 (Y 오프셋은 무시, X 오프셋은 사용)
}

/// <summary>
/// 엑스트라 프리팹에 붙여서 개별 배치 정책을 지정.
/// - groundMode로 배치 기준을 3가지 중 하나 선택
/// - BubbleSpace 모드: offsetLeft/RightWorld(X,Y)로 버블 기준 위치 보정
/// - SnapToGround 모드: groundYOffset으로 지면 기준 높이 보정 (Y 오프셋은 무시, X 오프셋만 적용)
/// - Hover(공중 바운스) 옵션
/// </summary>
public class ExtraSpawnConfig : MonoBehaviour
{
	[Header("Placement Mode")]
	public GroundMode groundMode = GroundMode.Inherit;

	[Header("Offsets (BubbleSpace 모드 또는 SnapToGround의 X 보정에 사용)")]
	public Vector2 offsetLeftWorld = Vector2.zero;
	public Vector2 offsetRightWorld = Vector2.zero;

	[Header("Ground (SnapToGround 모드에서만 사용)")]
	public float groundYOffset = 0f;

	[Header("Hover (공중 바운스)")]
	public bool enableHover = false;
	public float hoverAmplitude = 0.15f;
	public float hoverSpeed = 1.2f;
}
