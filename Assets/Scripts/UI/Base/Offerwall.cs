using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Offerwall : BaseUI
{
    public Button helpButton;
    public Button cashoutButton;
    [Space(15)]
    public Button adgemButton;
    public GameObject adgem_coinGo;
    [Space(15)]
    public Button isButton;
    public GameObject is_coinGo;
    [Space(15)]
    public Button fyberButton;
    public GameObject fyber_coinGo;
    [Space(15)]
    public RectTransform topRect;
    public RectTransform viewportRect;

    protected override void Awake()
    {
        base.Awake();
        helpButton.AddClickEvent(OnHelpButtonClick);
#if UNITY_IOS
        helpButton.gameObject.SetActive(Save.data.isPackB);
#endif
        cashoutButton.AddClickEvent(OnCashoutButtonClick);
        adgemButton.AddClickEvent(OnAdgemButtonClick);
        isButton.AddClickEvent(OnISButtonClick);
        fyberButton.AddClickEvent(OnFyberButtonClick);
        if (Master.IsBigScreen)
        {
            topRect.sizeDelta = new Vector2(topRect.sizeDelta.x, topRect.sizeDelta.y + Master.TopMoveDownOffset);
            viewportRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
        }
        StartCoroutine("UpdateOfferwallState");
    }
    IEnumerator UpdateOfferwallState()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            adgem_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.AdGem) ? ready : loading;
            is_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.IS) ? ready : loading;
            fyber_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.Fyber) ? ready : loading;
        }
    }
    private void OnHelpButtonClick()
    {
        UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.Offerwall);
    }
    private void OnCashoutButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Cashout);
    }
    private void OnAdgemButtonClick()
    {
        if (!Ads._instance.ShowOfferwallAd(Offerwall_Co.AdGem))
        {
            Master.Instance.ShowTip("AdGem " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_OfferwallNotAvailable));
        }
    }
    private void OnISButtonClick()
    {
        if (!Ads._instance.ShowOfferwallAd(Offerwall_Co.IS))
        {
            Master.Instance.ShowTip("Ironsource " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_OfferwallNotAvailable));
        }
    }
    private void OnFyberButtonClick()
    {
        if (!Ads._instance.ShowOfferwallAd(Offerwall_Co.Fyber))
        {
            Master.Instance.ShowTip("Fyber " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_OfferwallNotAvailable));
        }
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        base.BeforeShowAnimation(args);
        cashoutButton.gameObject.SetActive(Save.data.isPackB);
    }
    [Space(15)]
    public Text helpText;
    public Text balanceText;
    public Text pt_numText;
    public Text cashoutText;
    public Text sponsorshipText;
    public Text adgem_button_contentText;
    public Text is_button_contentText;
    public Text fyber_button_contentText;
    string loading;
    string ready;
    public override void SetContent()
    {
        helpText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Help);
        balanceText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Balance);
        pt_numText.text = ((int)Save.data.allData.fission_info.live_balance).GetTokenShowString() + " <size=70>" + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.PT) + "</size>";
        cashoutText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CASHOUT);
        sponsorshipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Offerwall_SPONSORSHIP);
        loading = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Offerwall_Loading);
        ready = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Offerwall_EranPts);
        adgem_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.AdGem) ? ready : loading;
        is_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.IS) ? ready : loading;
        fyber_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.Fyber) ? ready : loading;
    }
}
