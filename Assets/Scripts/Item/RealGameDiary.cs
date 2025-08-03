using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealGameDiary : MonoBehaviour
{
    [SerializeField] private ColliderEvent _diaryObject; // �ϱ��� ������Ʈ
    [SerializeField] private SpriteRenderer _diarySpriteRenderer;
    [SerializeField] private GameObject _diaryUI;
    [SerializeField] private bool _diaryDisappear = true;
    bool _isDiaryShown = false;
    private void Update()
    {
        //Debug.Log($"_diaryUI.activeInHierarchy: {_diaryUI.activeInHierarchy}, _diaryObject.IsCollidingWithPlayer: {_diaryObject.IsCollidingWithPlayer}, Input.GetKeyDown(KeyCode.Z): {Input.GetKeyDown(KeyCode.Z)}");
        if (_diaryUI.activeInHierarchy == false && _diaryObject.IsCollidingWithPlayer && Input.GetKeyDown(KeyCode.Z)) 
        {
            Debug.Log("�ϱ��� ����");
            Managers.Sound.Play("s2_book1", Sound.Effect);
            _diaryUI.SetActive(true);                    
        }

        if(_diaryDisappear && _isDiaryShown == false && _diarySpriteRenderer.isVisible)
        {
            _isDiaryShown = true;
            StartCoroutine(FadeThis());
        }
       
        

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _diaryUI.SetActive(false); // UI �ݱ�
            Time.timeScale = 1f;
        }
#endif
    }

    private IEnumerator FadeThis()
    {   
        for(int i = 0; i < 2; i++)
        {
            _diaryObject.gameObject.SetActive(false); // �ϱ��� UI Ȱ��ȭ
            yield return new WaitForSeconds(0.5f);
            _diaryObject.gameObject.SetActive(true); // �ϱ��� UI ��Ȱ��ȭ
            yield return new WaitForSeconds(1.5f);
        }
        // �ϱ��� UI�� Ȱ��ȭ�Ǹ� 3�� �Ŀ� ��Ȱ��ȭ
        yield return StartCoroutine(_diarySpriteRenderer.CoFadeIn(5f));
        this.gameObject.SetActive(false);
    }
}