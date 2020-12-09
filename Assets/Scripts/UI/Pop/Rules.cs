using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rules : PopUI
{
    public Text titleText;
    [Space(15)]
    public Text oneRule;
    public ContentSizeFitter all_rules_content;
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
                centerAlignStr = Tools.GetTextMiddleCenterContent(oneRule, behind);
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                break;
            case RuleArea.PlaySlots:
                titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_PlaySlotsTitle);
                allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_PlaySlots));
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                break;
            case RuleArea.InviteFriend:
                titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_InviteFrineds));
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                break;
            case RuleArea.MyInfo:
                titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Me));
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                break;
            case RuleArea.Cashout:
                titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_CashoutTitle);
                allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Cashout));
                cashoutButton.gameObject.SetActive(true);
                sureButton.gameObject.SetActive(false);
                break;
            case RuleArea.Offerwall:
                titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
                allStr = Tools.GetTextFlexiableContent(oneRule, Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Rules_Offerwall));
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
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
            all_rules[i].alignment= TextAnchor.UpperLeft;
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
}
