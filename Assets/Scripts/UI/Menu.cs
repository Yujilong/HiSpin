using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
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
        public Button taskButton;
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
            taskButton.AddClickEvent(OnTaskButtonClick);
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
#if UNITY_IOS
        play_slots_helpButton.gameObject.SetActive(Save.data.isPackB);
#endif
        }
        private void Start()
        {
            OnSlotsButtonClick();
        }
        public void OnTaskFinishChange(bool hasFinish)
        {
            setting_rpGo.SetActive(hasFinish);
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
        #region button event
        private void OnCashButtonClick()
        {
            if (Save.data.isPackB)
                UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.Cashout);
        }
        private void OnOfferwallButtonClick()
        {
            if (Save.data.allData.user_panel.user_level >= 2)
                UI.ShowBasePanel(BasePanel.Offerwall);
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_ClickUnlockOfferwall), 2);
        }
        private void OnRankButtonClick()
        {
            if (GameManager.Instance.PlayerDataManager.playerData.hasUnlockRankAndLottery)
                UI.ShowBasePanel(BasePanel.Rank);
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_LockRankAndLottery), 2);
        }
        private void OnSlotsButtonClick()
        {
            UI.ShowBasePanel(BasePanel.MergeBall);
        }
        private void OnLotteryButtonClick()
        {
            if (GameManager.Instance.PlayerDataManager.playerData.hasUnlockRankAndLottery)
                UI.ShowBasePanel(BasePanel.Betting);
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_LockRankAndLottery), 2);
        }
        private void OnFriendButtonClick()
        {
            if (Save.data.allData.user_panel.user_level >= 2)
            {
                UI.ShowBasePanel(BasePanel.Friend);
                if (friend_rpGo.activeSelf)
                {
                    Master.Instance.SendAdjustEnterInvitePageEvent();
                    friend_rpGo.SetActive(false);
                }
                Save.data.lastClickFriendTime = DateTime.Now;
            }
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_ClickUnlockOfferwall), 2);
        }
        private void OnTaskButtonClick()
        {
            UI.ShowBasePanel(BasePanel.Task);
        }
        private void OnSettingButtonClick()
        {
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
            {
                currentBottomButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, offSpriteName);
                if (currentBottomButton != slotsButton)
                    currentBottomButton.GetComponentInChildren<Text>().color = button_off_textColor;
            }
            currentBottomButton = clickButton;
            currentBottomButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, onSpriteName);
            if (currentBottomButton != slotsButton)
                currentBottomButton.GetComponentInChildren<Text>().color = button_on_textColor;
        }
        #region update top token text
        public void UpdateGoldText()
        {
            gold_numText.text = Save.data.allData.user_panel.user_gold_live.GetTokenShowString();
        }
        public void UpdateCashText()
        {
            cash_numText.text = (Save.data.allData.user_panel.user_doller_live / Cashout.CashToDollerRadio).GetCashShowString();
        }
        public void UpdateTicketText()
        {
            ticket_numText.text = Save.data.allData.user_panel.user_tickets.GetTokenShowString();
        }
        public void UpdateHeadIcon()
        {
            head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + Save.data.allData.user_panel.user_title);
            ticket_multipleText.text = "x " + Save.data.allData.user_panel.user_double.GetTicketMultipleString();
            exp_progressImage.fillAmount = (float)Save.data.allData.user_panel.user_exp / Save.data.allData.user_panel.level_exp;

            setting_rpGo.SetActive(false);
            foreach (var task in Save.data.allData.lucky_schedule.user_task)
            {
#if UNITY_IOS
                if (!Save.data.isPackB)
                    if (task.task_type == 3)
                        continue;
#endif
                if (!Tasks.CheckIOSTaskIsShow(task.taskTargetId))
                    continue;
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
        public void UpdateEnergyNumText()
        {
            int mergeballEnergy = GameManager.Instance.PlayerDataManager.playerData.energy;
            slots_left_free_numText.text = mergeballEnergy.ToString();
            slots_left_free_numText.transform.parent.gameObject.SetActive(mergeballEnergy > 0);
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
            UpdateEnergyNumText();
            UpdateFriendWetherClickToday();

#if UNITY_IOS
        bool isPackB = Save.data.isPackB;
        offerwallButton.gameObject.SetActive(isPackB);
        lotteryButton.gameObject.SetActive(isPackB);
        firendButton.gameObject.SetActive(isPackB);
        taskButton.gameObject.SetActive(!isPackB);
#endif
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            yield return null;
        }
        public void RefreshTokenText()
        {
            UpdateGoldText();
            UpdateTicketText();
#if UNITY_IOS
        if(Save.data.isPackB)
#endif
            if (!Save.data.allData.user_panel.new_reward)
                UI.ShowPopPanel(PopPanel.GetCash, (int)GetCashArea.NewPlayerReward, Save.data.allData.user_panel.new_data_num);
#if UNITY_IOS
        if (!Save.data.isPackB)
        {
            UpdateCashText();
            return;
        }
#endif
            bool hasSelf = false;
            List<AllData_BettingWinnerData_Winner> bettingDatas = Save.data.allData.award_ranking.ranking;
            if (!Save.data.allData.day_flag && bettingDatas != null)
            {
                foreach (var winner in bettingDatas)
                {
                    if (winner.user_id == Save.data.allData.user_panel.user_id)
                    {
                        hasSelf = true;
                        cash_numText.text = (Save.data.allData.user_panel.user_doller_live - winner.user_num).GetBigTokenString();
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
                    top_titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);
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
                    top_titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.RECORD);
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
                    top_titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Menu_Title_LastDayRanking);
                    backButton.gameObject.SetActive(false);
                    settingButton.gameObject.SetActive(true);
                    all_bottomGo.SetActive(true);
                    add_ticketButton.gameObject.SetActive(true);
                    play_slots_helpButton.gameObject.SetActive(false);
                    break;
                case BasePanel.MergeBall:
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
                    top_titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Menu_Title_ME);
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
#if UNITY_IOS
            if (!Save.data.isPackB)
                all_bottomGo.SetActive(false);
#endif
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
        public void FlyReward_GetTargetPosAndCallback_ThenFly(Reward type, int num, Vector3 startWorldPos,bool isMergeball=false)
        {
            FlyReward.Instance.FlyToTarget(startWorldPos, GetFlyTargetPos(type), num, type, FlyOverCallback,isMergeball);
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
        public Text offerwallText;
        public Text rankText;
        public Text giveawaysText;
        public Text friendsText;
        public Text taskText;
        public Text top_titleText;
        public Color button_on_textColor;
        public Color button_off_textColor;
        public void SetContent()
        {
            offerwallText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Menu_Offerwall);
            rankText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Menu_Ranking);
            giveawaysText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Menu_Giveaways);
            friendsText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Menu_Friends);
            taskText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Menu_Tasks);

            offerwallText.color = button_off_textColor;
            rankText.color = button_off_textColor;
            giveawaysText.color = button_off_textColor;
            friendsText.color = button_off_textColor;
            taskText.color = button_off_textColor;
        }
    }
}
