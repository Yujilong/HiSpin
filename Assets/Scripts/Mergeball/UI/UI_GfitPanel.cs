using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_GfitPanel : UI_PopPanelBase
    {
        public GameObject paypalGo;
        public Button openButton;
        public Button nothanksButton;
        public Button closeButton;
        public GameObject adIcon;
        public GameObject redeemTip;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.GiftPanel;
            openButton.onClick.AddListener(OnOpenClick);
            closeButton.onClick.AddListener(OnCloseClick);
            nothanksButton.onClick.AddListener(OnCloseClick);
            paypalGo.SetActive(HiSpin.Language_M.isJapanese);
        }
        int clickAdTime = 0;
        private void OnOpenClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
            {
                clickAdTime++;
                GameManager.PlayRV(OnOpenAdCallback, clickAdTime, "打开礼盒", OnCloseClick);
            }
            else
            {
                GameManager.SetHasGetFreeGift();
                OnOpenAdCallback();
            }
        }
        private void OnOpenAdCallback()
        {
            if (GameManager.isPropGift)
                GameManager.isPropGift = false;
            else
                GameManager.AddOpenGiftBallNum();
            Reward type = GameManager.RandomGiftReward(out int num);
            GameManager.ShowConfirmRewardPanel(type, num, needAd);
            UIManager.ClosePopPanel(this);
        }
        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            GameManager.PlayIV("放弃礼盒", OnCloseIVCallback);
        }
        private void OnCloseIVCallback()
        {
            UIManager.ClosePopPanel(this);
            GameManager.ShowNextPanel();
        }
        bool needAd = false;
        Coroutine nothanksDelay = null;
        Coroutine closeDelay = null;
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            needAd = GameManager.GetHasGetFreeGift();
#if UNITY_IOS
            if (!GameManager.GetIsPackB())
                needAd = false;
#endif
            nothanksButton.gameObject.SetActive(needAd);
            adIcon.SetActive(needAd);
            closeButton.gameObject.SetActive(needAd);
            redeemTip.SetActive(GameManager.GetIsPackB());
            openText.transform.localPosition = needAd ? new Vector3(39.6f, openText.transform.localPosition.y, 0) : new Vector3(0, openText.transform.localPosition.y, 0);
            if (needAd)
            {
                nothanksDelay= StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(nothanksButton.gameObject));
                closeDelay= StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(closeButton.gameObject));
            }
        }
        protected override void OnEndClose()
        {
            if (needAd)
            {
                StopCoroutine(nothanksDelay);
                StopCoroutine(closeDelay);
            }
        }
        [Space(15)]
        public Text tipText;
        public Text openText;
        public Text nothanksText;
        public override void SetContent()
        {
            tipText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Mergeball_GiftTip);
            openText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.OPEN);
            nothanksText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Nothanks);
        }
    }
}
