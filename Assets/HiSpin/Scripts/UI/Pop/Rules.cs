using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Rules : PopUI
    {
        public Text titleText;
        [Space(15)]
        public Text oneRule;
        public Image one_rule_pointImage;
        public ContentSizeFitter all_rules_content;
        public ScrollRect all_rulesScorllrect;
        private List<Text> all_rules = new List<Text>();
        [Space(15)]
        public Button closeButton;
        public Button sureButton;
        public Button termsButton;
        public Button cashoutButton;
        protected override void Awake()
        {
            base.Awake();
            closeButton.AddClickEvent(ClosePop);
            sureButton.AddClickEvent(ClosePop);
            termsButton.AddClickEvent(OnTermsClick);
            cashoutButton.AddClickEvent(OnCashoutButtonClick);
            all_rules.Add(oneRule);
            if (Language_M.isJapanese)
                one_rule_pointImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetReward, "point_japanese");
        }
        private void OnCashoutButtonClick()
        {
            UI.ClosePopPanel(this);
            UI.ShowBasePanel(BasePanel.Cashout_Cash);
        }
        private void OnTermsClick()
        {
            Application.OpenURL("http://luckyclub.vip/hispin-termofuse");
        }
        private void ClosePop()
        {
            UI.ClosePopPanel(this);
        }
        private RuleArea ruleArea;
        protected override void BeforeShowAnimation(params int[] args)
        {
            List<string> allStr = null;
            List<string> centerAlignStr = null;
            ruleArea = (RuleArea)args[0];
            bool isCenter = false;
            sureButton.GetComponentInChildren<Text>().text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_LetusPlay);
            switch (ruleArea)
            {
                case RuleArea.Betting:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_BettingFront));
                    allStr[1] = string.Format(allStr[1], Save.data.allData.award_ranking.ticktes_flag);
                    List<string> behind = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_BettingBehind));
                    centerAlignStr = Tools.GetTextMiddleCenterContent(oneRule, behind);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.PlaySlots:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_PlaySlotsTitle);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_PlaySlots));
                    termsButton.GetComponent<Text>().text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_PlaySlotsBottom);
                    termsButton.gameObject.SetActive(true);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    break;
                case RuleArea.InviteFriend:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_InviteFrineds));
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.MyInfo:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Me));
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.Cashout:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_CashoutTitle);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Cashout));
                    cashoutButton.gameObject.SetActive(true);
                    sureButton.gameObject.SetActive(false);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.Offerwall:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Offerwall));
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.Sign:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Sign));
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.FriendEvent:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Rules);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_FriendEvent));
                    sureButton.GetComponentInChildren<Text>().text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InviteFriends);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.Expect:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_RewardsDescription);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_FriendEventExpect));
                    List<string> orderList = new List<string>();
                    List<int> indexList = new List<int>();
                    int allCount = allStr.Count;
                    for(int i = 0; i < allCount; i++)
                    {
                        if (!string.IsNullOrEmpty(allStr[i]))
                        {
                            if(int.TryParse(allStr[i][0].ToString(),out int num))
                            {
                                orderList.Add(allStr[i].Substring(2));
                                int index = i;
                                indexList.Add(index);
                            }
                        }
                    }
                    orderList = Tools.TextToSameLengthByFillSpaceAsBehind(oneRule, orderList);
                    int orderCount = orderList.Count;
                    for(int i = 0; i < orderCount; i++)
                    {
                        allStr[indexList[i]] = (i + 1) + "." + orderList[i];
                    }
                    sureButton.GetComponentInChildren<Text>().text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InviteFriends);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    isCenter = true;
                    break;
            }
            foreach (var text in all_rules)
                text.gameObject.SetActive(false);
            int sectionCount = allStr.Count;
            float pontEndX = 0;
            for (int i = 0; i < sectionCount; i++)
            {
                if (i > all_rules.Count - 1)
                {
                    Text newRule = Instantiate(oneRule.gameObject, oneRule.transform.parent).GetComponent<Text>();
                    all_rules.Add(newRule);
                }
                Text ruleText = all_rules[i];
                if (!isCenter)
                    ruleText.alignment = TextAnchor.UpperLeft;
                else
                    ruleText.alignment = TextAnchor.UpperCenter;
                ruleText.gameObject.SetActive(true);
                ruleText.text = allStr[i];
                Transform frontpointTrans = ruleText.transform.GetChild(0);
                if (!string.IsNullOrEmpty(allStr[i]))
                {
                    if (int.TryParse(allStr[i][0].ToString(), out int num))
                    {
                        frontpointTrans.gameObject.SetActive(true);
                        ruleText.text = allStr[i].Substring(2);
                        if (isCenter)
                            pontEndX = Mathf.Min(pontEndX, -ruleText.preferredWidth / 2f - 30);
                        else
                            pontEndX = Mathf.Min(pontEndX, -450);
                    }
                    else
                        frontpointTrans.gameObject.SetActive(false);
                }
                else
                    frontpointTrans.gameObject.SetActive(false);
            }
            for(int i = 0; i < sectionCount; i++)
            {
                Transform frontpointTrans = all_rules[i].transform.GetChild(0);
                frontpointTrans.localPosition = new Vector3(pontEndX, frontpointTrans.localPosition.y);
            }
            if (centerAlignStr != null && centerAlignStr.Count > 0)
            {
                int centerStrCount = centerAlignStr.Count;
                for (int i = 0; i < centerStrCount; i++)
                {
                    if (i + sectionCount > all_rules.Count - 1)
                    {
                        Text newRule = Instantiate(oneRule.gameObject, oneRule.transform.parent).GetComponent<Text>();
                        all_rules.Add(newRule);
                    }
                    all_rules[i + sectionCount].alignment = TextAnchor.UpperCenter;
                    all_rules[i + sectionCount].gameObject.SetActive(true);
                    all_rules[i + sectionCount].text = centerAlignStr[i];
                    all_rules[i + sectionCount].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            StartCoroutine("DelayRefreshLayout");
        }
        private IEnumerator DelayRefreshLayout()
        {
            all_rules_content.enabled = false;
            yield return new WaitForEndOfFrame();
            all_rules_content.enabled = true;
        }
        protected override void AfterShowAnimation(params int[] args)
        {
            all_rulesScorllrect.normalizedPosition = Vector2.one;
        }
        public override void SetContent()
        {
            cashoutButton.GetComponentInChildren<Text>().text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);
        }
    }
    public enum RuleArea
    {
        Betting,
        PlaySlots,
        InviteFriend,
        MyInfo,
        Cashout,
        Offerwall,
        Sign,
        FriendEvent,
        Expect,
    }
}
