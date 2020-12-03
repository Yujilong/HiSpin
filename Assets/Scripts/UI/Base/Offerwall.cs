using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Offerwall : BaseUI
{
    public Button helpButton;
    public Text pt_numText;
    public Button cashoutButton;
    [Space(15)]
    public Button adgemButton;
    public Text adgem_button_contentText;
    public GameObject adgem_coinGo;
    [Space(15)]
    public Button isButton;
    public Text is_button_contentText;
    public GameObject is_coinGo;
    [Space(15)]
    public Button fyberButton;
    public Text fyber_button_contentText;
    public GameObject fyber_coinGo;

    [Space(15)]
    public RectTransform topRect;
    public RectTransform viewportRect;

    protected override void Awake()
    {
        base.Awake();
        string loading = FontContains.getInstance().GetString("lang0054");
        string ready =  FontContains.getInstance().GetString("lang0055") + "     ";
        helpButton.AddClickEvent(OnHelpButtonClick);
        cashoutButton.AddClickEvent(OnCashoutButtonClick);
        adgemButton.AddClickEvent(OnAdgemButtonClick);
        isButton.AddClickEvent(OnISButtonClick);
        fyberButton.AddClickEvent(OnFyberButtonClick);
        if (Master.IsBigScreen)
        {
            topRect.sizeDelta = new Vector2(topRect.sizeDelta.x, topRect.sizeDelta.y + Master.TopMoveDownOffset);
            viewportRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
        }
        adgem_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.AdGem) ? ready : loading;
        is_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.IS) ? ready : loading;
        fyber_button_contentText.text = Ads._instance.CheckOfferwallAvailable(Offerwall_Co.Fyber) ? ready : loading;
        StartCoroutine("UpdateOfferwallState");
    }
    IEnumerator UpdateOfferwallState()
    {
        while (true)
        {
            string loading = FontContains.getInstance().GetString("lang0054");
            string ready = FontContains.getInstance().GetString("lang0055") + "     ";
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
            Master.Instance.ShowTip(FontContains.getInstance().GetString("lang0136"));
        }
    }
    private void OnISButtonClick()
    {
        if (!Ads._instance.ShowOfferwallAd(Offerwall_Co.IS))
        {
            Master.Instance.ShowTip(FontContains.getInstance().GetString("lang0137"));
        }
    }
    private void OnFyberButtonClick()
    {
        if (!Ads._instance.ShowOfferwallAd(Offerwall_Co.Fyber))
        {
            Master.Instance.ShowTip(FontContains.getInstance().GetString("lang0138"));
        }
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        base.BeforeShowAnimation(args);
        cashoutButton.gameObject.SetActive(Save.data.isPackB);
        pt_numText.text = FontContains.getInstance().GetString("lang0020", ((int)Save.data.allData.fission_info.live_balance).GetTokenShowString()); 
    }
}
