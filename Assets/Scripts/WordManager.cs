using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

public class WordManager : MonoBehaviour
{    
    DatabaseHelper helper;   

    [Header("read & write")]
    [SerializeField] TMP_InputField word;
    [SerializeField] TMP_InputField mean;
    [SerializeField] TMP_InputField description;

    [Header("Buttons")]
    [SerializeField] Button saveBtn;
    [SerializeField] Button nextBtn;
    [SerializeField] Button previousBtn;
    [SerializeField] Button rightOverwriteBtn;
    [SerializeField] Button wrongOverwriteBtn;
    [SerializeField] Button overwriteTextBtn;

    [Header("Refrences")]
    [SerializeField] _NotificationManager notifManager;

    CancellationToken cts;

    List<int> currentIDs;
    int currentIndex;

    Word current;
    DateTime? nearestCheck;
    Dictionary<int, bool> index_Review = new Dictionary<int, bool>();

    int hour;
    int minute;
    int second;

    private async void Start()
    {
        cts = this.GetCancellationTokenOnDestroy();             
        helper = new DatabaseHelper();
        await helper.Initialize(100, "English.db", "Words", cts);

        saveBtn.onClick.AddListener(UniTask.UnityAction(() => SaveToDatabaseAsync()));
        nextBtn.onClick.AddListener(UniTask.UnityAction(() => NextAsync()));
        previousBtn.onClick.AddListener(UniTask.UnityAction(() => PreviousAsync()));

        rightOverwriteBtn.onClick.AddListener(UniTask.UnityAction(() => R_OverwriteAsync()));
        wrongOverwriteBtn.onClick.AddListener(UniTask.UnityAction(() => W_OverwriteAsync()));
        overwriteTextBtn.onClick.AddListener(UniTask.UnityAction(() => OverwriteTextAsync()));


        if (PlayerPrefs.HasKey("Hour"))
        {
            hour = PlayerPrefs.GetInt("Hour");
            minute = PlayerPrefs.GetInt("Minute");
            second = PlayerPrefs.GetInt("Second");
        }
        else
        {
            hour = 22;
            minute = 0;
            second = 0;
        }
    }

    public async UniTask SetNotifTime(int hour , int minute , int second)
    {
        this.hour = hour;
        this.minute = minute;
        this.second = second;

        PlayerPrefs.SetInt("Hour" , hour);
        PlayerPrefs.SetInt("Minute" , minute);
        PlayerPrefs.SetInt("Second" , second);

        nearestCheck = await helper.GetNearestCheckDate();

        if (nearestCheck != null)
            SetNewNotification();
    }

    public async UniTask LoadWords()
    {
        await GetTodayIDs();

        if (currentIDs == null || currentIDs.Count == 0)
        {
            nextBtn.interactable = false;
            previousBtn.interactable = false;
            rightOverwriteBtn.interactable = false;
            wrongOverwriteBtn.interactable = false;
            overwriteTextBtn.interactable = false;
            return;
        }

        currentIndex = 0;
        await ShowWordAsync();
    }

    async UniTaskVoid SaveToDatabaseAsync()
    {
        int delay = FibonacciNumbers.GetNumber(0);
        DateTime nextCheck = DateTime.Today.AddDays(delay);

        await helper.InsertDataAsync(word.text, mean.text, description.text, nextCheck.ToString("yyyy-MM-dd"), 0);
        EmptyLines();

        nearestCheck = await helper.GetNearestCheckDate();

        if (nearestCheck != null)
            SetNewNotification();
    }

    public void EmptyLines()
    {
        word.text = "";
        mean.text = "";
        description.text = "";
    }

    public async UniTaskVoid R_OverwriteAsync()
    {
        index_Review[current.id] = true;

        rightOverwriteBtn.interactable = false;
        wrongOverwriteBtn.interactable = false;

        int numLength = FibonacciNumbers.numbers.Length - 1;
        if (current.repeat > numLength)
            current.repeat = numLength;
        else
            current.repeat++;

        await SetNextCheckTime();
    }

    public async UniTaskVoid W_OverwriteAsync()
    {
        index_Review[current.id] = true;

        rightOverwriteBtn.interactable = false;
        wrongOverwriteBtn.interactable = false;

        if (current.repeat > 0)
            current.repeat--;
        else
            current.repeat = 0;

        await SetNextCheckTime();
    }

    public async UniTaskVoid OverwriteTextAsync()
    {
        current.word = word.text;
        current.meaning = mean.text;
        current.description = description.text;

        await helper.ChangeDataAsync(current);
    }

    async UniTask SetNextCheckTime()
    {
        DateTime nextCheck = DateTime.Today.AddDays(FibonacciNumbers.GetNumber(current.repeat));
        current.nextCheckDate = nextCheck.ToString("yyyy-MM-dd");

        await helper.ChangeDataAsync(current);

        nearestCheck = await helper.GetNearestCheckDate();

        if(nearestCheck != null)
            SetNewNotification();       
    }

    async UniTaskVoid NextAsync()
    {
        currentIndex++;
        nextBtn.interactable = false;
        previousBtn.interactable = false;

        current = await helper.GetFullRowAsync(currentIDs[currentIndex] , cts);
        SetReviewValues();

        if (isHasNext())
            nextBtn.interactable = true;
        if (isHasPrevious())
            previousBtn.interactable = true;

        CheckIfReviewed();
    }

    async UniTaskVoid PreviousAsync()
    {
        currentIndex--;
        nextBtn.interactable = false;
        previousBtn.interactable = false;

        current = await helper.GetFullRowAsync(currentIDs[currentIndex], cts);
        SetReviewValues();

        if (isHasNext())
            nextBtn.interactable = true;
        if (isHasPrevious())
            previousBtn.interactable = true;

        CheckIfReviewed();
    }

    void CheckIfReviewed()
    {
        if (index_Review[current.id])
        {
            rightOverwriteBtn.interactable = false;
            wrongOverwriteBtn.interactable = false;
        }
        else
        {
            rightOverwriteBtn.interactable = true;
            wrongOverwriteBtn.interactable = true;
        }
    }

    async UniTask ShowWordAsync()
    {
        current = await helper.GetFullRowAsync(currentIDs[currentIndex], cts);
        SetReviewValues();

        previousBtn.interactable = false;
        if (isHasNext())
            nextBtn.interactable = true;
        else
            nextBtn.interactable = false;
    }

    bool isHasNext() => (currentIndex + 1) < currentIDs.Count;
    bool isHasPrevious() => (currentIndex - 1) >= 0;

    async UniTask GetTodayIDs()
    {
        DateTime today = DateTime.Now;
        currentIDs = await helper.GetIDsWithDaytime(today.ToString("yyyy-MM-dd"), cts);

        index_Review.Clear();
        for (int i = 0; i < currentIDs.Count; i++)
        {
            index_Review.Add(currentIDs[i], false);
        }
    }

    void SetNewNotification()
    {
        DateTime nextNotifyDay = new DateTime(nearestCheck.Value.Year, nearestCheck.Value.Month, 
            nearestCheck.Value.Day, hour, minute, second , DateTimeKind.Local);

        notifManager.ScheduleNotification(nextNotifyDay);
    }

    void SetReviewValues()
    {
        word.text = current.word;
        mean.text = current.meaning;
        description.text = current.description;
    }

    private void OnDestroy()
    {
        helper.OnDestroy();
    }
}
