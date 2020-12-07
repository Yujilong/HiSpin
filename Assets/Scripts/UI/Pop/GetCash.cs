using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCash : PopUI
{
    public Button tribleButton;
    public Button nothanksButton;
    public Text cash_numText;
    public Text add_cashpt_numText;
    public Text trible_button_contentText;
    public GameObject ad_iconGo;
    protected override void Awake()
    {
        base.Awake();
        tribleButton.AddClickEvent(OnGetClick);
        nothanksButton.AddClickEvent(OnNothanksClick);
    }
    private void OnNothanksClick()
    {
        switch (getCashArea)
        {
            case GetCashArea.PlaySlots:
                Server_New.Instance.ConnectToServer_GetSlotsReward(OnGetOneSlotsRewardCallback, null, null, true, Reward.Cash, getcashNum);
                break;
            default:
                Master.Instance.ShowTip("Error : Cash Area is not correct.");
                break;
        }
    }
    private void OnGetClick()
    {
        switch (getCashArea)
        {
            case GetCashArea.NewPlayerReward:
                //Server.Instance.OperationData_GetNewPlayerReawrd(OnGetNewplayerRewardCallback, null);
                Server_New.Instance.ConnectToServer_GetNewPlayerReward(OnGetNewplayerRewardCallback, null, null, true);
                break;
            case GetCashArea.PlaySlots:
                //Server.Instance.OperationData_GetSlotsReward(OnGetSlotsRewardCallback, null, Reward.Cash, getcashNum);
                Server_New.Instance.ConnectToServer_GetSlotsReward(OnGetTribleSlotsRewardCallback, null, null, true, Reward.Cash, getcashNum * 3);
                break;
        }
    }
    private void OnGetTribleSlotsRewardCallback()
    {
        Save.data.allData.user_panel.lucky_total_cash += getcashNum * 3;
        UI.FlyReward(Reward.Cash, getcashNum, tribleButton.transform.position);
        UI.ClosePopPanel(this);
    }
    private void OnGetOneSlotsRewardCallback()
    {
        Save.data.allData.user_panel.lucky_total_cash += getcashNum;
        UI.FlyReward(Reward.Cash, getcashNum, tribleButton.transform.position);
        UI.ClosePopPanel(this);
    }
    private void OnGetNewplayerRewardCallback()
    {
        UI.FlyReward(Reward.Cash, getcashNum, tribleButton.transform.position);
        UI.ClosePopPanel(this);
        if (Save.data.isPackB)
            UI.ShowPopPanel(PopPanel.Guide);
    }
    GetCashArea getCashArea;
    int getcashNum;
    protected override void BeforeShowAnimation(params int[] args)
    {
        getCashArea = (GetCashArea)args[0];
        getcashNum = args[1];

        switch (getCashArea)
        {
            case GetCashArea.NewPlayerReward:
                ad_iconGo.SetActive(false);
                trible_button_contentText.text = "Save in wallet";
                cash_numText.text = "$" + getcashNum.GetCashShowString();
                add_cashpt_numText.transform.parent.gameObject.SetActive(false);
                break;
            case GetCashArea.PlaySlots:
                ad_iconGo.SetActive(true);
                trible_button_contentText.text = "     Get x3";
                cash_numText.text = "$" + (getcashNum / Cashout.CashToDollerRadio).GetCashShowString();
                add_cashpt_numText.transform.parent.gameObject.SetActive(true);
                add_cashpt_numText.text = "+" + getcashNum.GetTokenShowString();
                break;
        }

        cash_numText.text = "$" + getcashNum.GetCashShowString();

        nothanksButton.gameObject.SetActive(false);
    }
    protected override void AfterShowAnimation(params int[] args)
    {
        Master.Instance.ShowEffect(Reward.Cash);
        if (getCashArea == GetCashArea.PlaySlots)
        {
            StartCoroutine("DelayShowNothanks");
        }
    }
    protected override void BeforeCloseAnimation()
    {
        StopCoroutine("DelayShowNothanks");
    }
    private IEnumerator DelayShowNothanks()
    {
        yield return new WaitForSeconds(1);
        nothanksButton.gameObject.SetActive(true);
    }
}
public enum GetCashArea
{
    NewPlayerReward,
    PlaySlots
}
