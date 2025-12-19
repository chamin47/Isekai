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
    [SerializeField] private Button closeButton;

    [Header("첫번째 페이지")]
    [SerializeField] private TMP_InputField _commentInput;
    [SerializeField] private Button _saveButton;
    [SerializeField] private GameObject comment;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private List<Sprite> diarySprites;
    [SerializeField] private List<Image> diaryImages;
    
    [SerializeField] private int currentPageIndex = 0;
    private int MaxPageIndex => diarySprites.Count - 1;

    [SerializeField] private float effectTime = 2f;
    private void Start()
    {
        closeButton.onClick.AddListener(() => this.gameObject.SetActive(false));

        nextButton.onClick.AddListener(OnNextButtonClick);
        prevButton.onClick.AddListener(OnPrevButtonClick);
        _saveButton.onClick.AddListener(OnSaveButtonClick);
        
    }

    private void OnEnable()
    {
        if (!Managers.Game.IsIntroCommentSaved)
        {
            _commentInput.gameObject.SetActive(true);
            _commentInput.Select();
            _saveButton.gameObject.SetActive(true);
        }
        else
        {
            comment.SetActive(true);
            _saveButton.gameObject.SetActive(false);
            _commentInput.text = Managers.Game.IntroCommentText;
            _commentInput.readOnly = true;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnSaveButtonClick()
    {
        if(string.IsNullOrWhiteSpace(_commentInput.text))
        {
            return;
        }

        _commentInput.readOnly = true;
        Managers.Game.IsIntroCommentSaved = true;
        Managers.Game.IntroCommentText = _commentInput.text;

        _saveButton.enabled = false;
        StartCoroutine(_saveButton.image.CoFadeIn(1));
    }

    private void OnPrevButtonClick()
    {
        if(currentPageIndex == 1)
        {
            if(_commentInput.readOnly == false)
                _commentInput.Select();

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