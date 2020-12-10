using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputPaypalEmail : PopUI
{
    public Button closeButton;
    public Button confirmButton;
    public InputField firstnameInput;
    public InputField lastnameInput;
    public InputField paypalemailInput;
    protected override void Awake()
    {
        base.Awake();
        closeButton.AddClickEvent(ClosePanel);
        confirmButton.AddClickEvent(OnConfirmClick);
    }
    private void ClosePanel()
    {
        UI.ClosePopPanel(this);
    }
    private void OnConfirmClick()
    {
        string firstname = firstnameInput.text;
        string lastname = lastnameInput.text;
        string paypal = paypalemailInput.text;
        if (string.IsNullOrEmpty(firstname) || string.IsNullOrWhiteSpace(firstname) ||
            string.IsNullOrEmpty(lastname) || string.IsNullOrWhiteSpace(lastname) ||
            string.IsNullOrEmpty(paypal) || string.IsNullOrWhiteSpace(paypal))
        {
            Master.Instance.ShowTip(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_EmptyEmail), 3);
        }
        else
            Server_New.Instance.ConnectToServer_BindPaypal(OnConfirmCallback, null, null, true, paypal, firstname, lastname);
    }
    private void OnConfirmCallback()
    {
        Master.Instance.SendAdjustInputEmailEvent(paypalemailInput.text);
        TaskAgent.TriggerTaskEvent(PlayerTaskTarget.WritePaypalEmail, 1);
        UI.ClosePopPanel(this);
    }
    [Space(15)]
    public Text titleText;
    public Text firstnamePlaceholder;
    public Text lastnamePlaceholder;
    public Text paypalemailPlaceholder;
    public Text confirm_button_contentText;
    public Text tipText;
    public override void SetContent()
    {
        titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputPaypal_WindowTitle);
        firstnamePlaceholder.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputPaypal_FirstNamePlaceholder);
        lastnamePlaceholder.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputPaypal_LastNamePlaceholder);
        paypalemailPlaceholder.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InputPaypal_PayapalPlaceholder);
        confirm_button_contentText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.CONFIRM);
        tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Check_Caution);
    }
}
