using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSystem : MonoBehaviour
{
    public static bool IsDoorOpen = false;
    [SerializeField] private GameObject _player;
    public DoorController doorController;
    public DreamSystem dreamSystem;

    [ContextMenu("MoveIntroScene")]
    public void MoveIntroScene()
    {
        dreamSystem.TriggerWakeUp();
        doorController.SkipHome();
    }

}
