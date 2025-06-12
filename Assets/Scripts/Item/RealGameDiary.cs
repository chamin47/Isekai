using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealGameDiary : MonoBehaviour
{
    [SerializeField] private GameObject _diaryObject; // 일기장 오브젝트
    [SerializeField] private SpriteRenderer _diarySpriteRenderer;
    [SerializeField] private GameObject _diaryUI;
    [SerializeField] private bool _diaryDisappear = true;
    bool _isDiaryShown = false;
    private void Update()
    {
        if (_diaryUI.activeInHierarchy == false && Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log($"Hit: {hit.collider?.gameObject.name}");
                if (hit.collider != null && hit.collider.gameObject == _diaryObject)
                {
                    // 일기장 오브젝트를 클릭했을 때
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
            _diaryUI.SetActive(false); // UI 닫기
            Time.timeScale = 1f;
        }
#endif
    }

    private IEnumerator FadeThis()
    {   
        for(int i = 0; i < 2; i++)
        {
            _diaryObject.SetActive(false); // 일기장 UI 활성화
            yield return new WaitForSeconds(0.5f);
            _diaryObject.SetActive(true); // 일기장 UI 비활성화
            yield return new WaitForSeconds(1.5f);
        }
        // 일기장 UI가 활성화되면 3초 후에 비활성화
        yield return StartCoroutine(_diarySpriteRenderer.CoFadeIn(5f));
        this.gameObject.SetActive(false);
    }
}