﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Menu : MonoBehaviour, IUIBase
{
    CanvasGroup canvasGroup;
    public GameObject setting_rpGo;
    public GameObject friend_rpGo;

    public Button cashButton;
    public Button offerwallButton;
    public Button rankButton;
    public Button slotsButton;
    public Button lotteryButton;
    public Button firendButton;
    public Button settingButton;
    public Button add_ticketButton;
    public Button backButton;
    public Button play_slots_helpButton;
    public Text slots_left_free_numText;
    [Space(15)]
    public Text gold_numText;
    public Text cash_numText;
    public Text ticket_numText;
    [Space(15)]
    public GameObject all_bottomGo;
    public GameObject all_topGo;
    public GameObject all_tokenGo;
    public Text top_titleText;
    [Space(15)]
    public Image head_iconImage;
    public Image exp_progressImage;
    public Text ticket_multipleText;
    [NonSerialized]
    public readonly Dictionary<Reward, Transform> fly_target_dic = new Dictionary<Reward, Transform>();
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        cashButton.AddClickEvent(OnCashButtonClick);
        offerwallButton.AddClickEvent(OnOfferwallButtonClick);
        rankButton.AddClickEvent(OnRankButtonClick);
        slotsButton.AddClickEvent(OnSlotsButtonClick);
        lotteryButton.AddClickEvent(OnLotteryButtonClick);
        firendButton.AddClickEvent(OnFriendButtonClick);
        settingButton.AddClickEvent(OnSettingButtonClick);
        add_ticketButton.AddClickEvent(OnAddTicketButtonClick);
        backButton.AddClickEvent(OnBackButtonClick);
        play_slots_helpButton.AddClickEvent(OnPlayslotsHelpButtonClick);
        fly_target_dic.Add(Reward.Gold, gold_numText.transform.parent);
        fly_target_dic.Add(Reward.Cash, cash_numText.transform.parent);
        fly_target_dic.Add(Reward.Ticket, ticket_numText.transform.parent);
        if (Master.IsBigScreen)
        {
            RectTransform topRect = all_topGo.transform as RectTransform;
            topRect.sizeDelta = new Vector2(topRect.sizeDelta.x, topRect.sizeDelta.y + 100);
        }
    }
    private void Start()
    {
        OnSlotsButtonClick();
    }
    public void OnTaskFinishChange(bool hasFinish)
    {
        setting_rpGo.SetActive(hasFinish);
    }
    #region button event
    private void OnCashButtonClick()
    {
        if (Save.data.isPackB)
            UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.Cashout);
    }
    private void OnOfferwallButtonClick()
    {
#if UNITY_IOS
        if (!Save.data.isPackB)
        {
            UI.ShowBasePanel(BasePanel.Task);
            return;
        }
#endif
        if (Save.data.allData.user_panel.user_level >= 4)
            UI.ShowBasePanel(BasePanel.Offerwall);
        else
            Master.Instance.ShowTip(FontContains.getInstance().GetString("lang0139", 4), 2);
    }
    private void OnRankButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Rank);
    }
    private void OnSlotsButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Slots);
    }
    private void OnLotteryButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Betting);
    }
    private void OnFriendButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Friend);
        if (friend_rpGo.activeSelf)
        {
            Master.Instance.SendAdjustEnterInvitePageEvent();
            friend_rpGo.SetActive(false);
        }
        Save.data.lastClickFriendTime = DateTime.Now;
    }
    private void OnSettingButtonClick()
    {
        if (UI.CurrentBasePanel == UI.GetUI(BasePanel.PlaySlots))
            return;
        UI.ShowPopPanel(PopPanel.Setting);
    }
    private void OnAddTicketButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Task);
    }
    private void OnBackButtonClick()
    {
        UI.CloseCurrentBasePanel();
    }
    private void OnPlayslotsHelpButtonClick()
    {
        UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.PlaySlots);
    }
#endregion
    private Button currentBottomButton = null;
    private void OnChangeBottomButton(Button clickButton)
    {
        string offSpriteName = currentBottomButton == null ? "" : currentBottomButton.name + "_Off";
        string onSpriteName = clickButton.name + "_On";
        if (currentBottomButton != null)
            currentBottomButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, offSpriteName);
        currentBottomButton = clickButton;
        currentBottomButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, onSpriteName);
    }
