using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace HiSpin
{
    public class Master : MonoBehaviour
    {
        public static bool IsBigScreen = false;
        public static float ExpandCoe = 1;
        public const float TopMoveDownOffset = 100;
        public const string PackageName = "com.MergeBall.LuckyGame.HugePrizes.Rewards";
        public const int BundleID = 6;
        public const string AppleId = "1545540507";
        public static bool isLoadingEnd = false;
        public static bool isPackB_Advance = false;
        public static Master Instance;
        public Image bgImage;
        public Transform BaseRoot;
        public Transform MenuRoot;
        public Transform PopRoot;
        public GameObject AudioRoot;
        public AnimationCurve popAnimationScale;
        public AnimationCurve popAnimationAlpha;
        [HideInInspector]
        public UI UI;
        [HideInInspector]
        public Save Save;
        [HideInInspector]
        public Audio Audio;
        [HideInInspector]
        public Language_M Language_M;
        private void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
            UI = new UI(this, BaseRoot, MenuRoot, PopRoot);
            Save = new Save();
            Audio = new Audio(AudioRoot);
            Language_M = new Language_M();
            float coe = (float)Screen.height / Screen.width;
            float originCoe = 16f / 9;
            ExpandCoe = coe > originCoe ? coe / originCoe : originCoe / coe;
            IsBigScreen = ExpandCoe != 1;
            bgImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1080 * ExpandCoe, 1920 * ExpandCoe);
        }
        private void Start()
        {
            UI.ShowPopPanel(PopPanel.Loading);
        }
        public static void PlayButtonClickSound()
        {
            Audio.PlayOneShot(AudioPlayArea.Button);
        }
        public void OnLoadingEnd()
        {
            isLoadingEnd = true;
            CheckLocalSavaData();
            StartTimeDown();
            if (!Save.data.isPackB)
            {
                Save.data.isPackB = Save.data.allData.fission_info.up_user;
                if (!Save.data.isPackB)
                    Save.data.isPackB = isPackB_Advance;
                if (!Save.data.isPackB)
                    Save.data.isPackB = GameManager.Instance.PlayerDataManager.playerData.isPackB;
                if (Save.data.isPackB)
                    SendAdjustPackBEvent();
            }
#if UNITY_EDITOR
            Save.data.isPackB = true;
#endif
            UI.ShowMenuPanel();
            GameManager.Instance.WhenLoadingGameEnd();
        }
        public void StartTimeDown()
        {
            StopCoroutine("AutoTimedown");
            StartCoroutine("AutoTimedown", Save.data.allData.get_time.server_time + 5);
        }
        private void CheckLocalSavaData()
        {
            List<bool> head_new = Save.data.head_icon_hasCheck;
            if (head_new == null || head_new.Count != Save.data.allData.user_panel.title_list.Count)
            {
                List<int> headList = Save.data.allData.user_panel.title_list;
                int count = headList.Count;
                int userHeadId = Save.data.allData.user_panel.user_title;
                head_new = new List<bool>();
                for (int i = 0; i < count; i++)
                {
                    head_new.Add(headList[i] == userHeadId);
                }
                if (head_new.Count > 1)
                    head_new[0] = true;
            }
            Save.data.head_icon_hasCheck = head_new;
        }
        public void OnChangePackb()
        {
            Mergeball mergeball = UI.GetUI(BasePanel.MergeBall) as Mergeball;


            Friends friends = UI.GetUI(BasePanel.Friend) as Friends;
            if (friends != null)
                friends.OnChangePackB();
            Setting setting = UI.GetUI(PopPanel.Setting) as Setting;
            if (setting != null)
                setting.OnChangePackB();
        }
        public void ChangeBg(int index)
        {
            bgImage.sprite = Sprites.GetBGSprite("bg_" + index);
        }
        public void SetBgState(bool show)
        {
            bgImage.gameObject.SetActive(show);
        }
        public static string time;
        private IEnumerator AutoTimedown(int leftSeconds)
        {
            WaitForSeconds oneSecond = new WaitForSeconds(1);
            while (true)
            {
                int second = leftSeconds % 60;
                int minute = leftSeconds % 3600 / 60;
                int hour = leftSeconds / 3600;
                time = (hour < 10 ? "0" + hour : hour.ToString()) + ":" + (minute < 10 ? "0" + minute : minute.ToString()) + ":" + (second < 10 ? "0" + second : second.ToString());
                Betting betting = UI.GetUI(BasePanel.Betting) as Betting;
                if (betting != null)
                    betting.UpdateTimeDownText(time);
                yield return oneSecond;
                leftSeconds--;
                if (leftSeconds == 0)
                {
                    RequestAllData();
                    yield break;
                }
            }
        }
        public void RequestAllData()
        {
            //Server.Instance.RequestData(Server.Server_RequestType.AllData, OnRequestAllDataCallback, null);
            Server_New.Instance.ConnectToServer_GetAllData(OnRequestAllDataCallback, null, null, true);
        }
        private void OnRequestAllDataCallback()
        {
            StartTimeDown();
            UI.MenuPanel.RefreshTokenText();
            //if (UI.CurrentBasePanel == UI.GetUI(BasePanel.MergeBall))
            //{
            //    Mergeball slots = UI.GetUI(BasePanel.MergeBall) as Slots;
            //    slots.RefreshSlotsCardState();
            //}
            if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Task))
            {
                Tasks tasks = UI.GetUI(BasePanel.Task) as Tasks;
                tasks.RefreshTaskInfo();
            }
            if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Betting))
            {
                Betting betting = UI.GetUI(BasePanel.Betting) as Betting;
                betting.RefreshBettingWinner();
            }
            if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Rank))
            {
                Rank rank = UI.GetUI(BasePanel.Rank) as Rank;
                rank.RefreshRankList();
            }
            if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Friend))
            {
                Friends friends = UI.GetUI(BasePanel.Friend) as Friends;
                friends.RefreshFriendList();
            }
            if (UI.CurrentBasePanel == UI.GetUI(BasePanel.CashoutRecord))
            {
                CashoutRecord cashoutRecord = UI.GetUI(BasePanel.CashoutRecord) as CashoutRecord;
                cashoutRecord.InitRecord();
            }
        }
        public void SetBgDefault()
        {
            bgImage.sprite = Sprites.GetBGSprite("bg");
        }
        public void AddLocalExp()
        {
            Save.data.allData.user_panel.user_exp += 2;
            UI.MenuPanel.UpdateHeadIcon();
            if (Save.data.allData.user_panel.user_exp >= Save.data.allData.user_panel.level_exp)
            {
                Save.data.allData.user_panel.user_level++;
                Save.data.allData.user_panel.user_exp -= Save.data.allData.user_panel.level_exp;
                UI.ShowPopPanel(PopPanel.GetReward, (int)Save.data.allData.user_panel.level_type, Save.data.allData.user_panel.next_level, (int)GetRewardArea.LevelUp, Save.data.allData.user_panel.user_level);
                GameManager.Instance.OnLevelUpAndStopSendMergeNum();
                UI.MenuPanel.UpdateOfferwallRedpoint();
            }
        }
        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                Save.SaveLocalData();
            }
        }
        private void OnApplicationQuit()
        {
            Save.SaveLocalData();
        }
        public GameObject tipGo;
        public Text tipText;
        public void ShowTip(string content, float hideDelayTime = 1)
        {
            tipText.text = content;
            tipGo.SetActive(true);
            StopCoroutine("DelayHideTip");
            StartCoroutine("DelayHideTip", hideDelayTime);
        }
        private IEnumerator DelayHideTip(float time)
        {
            yield return new WaitForSeconds(time);
            tipGo.SetActive(false);
        }
        public void SendAdjustGameStartEvent()
        {
#if UNITY_EDITOR
            return;
#endif
            string player_id = "None";
            if (Save.data == null || Save.data.allData == null || Save.data.allData.user_panel == null || string.IsNullOrEmpty(Save.data.allData.user_panel.user_id))
                player_id = "None";
            else
                player_id = Save.data.allData.user_panel.user_id;
            string player_cash = "0";
            string player_gold = "0";
            if (Save.data == null || Save.data.allData == null || Save.data.allData.user_panel == null)
            {
                player_cash = "0";
                player_gold = "0";
            }
            else
            {
                player_cash = Save.data.allData.user_panel.user_doller_live.ToString();
                player_gold = Save.data.allData.user_panel.user_gold_live.ToString();
            }
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_open,
                ("player_id", player_id),
                ("install_version", BundleID.ToString()),
                ("other_int1",player_cash),
                ("other_int2", player_gold)
                );
        }
        public void SendAdjustPlayAdEvent(bool hasAd, bool isRewardAd, string adByWay)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(hasAd ? AdjustEventLogger.TOKEN_ad : AdjustEventLogger.TOKEN_noads,
                ("player_id", Save.data.allData.user_panel.user_id),
                //广告位置
                ("id", adByWay),
                //广告类型，0插屏1奖励视频
                ("type", isRewardAd ? "1" : "0"),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustPerTenBallEvent(int time,int currentMaxNum)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_stage_end,
                ("player_id", Save.data.allData.user_panel.user_id),
                //第几次10次掉落
                ("id", time.ToString()),
                //当前合成的最大数字
                ("reason", currentMaxNum.ToString()),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustGameOverEvent(int timeGame, int lv, bool passive, int stageMaxNum)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_stage_over,
                ("player_id", Save.data.allData.user_panel.user_id),
                ("id", timeGame.ToString()),
                ("next_stage_id", lv.ToString()),
                ("result", passive ? "0" : "1"),
                ("reason", stageMaxNum.ToString()),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustPropChangeEvent(int propID,int operation,int stageBallNum,string offset)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_item_change,
                ("player_id", Save.data.allData.user_panel.user_id),
                ("id", propID.ToString()),
                ("type", operation.ToString()),
                ("stage_id", stageBallNum.ToString()),
                ("value", offset),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustFinishTaskEvent(int taskId, int taskType, Reward rewardType, int rewardNum)
        {
#if UNITY_EDITOR
            return;
#endif
            int rewardTypeIndex = 0;
            switch (rewardType)
            {
                case Reward.Gold:
                    rewardTypeIndex = 0;
                    break;
                case Reward.Cash:
                    rewardTypeIndex = 1;
                    break;
                case Reward.Ticket:
                    rewardTypeIndex = 2;
                    break;
            }
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_task,
                ("player_id", Save.data.allData.user_panel.user_id),
                //任务id
                ("id", taskId.ToString()),
                //任务类型
                ("type", taskType.ToString()),
                //奖励数量
                ("value", rewardNum.ToString()),
                //奖励类型
                ("power_ok", rewardTypeIndex.ToString()),
                //当前票
                ("other_int1", Save.data.allData.user_panel.user_tickets.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustInputEmailEvent(string email)
        {
            Save.data.input_eamil_time++;
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_eamil,
                ("player_id", Save.data.allData.user_panel.user_id),
                //填写内容
                ("id", email),
                //第几次填写
                ("type", Save.data.input_eamil_time.ToString()),
                //当前票
                ("other_int1", Save.data.allData.user_panel.user_tickets.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustDeeplinkEvent(string uri)
        {
#if UNITY_EDITOR
            return;
#endif
            string player_id = "None";
            if (Save.data == null || Save.data.allData == null || Save.data.allData.user_panel == null || string.IsNullOrEmpty(Save.data.allData.user_panel.user_id))
                player_id = "None";
            else
                player_id = Save.data.allData.user_panel.user_id;
            string player_cash = "0";
            string player_gold = "0";
            if (Save.data == null || Save.data.allData == null || Save.data.allData.user_panel == null)
            {
                player_cash = "0";
                player_gold = "0";
            }
            else
            {
                player_cash = Save.data.allData.user_panel.user_doller_live.ToString();
                player_gold = Save.data.allData.user_panel.user_gold_live.ToString();
            }
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_deeplink,
                 ("link", uri),
                 ("order_id", uri),
                 ("player_id", player_id),
                 ("other_int1", player_cash),
                 ("other_int2", player_gold)
                );
        }
        public void SendAdjustPackBEvent()
        {
#if UNITY_EDITOR
            return;
#endif
            string player_id = Save.data.allData.user_panel.user_id;
            string player_cash = Save.data.allData.user_panel.user_doller_live.ToString();
            string player_gold = Save.data.allData.user_panel.user_gold_live.ToString();
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_packb,
                 ("player_id", player_id),
                 ("other_int1", player_cash),
                 ("other_int2", player_gold)
                );
        }
        public void SendAdjustSpawnGiftballEvent(int appearTime,int openTime)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_box,
                ("player_id", Save.data.allData.user_panel.user_id),
                ("id", appearTime.ToString()),
                ("time", openTime.ToString()),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustSpinWheelEvent(int spinTime)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_wheel,
                ("player_id", Save.data.allData.user_panel.user_id),
                ("id", spinTime.ToString()),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustSpinSlotsEvent(int spinTime)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_slots,
                ("player_id", Save.data.allData.user_panel.user_id),
                ("id", spinTime.ToString()),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustSpawnGoldBallEvent(int appearTime,int adTime)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_Gold_ball,
                ("player_id", Save.data.allData.user_panel.user_id),
                ("id", appearTime.ToString()),
                ("time", adTime.ToString()),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustGuideEvent(int step,bool isMergeball)
        {
#if UNITY_EDITOR
            return;
#endif
            string guide = isMergeball ? (step * 10).ToString() : step.ToString();
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_novice,
                ("player_id", Save.data.allData.user_panel.user_id),
                ("id", guide)
                );
        }
        public void SendAdjustSpawnTicketBallEvent(int appearTime)
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_Ticket_ball,
                ("player_id", Save.data.allData.user_panel.user_id),
                ("id", appearTime.ToString()),
                //当前美元
                ("other_int1", Save.data.allData.user_panel.user_doller_live.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustClickInviteButtonEvent()
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_invite_button,
                ("player_id", Save.data.allData.user_panel.user_id),
                //当前票
                ("other_int1", Save.data.allData.user_panel.user_tickets.ToString()),
                //当前金币
                ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
                );
        }
        public void SendAdjustEnterInvitePageEvent()
        {
            Save.data.hasSendToThoundsEvent = true;
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_invite_page,
                ("player_id", Save.data.allData.user_panel.user_id)
                );
        }
        public void SendAdjustTicketOver1000Event()
        {
#if UNITY_EDITOR
            return;
#endif
            AdjustEventLogger.Instance.AdjustEventNoParam(AdjustEventLogger.TOKEN_ticket_over1000);
        }
        public ParticleSystem left_particle;
        public ParticleSystem right_particle;
        public void ShowEffect(Reward reward, float time = 0.5f)
        {
            Color color;
            switch (reward)
            {
                case Reward.Gold:
                    color = new Color(0.99f, 1, 0.23f);
                    break;
                case Reward.Cash:
                    color = Color.green;
                    break;
                case Reward.Ticket:
                    color = new Color(1, 0.5f, 0);
                    break;
                default:
                    color = Color.clear;
                    break;
            }
            left_particle.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);
            left_particle.Play();
            right_particle.Play();
            StopCoroutine("DealyStopPartical");
            StartCoroutine("DealyStopPartical", time);
        }
        IEnumerator DealyStopPartical(float time)
        {
            yield return new WaitForSeconds(time);
            left_particle.Stop();
            right_particle.Stop();
        }
        public RectTransform guidemaskRectRoot;
        public GameObject guidemaskGo1;
        public GameObject guidemaskGo2;
        public GameObject guidemaskGo3;
        public Camera guideCamera;
        public GameObject guideGo;
        public void SetGuideMask(int guideStep)
        {
            guideGo.SetActive(true);
            switch (guideStep)
            {
                case 1:
                    guidemaskGo1.SetActive(true);
                    if (IsBigScreen)
                        guidemaskGo1.transform.localPosition -= new Vector3(0, TopMoveDownOffset, 0);
                    guidemaskGo2.SetActive(false);
                    guidemaskGo3.SetActive(false);
                    break;
                case 2:
                    guidemaskGo1.SetActive(false);
                    guidemaskGo2.SetActive(true);
                    guidemaskGo3.SetActive(false);
                    break;
                case 3:
                    guidemaskGo1.SetActive(false);
                    guidemaskGo2.SetActive(false);
                    guidemaskGo3.SetActive(true);
                    break;
                default:
                    guideGo.SetActive(false);
                    break;
            }
        }

        public static Coroutine ConnectServerToSendMergeNum(Action successCallback,int num)
        {
            return Server_New.Instance.ConnectToServer_SendMergeballNum_Mute(successCallback, null, null, num);
        }
    }
    public enum Reward
    {
        Null,
        Gold,
        Cash,
        Ticket,
        Paypal
    }
}
