using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
namespace HiSpin
{
    public class GetReward : PopUI
    {
        public Image reward_iconImage;
        public Text reward_numText;
        public Text ticket_multipleText;
        public GameObject ticket_multipleGo;
        public Button double_getButton;
        public Button nothanksButton;
        public GameObject adiconGo;
        protected override void Awake()
        {
            base.Awake();
            double_getButton.AddClickEvent(OnDoublegetButtonClick);
            nothanksButton.AddClickEvent(OnNothanksButtonClick);
        }
        int clickAdTime = 0;
        private void OnDoublegetButtonClick()
        {
            clickAdTime++;
            Ads._instance.ShowRewardVideo(OnAdCallback, clickAdTime, "多倍奖励" + reward_type, OnNothanksButtonClick);
        }
        private void OnAdCallback()
        {
            Get(true);
        }
        private void OnNothanksButtonClick()
        {
            Ads._instance.ShowInterstialAd(() => { Get(false); }, "放弃多倍" + reward_type);
        }
        private void Get(bool multiple)
        {
            switch (reward_area)
            {
                case GetRewardArea.PlaySlots:
                    //Server_New.Instance.ConnectToServer_GetSlotsReward(OnRequestCallback, null, null, true, reward_type, multiple ? reward_num * reward_mutiple : reward_num);
                    break;
                case GetRewardArea.LevelUp:
                    sendMergeNum = GameManager.Instance.PlayerDataManager.playerData.unSendMergeNum;
                    Server_New.Instance.ConnectToServer_GetLevelupReward(OnRequestCallback, null, null, true, multiple ? reward_mutiple : 1, sendMergeNum);
                    break;
                default:
                    Debug.LogError("奖励获得区域错误");
                    break;
            }
        }
        int sendMergeNum = 0;
        private void OnRequestCallback()
        {
            GameManager.Instance.PlayerDataManager.playerData.unSendMergeNum -= sendMergeNum;
            TaskAgent.TriggerTaskEvent(PlayerTaskTarget.MergeballOnce, sendMergeNum);
            UI.FlyReward(reward_type, reward_num, double_getButton.transform.position);
            if (reward_type == Reward.Gold)
            {
                TaskAgent.TriggerTaskEvent(PlayerTaskTarget.OwnSomeGold, reward_num);
            }
            else if (reward_type == Reward.Ticket)
            {
                if (reward_area == GetRewardArea.PlaySlots)
                    TaskAgent.TriggerTaskEvent(PlayerTaskTarget.GetTicketFromSlotsOnce, 1);
            }
            UI.ClosePopPanel(this);
        }
        Reward reward_type = Reward.Null;
        GetRewardArea reward_area = GetRewardArea.Null;
        int reward_num = 0;
        int reward_mutiple = 1;
        Coroutine raiseAniamtion = null;
        protected override void BeforeShowAnimation(params int[] args)
        {
            clickAdTime = 0;
            reward_type = (Reward)args[0];
            reward_num = args[1];
            reward_area = (GetRewardArea)args[2];
            reward_numText.text = reward_num.ToString();
            reward_mutiple = 1;
            ticket_multipleGo.SetActive(false);
            nothanksButton.gameObject.SetActive(false);
            switch (reward_area)
            {
                case GetRewardArea.PlaySlots:
                    titleText.text = "";
                    switch (reward_type)
                    {
                        case Reward.Gold:
                            reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetReward, "gold");
                            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GetReward_GetgoldTip);
                            double_getText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GET) + "   x2";
                            reward_mutiple = 2;
                            break;
                        case Reward.Ticket:
                            reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetReward, "ticket");
                            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GetReward_GetticketsTip);
                            double_getText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GET) + "   x3";
                            reward_mutiple = 3;
                            int ticket_multiple = Save.data.allData.user_panel.user_double;
                            ticket_multipleText.text = "x " + ticket_multiple.GetTicketMultipleString();
                            break;
                        default:
                            Debug.LogError("奖励类型错误");
                            break;
                    }
                    break;
                case GetRewardArea.LevelUp:
                    tipText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GetReward_LevelupTip), args[3]);
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Congratulation);
                    reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetReward, reward_type.ToString().ToLower());
                    double_getText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GET) + "   x3";
                    reward_mutiple = 3;
                    break;
            }
            double_getText.GetComponent<RectTransform>().sizeDelta = new Vector2(524, 107);
#if UNITY_IOS
        if (!Save.data.isPackB)
        {
            double_getText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GET) ;
            adiconGo.SetActive(false);
            double_getText.GetComponent<RectTransform>().sizeDelta = new Vector2(666, 107);
        }
#endif
        }
        protected override void AfterShowAnimation(params int[] args)
        {
            if (reward_area == GetRewardArea.PlaySlots && reward_type == Reward.Ticket)
            {
                int ticket_multiple = Save.data.allData.user_panel.user_double;
                int correntNum = Mathf.CeilToInt(reward_num * ticket_multiple * 0.1f);
                raiseAniamtion = StartCoroutine(NumRaiseAnimation(reward_num, correntNum, ticket_multiple));
                reward_num = correntNum;
            }
            Master.Instance.ShowEffect(reward_type);
#if UNITY_IOS
        if(Save.data.isPackB)
#endif
            StartCoroutine("DelayShowNothanks");
        }
        IEnumerator NumRaiseAnimation(int startNum, int endNum, int multiple)
        {
            AnimationCurve scaleCurve = Master.Instance.popAnimationScale;
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
                reward_numText.text = num.ToString();
                //ticket_multipleText.text = "x " + currentmultiple.GetTicketMultipleString();
            }
        }
        protected override void BeforeCloseAnimation()
        {
            StopCoroutine("DelayShowNothanks");
            if (raiseAniamtion != null)
                StopCoroutine(raiseAniamtion);
            GameManager.Instance.nextSlotsIsUpgradeSlots = true;
            if (!GameManager.Instance.UIManager.PanelWhetherShowAnyone() && GameManager.Instance.WillShowSlots <= 0)
                GameManager.Instance.UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.SlotsPanel);
            else
                GameManager.Instance.WillShowSlots++;
            GameManager.Instance.SpawnAGiftBall();
        }
        IEnumerator DelayShowNothanks()
        {
            yield return new WaitForSeconds(1);
            nothanksButton.gameObject.SetActive(true);
        }
        [Space(15)]
        public Text titleText;
        public Text tipText;
        public Text double_getText;
        public Text nothanksText;
        public override void SetContent()
        {

            nothanksText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Nothanks);
        }
    }
    public enum GetRewardArea
    {
        Null,
        PlaySlots,
        LevelUp,
    }
}
