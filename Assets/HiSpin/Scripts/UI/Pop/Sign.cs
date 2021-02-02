using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Sign : PopUI
    {
        public Button closeButton;
        public Button helpButton;
        public Image bonus_baseImage;
        public Text bonus_numText;
        public SignDayItem single_signdayItem;
        private List<SignDayItem> all_signdayItems = new List<SignDayItem>();
        public RectTransform all_signdayRect;
        public Image day_15Image;
        public Button sign_inButton;
        public GameObject sign_task_redpointGo;
        private int[] all_sign_reward_cash_pt = new int[14]
        {
            50000,
            25000,
            12500,
            12500,
            7500,
            5000,
            2500,
            2500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500
        };
        protected override void Awake()
        {
            base.Awake();
            all_signdayItems.Add(single_signdayItem);
            closeButton.AddClickEvent(OnCloseButtonClick);
            helpButton.AddClickEvent(OnHelpButtonClick);
            sign_inButton.AddClickEvent(OnSigninButtonClick);
            UI.MenuPanel.fly_target_dic.Add(Reward.SignCash, bonus_numText.transform);
            if (Language_M.isJapanese)
            {
                bonus_baseImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Sign, "bonus_base_japanese");
                day_15Image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Sign, "day15_japanese");
            }
        }
        private void OnCloseButtonClick()
        {
            UI.ClosePopPanel(this);
        }
        private void OnHelpButtonClick()
        {
            UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.Sign);
        }
        private void OnSigninButtonClick()
        {
            if (hasSignin)
                return;
            else
            {
                if (!taskComplete)
                {
                    UI.ClosePopPanel(this);
                    UI.ShowPopPanel(PopPanel.SignTasks);
                }
                else
                    Server.Instance.ConnectToServer_SignIn(OnSigninSuccessCallback, OnSignFailCallback, null, true);
            }
        }
        private void OnSigninSuccessCallback()
        {
            if (today < 14)
            {
                UI.FlyReward(Reward.SignCash, all_sign_reward_cash_pt[today], all_signdayItems[today].transform.position);
                RefreshDaily();
            }
            else
            {
                UI.ShowPopPanel(PopPanel.GetCash, (int)GetCashArea.Signin, Save.data.allData.check_task.check_coin);
                UI.ClosePopPanel(this);
            }
            Master.Instance.SendAdjustCheckinEvent(today + 1);
        }
        private void OnSignFailCallback()
        {
            Server.Instance.ConnectToServer_GetSignData(OnGetNewSignDataCallback, null, null, true);
        }
        private void OnGetNewSignDataCallback()
        {
            BeforeShowAnimation();
        }
        private bool CheckCanCashout_15thCash(int today)
        {
            bool can = true;
            List<int> dayStates = Save.data.allData.check_task.task_list;
            int dayCount = dayStates.Count;
            int min = Mathf.Min(dayCount, today);
            for(int i = 0; i < min; i++)
            {
                if (dayStates[i] == 0)
                {
                    can = false;
                    break;
                }
            }
            return can;
        }
        private bool taskComplete = false;
        private bool hasSignin = false;
        private int today = 0;
        protected override void BeforeShowAnimation(params int[] args)
        {
            UpdateBonusNumText();
            RefreshDaily();
            tipText.gameObject.SetActive(!CheckCanCashout_15thCash(today));
        }
        private void RefreshDaily()
        {
            foreach (var dayitem in all_signdayItems)
                dayitem.gameObject.SetActive(false);
            List<int> dayStates = Save.data.allData.check_task.task_list;
            int dayCount = dayStates.Count;
            today = Save.data.allData.check_task.cur_day;
            taskComplete = true;
            List<AllData_SignTaskData> _SignTaskDatas = Save.data.allData.check_task.tar_task;
            foreach(var task in _SignTaskDatas)
                if (task.cur_num < task.tar_num)
                {
                    taskComplete = false;
                    break;
                }

            for (int i = 0; i < dayCount; i++)
            {
                if (i > all_signdayItems.Count - 1)
                {
                    SignDayItem newDayItem = Instantiate(single_signdayItem.gameObject, single_signdayItem.transform.parent).GetComponent<SignDayItem>();
                    all_signdayItems.Add(newDayItem);
                }
                all_signdayItems[i].gameObject.SetActive(true);
                if (i == today)
                    hasSignin = dayStates[i] == 1;
                all_signdayItems[i].Init(i + 1, today + 1, Reward.Cash, dayStates[i] == 1);
            }
            int currentDay = Save.data.allData.check_task.cur_day;
            if (currentDay == 14)
            {
                checkinText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);
                sign_inButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Sign, "signin_on");
                sign_task_redpointGo.SetActive(false);
            }
            else
            {
                checkinText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Sign_Checkin);
                sign_inButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Sign, hasSignin ? "signin_off" : "signin_on");
                sign_task_redpointGo.SetActive(!hasSignin && taskComplete);
            }
        }
        public void UpdateBonusNumText()
        {
            bonus_numText.text = Save.data.allData.check_task.check_coin.ToString();
            StopCoroutine("DelaySetCenter");
            StartCoroutine("DelaySetCenter");
        }
        protected override void AfterShowAnimation(params int[] args)
        {
            if (today <= 1)
                all_signdayRect.GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;
            else if (today >= 12)
                all_signdayRect.GetComponent<ScrollRect>().normalizedPosition = Vector2.one;
            else
                all_signdayRect.localPosition = new Vector3(-all_signdayItems[today].transform.localPosition.x + 332, 0);
        }
        IEnumerator DelaySetCenter()
        {
            yield return null;
            RectTransform leftRect = extrabonusText.GetComponent<RectTransform>();
            RectTransform rightRect = bonus_numText.GetComponent<RectTransform>();
            Tools.SetTwoUICenterInParent(leftRect, rightRect);
            leftRect.localPosition -= new Vector3(62, 0);
            rightRect.localPosition -= new Vector3(62, 0);
        }
        protected override void AfterCloseAnimation()
        {
            if (!Save.data.hasWatchThreeCardGuide)
            {
                UI.ShowPopPanel(PopPanel.Guide, 1);
            }
        }
        [Space(15)]
        public Text card_cashnumText;
        public Text card_tip;
        public Text extrabonusText;
        public Text dailysigninText;
        public Text checkinText;
        public Text tipText;
        public Text day15Text;
        public override void SetContent()
        {
            card_cashnumText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), Language_M.isJapanese ? "5000" : "50");
            card_tip.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Guide4);
            day15Text.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Sign_Day), 15);
            extrabonusText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Sign_ExtraBonusTitle);
            dailysigninText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Sign_DailySignin);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Sign_Tip);
        }
    }
}
