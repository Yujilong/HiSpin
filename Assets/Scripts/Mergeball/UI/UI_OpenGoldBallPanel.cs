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
        public Image ad_button_contentText;
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
            rewardNum.text = "x" + num;
            iconImage.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.RewardNoCash, num>100?"Coin":"Ticket");
            ad_button_contentText.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.RewardNoCash, num > 100 ? "GETx10" : "GETx2");
            nothanksDelay = StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(nothanksButton.gameObject));
        }
        int rewardnum = 0;
        private void GetReward(bool addmultiple)
        {
            HiSpin.Reward type;
            if (num > 100)
            {
                type = HiSpin.Reward.Gold;
                rewardnum = addmultiple ? num * 10 : num;
            }
            else
            {
                type = HiSpin.Reward.Ticket;
                rewardnum = addmultiple ? num * 2 : num;
            }
            HiSpin.Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetRewardCallback, null, null, true, type, rewardnum);
        }
        private void OnGetRewardCallback()
        {
            UIManager.FlyReward(num > 100 ? Reward.Coin : Reward.Ticket, num, transform.position);
            if (rewardnum > 1000)
                GameManager.AddGoldBallx10Time();
            UIManager.ClosePopPanel(this);
        }
        protected override void OnEndClose()
        {
            StopCoroutine(nothanksDelay);
            GameManager.ShowNextPanel();
        }
    }
}
