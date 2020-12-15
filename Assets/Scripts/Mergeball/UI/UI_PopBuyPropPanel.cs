using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_PopBuyPropPanel : UI_PopPanelBase
    {
        public Button closeButton;
        public Button coinBuyButton;
        public Button adBuyButton;
        public Image icon;
        public Text needCoinNumText;
        Sprite prop1icon;
        Sprite prop2icon;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.BuyPropPanel;
            closeButton.onClick.AddListener(OnCloseClick);
            coinBuyButton.onClick.AddListener(OnCoinBuyClick);
            adBuyButton.onClick.AddListener(OnAdBuyClick);
            prop1icon = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.BuyProp, "prop1");
            prop2icon = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.BuyProp, "prop2");
        }
        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            GameManager.PlayIV("放弃购买道具" + (isProp1 ? "1" : "2"));
            UIManager.ClosePopPanel(this);
        }
        private void OnCoinBuyClick()
        {
            GameManager.PlayButtonClickSound();
            HiSpin.Server_New.Instance.ConnectToServer_BuyMergeball(OnCoinBuyCallback, null, null, true, 2500, HiSpin.Reward.Cash);
        }
        private void OnCoinBuyCallback()
        {
            if (isProp1)
            {
                GameManager.AddProp1Num(1);
                UIManager.FlyReward(Reward.Prop1, 1, coinBuyButton.transform.position);
                GameManager.IncreaseByProp1NeedCoin();
                GameManager.SendAdjustPropChangeEvent(1, 1);
            }
            else
            {
                GameManager.AddProp2Num(1);
                UIManager.FlyReward(Reward.Prop2, 1, coinBuyButton.transform.position);
                GameManager.IncreaseByProp2NeedCoin();
                GameManager.SendAdjustPropChangeEvent(2, 1);
            }
            UIManager.ClosePopPanel(this);
        }
        int clickAdTime = 0;
        private void OnAdBuyClick()
        {
            GameManager.PlayButtonClickSound();
            clickAdTime++;
            StopCoroutine("DelayShowBuyByCoin");
            GameManager.PlayRV(OnAdBuyCallback, clickAdTime, isProp1 ? "获得道具1" : "获得道具2",OnEndShow);
        }
        private void OnAdBuyCallback()
        {
            if (isProp1)
            {
                GameManager.AddProp1Num(1);
                GameManager.SendAdjustPropChangeEvent(1, 2);
                UIManager.FlyReward(Reward.Prop1, 1, coinBuyButton.transform.position);
            }
            else
            {
                GameManager.AddProp2Num(1);
                GameManager.SendAdjustPropChangeEvent(2, 2);
                UIManager.FlyReward(Reward.Prop2, 1, coinBuyButton.transform.position);
            }
            UIManager.ClosePopPanel(this);
        }
        bool isProp1 = false;
        int needCoinNum = 0;
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            isProp1 = GameManager.WillBuyProp == Reward.Prop1;
            needCoinNum = isProp1 ? GameManager.GetProp1NeedCoinNum() : GameManager.GetProp2NeedCoinNum();
            needCoinNumText.text = GameManager.GetIsPackB() ? "1.00" : HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + "1.00";
            icon.sprite = isProp1 ? prop1icon : prop2icon;
        }
        protected override void OnEndClose()
        {
            GameManager.ShowNextPanel();
        }
        [Space(15)]
        public Text buyText;
        public Text freeText;
        public override void SetContent()
        {
            buyText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.BUY);
            freeText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FREE);
        }
    }
}
