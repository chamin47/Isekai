using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class ConvertSpriteClipToImageClip
{
	[MenuItem("Tools/Animation/Convert SpriteClip → UI Image Clip")]
	private static void Convert()
	{
		// 선택된 AnimationClip
		AnimationClip sourceClip = Selection.activeObject as AnimationClip;
		if (sourceClip == null)
		{
			Debug.LogError("AnimationClip을 선택한 상태에서 실행하세요.");
			return;
		}

		// 새 UI AnimationClip 생성
		AnimationClip uiClip = new AnimationClip
		{
			frameRate = sourceClip.frameRate
		};

		bool converted = false;

		// SpriteRenderer.sprite 바인딩 추출
		var bindings = AnimationUtility.GetObjectReferenceCurveBindings(sourceClip);

		foreach (var binding in bindings)
		{
			if (binding.type != typeof(SpriteRenderer))
				continue;

			if (binding.propertyName != "m_Sprite")
				continue;

			// 키프레임 그대로 복사
			var keyframes =
				AnimationUtility.GetObjectReferenceCurve(sourceClip, binding);

			// 루트 Image + path = ""
			EditorCurveBinding uiBinding = new EditorCurveBinding
			{
				path = "",                      // 루트 자기 자신
				type = typeof(Image),           // UI Image
				propertyName = "m_Sprite"
			};

			AnimationUtility.SetObjectReferenceCurve(
				uiClip,
				uiBinding,
				keyframes
			);

			converted = true;
		}

		if (!converted)
		{
			Debug.LogWarning("SpriteRenderer.sprite 바인딩을 찾지 못했습니다.");
			return;
		}

		// 저장
		string srcPath = AssetDatabase.GetAssetPath(sourceClip);
		string newPath = srcPath.Replace(".anim", "_UI.anim");

		AssetDatabase.CreateAsset(uiClip, newPath);
		AssetDatabase.SaveAssets();

		Debug.Log($"UI AnimationClip 생성 완료 (Root Image 기준): {newPath}");
	}
}
