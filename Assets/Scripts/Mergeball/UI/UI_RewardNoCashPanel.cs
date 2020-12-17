﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace UI
{
    public class UI_RewardNoCashPanel : UI_PopPanelBase
    {
        public Image rewardIcon;
        public Text rewardNum;
        public Button adButton;
        public Button nothanksButton;
        public GameObject adicon;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.RewardNoCashPanel;
            adButton.onClick.AddListener(OnAdClick);
            nothanksButton.onClick.AddListener(OnNothanksClick);
        }
        int clickAdTime = 0;
        private void OnAdClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
            {
                clickAdTime++;
                GameManager.PlayRV(OnAdGetDoubleCallback, clickAdTime, "获得双倍" + type, OnNothanksClick);
            }
            else
                OnAdGetDoubleCallback();
        }
        private void OnAdGetDoubleCallback()
        {
            num *= 2;
            GetReward();
        }
        private void OnNothanksClick()
        {
            GameManager.PlayButtonClickSound();
            GameManager.PlayIV("放弃双倍奖励" + type, OnNothanksIVCallback);
        }
        private void OnNothanksIVCallback()
        {
            GetReward();
        }
        Reward type = Reward.Null;
        int num = 0;
        bool needAd = true;
        Coroutine nothanksDelay = null;
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            type = GameManager.ConfirmReward_Type;
            num = GameManager.ConfirmRewrad_Num;
            needAd = GameManager.ConfirmReward_Needad;
#if UNITY_IOS
            if (!GameManager.GetIsPackB())
                needAd = false;
#endif
            rewardIcon.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.RewardNoCash, type.ToString());
            rewardNum.text = "x" + num;
            adicon.SetActive(needAd);
            getText.transform.localPosition = needAd ? new Vector3(60.948f, getText.transform.localPosition.y, 0) : new Vector3(0, getText.transform.localPosition.y, 0);
            nothanksButton.gameObject.SetActive(needAd);
            if(needAd)
                nothanksDelay= StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(nothanksButton.gameObject));
        }
        private void GetReward()
        {
            switch (type)
            {
                case Reward.Prop1:
                    GameManager.AddProp1Num(num);
                    HiSpin.Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetRewardCallback, null, null, true, HiSpin.Reward.Null, 0, GameManager.ConfirmReward_IsWheel);
                    break;
                case Reward.Prop2:
                    GameManager.AddProp2Num(num);
                    HiSpin.Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetRewardCallback, null, null, true, HiSpin.Reward.Null, 0, GameManager.ConfirmReward_IsWheel);
                    break;
                case Reward.Cash:
                    Debug.LogError("奖励类型错误，该面板不会奖励现金");
                    break;
                case Reward.Coin:
                    HiSpin.Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetRewardCallback, null, null, true, HiSpin.Reward.Gold, num, GameManager.ConfirmReward_IsWheel);
                    break;
                case Reward.Amazon:
                    GameManager.AddAmazon(num);
                    HiSpin.Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetRewardCallback, null, null, true, HiSpin.Reward.Null, 0, GameManager.ConfirmReward_IsWheel);
                    break;
                case Reward.WheelTicket:
                    GameManager.AddWheelTicket(num);
                    HiSpin.Server_New.Instance.ConnectToServer_GetMergeballReward(OnGetRewardCallback, null, null, true, HiSpin.Reward.Null, 0, GameManager.ConfirmReward_IsWheel);
                    break;
            }
        }
        private void OnGetRewardCallback()
        {
            UIManager.FlyReward(type, num, transform.position);
            UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
            if (_MenuPanel != null)
            {
                _MenuPanel.RefreshProp1();
                _MenuPanel.RefreshProp2();
            }
            UIManager.ClosePopPanel(this);
        }
        protected override void OnEndClose()
        {
            if (needAd)
                StopCoroutine(nothanksDelay);
            GameManager.ShowNextPanel();
        }
        [Space(15)]
        public Text titleText;
        public Text getText;
        public Text nothanksText;
        public override void SetContent()
        {
            titleText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Congratulation);
            getText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GET) + " x2";
            nothanksText.text = HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Nothanks);
        }
    }
}
