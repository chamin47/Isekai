using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

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

    [Header("Navigation")]
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [Header("Pages")]
    [SerializeField] private List<Sprite> diarySprites;
    [SerializeField] private List<Image> diaryImages;

    [Header("Setting")]
    [SerializeField] private float effectTime = 2f;

    [Header("확인용")]
    [SerializeField] private int currentPageIndex = 0;

	[Header("Thickness Lines")]
	[SerializeField] private GameObject[] leftThicknessLines;
	[SerializeField] private GameObject[] rightThicknessLines;

	private int MaxPageIndex => Mathf.Max(0, diaryImages.Count - 1);
    private bool _isPageTurning = false;

    private void Start()
    {
        //prevButton.enabled = true;
        //nextButton.enabled = true;

        closeButton.onClick.AddListener(Close);

        nextButton.onClick.AddListener(OnNextButtonClick);
        prevButton.onClick.AddListener(OnPrevButtonClick);
        _saveButton.onClick.AddListener(OnSaveButtonClick);
        
    }

    private void OnEnable()
    {
        if (!Managers.Game.IsIntroCommentSaved)
        {
            _commentInput.gameObject.SetActive(true);
            _saveButton.gameObject.SetActive(true);
            StartCoroutine(Focus());
    }
        else
        {
            comment.SetActive(true);
            _saveButton.gameObject.SetActive(false);
            _commentInput.text = Managers.Game.IntroCommentText;
            _commentInput.readOnly = true;
        }

        Load();
		IntializeThicknessLines();
	}

	private void IntializeThicknessLines()
	{		
		UpdateLeftThickness();
		UpdateRightThickness();
	}

	private void Close()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator Focus()
    {
        yield return null;
        _commentInput.Select();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && _isPageTurning == false)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnSaveButtonClick()
    {
        _commentInput.Select();
        if (string.IsNullOrWhiteSpace(_commentInput.text))
        {
            return;
        }

        _commentInput.readOnly = true;
        _commentInput.selectionColor = new Color(0, 0, 0, 0);

        Managers.Game.IsIntroCommentSaved = true;
        Managers.Game.IntroCommentText = _commentInput.text;

        _saveButton.enabled = false;
        StartCoroutine(_saveButton.image.CoFadeIn(1));
    }

    private void OnPrevButtonClick()
    {
        if (Managers.Game.IsIntroCommentSaved)
        {
            comment.SetActive(true);
        }

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
        _isPageTurning = true;

        Managers.Sound.Play("diary_page_turn", Sound.Effect);
		prevButton.enabled = false;
        nextButton.enabled = false;

		var curImage = diaryImages[currentPageIndex];
        currentPageIndex--;

		UpdateLeftThickness();

		float elapsedTime = 0f;
		while (elapsedTime <= effectTime)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = 1- (elapsedTime / effectTime);

            curImage.fillAmount = fillAmount;
            yield return null;
        }

		UpdateRightThickness();

		prevButton.enabled = true;
        nextButton.enabled = true;
        _isPageTurning = false;
    }

    public void OnNextButtonClick()
    {
        if (Managers.Game.IsIntroCommentSaved)
        {
            comment.SetActive(true);
        }

        if (currentPageIndex == MaxPageIndex -1)
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
        _isPageTurning = true;
        Managers.Sound.Play("diary_page_turn", Sound.Effect);
        prevButton.enabled = false;
        nextButton.enabled = false;
		var nextIndex = currentPageIndex + 1;
        var nextImage = diaryImages[nextIndex];
        currentPageIndex = nextIndex;

		UpdateRightThickness();

		float elapsedTime = 0f;
		while (elapsedTime <= effectTime)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = elapsedTime / effectTime;
            nextImage.fillAmount = fillAmount;
            yield return null;
        }

		UpdateLeftThickness();

		prevButton.enabled = true;
        nextButton.enabled = true;
        _isPageTurning = false;
    }

	private void UpdateLeftThickness()
	{
		if (currentPageIndex % 2 != 0) return; 
        
        int thickness = currentPageIndex / 2;

		for (int i = 0; i < leftThicknessLines.Length; i++)
			leftThicknessLines[i].SetActive(i < thickness);
	}

	private void UpdateRightThickness()
	{
		if (currentPageIndex % 2 != 0) return; 
        int thickness = currentPageIndex / 2;

		int rightRemain = rightThicknessLines.Length - thickness;
		for (int i = 0; i < rightThicknessLines.Length; i++)
			rightThicknessLines[i].SetActive(i < rightRemain);
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