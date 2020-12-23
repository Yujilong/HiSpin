using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class FriendEvent : BaseUI
    {
        public RectTransform topRect;
        public Button backButton;
        public Button helpButton;
        public Button inviteButton;
        public Button copyButton;
        public Button expect_helpButton;
        public EventFriendItem single_friend_item;
        private List<EventFriendItem> all_friend_items = new List<EventFriendItem>();
        public GameObject nofriendGo;
        public Button withdrawButton;
        protected override void Awake()
        {
            base.Awake();
            all_friend_items.Add(single_friend_item);
            backButton.AddClickEvent(OnBackClick);
            helpButton.AddClickEvent(OnHelpClick);
            inviteButton.AddClickEvent(OnInviteButtonClick);
            copyButton.AddClickEvent(OnCopyClick);
            expect_helpButton.AddClickEvent(OnExpectHelpClick);
            withdrawButton.AddClickEvent(OnWithDrawClick);
            if (Master.IsBigScreen)
            {
                topRect.sizeDelta += new Vector2(0, Master.TopMoveDownOffset);
            }
        }
        private void OnBackClick()
        {
            UI.CloseCurrentBasePanel();
        }
        private void OnHelpClick()
        {
            UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.FriendEvent);
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
            Master.Instance.SendAdjustClickInviteButtonEvent(true);
#if UNITY_ANDROID
            _AJ.CallStatic("ShareString", Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Invite_word) + " http://aff.luckyclub.vip:8000/Hispin/" + Save.data.allData.user_panel.user_id);
            return;
#endif
            GJCNativeShare.Instance.NativeShare(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Invite_word) + " http://aff.luckyclub.vip:8000/Hispin/" + Save.data.allData.user_panel.user_id);

        }
        private void OnCopyClick()
        {
            ClipboardHelper.Copy(Save.data.allData.user_panel.invita_code);
        }
        private void OnExpectHelpClick()
        {
            UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.Expect);
        }
        private void OnWithDrawClick()
        {
            if (canCashout)
                UI.ShowBasePanel(BasePanel.Cashout, 1);
        }
        bool canCashout = false;
        protected override void BeforeShowAnimation(params int[] args)
        {
            invite_codeText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_MyInviteCode) + Save.data.allData.user_panel.invita_code;
            canCashout = Save.data.allData.user_panel.seven_doller >= Cashout.FriendEventCashoutMinCash * 100;
            withdrawButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.FriendEvent, canCashout ? "buttonBg_on" : "buttonBg_off");
            RefreshAllFriends();
        }
        protected override void AfterShowAnimation(params int[] args)
        {
            Tools.SetTwoUICenterInParent(invite_codeText.GetComponent<RectTransform>(), copyButton.GetComponent<RectTransform>());
            Tools.SetTwoUICenterInParent(expectText.GetComponent<RectTransform>(), expect_helpButton.GetComponent<RectTransform>());
        }
        private void RefreshAllFriends()
        {
            foreach (var friend in all_friend_items)
                friend.gameObject.SetActive(false);
            List<AllData_FriendEventData> _FriendEventDatas = Save.data.allData.seven_list;
            int friendCount = _FriendEventDatas.Count;
            for(int i = 0; i < friendCount; i++)
            {
                if (i > all_friend_items.Count - 1)
                {
                    EventFriendItem newFriend = Instantiate(single_friend_item.gameObject, single_friend_item.transform.parent).GetComponent<EventFriendItem>();
                    all_friend_items.Add(newFriend);
                }
                all_friend_items[i].gameObject.SetActive(true);
                AllData_FriendEventData friendEventData = _FriendEventDatas[i];
                all_friend_items[i].Init(friendEventData.user_title_id, friendEventData.username, friendEventData.current, friendEventData.expect);
            }
            nofriendGo.SetActive(friendCount == 0);
        }
        [Space(15)]
        public Text invitationText;
        public Text invite_bannerText;
        public Text invite_per_rewardText;
        public Text next_rewardText;
        public Text invite_and_earnText;
        public Text invite_codeText;
        public Text copyText;
        public Text nameText;
        public Text currentText;
        public Text expectText;
        public Text nofriendText;
        public Text referring_bonusText;
        public Text withdrawText;
        public override void SetContent()
        {
            invitationText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_Invitation);
            invite_bannerText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Friend_InviteBanner), 80, 1500);
            invite_per_rewardText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_InvitationReward);
            next_rewardText.text = "+" + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + "0.50";
            invite_and_earnText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_InviteAndEarn);
            copyText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_Copy);
            nameText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_Name);
            currentText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_Current);
            expectText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_Expect);
            nofriendText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_NoRecords);
            referring_bonusText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.FriendEvent_ReferringBonus) + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + Save.data.allData.user_panel.seven_doller.GetCashShowString();
            withdrawText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Withdraw);
        }
    }
}
