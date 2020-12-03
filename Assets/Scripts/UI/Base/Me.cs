using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Me : BaseUI
{
    public Image head_iconImage;
    public Image exp_progress_fillImage;
    public Text lvText;
    public InputField nameInputfield;
    public Text ticket_multipleText;
    public Button helpButton;
    [Space(15)]
    public Text current_levelText;
    public Text next_levelText;
    public Image lv_progress_fillImage;
    public Text lv_progress_desText;
    public Text current_ticket_multipleText;
    public Text next_ticket_multipleText;
    public Image level_up_reward_iconImage;
    public Text level_up_reward_numText;
    [Space(15)]
    public ContentSizeFitter bottom_sizefitter;
    public AvatarItem sigle_avatar_item;
    private List<AvatarItem> all_avatar_items = new List<AvatarItem>();
    public RectTransform allRect;
    public RectTransform viewport;
    protected override void Awake()
    {
        base.Awake();
        nameInputfield.onValueChanged.AddListener(OnInputNameValueChanged);
        nameInputfield.onEndEdit.AddListener(OnInputNameEnd);
        helpButton.AddClickEvent(OnHelpButtonClick);
        if (Master.IsBigScreen)
        {
            allRect.localPosition -= new Vector3(0, Master.TopMoveDownOffset);
            viewport.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
        }

        all_avatar_items.Add(sigle_avatar_item);
    }
    private void OnInputNameValueChanged(string value)
    {
        nameInputfield.SetTextWithoutNotify(value.CheckName());
    }
    private void OnInputNameEnd(string value)
    {
        string endValue = value.CheckName();
        nameInputfield.SetTextWithoutNotify(endValue);
        //Server.Instance.OperationData_ChangeHead_Name(null, null, -1, endValue);
        Server_New.Instance.ConnectToServer_ChangeHedOrName(RefreshName, RefreshName, null, true, -1, endValue);
    }
    private void RefreshName()
    {
        nameInputfield.SetTextWithoutNotify(Save.data.allData.user_panel.user_name);
    }
    private void OnHelpButtonClick()
    {
        UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.MyInfo);
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        exp_progress_fillImage.fillAmount= (float)Save.data.allData.user_panel.user_exp / Save.data.allData.user_panel.level_exp;
        lvText.text = FontContains.getInstance().GetString("lang0042", Save.data.allData.user_panel.user_level);
        ticket_multipleText.text = FontContains.getInstance().GetString("lang0044", "x "+Save.data.allData.user_panel.user_double.GetTicketMultipleString());

        current_levelText.text = Save.data.allData.user_panel.user_level.ToString();
        next_levelText.text = (Save.data.allData.user_panel.user_level + 1).ToString();
        lv_progress_fillImage.fillAmount = exp_progress_fillImage.fillAmount;
        lv_progress_desText.text = Save.data.allData.user_panel.user_exp + "/" + Save.data.allData.user_panel.level_exp;
        current_ticket_multipleText.text = FontContains.getInstance().GetString("lang0046"," x" + Save.data.allData.user_panel.user_double.GetTicketMultipleString());
        next_ticket_multipleText.text = FontContains.getInstance().GetString("lang0047", Save.data.allData.user_panel.next_double.GetTicketMultipleString());
        level_up_reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, Save.data.allData.user_panel.level_type.ToString());
        string text = "";
        switch (Save.data.allData.user_panel.level_type)
        {
            case Reward.Cash:
                text = FontContains.getInstance().GetString("lang0164");
                break;
            case Reward.Gold:
                text = FontContains.getInstance().GetString("lang0163");
                break;
            case Reward.Ticket:
                text = FontContains.getInstance().GetString("lang0165");
                break;
        }
        level_up_reward_numText.text = text + "+" + Save.data.allData.user_panel.next_level;

        RefreshName();
        RefreshAvatarList();
    }
    public void RefreshAvatarList()
    {
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + Save.data.allData.user_panel.user_title);
        foreach (var avatar in all_avatar_items)
            avatar.gameObject.SetActive(false);
        int user_head_id = Save.data.allData.user_panel.user_title;
        List<int> avatar_id_list = Save.data.allData.user_panel.title_list;
        List<int> avatar_id_level_list = Save.data.allData.user_panel.title_level;
        List<bool> avatar_hasCheck_list = Save.data.head_icon_hasCheck;
        int idCount = avatar_id_list.Count;
        int idlevelCount = avatar_id_level_list.Count;
        if (idCount != idlevelCount)
            Debug.LogError("头像列表和头像等级限制列表不匹配");
        else
        {
            for (int i = 0; i < idCount; i++)
            {
                if (i > all_avatar_items.Count - 1)
                {
                    AvatarItem newAvatarItem = Instantiate(sigle_avatar_item, sigle_avatar_item.transform.parent).GetComponent<AvatarItem>();
                    all_avatar_items.Add(newAvatarItem);
                }
                all_avatar_items[i].gameObject.SetActive(true);
                int index = i;
                all_avatar_items[i].Init(avatar_id_list[i], avatar_id_level_list[i], !avatar_hasCheck_list[i], avatar_id_list[i] == user_head_id, index);
            }
        }
        StartCoroutine(DelayRefreshLayout());
    }
    private IEnumerator DelayRefreshLayout()
    {
        bottom_sizefitter.enabled = false;
        yield return new WaitForEndOfFrame();
        bottom_sizefitter.enabled = true;
    }
}
