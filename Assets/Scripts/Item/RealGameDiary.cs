using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealGameDiary : MonoBehaviour
{
    [SerializeField] private GameObject _diaryObject; // �ϱ��� ������Ʈ
    [SerializeField] private SpriteRenderer _diarySpriteRenderer;
    [SerializeField] private GameObject _diaryUI;
    [SerializeField] private bool _diaryDisappear = true;
    bool _isDiaryShown = false;
    private void Update()
    {
        if (_diaryUI.activeInHierarchy == false && Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log($"Hit: {hit.collider?.gameObject.name}");
                if (hit.collider != null && hit.collider.gameObject == _diaryObject)
                {
                    // �ϱ��� ������Ʈ�� Ŭ������ ��
                    Managers.Sound.Play("s2_book1", Sound.Effect);
                    _diaryUI.SetActive(true);
                }                
            }
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
            _diaryObject.SetActive(false); // �ϱ��� UI Ȱ��ȭ
            yield return new WaitForSeconds(0.5f);
            _diaryObject.SetActive(true); // �ϱ��� UI ��Ȱ��ȭ
            yield return new WaitForSeconds(1.5f);
        }
        // �ϱ��� UI�� Ȱ��ȭ�Ǹ� 3�� �Ŀ� ��Ȱ��ȭ
        yield return StartCoroutine(_diarySpriteRenderer.CoFadeIn(5f));
        this.gameObject.SetActive(false);
    }
}