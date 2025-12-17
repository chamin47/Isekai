using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

using static UnityEngine.UI.Image;
using System.Collections.Generic;

class DiaryContent
{
    public Image leftImage;
    public Image rightImage;
    public bool isFirstPage;
    public bool isLastPage; 
    // 책을 넘기는 듯한 효과를 준다

    public void Init()
    {
        leftImage.fillMethod = FillMethod.Horizontal;
        leftImage.type = Type.Filled;
    }

   
}


public class DiarySystem: MonoBehaviour
{
    [Header("첫번째 페이지")]
    [SerializeField] private TMP_InputField commentInput;
    [SerializeField] private Button saveButton;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private List<Sprite> diarySprites;
    [SerializeField] private List<Image> diaryImages;
    
    [SerializeField] private int currentPageIndex = 0;
    private int MaxPageIndex => diarySprites.Count - 1;

    [SerializeField] private float effectTime = 2f;
    private void Start()
    {
        nextButton.onClick.AddListener(OnNextButtonClick);
        prevButton.onClick.AddListener(OnPrevButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        if(!Managers.Game.IsIntroCommentSaved)
        {
            commentInput.gameObject.SetActive(true);
            commentInput.Select();
            saveButton.gameObject.SetActive(true);
        }
        else
        {
            commentInput.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(false);
            commentInput.text = Managers.Game.IntroCommentText;
            commentInput.readOnly = true;
        }
    }

    private void OnSaveButtonClick()
    {
        if(string.IsNullOrWhiteSpace(commentInput.text))
        {
            return;
        }

        commentInput.readOnly = true;
        Managers.Game.IsIntroCommentSaved = true;
        Managers.Game.IntroCommentText = commentInput.text;

        saveButton.enabled = false;
        StartCoroutine(saveButton.image.CoFadeIn(1));
    }

    private void OnPrevButtonClick()
    {
        if(currentPageIndex == 1)
        {
            if(commentInput.readOnly == false)
                commentInput.Select();

            prevButton.gameObject.SetActive(false);
        }

        if(currentPageIndex == MaxPageIndex)
        {
            nextButton.gameObject.SetActive(true);
        }

        StartCoroutine(CoPrevPage());
    }

    private IEnumerator CoPrevPage()
    {
        prevButton.enabled = false;
        nextButton.enabled = false;

        var curImage = diaryImages[currentPageIndex];
        float elapsedTime = 0f;

        while (elapsedTime <= effectTime)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = 1- (elapsedTime / effectTime);

            curImage.fillAmount = fillAmount;
            yield return null;
        }

        currentPageIndex--;
        prevButton.enabled = true;
        nextButton.enabled = true;
    }

    public void OnNextButtonClick()
    {
        if(currentPageIndex == MaxPageIndex -1)
        {
            nextButton.gameObject.SetActive(false);
        }

        if(currentPageIndex == 0)
        {
            prevButton.gameObject.SetActive(true);
        }

        StartCoroutine(CoNextPage());
    }

    private IEnumerator CoNextPage()
    {
        prevButton.enabled = false;
        nextButton.enabled = false;
        var nextIndex = currentPageIndex + 1;
        var nextImage = diaryImages[nextIndex];
        float elapsedTime = 0f;
        while (elapsedTime <= effectTime)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = elapsedTime / effectTime;
            nextImage.fillAmount = fillAmount;
            yield return null;
        }
        currentPageIndex = nextIndex;
        prevButton.enabled = true;
        nextButton.enabled = true;
    }



    [ContextMenu("Load")]
    private void Load()
    {
        for(int i = 0; i < diaryImages.Count; i++)
        {
            diaryImages[i].sprite = diarySprites[i];
        }
    }
}