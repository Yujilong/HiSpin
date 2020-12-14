using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class GetCash : PopUI
    {
        public Button tribleButton;
        public Button nothanksButton;
        public Text add_cashpt_numText;
        public GameObject ad_iconGo;
        protected override void Awake()
        {
            base.Awake();
            tribleButton.AddClickEvent(OnGetClick);
            nothanksButton.AddClickEvent(OnNothanksClick);
        }
        private void OnNothanksClick()
        {
            switch (getCashArea)
            {
                case GetCashArea.Mergeball:
                    Ads._instance.ShowInterstialAd(OnGetOneSlotsRewardCallback, "获得现金 nothanks");
                    break;
                default:
                    Master.Instance.ShowTip("Error : Cash Area is not correct.");
                    break;
            }
        }
        int clickAdTime = 0;
        private void OnGetClick()
        {
            switch (getCashArea)
            {
                case GetCashArea.NewPlayerReward:
                    Server_New.Instance.ConnectToServer_GetNewPlayerReward(OnGetNewplayerRewardCallback, null, null, true);
                    break;
                case GetCashArea.Mergeball:
                    clickAdTime++;
                    Ads._instance.ShowRewardVideo(() => { Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetTribleSlotsRewardCallback, null, null, true, Reward.Cash, getcashNum , isMergeballSlots); }, clickAdTime, "现金翻倍", OnNothanksClick);
                    break;
            }
        }
        private void OnGetTribleSlotsRewardCallback()
        {
            Save.data.allData.user_panel.lucky_total_cash += getcashNum;
            UI.FlyReward(Reward.Cash, getcashNum, tribleButton.transform.position);
            UI.ClosePopPanel(this);
        }
        private void OnGetOneSlotsRewardCallback()
        {
            UI.ClosePopPanel(this);
        }
        private void OnGetNewplayerRewardCallback()
        {
            UI.FlyReward(Reward.Cash, getcashNum, tribleButton.transform.position);
            UI.ClosePopPanel(this);
            if (Save.data.isPackB)
                UI.ShowPopPanel(PopPanel.Guide);
        }
        GetCashArea getCashArea;
        int getcashNum;
        bool isMergeballSlots;
        protected override void BeforeShowAnimation(params int[] args)
        {
            clickAdTime = 0;
            getCashArea = (GetCashArea)args[0];
            getcashNum = args[1];
            isMergeballSlots = args[2] == 1;
            bool isPackB = Save.data.isPackB;

            switch (getCashArea)
            {
                case GetCashArea.NewPlayerReward:
                    ad_iconGo.SetActive(false);
                    trible_button_contentText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GetCash_SaveInWallet);
                    trible_button_contentText.GetComponent<RectTransform>().sizeDelta = new Vector2(657, 110);
                    cash_numText.text = (isPackB ? Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) : "") + getcashNum.GetCashShowString();
                    add_cashpt_numText.transform.parent.gameObject.SetActive(false);
                    nothanksText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Nothanks);
                    break;
                case GetCashArea.Mergeball:
                    ad_iconGo.SetActive(true);
                    trible_button_contentText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GetCash_SaveInWallet);
                    trible_button_contentText.GetComponent<RectTransform>().sizeDelta = new Vector2(534, 110);
                    cash_numText.text = (isPackB ? Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) : "") + (Save.data.allData.user_panel.user_doller_live / Cashout.CashToDollerRadio).GetCashShowString();
                    add_cashpt_numText.transform.parent.gameObject.SetActive(true);
                    add_cashpt_numText.text = "+" + (isPackB ? Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) : "") + (getcashNum / Cashout.CashToDollerRadio).GetCashShowString();
                    nothanksText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Nothanks);
                    break;
            }

            nothanksButton.gameObject.SetActive(false);
        }
        protected override void AfterShowAnimation(params int[] args)
        {
            Master.Instance.ShowEffect(Reward.Cash);
            if (getCashArea == GetCashArea.Mergeball)
            {
                StartCoroutine("DelayShowNothanks");
            }
        }
        protected override void BeforeCloseAnimation()
        {
            StopCoroutine("DelayShowNothanks");
            if (getCashArea == GetCashArea.Mergeball)
                GameManager.Instance.ReduceTodayCanGetCashTime();
        }
        private IEnumerator DelayShowNothanks()
        {
            yield return new WaitForSeconds(1);
            nothanksButton.gameObject.SetActive(true);
        }
        [Space(15)]
        public Text titleText;
        public Text cash_numText;
        public Text trible_button_contentText;
        public Text nothanksText;
        public override void SetContent()
        {
            titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Congratulation);


        }
    }
    public enum GetCashArea
    {
        NewPlayerReward,
        Mergeball
    }
}
