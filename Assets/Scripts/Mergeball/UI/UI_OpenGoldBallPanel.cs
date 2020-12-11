using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_OpenGoldBallPanel : UI_PopPanelBase
    {
        public Text rewardNum;
        public Button adButton;
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
            num *= 10;
            GetReward();
            GameManager.AddGoldBallx10Time();
            UIManager.ClosePopPanel(this);
        }
        private void OnNothanksClick()
        {
            GameManager.PlayButtonClickSound();
            GameManager.PlayIV("放弃十倍金币球奖励", OnNothanksIVCallback);
        }
        private void OnNothanksIVCallback()
        {
            GetReward();
            UIManager.ClosePopPanel(this);
        }
        int num = 0;
        Coroutine nothanksDelay = null;
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            num = GameManager.OpenGoldBallReward_Num;
            rewardNum.text = "x" + num;
            nothanksDelay = StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(nothanksButton.gameObject));
        }
        private void GetReward()
        {
            GameManager.AddCoin(num);
            UIManager.FlyReward(Reward.Coin, num, transform.position);
        }
        protected override void OnEndClose()
        {
            StopCoroutine(nothanksDelay);
            GameManager.ShowNextPanel();
        }
    }
}
