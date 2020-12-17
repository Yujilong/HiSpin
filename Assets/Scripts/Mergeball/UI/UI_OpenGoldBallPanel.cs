using HiSpin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_OpenGoldBallPanel : UI_PopPanelBase
    {
        public Text rewardNum;
        public Image iconImage;
        public Button adButton;
        public GameObject ad_iconGo;
        public Button nothanksButton;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.OpenGoldBallPanel;
            adButton.onClick.AddListener(OnAdClick);
            nothanksButton.onClick.AddListener(OnNothanksClick);
        }
        int clickAdTime = 0;
        private void OnAdClick()
        {
            GameManager.PlayButtonClickSound();
            clickAdTime++;
            GameManager.PlayRV(OnAdGetDoubleCallback, clickAdTime, "获得十倍金币球奖励", OnNothanksClick);
        }
        private void OnAdGetDoubleCallback()
        {
            GetReward(true);
        }
        private void OnNothanksClick()
        {
            GameManager.PlayButtonClickSound();
            GameManager.PlayIV("放弃十倍金币球奖励", OnNothanksIVCallback);
        }
        private void OnNothanksIVCallback()
        {
            GetReward(false);
        }
        int num = 0;
        Coroutine nothanksDelay = null;
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            num = GameManager.OpenGoldBallReward_Num;
            rewardNum.text = "x" + Mathf.Abs(num);
            iconImage.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.RewardNoCash, num > 0 ? "Coin":"Ticket");
            ad_button_contentText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GET) + (num > 0 ? "x10" : "x2");
            nothanksDelay = StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(nothanksButton.gameObject));
            ticket_multipleGo.SetActive(num < 0);
            int ticket_multiple = HiSpin.Save.data.allData.user_panel.user_double;
            ticket_multipleText.text = "x " + ticket_multiple.GetTicketMultipleString();
#if UNITY_IOS
            if (!GameManager.GetIsPackB())
            {
                ad_iconGo.SetActive(false);
                ad_button_contentText.transform.localPosition = new Vector3(0, ad_button_contentText.transform.localPosition.y, 0);
                StopCoroutine(nothanksDelay);
                nothanksButton.gameObject.SetActive(false);
            }
            else
            {
                ad_iconGo.SetActive(true);
                ad_button_contentText.transform.localPosition = new Vector3(51.031f, ad_button_contentText.transform.localPosition.y, 0);
            }
#endif
        }
        Coroutine raiseAniamtion = null;
        protected override void OnEndShow()
        {
            if (num < 0)
            {
                int ticket_multiple = HiSpin.Save.data.allData.user_panel.user_double;
                int correntNum = Mathf.CeilToInt(num * ticket_multiple * 0.1f);
                raiseAniamtion = StartCoroutine(NumRaiseAnimation(num, correntNum, ticket_multiple));
                num = correntNum;
            }
        }
        int rewardnum = 0;
        private void GetReward(bool addmultiple)
        {
            HiSpin.Reward type;
            if (num > 0)
            {
                type = HiSpin.Reward.Gold;
                rewardnum = addmultiple ? num * 10 : num;
                HiSpin.Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetRewardCallback, null, null, true, type, rewardnum);
            }
            else
            {
                type = HiSpin.Reward.Ticket;
                rewardnum = addmultiple ? num * 2 : num;
                HiSpin.Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetRewardCallback, null, null, true, type, -rewardnum);
            }
        }
        private void OnGetRewardCallback()
        {
            UIManager.FlyReward(num > 0 ? Reward.Coin : Reward.Ticket, Mathf.Abs(num), transform.position);
            if (rewardnum > 100)
                GameManager.AddGoldBallx10Time();
            UIManager.ClosePopPanel(this);
        }
        protected override void OnEndClose()
        {
            StopCoroutine(nothanksDelay);
            GameManager.ShowNextPanel();
        }
        [Space(15)]
        public Text titleText;
        public Text ad_button_contentText;
        public Text nothanksText;
        public override void SetContent()
        {
            titleText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Congratulation);
            nothanksText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Nothanks);
        }
        [Space(15)]
        public Text ticket_multipleText;
        public GameObject ticket_multipleGo;
        IEnumerator NumRaiseAnimation(int startNum, int endNum, int multiple)
        {
            AnimationCurve scaleCurve = HiSpin.Master.Instance.popAnimationScale;
            ticket_multipleGo.transform.localScale = Vector3.one * scaleCurve[0].value;
            ticket_multipleGo.SetActive(true);
            float scaleEndTime = scaleCurve[scaleCurve.length - 1].time;
            float maxTime = scaleEndTime;
            float progress = 0;
            while (progress < maxTime)
            {
                progress += Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
                progress = Mathf.Clamp(progress, 0, maxTime);
                ticket_multipleGo.transform.localScale = Vector3.one * scaleCurve.Evaluate(progress > scaleEndTime ? scaleEndTime : progress);
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
            float timer = 0;
            int num;
            int currentmultiple;
            while (timer < 1)
            {
                yield return null;
                timer += Time.deltaTime * 2;
                timer = Mathf.Clamp(timer, 0, 1);
                num = (int)Mathf.Lerp(startNum, endNum, timer);
                //currentmultiple = (int)Mathf.Lerp(multiple, 10, timer);
                rewardNum.text = "x" + (-num);
                //ticket_multipleText.text = "x " + currentmultiple.GetTicketMultipleString();
            }
        }
    }
}
