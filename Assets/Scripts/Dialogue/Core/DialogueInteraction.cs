using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    private NPCController _interactNPC;
    private PlayerController _playerController;
    private bool _isInDialogue = false;
    [SerializeField] private NewDialogueRunner _dialogueRunner;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_interactNPC == null || _interactNPC.CanInteract == false)
            return;

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) == false)
            return;

        if(_interactNPC.IsInteracted)
        {
            Debug.Log("��ȭ ����");
            _interactNPC.ShowDialogue();
            _interactNPC.LookTarget(this.transform.position);
            return;
        }

        // ��ȭ�� �����Ѵ�
        if (_isInDialogue == false)
        {
            Debug.Log("��ȭ ����");
            _isInDialogue = true;
            _interactNPC.State = NPCState.Event;
            _interactNPC.LookTarget(this.transform.position);
            _playerController.canMove = false;
            _dialogueRunner.Play(_interactNPC.StartID, _interactNPC.ActorDirector, _interactNPC.OnEventEnd);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_interactNPC != null)
            return;

        if (other.CompareTag("NPC"))
        {
            if(other.TryGetComponent<NPCController>(out var npc))
            {
                _interactNPC = npc;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_interactNPC == null)
            return;

        if(_interactNPC.gameObject == other.gameObject)
        {
            _interactNPC = null;
            _isInDialogue = false;
        }
    }
}
