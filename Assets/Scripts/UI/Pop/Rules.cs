using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Rules : PopUI
    {
        public Text titleText;
        public Image iconImage;
        [Space(15)]
        public Text oneRule;
        public ContentSizeFitter all_rules_content;
        private List<Text> all_rules = new List<Text>();
        public GameObject itemofuse1;
        public GameObject itemofuse2;
        public ScrollRect scroll;
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
                iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetReward, "point_japan");
        }
        private void OnCashoutButtonClick()
        {
            UI.ClosePopPanel(this);
            UI.ShowBasePanel(BasePanel.Cashout);
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
            switch (ruleArea)
            {
                case RuleArea.Betting:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_BettingFront));
                    allStr[1] = string.Format(allStr[1], Save.data.allData.award_ranking.ticktes_flag);
                    List<string> behind = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_BettingBehind));
                    itemofuse1.SetActive(false);
                    itemofuse2.SetActive(false);
                    centerAlignStr = Tools.GetTextMiddleCenterContent(oneRule, behind);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.PlaySlots:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_PlaySlotsTitle);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_PlaySlots));
                    termsButton.GetComponent<Text>().text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_PlaySlotsBottom);
                    itemofuse1.SetActive(false);
                    itemofuse2.SetActive(false);
                    termsButton.gameObject.SetActive(true);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    break;
                case RuleArea.InviteFriend:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_InviteFrineds));
                    itemofuse1.SetActive(false);
                    itemofuse2.SetActive(false);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.MyInfo:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Me));
                    itemofuse1.SetActive(false);
                    itemofuse2.SetActive(false);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.Cashout:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_CashoutTitle);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Cashout));
                    itemofuse1.SetActive(false);
                    itemofuse2.SetActive(false);
                    cashoutButton.gameObject.SetActive(true);
                    sureButton.gameObject.SetActive(false);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.Offerwall:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                    allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Offerwall));
                    itemofuse1.SetActive(false);
                    itemofuse2.SetActive(false);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
                case RuleArea.ItemOfUse:
                    titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.ItemOfUse);
                    allStr = new List<string>();
                    itemofuse1.SetActive(true);
                    itemofuse2.SetActive(true);
                    cashoutButton.gameObject.SetActive(false);
                    sureButton.gameObject.SetActive(true);
                    termsButton.gameObject.SetActive(false);
                    break;
            }
            foreach (var text in all_rules)
                text.gameObject.SetActive(false);
            int sectionCount = allStr.Count;
            for (int i = 0; i < sectionCount; i++)
            {
                if (i > all_rules.Count - 1)
                {
                    Text newRule = Instantiate(oneRule.gameObject, oneRule.transform.parent).GetComponent<Text>();
                    all_rules.Add(newRule);
                }
                all_rules[i].alignment = TextAnchor.UpperLeft;
                all_rules[i].gameObject.SetActive(true);
                all_rules[i].text = allStr[i];
                if (!string.IsNullOrEmpty(allStr[i]))
                {
                    if (int.TryParse(allStr[i][0].ToString(), out int num))
                    {
                        all_rules[i].transform.GetChild(0).gameObject.SetActive(true);
                        all_rules[i].text = allStr[i].Substring(2);
                    }
                    else
                        all_rules[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                    all_rules[i].transform.GetChild(0).gameObject.SetActive(false);
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
            scroll.normalizedPosition = Vector2.one;
        }
        public override void SetContent()
        {
            cashoutButton.GetComponentInChildren<Text>().text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);
            sureButton.GetComponentInChildren<Text>().text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_LetusPlay);
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
        ItemOfUse,
    }
}