#region update top token text
    public void UpdateGoldText()
    {
        gold_numText.text = Save.data.allData.user_panel.user_gold_live.GetTokenShowString();
    }
    public void UpdateCashText()
    {
        cash_numText.text = Save.data.allData.user_panel.user_doller_live.GetCashShowString();
    }
    public void UpdateTicketText()
    {
        ticket_numText.text = Save.data.allData.user_panel.user_tickets.GetTokenShowString();
    }
    public void UpdateHeadIcon()
    {
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + Save.data.allData.user_panel.user_title);
        ticket_multipleText.text = FontContains.getInstance().GetString("lang0069", Save.data.allData.user_panel.user_double.GetTicketMultipleString());
        exp_progressImage.fillAmount= (float)Save.data.allData.user_panel.user_exp / Save.data.allData.user_panel.level_exp;

        setting_rpGo.SetActive(false);
        foreach (var task in Save.data.allData.lucky_schedule.user_task)
        {
            if (task.taskTargetId == PlayerTaskTarget.InviteAFriend)
                continue;
            if (task.task_cur >= task.task_tar && !task.task_receive)
            {
                setting_rpGo.SetActive(true);
                break;
            }
        }
        int lv = Save.data.allData.user_panel.user_level;
        List<int> avatar_level_list = Save.data.allData.user_panel.title_level;
        List<bool> avatar_check = Save.data.head_icon_hasCheck;
        int count = avatar_level_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (lv >= avatar_level_list[i] && !avatar_check[i])
            {
                setting_rpGo.SetActive(true);
                break;
            }
        }
    }
    public void UpdateFreeSlotsLeftNumText()
    {
        int freeNum = 0;
        foreach (var free in Save.data.allData.lucky_status.white_lucky)
            if (free == 0)
                freeNum++;
        slots_left_free_numText.text = freeNum.ToString();
        slots_left_free_numText.transform.parent.gameObject.SetActive(freeNum > 0);
    }
    public void UpdateFriendWetherClickToday()
    {
        DateTime last = Save.data.lastClickFriendTime;
        DateTime now = DateTime.Now;
        bool nextDay = false;
        if (now.Year == last.Year)
        {
            if (now.Month == last.Month)
            {
                if (now.Day > last.Day)
                    nextDay = true;
                else
                    nextDay = false;
            }
            else if (now.Month > last.Month)
                nextDay = true;
        }
        else if (now.Year > last.Year)
            nextDay = true;
        friend_rpGo.SetActive(nextDay);
    }
#endregion
#region stateChange
    public IEnumerator Show(params int[] args)
    {
        RefreshTokenText();
        UpdateHeadIcon();
        UpdateFreeSlotsLeftNumText();
        UpdateFriendWetherClickToday();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
#if UNITY_IOS
        offerwallButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, Save.data.isPackB ? "Offerwall_Off" : "Task_Off");
#endif
        yield return null;
    }
    public void RefreshTokenText()
    {
        UpdateGoldText();
        UpdateTicketText();
        if (!Save.data.allData.user_panel.new_reward)
            UI.ShowPopPanel(PopPanel.GetCash, (int)GetCashArea.NewPlayerReward, Save.data.allData.user_panel.new_data_num);
        bool hasSelf = false;
        List<AllData_BettingWinnerData_Winner> bettingDatas = Save.data.allData.award_ranking.ranking;
        if (!Save.data.allData.day_flag && bettingDatas != null)
        {
            foreach (var winner in bettingDatas)
            {
                if (winner.user_id == Save.data.allData.user_panel.user_id)
                {
                    hasSelf = true;
                    cash_numText.text = (Save.data.allData.user_panel.user_doller_live - winner.user_num).GetCashShowString();
                    break;
                }
            }
            if (!hasSelf)
                UpdateCashText();
            UI.ShowPopPanel(PopPanel.StartBetting);
        }
        else
            UpdateCashText();
    }
    public IEnumerator Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        yield return null;
    }
    bool isPause = false;
    public void Pause()
    {
        isPause = true;
    }

    public void Resume()
    {
        if (!isPause) return;
        isPause = false;
        UpdateGoldText();
        UpdateCashText();
        UpdateTicketText();
        UpdateHeadIcon();
#if UNITY_IOS
        if (currentBottomButton == offerwallButton)
            offerwallButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, Save.data.isPackB ? "Offerwall_On" : "Task_On");
        else
            offerwallButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, Save.data.isPackB ? "Offerwall_Off" : "Task_Off");
#endif
    }
