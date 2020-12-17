using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class CashoutPop : PopUI
    {
        public Image baseImage;
        public Button closeButton;
        [Space(15)]
        public CanvasGroup Input_emailCg;
        public InputField paypal_accountInput;
        public Button confirm_accountButton;
        [Space(15)]
        public CanvasGroup Cash_outCg;
        public Button cashoutButton;
        [Space(15)]
        public CanvasGroup cashout_fail_helpCg;
        [Space(15)]
        public CanvasGroup rate_usCg;
        public Button no_starButton;
        public Button yes_starButton;
        protected override void Awake()
        {
            base.Awake();
            closeButton.AddClickEvent(OnCloseButtonClick);
            confirm_accountButton.AddClickEvent(OnConfirmAccountClick);
            cashoutButton.AddClickEvent(OnCashoutClick);
            no_starButton.AddClickEvent(OnCloseButtonClick);
            yes_starButton.AddClickEvent(OnFiveStarClick);
        }
        private void OnFiveStarClick()
        {
#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Master.PackageName);
#elif UNITY_IOS
        var url = string.Format(
           "itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review",
           Master.AppleId);
        Application.OpenURL(url);
#endif
            UI.ClosePopPanel(this);
        }
        private void OnCloseButtonClick()
        {
            UI.ClosePopPanel(this);
        }
        private void OnConfirmCallback()
        {
            Master.Instance.SendAdjustInputEmailEvent(paypal_accountInput.text);
            TaskAgent.TriggerTaskEvent(PlayerTaskTarget.WritePaypalEmail, 1);
            UI.ClosePopPanel(this);
        }
        private void OnConfirmAccountClick()
        {
            //string email = paypal_accountInput.text;
            //if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            //    Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_EmptyEmail));
            //else
            //    Server_New.Instance.ConnectToServer_BindPaypal(OnConfirmCallback, null, null, true, email);
        }
        private void OnCashoutClick()
        {
            //Server.Instance.OperationData_Cashout(OnCashoutCallback, null, cashoutType, cashoutTypeNum, cashoutNum);
            Server_New.Instance.ConnectToServer_Cashout(OnCashoutCallback, null, null, true, cashoutType, cashoutTypeNum, cashoutNum);
        }
        private void OnCashoutCallback()
        {
            Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_CashoutSuccess), 3);
            UI.ClosePopPanel(this);
        }
        AsCashoutArea asArea;
        CashoutType cashoutType;
        int cashoutTypeNum;
        int cashoutNum;
        protected override void BeforeShowAnimation(params int[] args)
        {
            asArea = (AsCashoutArea)args[0];
            switch (asArea)
            {
                case AsCashoutArea.Cashout:
                    closeButton.gameObject.SetActive(true);
                    Input_emailCg.alpha = 0;
                    Input_emailCg.blocksRaycasts = false;
                    Cash_outCg.alpha = 1;
                    Cash_outCg.blocksRaycasts = true;
                    cashout_fail_helpCg.alpha = 0;
                    cashout_fail_helpCg.blocksRaycasts = false;
                    rate_usCg.alpha = 0;
                    rate_usCg.blocksRaycasts = false;
                    cashoutNum = args[1];
                    cashoutType = (CashoutType)args[2];
                    cashoutTypeNum = args[3];
                    cashout_numText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + " " + args[1].GetTokenShowString();
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);
                    baseImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.AsCashoutPop, "base_n");
                    break;
                case AsCashoutArea.FailHelp:
                    closeButton.gameObject.SetActive(true);
                    Input_emailCg.alpha = 0;
                    Input_emailCg.blocksRaycasts = false;
                    Cash_outCg.alpha = 0;
                    Cash_outCg.blocksRaycasts = false;
                    cashout_fail_helpCg.alpha = 1;
                    cashout_fail_helpCg.blocksRaycasts = true;
                    rate_usCg.alpha = 0;
                    rate_usCg.blocksRaycasts = false;
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Failed).ToUpper();
                    baseImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.AsCashoutPop, "base_f");
                    break;
                case AsCashoutArea.Rateus:
                    closeButton.gameObject.SetActive(false);
                    Input_emailCg.alpha = 0;
                    Input_emailCg.blocksRaycasts = false;
                    Cash_outCg.alpha = 0;
                    Cash_outCg.blocksRaycasts = false;
                    cashout_fail_helpCg.alpha = 0;
                    cashout_fail_helpCg.blocksRaycasts = false;
                    rate_usCg.alpha = 1;
                    rate_usCg.blocksRaycasts = true;
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rateus_WindowTitle);
                    baseImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.AsCashoutPop, "base_n");
                    break;
            }
        }
        [Space(15)]
        public Text titleText;

        public Text input_paypal_account_tipText;
        public Text input_paypal_placeholderText;
        public Text input_paypal_confirmText;
        public Text input_paypal_cautionText;

        public Text cashout_numText;
        public Text cashout_cashoutText;
        public Text cashout_cautionText;

        public Text cashoutfailed_titleText;
        public Text cashoutfailed_reasonText;
        public Text cashoutfailed_contactText;

        public Text rateus_tipText;
        public Text rateus_noText;
        public Text rateus_yesText;
        public override void SetContent()
        {

            cashout_cashoutText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT) + "!";
            cashout_cautionText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Check_Caution);

            cashoutfailed_titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashoutfailed_Title);
            cashoutfailed_reasonText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashoutfailed_Reason);
            cashoutfailed_contactText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Cashoutfailed_Contact);

            rateus_tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rateus_Tip);
            rateus_noText.text = "1~4 " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rateus_Stars);
            rateus_yesText.text = "5 " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rateus_Stars);
        }
    }
    public enum AsCashoutArea
    {
        Cashout,
        FailHelp,
        Rateus,
    }
}
