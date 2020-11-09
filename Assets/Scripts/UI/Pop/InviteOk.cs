﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteOk : PopUI
{
    public Image reward_iconImage;
    public Text reward_numText;
    public Text double_buttonText;
    public Button double_rewardButton;
    public Button single_rewardButton;
    protected override void Awake()
    {
        base.Awake();
        double_rewardButton.AddClickEvent(OnDoubleClick);
        single_rewardButton.AddClickEvent(OnSingleClick);
    }
    private void OnDoubleClick()
    {
        Server.Instance.RequestData(Server.Server_RequestType.TaskData, () => { OnGetTaskListCallback(true); }, null);
    }
    private void OnGetTaskListCallback(bool doublReward)
    {
        List<PlayerTaskData> taskDatas = Save.data.task_list.user_task;
        PlayerTaskData inviteTaskData = null;
        if (taskDatas != null && taskDatas.Count > 0)
        {
            foreach (var task in taskDatas)
            {
                if (task.taskTargetId == PlayerTaskTarget.InviteAFriend)
                {
                    inviteTaskData = task;
                    break;
                }
            }
        }
        if (inviteTaskData == null)
            Master.Instance.ShowTip("Error: can not get task id", 2);
        else
            Server.Instance.OperationData_FinishTask(OnGetRewardCallback, null, inviteTaskData.task_id, doublReward, Reward.Cash, Reward.Ticket);
    }
    private void OnGetRewardCallback()
    {
        UI.ClosePopPanel(this);
    }
    private void OnSingleClick()
    {
        Server.Instance.RequestData(Server.Server_RequestType.TaskData, () => { OnGetTaskListCallback(false); }, null);
    }
    Reward invite_ok_reward_type;
    int invite_ok_reward_num;
    protected override void BeforeShowAnimation(params int[] args)
    {
        invite_ok_reward_type = (Reward)args[0];
        invite_ok_reward_num = args[1];
        reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.InviteOk, invite_ok_reward_type.ToString());
        if (invite_ok_reward_type == Reward.Cash)
        {
            single_rewardButton.gameObject.SetActive(false);
            double_buttonText.text = "GET";
            reward_numText.text = "x " + invite_ok_reward_num.GetCashShowString();
        }
        else
        {
            single_rewardButton.gameObject.SetActive(true);
            double_buttonText.text = "GET x 2";
            reward_numText.text = "x " + invite_ok_reward_num;
        }
    }
}
