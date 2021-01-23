using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Friends : BaseUI
    {
        public RectTransform topRect;
        [Space(15)]
        public Button backButton;
        public Button helpButton;
        public Button cashoutButton;
        [Space(15)]
        public Button myfriendsButton;
        public Image friend_headImage1;
        public Image friend_headImage2;
        public Image friend_headImage3;
        [Space(15)]
        public Button inviteButton;
        public Image invite_reward_iconImage;
        [Space(15)]
        public GameObject lastdayGo;
        public GameObject nofriend_tipGo;
        public FriendInviteRecordItem single_invite_record_item;
        private List<FriendInviteRecordItem> all_invite_friend_items = new List<FriendInviteRecordItem>();
        [Space(15)]
        public RectTransform viewport;
        protected override void Awake()
        {
            base.Awake();
            backButton.AddClickEvent(OnBackButtonClick);
            helpButton.AddClickEvent(OnHelpButtonClick);
            cashoutButton.AddClickEvent(OnCashoutButtonClick);
            inviteButton.AddClickEvent(OnInviteButtonClick);
            myfriendsButton.AddClickEvent(OnMyfriendsButtonClick);
            all_invite_friend_items.Add(single_invite_record_item);
            if (Master.IsBigScreen)
            {
                topRect.sizeDelta = new Vector2(topRect.sizeDelta.x, topRect.sizeDelta.y + Master.TopMoveDownOffset);
                viewport.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
            }
            cashoutButton.gameObject.SetActive(Save.data.isPackB);
#if UNITY_IOS
        helpButton.gameObject.SetActive(Save.data.isPackB);
#endif
        }
        #region ios share
        public void Init()
        {
            GJCNativeShare.Instance.onShareSuccess = OnShareSuccess;
            GJCNativeShare.Instance.onShareCancel = OnShareCancel;
        }
        void OnShareSuccess(string platform)
        {
            //...your code
        }
        void OnShareCancel(string platform)
        {
            //...your code
        }
        #endregion
        private void OnBackButtonClick()
        {
            UI.CloseCurrentBasePanel();
        }
        private void OnHelpButtonClick()
        {
            UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.InviteFriend);
        }
        private void OnCashoutButtonClick()
        {
            UI.ShowBasePanel(BasePanel.Cashout);
        }
        private AndroidJavaClass _aj;
        private AndroidJavaClass _AJ
        {
            get
            {
                if (_aj == null)
                    _aj = new AndroidJavaClass("com.wyx.shareandcopy.Share_Copy");
                return _aj;

            }
        }
        private void OnInviteButtonClick()
        {
#if UNITY_EDITOR
            return;
#endif
            Master.Instance.SendAdjustClickInviteButtonEvent();
#if UNITY_ANDROID
            _AJ.CallStatic("ShareString", Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Invite_word) + "\nhttp://aff.luckyclub.vip:8000/2048MergeBall/" + Save.data.allData.user_panel.user_id);
            return;
