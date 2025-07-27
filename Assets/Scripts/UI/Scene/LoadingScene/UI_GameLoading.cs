using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// �縶���� ������ �ε�â�� �������� �ʽ��ϴ�.
/// �̼��� �� -> ���Ǽ���� �̵�
/// </summary>
public class UI_GameLoading : UI_Scene
{
    private Animator _animator;

    [SerializeField] private LoadingGameSceneData _data;

    [SerializeField] private UI_TodoList _todoList;
    [SerializeField] private TMP_Text _curDateText;
    [SerializeField] private TMP_Text _totalDateText;
    [SerializeField] private TMP_Text _worldNameText;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        
    }

    public override void Init()
    {
        base.Init();

        _animator.SetTrigger("StartAnimation");
        _data = Managers.DB.GetLoadingGameSceneData(Managers.World.CurrentWorldType);

        _totalDateText.text = "";
        _worldNameText.text = "";

        StartCoroutine(AnimateDateRange($"{_data.startDate} ~ {_data.endDate}", 1.5f));
    }

    IEnumerator AnimateDateRange(string range, float totalTime)
    {
        Managers.Sound.Play("counting",Sound.Effect);

        // �Էµ� ���ڿ��� �Ľ��ؼ� ����/�� ��¥ ��������
        string[] parts = range.Split('~');
        string[] startParts = parts[0].Trim().Split('.');
        string[] endParts = parts[1].Trim().Split('.');

        int startYear = int.Parse(startParts[0]);
        int startMonth = int.Parse(startParts[1]);
        int startDay = int.Parse(startParts[2]);
        int endYear = int.Parse(endParts[0]);
        int endMonth = int.Parse(endParts[1]);
        int endDay = int.Parse(endParts[2]);

        DateTime currentDate = new DateTime(startYear, startMonth, startDay);
        DateTime endDate = new DateTime(endYear, endMonth, Mathf.Min(endDay, DateTime.DaysInMonth(endYear, endMonth)));

        // �� ���� ���ٸ� 0
        // �� ���� �ٸ��� ���̳��� �� ��ŭ 
        int totalMonths = (endDate.Year * 12 + endDate.Month) - (currentDate.Year * 12 + currentDate.Month);

        if(totalMonths == 0)
        {
            totalMonths = 1; // �ּ� 1������ ����
        }

        float timePerMonth = totalTime / totalMonths;

        while (currentDate < endDate)
        {
            // ������ ���� ��������
            int totalDays = DateTime.DaysInMonth(currentDate.Year, currentDate.Month) - currentDate.Day;
            float elapsedTime = 0f;
            int currentDay = currentDate.Day;

            _curDateText.text = $"{currentDate.Year}.{currentDate.Month:D2}.{currentDay:D2}"; // "YYYY.MM" �ʱ� ǥ��

            // �ش� ���� 1�Ϻ��� ������ �ϱ��� ����
            while (elapsedTime < timePerMonth)
            {
                elapsedTime += Time.deltaTime;
                int newDay = Mathf.Clamp(Mathf.FloorToInt((elapsedTime / timePerMonth) * totalDays), 1, totalDays);

                if (newDay != currentDay)
                {
                    currentDay = newDay;
                    _curDateText.text = $"{currentDate.Year}.{currentDate.Month:D2}.{currentDay:D2}";
                }

                yield return null;
            }

            // ���� ���ʷ� �̵�
            currentDate = currentDate.AddMonths(1);
        }

        // ������ ��.�� ǥ��
        _curDateText.text = $"{endDate.Year}.{endDate.Month:D2}.{endDate.Day:D2}";

        yield return WaitForSecondsCache.Get(0.5f);

        OnAnimationEnd();
    }

    public void OnAnimationEnd()
    {
        StartCoroutine(ShowWorldNameText());
    }

    private IEnumerator ShowWorldNameText()
    {
        _worldNameText.gameObject.SetActive(true);

        Managers.Sound.Play("keyboard_long", Sound.SubEffect);
        yield return _worldNameText.CoTypingEffect(_data.worldName, 0.1f);
        Managers.Sound.StopSubEffect();

        yield return WaitForSecondsCache.Get(1f);
        yield return _totalDateText.CoTypingEffect($"{_data.startDate} ~ {_data.endDate}", 0.1f, true, "textbox");
        yield return WaitForSecondsCache.Get(1f);

        ShowToDoList();
    }

    private void ShowToDoList()
    {
        Managers.Sound.Play("s2_book1", Sound.Effect);
        _todoList.gameObject.SetActive(true);
        _todoList.Init(_data);
    }
}
