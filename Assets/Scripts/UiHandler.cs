using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class UiHandler : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] GameObject reviewBtns;
    [SerializeField] GameObject inputBtns;
    [SerializeField] Button reviewBtn;
    [SerializeField] Button scheduleBtn;

    [Header("panels")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject settingPanel;

    [Header("Time Inputs")]
    [SerializeField] TMP_InputField hour;
    [SerializeField] TMP_InputField minute;
    [SerializeField] TMP_InputField second;


    [Header("Refrences")]
    [SerializeField] WordManager manager;

    private void Start()
    {
        reviewBtn.onClick.AddListener(UniTask.UnityAction(() => ActivateReviewState()));
        scheduleBtn.onClick.AddListener(UniTask.UnityAction(() => RescheduleNotifTime()));
    }

    public async UniTaskVoid ActivateReviewState()
    {
        inputBtns.SetActive(false);
        reviewBtns.SetActive(true);
        await manager.LoadWords();
    }

    public void ActivateInputState()
    {
        reviewBtns.SetActive(false);        
        inputBtns.SetActive(true);
        manager.EmptyLines();
    }

    public void OpenSettingPanel()
    {
        mainPanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void OpenMainPanel()
    {
        settingPanel.SetActive(false);
        mainPanel.SetActive(true);

        ActivateInputState();
    }

    public async UniTaskVoid RescheduleNotifTime()
    {
        bool isHour = int.TryParse(hour.text , out int h_Result);
        bool isMinute = int.TryParse(minute.text, out int m_Result);
        bool isSecond = int.TryParse(second.text, out int s_Result);
        int _hour;
        int _min;
        int _sec;

        if (isHour)
            _hour = h_Result;
        else
            _hour = 0;

        if (isMinute)
            _min = m_Result;
        else
            _min = 0;

        if (isSecond)
            _sec = s_Result;
        else
            _sec = 0;

        await manager.SetNotifTime(_hour, _min, _sec);
    }
}
