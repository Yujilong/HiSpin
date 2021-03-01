using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_GfitPanel : UI_PopPanelBase
    {
        public Image paypalImage;
        public Button openButton;
        public GameObject redeemTip;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.GiftPanel;
            openButton.onClick.AddListener(OnOpenClick);
            if (HiSpin.Language_M.isJapanese)
                paypalImage.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Gift, "paypay");
            else if(HiSpin.Language_M.isKorean)
                paypalImage.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Gift, "naverpay");
            else
                paypalImage.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Gift, "paypal");
            if (!GameManager.Instance.GetIsPackB())
                paypalImage.gameObject.SetActive(false);
        }
        int clickAdTime = 0;
        private void OnOpenClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
            {
                clickAdTime++;
                GameManager.PlayIV("打开礼盒", OnOpenAdCallback);
            }
            else
            {
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
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            needAd = GameManager.GetHasGetFreeGift();
#if UNITY_IOS
            if (!GameManager.GetIsPackB())
                needAd = false;
#endif
            redeemTip.SetActive(GameManager.GetIsPackB());
        }
        protected override void OnEndClose()
        {
        }
        [Space(15)]
        public Text tipText;
        public Text openText;
        public override void SetContent()
        {
            tipText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Mergeball_GiftTip);
            openText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.OPEN);
        }
    }
}
