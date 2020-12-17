using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_BuyEnergyPanel : UI_PopPanelBase
    {
        public Button closeButton;
        public Button adbuyButton;
        public GameObject ad_iconGo;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.BuyEnergyPanel;
            closeButton.onClick.AddListener(OnCloseClick);
            adbuyButton.onClick.AddListener(OnAdbuyClick);
        }
        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
        }
        int clickAdTime = 0;
        private void OnAdbuyClick()
        {
            GameManager.PlayButtonClickSound();
            if (!GameManager.CheckHasBuyEnergyTime())
            {
                TipsManager.Instance.ShowTips("Not enough times to buy energy.");
                return;
            }
            clickAdTime++;
            GameManager.PlayRV(OnAdbuyCallback, clickAdTime, "购买体力");
        }
        private void OnAdbuyCallback()
        {
            GameManager.AddEnergy(GameManager.addEnergyPerAd);
            GameManager.AddBuyEnergyTime();
            UIManager.FlyReward(Reward.Energy, GameManager.addEnergyPerAd, transform.position);
            UIManager.ClosePopPanel(this);
        }
        Coroutine closeDelay = null;
        protected override void OnStartShow()
        {
            closeDelay = StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(closeButton.gameObject));
#if UNITY_IOS
            if (!GameManager.GetIsPackB())
            {
                ad_iconGo.SetActive(false);
                freeText.transform.localPosition = new Vector3(0, freeText.transform.localPosition.y, 0);
            }
            else
            {
                ad_iconGo.SetActive(true);
                freeText.transform.localPosition = new Vector3(53.971f, freeText.transform.localPosition.y, 0);
            }
#endif
        }
        protected override void OnEndClose()
        {
            StopCoroutine(closeDelay);
            MainController.Instance.hasShowBuyEnergyPanel = false;
            GameManager.ShowNextPanel();
        }
        [Space(15)]
        public Text energyText;
        public Text freeText;
        public override void SetContent()
        {
            energyText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.ENERGY);
            freeText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FREE);
        }
    }
}
