﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Cashout_Gold : BaseUI
    {
        public RectTransform anchor_rect;
        public ContentSizeFitter all_content_sizefitter;
        [Space(15)]
        public Button emailInputButton;
        public Button recordButton;
        [Space(15)]
        public Button pt_cashout_leftButton;
        public Button pt_cashout_midButton;
        public Button pt_cashout_rightButton;
        [Space(15)]
        public Image paypal_iconImage;
        public GameObject paypalGo;
        public Button paypalCashout_leftButton;
        public Button paypalCashout_midButton;
        public Button paypalCashout_rightButton;
        [Space(15)]
        public Text gold_numText;
        public Button gold_redeemButton;
        [Space(15)]
        public Button about_feeButton;
        [Space(15)]
        public Button friendevent_cashoutButton1;
        public Button friendevent_cashoutButton2;
        public Button friendevent_cashoutButton3;
        public Button friendevent_cashoutButton4;
        public Button friendevent_cashoutButton5;
        [Space(15)]
        public GameObject own_ptGo;
        public GameObject own_paypal_cashGo;
        public GameObject own_goldGo;
        public GameObject handing_feeGo;
        public GameObject own_friendevent_cashGo;
        protected override void Awake()
        {
            base.Awake();
            recordButton.AddClickEvent(OnRecordButtonClick);
            emailInputButton.AddClickEvent(OnEmailInput);
            about_feeButton.AddClickEvent(OnAboutFeeClick);
            if (Master.IsBigScreen)
            {
                anchor_rect.localPosition -= new Vector3(0, Master.TopMoveDownOffset, 0);
                anchor_rect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
                anchor_rect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
            }


            pt_cashout_leftButton.AddClickEvent(() => { OnPtCashoutButtonClick(5); });
            pt_cashout_midButton.AddClickEvent(() => { OnPtCashoutButtonClick(10); });
            pt_cashout_rightButton.AddClickEvent(() => { OnPtCashoutButtonClick(50); });
            paypalCashout_leftButton.AddClickEvent(() => { OnPaypalCashoutButtonClick(10); });
            paypalCashout_midButton.AddClickEvent(() => { OnPaypalCashoutButtonClick(50); });
            paypalCashout_rightButton.AddClickEvent(() => { OnPaypalCashoutButtonClick(100); });
            friendevent_cashoutButton1.AddClickEvent(() => { OnFriendEventCashoutButtonClick(FriendEventCashoutMinCash); });
            friendevent_cashoutButton2.AddClickEvent(() => { OnFriendEventCashoutButtonClick(300); });
            friendevent_cashoutButton3.AddClickEvent(() => { OnFriendEventCashoutButtonClick(500); });
            friendevent_cashoutButton4.AddClickEvent(() => { OnFriendEventCashoutButtonClick(700); });
            friendevent_cashoutButton5.AddClickEvent(() => { OnFriendEventCashoutButtonClick(1000); });

            gold_redeemButton.AddClickEvent(OnGoldCashoutButtonClick);
            if (Language_M.isJapanese)
            {
                paypal_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout_Gold, "paypay icon");
                paypalGo.SetActive(true);
                paypal_account_frontText.gameObject.SetActive(false);
                recordText.gameObject.SetActive(false);
                all_content_sizefitter.GetComponent<VerticalLayoutGroup>().padding.top = 32;
            }
        }
        private void OnRecordButtonClick()
        {
            UI.ShowBasePanel(BasePanel.CashoutRecord);
        }
        private void OnEmailInput()
        {
            UI.ShowPopPanel(PopPanel.InputPaypalEmail);
        }
        private void OnPtCashoutButtonClick(int cashoutNum)
        {
            if (Save.data.allData.fission_info.live_balance >= cashoutNum * PtCashoutRate * 100)
                UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Cashout, cashoutNum, (int)CashoutType.PT, cashoutNum * PtCashoutRate * 100);
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_CashOutNotEnough));
        }
        private bool CheckFriendEventCash(int cashoutNum)
        {
            List<AllData_CashoutRecordData_Record> recordData_Records = Save.data.allData.lucky_record.record;
            int recordCount = recordData_Records.Count;
            for(int i = 0; i < recordCount; i++)
            {
                AllData_CashoutRecordData_Record record = recordData_Records[i];
                if (record.apply_type == CashoutType.FriendEvent_Cash)
                {
                    if (record.apply_status == 0)
                        return false;
                    if (record.apply_doller == cashoutNum)
                        return false;
                }
            }
            return true;
        }
        private void OnCashoutButtonClick(int cashoutNum)
        {
            if (Save.data.allData.user_panel.user_doller_live >= cashoutNum * CashToDollerRadio * 100)
                UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Cashout, cashoutNum, (int)CashoutType.Cash, cashoutNum * CashToDollerRadio * 100);
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_CashOutNotEnough));
        }
        private void OnPaypalCashoutButtonClick(int cashoutNum)
        {
            if (Save.data.allData.user_panel.blue_cash >= cashoutNum * 100)
                UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Cashout, cashoutNum, (int)CashoutType.Blue_Cash, cashoutNum * 100);
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_CashOutNotEnough));
        }
        private void OnGoldCashoutButtonClick()
        {
            Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_CashOutNotEnough));
        }
        private void OnFriendEventCashoutButtonClick(int cashoutNum)
        {
            if (CheckFriendEventCash(cashoutNum))
            {
                if (Save.data.allData.user_panel.seven_doller >= cashoutNum * 100)
                    UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Cashout, cashoutNum, (int)CashoutType.FriendEvent_Cash, cashoutNum * 100);
                else
                    Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_CashOutNotEnough));
            }
            else
                Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_FriendEventWaitCashout));
        }
        private void OnAboutFeeClick()
        {
            //Server.Instance.RequestData_GetLocalcountry(OnRequestLocalcountyCallback, null);
            OnRequestLocalcountyCallback(Server.localCountry);
        }
        private void OnRequestLocalcountyCallback(string country)
        {
            Application.OpenURL(string.Format("https://www.paypal.com/{0}/webapps/mpp/paypal-fees", country));
        }
        const int CashoutNeedGold = 5000000;
        public const int GoldMaxNum = 4600000;
        public const int FriendEventCashoutMinCash = 100;
        const int PtCashoutRate = 1000;
        public const int CashToDollerRadio = 25;
        bool isFriendEventCashout = false;
        protected override void BeforeShowAnimation(params int[] args)
        {
            if (args.Length > 0)
                isFriendEventCashout = args[0] == 1;
            else
                isFriendEventCashout = false;
            own_ptGo.SetActive(!isFriendEventCashout);
            own_paypal_cashGo.SetActive(!isFriendEventCashout);
            own_goldGo.SetActive(!isFriendEventCashout);
            handing_feeGo.SetActive(!isFriendEventCashout);
            own_friendevent_cashGo.SetActive(isFriendEventCashout);
            if (Language_M.isJapanese)
            {
                handing_feeGo.SetActive(false);
                own_friendevent_cashGo.SetActive(false);
            }

            gold_numText.text = Save.data.allData.user_panel.user_gold_live.GetTokenShowString();
        }
        bool isPause = false;
        public override void Pause()
        {
            isPause = true;
        }
        public override void Resume()
        {
            if (!isPause) return;
            isPause = false;
            SetContent();
            BeforeShowAnimation(isFriendEventCashout ? 1 : 0);
        }
        [Space(15)]
        public Text paypal_account_frontText;
        public Text paypal_account_placeholderText;
        public Text recordText;
        public Text pt_numText;
        public Text pt_cashout_numText;
        public Text pt_cashout_left_buttonText;
        public Text pt_cashout_mid_buttonText;
        public Text pt_cashout_right_buttonText;
        public Text paypalCash_numText;
        public Text paypal_cashout_left_buttonText;
        public Text paypal_cashout_mid_buttonText;
        public Text paypal_cashout_right_buttonText;
        public Text gold_redeem_buttonText;
        public Text paypal_fee_ruleText;
        public Text about_paypal_feeText;

        public Text friendevent_cash_numText;
        public Text friendevent_cashout_buttonText1;
        public Text friendevent_cashout_buttonText2;
        public Text friendevent_cashout_buttonText3;
        public Text friendevent_cashout_buttonText4;
        public Text friendevent_cashout_buttonText5;
        public override void SetContent()
        {
            paypal_account_frontText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_EmailFront);

            bool hasEmail = !string.IsNullOrEmpty(Save.data.allData.user_panel.user_paypal);
            if (hasEmail)
                paypal_account_placeholderText.text = Save.data.allData.user_panel.user_paypal;
            else
                paypal_account_placeholderText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_NoEmailTip);

            recordText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.RECORD);
            string dollar = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar);
            bool isJapanese = Language_M.isJapanese;
            pt_numText.text = (int)Save.data.allData.fission_info.live_balance + "<size=40>  " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.PT) + "</size>";
            pt_cashout_numText.text = string.Format("≈" + dollar, ((int)((float)Save.data.allData.fission_info.live_balance / PtCashoutRate)).GetCashShowString());
            pt_cashout_left_buttonText.text = string.Format(dollar, isJapanese ? " 500" : " 5");
            pt_cashout_mid_buttonText.text = string.Format(dollar, isJapanese ? " 1000" : " 10");
            pt_cashout_right_buttonText.text = string.Format(dollar , isJapanese ? " 5000" : " 50");
            paypalCash_numText.text =string.Format(dollar , Save.data.allData.user_panel.blue_cash.GetCashShowString());
            paypal_cashout_left_buttonText.text =string.Format(dollar , isJapanese ? " 1000" : " 10");
            paypal_cashout_mid_buttonText.text =string.Format(dollar , isJapanese ? " 5000" : " 50");
            paypal_cashout_right_buttonText.text = string.Format(dollar , isJapanese ? " 10000" : " 100");
            gold_redeem_buttonText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_GoldRedeemTip);
            paypal_fee_ruleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_PaypalFeeTip);
            about_paypal_feeText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_AboutFee);


            friendevent_cash_numText.text = string.Format(dollar , Save.data.allData.user_panel.seven_doller.GetCashShowString());
            friendevent_cashout_buttonText1.text = string.Format(dollar , " " + FriendEventCashoutMinCash*100);
            friendevent_cashout_buttonText2.text = string.Format(dollar , isJapanese ? " 30000" : " 300");
            friendevent_cashout_buttonText3.text = string.Format(dollar ,isJapanese ? " 50000" :  " 500");
            friendevent_cashout_buttonText4.text = string.Format(dollar , isJapanese ? " 70000" : " 700");
            friendevent_cashout_buttonText5.text = string.Format(dollar , isJapanese ? " 100000" : " 1000");

            StartCoroutine("DelayRefreshLayout");
        }
        private IEnumerator DelayRefreshLayout()
        {
            all_content_sizefitter.enabled = false;
            yield return new WaitForEndOfFrame();
            all_content_sizefitter.enabled = true;
        }
    }
    public enum CashoutType
    {
        PT,
        Cash,
        Gold,
        Blue_Cash,
        FriendEvent_Cash,
    }
}