#endif
            GJCNativeShare.Instance.NativeShare(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Invite_word) + "\nhttp://aff.luckyclub.vip:8000/2048MergeBall/" + Save.data.allData.user_panel.user_id);

        }
        private void OnMyfriendsButtonClick()
        {
            UI.ShowBasePanel(BasePanel.FriendList);
        }
        protected override void BeforeShowAnimation(params int[] args)
        {
            RefreshFriendList();
        }
        public void RefreshFriendList()
        {
            string ptStr = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.PT);
            pt_numText.text = ((int)Save.data.allData.fission_info.live_balance).GetTokenShowString() + " <size=70>" + ptStr + "</size>";
            int invite_people_num = Save.data.allData.fission_info.user_invite_people;
            int invite_people_receive = Save.data.allData.fission_info.reward_conf.invite_receive;
            myfriends_numText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_Myfriends) + " <color=#0596E4>{0}</color>", invite_people_num.GetTokenShowString());
            yesterday_pt_numText.text = ((int)Save.data.allData.fission_info.up_user_info.yestday_team_all).GetTokenShowString() + " <size=55>" + ptStr + "</size>";
            total_pt_numText.text = ((int)Save.data.allData.fission_info.user_total).GetTokenShowString() + " <size=55>" + ptStr + "</size>";

            foreach (var friend in all_invite_friend_items)
                friend.gameObject.SetActive(false);

            List<AllData_FriendData_Friend> friend_Infos = Save.data.allData.fission_info.up_user_info.two_user_list;
            int count = friend_Infos.Count;
            List<AllData_FriendData_Friend> friend_Infos_order = new List<AllData_FriendData_Friend>();
            for (int i = 0; i < count; i++)
            {
                AllData_FriendData_Friend unorder_friend_info = friend_Infos[i];
                int orderCount = friend_Infos_order.Count;
                bool hasAdd = false;
                for (int j = 0; j < orderCount; j++)
                {
                    if (unorder_friend_info.yestday_doller < friend_Infos_order[j].yestday_doller)
                        continue;
                    else
                    {
                        friend_Infos_order.Insert(j, unorder_friend_info);
                        hasAdd = true;
                        break;
                    }
                }
                if (!hasAdd)
                    friend_Infos_order.Add(unorder_friend_info);
            }
            int hasPtFriendCount = 0;
            for (int i = 0; i < count; i++)
            {
                AllData_FriendData_Friend friendInfo = friend_Infos_order[i];
                if ((int)friendInfo.yestday_doller == 0)
                    continue;
                hasPtFriendCount++;
                if (i > all_invite_friend_items.Count - 1)
                {
                    FriendInviteRecordItem newRecordItem = Instantiate(single_invite_record_item, single_invite_record_item.transform.parent).GetComponent<FriendInviteRecordItem>();
                    all_invite_friend_items.Add(newRecordItem);
                }
                all_invite_friend_items[i].gameObject.SetActive(true);
                all_invite_friend_items[i].Init(friendInfo.user_img, friendInfo.user_name, (int)friendInfo.yestday_doller, friendInfo.distance);
            }
            bool noFriend = count == 0;
            bool noPtFriend = hasPtFriendCount == 0;
            lastdayGo.SetActive(!noPtFriend);
            nofriend_tipGo.SetActive(noFriend);
            myfriendsButton.gameObject.SetActive(!noFriend);
            if (count > 0)
                friend_headImage1.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + friend_Infos[0].user_img);
            if (count > 1)
                friend_headImage2.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + friend_Infos[1].user_img);
            if (count > 2)
                friend_headImage3.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + friend_Infos[2].user_img);

            int receiveTime = invite_people_receive + 1;
            if (receiveTime <= Save.data.allData.fission_info.reward_conf.invite_flag)
            {
                Reward lt = Save.data.allData.fission_info.reward_conf.lt_flag_type;
                int ltNum = Save.data.allData.fission_info.reward_conf.lt_flag_num;
                invite_reward_numText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_InviteRewardTip) + " <color=#FF9732>{0}</color>",
                    (lt == Reward.Cash || lt == Reward.Paypal) ? (Save.data.isPackB ? string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), ltNum.GetCashShowString()) : ltNum.GetCashShowString()) : ltNum.GetTokenShowString());
                invite_reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Friend, Save.data.allData.fission_info.reward_conf.lt_flag_type.ToString().ToLower());
            }
            else
            {
                Reward gt = Save.data.allData.fission_info.reward_conf.gt_flag_type;
                int gtNum = Save.data.allData.fission_info.reward_conf.gt_flag_num;
                invite_reward_numText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_InviteRewardTip) + " <color=#FF9732>{0}</color>",
                    (gt == Reward.Cash || gt == Reward.Paypal) ? (Save.data.isPackB ? string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), gtNum.GetCashShowString()) : gtNum.GetCashShowString()) : gtNum.GetTokenShowString());
                invite_reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Friend, Save.data.allData.fission_info.reward_conf.gt_flag_type.ToString().ToLower());
            }
            if (!Save.data.isPackB && Save.data.allData.fission_info.reward_conf.lt_flag_type == Reward.Paypal)
                invite_reward_iconImage.gameObject.SetActive(false);
            else
                invite_reward_iconImage.gameObject.SetActive(true);

            int not_received_invite_reward = invite_people_num - invite_people_receive;
            for (int i = 0; i < not_received_invite_reward; i++)
            {
                int receiveTimes = invite_people_receive + i + 1;
                if (receiveTimes <= Save.data.allData.fission_info.reward_conf.invite_flag)
                    UI.ShowPopPanel(PopPanel.InviteOk, (int)Save.data.allData.fission_info.reward_conf.lt_flag_type, Save.data.allData.fission_info.reward_conf.lt_flag_num);
                else
                    UI.ShowPopPanel(PopPanel.InviteOk, (int)Save.data.allData.fission_info.reward_conf.gt_flag_type, Save.data.allData.fission_info.reward_conf.gt_flag_num);
            }
        }
        public void OnChangePackB()
        {
            cashoutButton.gameObject.SetActive(true);
        }
        [Space(15)]
        public Text helpText;
        public Text balanceText;
        public Text pt_numText;
        public Text cashoutText;
        public Text myfriends_numText;
        public Text invitefriendText;
        public Text invite_reward_numText;
        public Text myincomeText;
        public Text yesterday_pt_numText;
        public Text yesterdatyincomeText;
        public Text total_pt_numText;
        public Text totalincomText;
        public Text lastdaydetailsText;
        public Text nofriend_tipText;
        public override void SetContent()
        {
            helpText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
            balanceText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Balance);

            cashoutText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);

            invitefriendText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InviteFriends);

            myincomeText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_MyIncome);

            yesterdatyincomeText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_YesterdayIncome);

            totalincomText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_TotalIncome);
            lastdaydetailsText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_LastDayDetails);
            nofriend_tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_NofriendTip);
        }
    }
}
