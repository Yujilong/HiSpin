using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cashout : BaseUI
{
    public RectTransform anchor_rect;
    [Space(15)]
    public Text emailText;
    public Button emailInputButton;
    public Button recordButton;
    [Space(15)]
    public Text pt_numText;
    public Text pt_cashout_numText;
    public Button pt_cashout_leftButton;
    public Button pt_cashout_midButton;
    public Button pt_cashout_rightButton;
    [Space(15)]
    public Text cash_numText;
    public Button cashout_leftButton;
    public Button cashout_midButton;
    public Button cashout_rightButton;
    [Space(15)]
    public Text gold_numText;
    public Button gold_redeemButton;
    [Space(15)]
    public Button about_feeButton;
    //L,M,R
    static readonly int[] Cashout_Nums = new int[3] { 10, 50, 100 };
    protected override void Awake()
    {
        base.Awake();
        recordButton.AddClickEvent(OnRecordButtonClick);
        emailInputButton.AddClickEvent(OnEmailInput);
        about_feeButton.AddClickEvent(OnAboutFeeClick);
        if (Master.IsBigScreen)
        {
            anchor_rect.localPosition -= new Vector3(0, Master.TopMoveDownOffset, 0);
            anchor_rect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
            anchor_rect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
        }

        pt_cashout_leftButton.GetComponentInChildren<Text>().text = FontContains.getInstance().GetString("lang0003", " 5"); 
        pt_cashout_midButton.GetComponentInChildren<Text>().text = FontContains.getInstance().GetString("lang0003", " 10"); 
        pt_cashout_rightButton.GetComponentInChildren<Text>().text = FontContains.getInstance().GetString("lang0003", " 50"); 
        cashout_leftButton.GetComponentInChildren<Text>().text = FontContains.getInstance().GetString("lang0003"," "+ Cashout_Nums[0]);
        cashout_midButton.GetComponentInChildren<Text>().text = FontContains.getInstance().GetString("lang0003", " " + Cashout_Nums[1]); 
        cashout_rightButton.GetComponentInChildren<Text>().text = FontContains.getInstance().GetString("lang0003"," "+ Cashout_Nums[2]);

        pt_cashout_leftButton.AddClickEvent(() => { OnPtCashoutButtonClick(5);});
        pt_cashout_midButton.AddClickEvent(() => { OnPtCashoutButtonClick(10); });
        pt_cashout_rightButton.AddClickEvent(() => { OnPtCashoutButtonClick(50); });
        cashout_leftButton.AddClickEvent(() => { OnCashoutButtonClick(Cashout_Nums[0]); });
        cashout_midButton.AddClickEvent(() => { OnCashoutButtonClick(Cashout_Nums[1]); });
        cashout_rightButton.AddClickEvent(() => { OnCashoutButtonClick(Cashout_Nums[2]); });

        gold_redeemButton.AddClickEvent(OnGoldCashoutButtonClick);
    }
    private void OnRecordButtonClick()
    {
        UI.ShowBasePanel(BasePanel.CashoutRecord);
    }
    private void OnEmailInput()
    {
        UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.PaypalEmail);
    }
    private void OnPtCashoutButtonClick(int cashoutNum)
    {
        if (Save.data.allData.fission_info.live_balance >= cashoutNum * PtCashoutRate * 100)
            UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Cashout, cashoutNum, (int)CashoutType.PT, cashoutNum * PtCashoutRate * 100);
        else
            Master.Instance.ShowTip(FontContains.getInstance().GetString("lang0134"));
    }
    private void OnCashoutButtonClick(int cashoutNum)
    {
        if (Save.data.allData.user_panel.user_doller_live >= cashoutNum * 100)
            UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Cashout, cashoutNum, (int)CashoutType.Cash, cashoutNum * 100);
        else
            Master.Instance.ShowTip(FontContains.getInstance().GetString("lang0134"));
    }
    private void OnGoldCashoutButtonClick()
    {
        Master.Instance.ShowTip(FontContains.getInstance().GetString("lang0134"));
    }
    private void OnAboutFeeClick()
    {
        //Server.Instance.RequestData_GetLocalcountry(OnRequestLocalcountyCallback, null);
        OnRequestLocalcountyCallback(Server_New.localCountry);
    }
    private void OnRequestLocalcountyCallback(string country)
    {
        Application.OpenURL(string.Format("https://www.paypal.com/{0}/webapps/mpp/paypal-fees", country));
    }
    const int CashoutNeedGold = 5000000;
    public const int GoldMaxNum = 4600000;
    const int PtCashoutRate = 1000;
    protected override void BeforeShowAnimation(params int[] args)
    {
        bool hasEmail = !string.IsNullOrEmpty(Save.data.allData.user_panel.user_paypal);
        if (hasEmail)
            emailText.text = Save.data.allData.user_panel.user_paypal;
        else
            emailText.text = FontContains.getInstance().GetString("lang0017");
        pt_numText.text = FontContains.getInstance().GetString("lang0020", (int)Save.data.allData.fission_info.live_balance);
        pt_cashout_numText.text = FontContains.getInstance().GetString("lang0013", ((int)((float)Save.data.allData.fission_info.live_balance / PtCashoutRate)).GetCashShowString()); 
        cash_numText.text = FontContains.getInstance().GetString("lang0003", " " + Save.data.allData.user_panel.user_doller_live.GetCashShowString()); 
        gold_numText.text = Save.data.allData.user_panel.user_gold_live.GetTokenShowString();
    }
    bool isPause = false;
    public override void Pause()
    {
        isPause = true;
    }
    public override void Resume()
    {
        if (!isPause) return;
        isPause = false;
        BeforeShowAnimation();
    }
}
public enum CashoutType
{
    PT,
    Cash,
    Gold
}