#endregion
    public void OnBasePanelShow(int panelIndex)
    {
        BasePanel basePanelType = (BasePanel)panelIndex;

        if (panelIndex <= (int)BasePanel.Betting)
        {
            UpdateHeadIcon();
        }
        switch (basePanelType)
        {
            case BasePanel.Cashout:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(false);
                top_titleText.gameObject.SetActive(true);
                top_titleText.text = FontContains.getInstance().GetString("lang0026");
                all_bottomGo.SetActive(false);
                backButton.gameObject.SetActive(true);
                settingButton.gameObject.SetActive(false);
                add_ticketButton.gameObject.SetActive(true);
                play_slots_helpButton.gameObject.SetActive(false);
                break;
            case BasePanel.CashoutRecord:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(false);
                top_titleText.gameObject.SetActive(true);
                top_titleText.text = FontContains.getInstance().GetString("lang0070");
                all_bottomGo.SetActive(false);
                backButton.gameObject.SetActive(true);
                settingButton.gameObject.SetActive(false);
                add_ticketButton.gameObject.SetActive(true);
                play_slots_helpButton.gameObject.SetActive(false);
                break;
            case BasePanel.Task:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(true);
                top_titleText.gameObject.SetActive(false);
                all_bottomGo.SetActive(false);
                add_ticketButton.gameObject.SetActive(true);
                play_slots_helpButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);
                settingButton.gameObject.SetActive(false);
#if UNITY_IOS
                if (!Save.data.isPackB)
                    OnChangeBottomButton(offerwallButton);
#endif
                break;
            case BasePanel.PlaySlots:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(true);
                top_titleText.gameObject.SetActive(false);
                all_bottomGo.SetActive(false);
                backButton.gameObject.SetActive(false);
                settingButton.gameObject.SetActive(true);
                add_ticketButton.gameObject.SetActive(false);
                play_slots_helpButton.gameObject.SetActive(false);
                break;
            case BasePanel.Friend:
                OnChangeBottomButton(firendButton);
                all_bottomGo.SetActive(true);
                all_topGo.SetActive(false);
                break;
            case BasePanel.Rank:
                OnChangeBottomButton(rankButton);
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(false);
                top_titleText.gameObject.SetActive(true);
                top_titleText.text = FontContains.getInstance().GetString("lang0071");
                backButton.gameObject.SetActive(false);
                settingButton.gameObject.SetActive(true);
                all_bottomGo.SetActive(true);
                add_ticketButton.gameObject.SetActive(true);
                play_slots_helpButton.gameObject.SetActive(false);
                break;
            case BasePanel.Slots:
                OnChangeBottomButton(slotsButton);
                OnBottomBasePanelShow();
                break;
            case BasePanel.Betting:
                OnChangeBottomButton(lotteryButton);
                OnBottomBasePanelShow();
                break;
            case BasePanel.Me:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(false);
                top_titleText.gameObject.SetActive(true);
                top_titleText.text = FontContains.getInstance().GetString("lang0072");
                all_bottomGo.SetActive(false);
                backButton.gameObject.SetActive(true);
                settingButton.gameObject.SetActive(false);
                add_ticketButton.gameObject.SetActive(true);
                play_slots_helpButton.gameObject.SetActive(false);
                break;
            case BasePanel.Offerwall:
                OnChangeBottomButton(offerwallButton);
                all_topGo.SetActive(false);
                all_bottomGo.SetActive(true);
                break;
            default:
                break;
        }
    }
    private void OnBottomBasePanelShow()
    {
        all_topGo.SetActive(true);
        all_tokenGo.SetActive(true);
        top_titleText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(true);
        all_bottomGo.SetActive(true);
        add_ticketButton.gameObject.SetActive(true);
        play_slots_helpButton.gameObject.SetActive(false);
    }
    public void FlyReward_GetTargetPosAndCallback_ThenFly(Reward type, int num, Vector3 startWorldPos)
    {
        FlyReward.Instance.FlyToTarget(startWorldPos, GetFlyTargetPos(type), num, type, FlyOverCallback);
    }
    private void FlyOverCallback(Reward type)
    {
        StopCoroutine("ExpandTarget");
        StartCoroutine("ExpandTarget", type);
    }
    IEnumerator ExpandTarget(Reward _flyTarget)
    {
        if (!fly_target_dic.TryGetValue(_flyTarget, out Transform tempTrans))
            yield break;
        bool toBiger = true;
        while (true)
        {
            yield return null;
            if (toBiger)
            {
                tempTrans.localScale += Vector3.one * Time.deltaTime * 3;
                if (tempTrans.localScale.x >= 1.3f)
                {
                    toBiger = false;
                    switch (_flyTarget)
                    {
                        case Reward.Gold:
                            UpdateGoldText();
                            break;
                        case Reward.Cash:
                            UpdateCashText();
                            break;
                        case Reward.Ticket:
                            UpdateTicketText();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                tempTrans.localScale -= Vector3.one * Time.deltaTime * 3;
                if (tempTrans.localScale.x <= 1f)
                    break;
            }
        }
        yield return null;
        tempTrans.localScale = Vector3.one;
    }
    private Vector3 GetFlyTargetPos(Reward type)
    {
        if (fly_target_dic.ContainsKey(type))
            return fly_target_dic[type].position;
        else
            return Vector3.zero;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI.CloseCurrentBasePanel(true);
        }
    }
    [Space(15)]
    public RectTransform guide_1Rect;
    public RectTransform guide_2Rect;
    public RectTransform guide_3Rect;
    public Vector3 GetGudieMaskPosAndSize(int guideStep, out Vector2 size)
    {
        switch (guideStep)
        {
            case 1:
                size = guide_1Rect.sizeDelta;
                return guide_1Rect.position;
            case 2:
                size = guide_2Rect.sizeDelta;
                return guide_2Rect.position;
            case 3:
                size = guide_3Rect.sizeDelta;
                return guide_3Rect.position;
            default:
                size = Vector2.zero;
                return Vector3.zero;
        }
    }
}
