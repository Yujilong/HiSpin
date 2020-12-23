using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class GetNewPlayerReward : PopUI
    {
        public CanvasGroup get_newplayer_rewardCg;
        public Button cashoutButton;
        [Space(15)]
        public CanvasGroup input_paypal_emailCg;
        public Button skipButton;
        public InputField emailInputfield;
        public Button claimButton;
        public Button agreeButton;
        public Image agreeIcon;
        public Button termsButton;
        [Space(15)]
        public CanvasGroup cashout_recordCg;
        public Text stateText;
        public Text timeValueText;
        public Text orderidValueText;
        public Text accountValueText;
        public Button nextButton;
        protected override void Awake()
        {
            base.Awake();
            cashoutButton.AddClickEvent(OnCashouButtonClick);
            skipButton.AddClickEvent(OnSkipButtonClick);
            claimButton.AddClickEvent(OnClaimButtonClick);
            agreeButton.AddClickEvent(OnAgreeButtonClick);
            termsButton.AddClickEvent(OnTermsButtonClick);
            nextButton.AddClickEvent(OnNextButtonClick);
        }
        private void Start()
        {
            OriginInputFieldLocalPos = emailInputfield.transform.localPosition;
            OriginAgreeLocalPos = agreeButton.transform.parent.localPosition;
        }
        private void OnCashouButtonClick()
        {
            Server_New.Instance.ConnectToServer_GetNewPlayerReward(OnCashoutCallback, null, null, true);
        }
        private void OnCashoutCallback()
        {
            get_newplayer_rewardCg.alpha = 0;
            get_newplayer_rewardCg.blocksRaycasts = false;
            input_paypal_emailCg.alpha = 1;
            input_paypal_emailCg.blocksRaycasts = true;
        }
        private void OnSkipButtonClick()
        {
            UI.ClosePopPanel(this);
            if (!Save.data.hasWatchThreeCardGuide)
                UI.ShowPopPanel(PopPanel.Guide, 1);
        }
        bool hasAgreeRule = false;
        Vector3 OriginInputFieldLocalPos;
        Vector3 OriginAgreeLocalPos;
        private void OnClaimButtonClick()
        {
            if (string.IsNullOrEmpty(emailInputfield.text) || string.IsNullOrWhiteSpace(emailInputfield.text))
            {
                StopCoroutine("ShakeSomething");
                emailInputfield.transform.localPosition = OriginInputFieldLocalPos;
                agreeButton.transform.parent.localPosition = OriginAgreeLocalPos;
                StartCoroutine("ShakeSomething", emailInputfield.transform);
            }
            else if (!hasAgreeRule)
            {
                StopCoroutine("ShakeSomething");
                emailInputfield.transform.localPosition = OriginInputFieldLocalPos;
                agreeButton.transform.parent.localPosition = OriginAgreeLocalPos;
                StartCoroutine("ShakeSomething", agreeButton.transform.parent);
            }
            else
                Server_New.Instance.ConnectToServer_BindPaypal(OnBindPaypalCallback, null, null, true, emailInputfield.text, " ", " ");
        }
        IEnumerator ShakeSomething(Transform targetTrans)
        {
            float shakeOffsetX = 15;
            float shakeOffsetY = 5;
            float startLocalX = targetTrans.localPosition.x - shakeOffsetX;
            float endLocalX = targetTrans.localPosition.x + shakeOffsetX;
            float startLocalY = targetTrans.localPosition.y - shakeOffsetY;
            float endLocalY = targetTrans.localPosition.y + shakeOffsetY;
            float progressX = 0.5f;
            float progressY = 0.5f;
            bool isUp = true;
            bool isRight = true;
            bool xEnd = false;
            bool yEnd = false;
            float speedX = 20;
            float speedY = 20;
            int turnX = 4;
            int turnY = 4;
            int turnIndexX = 0;
            int turnIndexY = 0;
            while (true)
            {
                if (!xEnd)
                {
                    progressX += isRight ? Time.deltaTime * speedX : -Time.deltaTime * speedX;
                    progressX = Mathf.Clamp(progressX, 0, 1);
                }
                if (!yEnd)
                {
                    progressY += isRight ? Time.deltaTime * speedY : -Time.deltaTime * speedY;
                    progressY = Mathf.Clamp(progressY, 0, 1);
                }
                if (turnIndexX >= turnX)
                {
                    if (turnX % 2 == 1)
                        progressX = Mathf.Clamp(progressX, 0.5f, 1);
                    else
                        progressX = Mathf.Clamp(progressX, 0, 0.5f);
                    if (progressX == 0.5f)
                    {
                        xEnd = true;
                    }
                }
                if (turnIndexY >= turnY)
                {
                    if (turnY % 2 == 1)
                        progressY = Mathf.Clamp(progressY, 0.5f, 1);
                    else
                        progressY = Mathf.Clamp(progressY, 0, 0.5f);
                    if (progressY == 0.5f)
                    {
                        xEnd = true;
                    }
                }
                targetTrans.localPosition = new Vector3(Mathf.Lerp(startLocalX, endLocalX, progressX), Mathf.Lerp(startLocalY, endLocalY, progressY), 0);
                if (isRight && progressX >= 1)
                {
                    turnIndexX++;
                    isRight = false;
                }
                else if (!isRight && progressX <= 0)
                {
                    turnIndexX++;
                    isRight = true;
                }
                if (isRight && progressY >= 1)
                {
                    turnIndexY++;
                    isUp = false;
                }
                else if (!isRight && progressY <= 0)
                {
                    turnIndexY++;
                    isUp = true;
                }
                yield return null;
                if (xEnd && yEnd)
                    break;
            }
        }
        private void OnBindPaypalCallback()
        {
            input_paypal_emailCg.alpha = 0;
            input_paypal_emailCg.blocksRaycasts = false;
            cashout_recordCg.alpha = 1;
            cashout_recordCg.blocksRaycasts = true;
            Master.Instance.SendAdjustCheckinEvent(0);
        }
        private void OnAgreeButtonClick()
        {
            hasAgreeRule = !hasAgreeRule;
            agreeIcon.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetNewPlayerReward, hasAgreeRule ? "agree" : "disagree");
        }
        private void OnTermsButtonClick()
        {
            Application.OpenURL("");
        }
        private void OnNextButtonClick()
        {
            UI.ClosePopPanel(this);
            UI.ShowPopPanel(PopPanel.Guide, -1);
        }
        int step = -1;
        protected override void BeforeShowAnimation(params int[] args)
        {
            step = args[0];
            hasAgreeRule = false;
            emailInputfield.SetTextWithoutNotify("");
            System.DateTime now = System.DateTime.Now;
            timeValueText.text = now.Year + "-" + now.Month + "-" + now.Day;
            StringBuilder orderid = new StringBuilder("PAY");
            orderid.Append(now.Year);
            orderid.Append(now.Month);
            orderid.Append(now.Day);
            for (int i = 0; i < 8; i++)
                orderid.Append(Random.Range(0, 10));
            orderidValueText.text = orderid.ToString();
            accountValueText.text = Save.data.allData.user_panel.user_paypal;
            switch (step)
            {
                case 0:
                    get_newplayer_rewardCg.alpha = 1;
                    get_newplayer_rewardCg.blocksRaycasts = true;
                    input_paypal_emailCg.alpha = 0;
                    input_paypal_emailCg.blocksRaycasts = false;
                    cashout_recordCg.alpha = 0;
                    cashout_recordCg.blocksRaycasts = false;
                    break;
                case 1:
                    get_newplayer_rewardCg.alpha = 0;
                    get_newplayer_rewardCg.blocksRaycasts = false;
                    input_paypal_emailCg.alpha = 1;
                    input_paypal_emailCg.blocksRaycasts = true;
                    cashout_recordCg.alpha = 0;
                    cashout_recordCg.blocksRaycasts = false;
                    break;
            }
            agreeIcon.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetNewPlayerReward, hasAgreeRule ? "agree" : "disagree");
        }
        [Space(15)]
        public Text getnewplayerrewardTitleText;
        public Text prizeText;
        public Text cashnumText;
        public Text cashout_buttonText;

        public Text skipText;
        public Text inputpaypal_prizeText;
        public Text card_cashnumText;
        public Text tipText;
        public Text inputpaypal_placeholderText;
        public Text claimText;
        public Text front_ruleText;
        public Text termsText;

        public Text cashout_cashnumText;
        public Text cashout_stateText;
        public Text timeText;
        public Text orderidText;
        public Text accountText;
        public Text nextText;
        public override void SetContent()
        {
            getnewplayerrewardTitleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Congratulation) + "!";
            prizeText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_Prize);
            cashnumText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + "50.00";
            cashout_buttonText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);

            skipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_Skip);
            inputpaypal_prizeText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_Prize);
            card_cashnumText.text= Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + "50.00";
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_Tip);
            inputpaypal_placeholderText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_InputTip);
            claimText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CLAIM);
            front_ruleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_FrontRule);
            termsText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_BehindRule);

            cashout_cashnumText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + "50.00";
            cashout_stateText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_State);
            timeText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_Time);
            orderidText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_Orderid);
            accountText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_Account);
            nextText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.NewPlayerReward_Next);
        }
        protected override void AfterShowAnimation(params int[] args)
        {
        }
    }
}
