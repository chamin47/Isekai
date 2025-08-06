using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealGameDiary : MonoBehaviour
{
    [SerializeField] private ColliderEvent _diaryObject; // �ϱ��� ������Ʈ
    [SerializeField] private SpriteRenderer _diarySpriteRenderer;
    [SerializeField] private GameObject _diaryUI;
    [SerializeField] private bool _diaryDisappear = true;
    [SerializeField] private GameObject _zButton;
    private bool _isDiaryShown = false;
    private bool _isUIActive = false;
    private void Update()
    {
        //Debug.Log($"_diaryUI.activeInHierarchy: {_diaryUI.activeInHierarchy}, _diaryObject.IsCollidingWithPlayer: {_diaryObject.IsCollidingWithPlayer}, Input.GetKeyDown(KeyCode.Z): {Input.GetKeyDown(KeyCode.Z)}");
        if (_diaryUI.activeInHierarchy == false && _diaryObject.IsCollidingWithPlayer && Input.GetKeyDown(KeyCode.Z)) 
        {
            Debug.Log("�ϱ��� ����");
            Managers.Sound.Play("s2_book1", Sound.Effect);
            _diaryUI.SetActive(true);
            _isUIActive = true;
        }
        else if(Input.GetKeyDown(KeyCode.Z) && _isUIActive == true)
        {
            _isUIActive = false;
            _diaryUI.SetActive(false); // UI �ݱ�
            Time.timeScale = 1f;
        }

        if (_diaryDisappear && _isDiaryShown == false && _diarySpriteRenderer.isVisible)
        {
            _isDiaryShown = true;
            StartCoroutine(FadeThis());
        }
    }

    private IEnumerator FadeThis()
    {   
        for(int i = 0; i < 2; i++)
        {
            if (_zButton != null)
                _zButton.gameObject.SetActive(false); // Z ��ư ��Ȱ��ȭ
            _diaryObject.gameObject.SetActive(false); // �ϱ��� UI Ȱ��ȭ
            yield return new WaitForSeconds(0.5f);
            if (_zButton != null)
                _zButton.gameObject.SetActive(true); // Z ��ư Ȱ��ȭ
            _diaryObject.gameObject.SetActive(true); // �ϱ��� UI ��Ȱ��ȭ
            yield return new WaitForSeconds(1.5f);
        }

        if(_zButton != null)
            StartCoroutine(_zButton.GetComponent<SpriteRenderer>().CoFadeIn(5f));
            
        // �ϱ��� UI�� Ȱ��ȭ�Ǹ� 5�� �Ŀ� ��Ȱ��ȭ
        yield return StartCoroutine(_diarySpriteRenderer.CoFadeIn(5f));
        _diaryObject.gameObject.SetActive(false);
    }
}