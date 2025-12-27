using System.Collections;
using UnityEngine;

public class EndScriptCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        Debug.Log("대화 종료.");

        var player = context.Player;
        if (player != null)
            player.canMove = true;

        context.OnDialogueEnd?.Invoke(context.EndEventParam);
        context.OnDialogueEnd = null;
        context.EndEventParam = -1;

        if (context.Happiness != null)
            context.Happiness.Appear();

        if (context.CameraController != null)
            context.CameraController.EnableCameraUpdate();

		float baseH = Screen.height * 0.1f;  
		float overshoot = baseH;
		float settle = baseH * 0.85f;        

		UILetterboxOverlay letterbox = UILetterboxOverlay.GetOrCreate();

		yield return letterbox.CloseOvershoot(settle, overshoot, 170f);

		// EndScript는 대화 종료이므로 NextID를 null로 설정
		context.NextID = null;
    }
}