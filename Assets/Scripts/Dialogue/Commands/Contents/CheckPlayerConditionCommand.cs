using System.Collections;
using UnityEngine;

public class CheckPlayerConditionCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        var player = context.Player;
        if (player != null)
            player.canMove = true;

        Debug.Log("플레이어의 움직임을 감지한다");

        // 5초동안 A나 D키를 안누르면 성공
        float waitTime = 5f;
        float elapsed = 0f;
        bool success = true;

        while (elapsed < waitTime)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                success = false;
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (player != null)
            player.canMove = false;

        string branchType = success ? "Success" : "Fail";
        context.EndEventParam = success ? 1 : 0;

        var nextID = context.BranchTable?.Resolve(row.eventParam?.Trim(), branchType);
        
        Debug.Log($"플레이어 감지 종료 : {nextID}");
        context.NextID = nextID;
    }
}