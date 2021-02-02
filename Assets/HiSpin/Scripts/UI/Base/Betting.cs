using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Betting : BaseUI
    {
        public ContentSizeFitter all_root;
        public RectTransform lotteryRect;
        public Button helpButton;
        public Button get_ticketsButton;
        public Button friend_bannerButton;
        public Text time_downText;
        //public Text ticket_numText;
        //public Text tipText;
        public BettingWinnerItem single_winner_item;
        private List<BettingWinnerItem> all_winner_items = new List<BettingWinnerItem>();
        protected override void Awake()
        {
            base.Awake();
            helpButton.AddClickEvent(OnHelpClick);
            get_ticketsButton.AddClickEvent(OnGetTicketsClick);
            friend_bannerButton.AddClickEvent(OnBannerClick);
            all_winner_items.Add(single_winner_item);
            if (Master.IsBigScreen)
            {
                RectTransform allRect = all_root.transform.parent as RectTransform;
                allRect.localPosition -= new Vector3(0, Master.TopMoveDownOffset, 0);
                allRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
                allRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
            }
            friend_bannerButton.gameObject.SetActive(Save.data.isPackB);
            if (Language_M.isJapanese)
                friend_bannerButton.gameObject.SetActive(false);
            if (!friend_bannerButton.gameObject.activeSelf)
                lotteryRect.sizeDelta -= new Vector2(0, friend_bannerButton.GetComponent<RectTransform>().rect.height);
#if UNITY_IOS
        helpButton.gameObject.SetActive(Save.data.isPackB);
#endif
        }
        private void OnHelpClick()
        {
            UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.Betting);
        }
        private void OnGetTicketsClick()
        {
            UI.ShowBasePanel(BasePanel.Task);
        }
        private void OnBannerClick()
        {
            UI.ShowBasePanel(BasePanel.FriendEvent);
        }
        protected override void BeforeShowAnimation(params int[] args)
        {
            RefreshBettingWinner();
            UpdateTimeDownText(Master.time);
        }
        public void UpdateTimeDownText(string time)
        {
            time_downText.text = time;
        }
        public void RefreshBettingWinner()
        {
            foreach (var winner in all_winner_items)
                winner.gameObject.SetActive(false);

            List<AllData_BettingWinnerData_Winner> winnerDatas = Save.data.allData.award_ranking.ranking;
            if (winnerDatas != null)
            {
                int winnerCount = winnerDatas.Count;
                for (int i = 0; i < winnerCount; i++)
                {
                    if (i > all_winner_items.Count - 1)
                    {
                        BettingWinnerItem newWinnerItem = Instantiate(single_winner_item, single_winner_item.transform.parent).GetComponent<BettingWinnerItem>();
                        all_winner_items.Add(newWinnerItem);
                    }
                    AllData_BettingWinnerData_Winner winnerInfo = winnerDatas[i];
                    all_winner_items[i].gameObject.SetActive(true);
                    all_winner_items[i].Init(winnerInfo.user_title, winnerInfo.user_id, winnerInfo.user_num);
                }
            }
            StartCoroutine("DelayRefreshLayout");
        }
        private IEnumerator DelayRefreshLayout()
        {
            all_root.enabled = false;
            yield return new WaitForEndOfFrame();
            all_root.enabled = true;
            all_root.GetComponent<ScrollRect>().normalizedPosition = Vector2.one;
        }
        [Space(15)]
        public Text titleText;
        public Text helpText;
        public Text prize_poolText;
        public Text prize_pool_prizeText;
        public Text ticket_numText;
        public Text tipText;
        public Text get_ticketsText;
        public Text invite_bannerText;
        public Text last_day_winnerText;
        public override void SetContent()
        {
            titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Betting_Title);
            helpText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
            prize_poolText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Betting_PrizePool);
            prize_pool_prizeText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), Language_M.isJapanese ? "100,000" : "1,000");

            ticket_numText.text = Save.data.allData.user_panel.user_tickets >= Save.data.allData.award_ranking.ticktes_flag ?
                string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Betting_TicketNumEnough), Save.data.allData.user_panel.user_tickets) : Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Betting_TicketNumNotEnough);

            if (Save.data.allData.user_panel.user_tickets >= Save.data.allData.award_ranking.ticktes_flag)
                tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Betting_Tip1);
            else
                tipText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Betting_Tip2), Save.data.allData.award_ranking.ticktes_flag);

            get_ticketsText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GetTickets);
            invite_bannerText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_InviteBanner), 80, 1500);
            last_day_winnerText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Betting_WinnerTitle);
        }
    }
}
