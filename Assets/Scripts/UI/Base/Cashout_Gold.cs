using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Cashout_Gold : BaseUI
    {
        public RectTransform anchor_rect;
        public ContentSizeFitter all_content_sizefitter;
        public GameObject top_right_paypayGo1;
        public GameObject top_right_paypayGo2;
        public GameObject top_right_paypayGo4;
        [Space(15)]
        public Button emailInputButton;
        public Button recordButton;
        [Space(15)]
        public Button pt_cashout_leftButton;
        public Button pt_cashout_midButton;
        public Button pt_cashout_rightButton;
        [Space(15)]
        public Image num_front_iconImage;
        public Button paypalCashout_leftButton;
        public Button paypalCashout_midButton;
        public Button paypalCashout_rightButton;
        [Space(15)]
        public Text gold_numText;
        public Button gold_redeemButton;
        [Space(15)]
        public Button about_feeButton;
        //L,M,R
        static readonly int[] Cashout_Nums = new int[3] { 10, 50, 100 };
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

            gold_redeemButton.AddClickEvent(OnGoldCashoutButtonClick);
            if (Language_M.isJapanese)
            {
                paypal_account_frontText.gameObject.SetActive(false);
                recordText.gameObject.SetActive(false);
                all_content_sizefitter.GetComponent<VerticalLayoutGroup>().padding.top = 44;
                top_right_paypayGo1.SetActive(true);
                top_right_paypayGo2.SetActive(true);
                top_right_paypayGo4.SetActive(true);
                num_front_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Cashout_Gold, "paypay");
                paypal_fee_ruleText.gameObject.SetActive(false);
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
        private void OnAboutFeeClick()
        {
            //Server.Instance.RequestData_GetLocalcountry(OnRequestLocalcountyCallback, null);
            OnRequestLocalcountyCallback(Server_New.localCountry);
        }
        private void OnRequestLocalcountyCallback(string country)
        {
            Application.OpenURL(string.Format("https://www.paypal.com/{0}/webapps/mpp/paypal-fees", country));
        }
        const int CashoutNeedGold = 5000000;
        public const int GoldMaxNum = 4600000;
        const int PtCashoutRate = 1000;
        public const int CashToDollerRadio = 25;
        protected override void BeforeShowAnimation(params int[] args)
        {
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
            BeforeShowAnimation();
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
        public override void SetContent()
        {
            paypal_account_frontText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_EmailFront);

            bool hasEmail = !string.IsNullOrEmpty(Save.data.allData.user_panel.user_paypal);
            if (hasEmail)
                paypal_account_placeholderText.text = Save.data.allData.user_panel.user_paypal;
            else
                paypal_account_placeholderText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_NoEmailTip);

            recordText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.RECORD);
            pt_numText.text = (int)Save.data.allData.fission_info.live_balance + "<size=40>  " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.PT) + "</size>";
            string dollar = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar);
            pt_cashout_numText.text = "≈" + string.Format(dollar, ((int)((float)Save.data.allData.fission_info.live_balance / PtCashoutRate)).GetCashShowString());
            pt_cashout_left_buttonText.text = string.Format(dollar, Language_M.isJapanese ? " 500" : " 5");
            pt_cashout_mid_buttonText.text = string.Format(dollar, Language_M.isJapanese ? " 1000" : " 10");
            pt_cashout_right_buttonText.text = string.Format(dollar, Language_M.isJapanese ? " 500" : " 50");
            paypalCash_numText.text = string.Format(dollar, Save.data.allData.user_panel.blue_cash.GetCashShowString());
            paypal_cashout_left_buttonText.text = string.Format(dollar, Language_M.isJapanese ? " 1000" : " 10");
            paypal_cashout_mid_buttonText.text = string.Format(dollar, Language_M.isJapanese ? " 5000" : " 50");
            paypal_cashout_right_buttonText.text = string.Format(dollar, Language_M.isJapanese ? " 10000" : " 100");
            gold_redeem_buttonText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_GoldRedeemTip);
            paypal_fee_ruleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_PaypalFeeTip);
            about_paypal_feeText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashout_AboutFee);
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
    }
}
