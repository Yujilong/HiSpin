﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tasks : BaseUI
{
    public GameObject all_get_tickets_root;
    public GameObject all_daily_task_root;
    public GameObject all_achievement_root;
    public ContentSizeFitter all_root;
    [Space(15)]
    public GameObject get_tickets_title;
    public GameObject daily_task_title;
    public GameObject achievement_task_title;
    public TaskItem single_get_tickets_task;
    public TaskItem single_daily_task;
    public TaskItem single_achievement_task;
    private List<TaskItem> get_tickets_items = new List<TaskItem>();
    private List<TaskItem> daily_task_items = new List<TaskItem>();
    private List<TaskItem> achievement_task_items = new List<TaskItem>();
    protected override void Awake()
    {
        base.Awake();
        get_tickets_items.Add(single_get_tickets_task);
        daily_task_items.Add(single_daily_task);
        achievement_task_items.Add(single_achievement_task);
        if (Master.IsBigScreen)
        {
            RectTransform all_anchorRect = all_root.transform.parent as RectTransform;
            all_anchorRect.localPosition -= new Vector3(0, Master.TopMoveDownOffset, 0);
            all_anchorRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
            all_anchorRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
        }
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        RefreshTaskInfo();
    }
    public void RefreshTaskInfo()
    {
        foreach (var task in get_tickets_items)
        {
            task.gameObject.SetActive(false);
        }
        foreach (var task in daily_task_items)
        {
            task.gameObject.SetActive(false);
        }
        foreach (var task in achievement_task_items)
        {
            task.gameObject.SetActive(false);
        }

        int getticketsTaskIndex = 0;
        int dailyTaskIndex = 0;
        int achievementIndex = 0;
        List<AllData_Task> taskList = Save.data.allData.lucky_schedule.user_task;
        int allTaskCount = taskList.Count;
        for (int i = 0; i < allTaskCount; i++)
        {
            AllData_Task taskData = taskList[i];
            if (!CheckIOSTaskIsShow(taskData.taskTargetId))
                continue;
            switch (taskData.task_type)
            {
                //gettickets
                case 1:
                    if (getticketsTaskIndex > get_tickets_items.Count - 1)
                    {
                        TaskItem newTaskItem = Instantiate(single_get_tickets_task.gameObject, single_get_tickets_task.transform.parent).GetComponent<TaskItem>();
                        get_tickets_items.Add(newTaskItem);
                    }
                    get_tickets_items[getticketsTaskIndex].gameObject.SetActive(true);
                    get_tickets_items[getticketsTaskIndex].Init(taskData.task_id, taskData.task_title, taskData.task_describe, taskData.taskTargetId, taskData.reward_type, taskData.task_reward, taskData.task_receive, taskData.task_complete, 0, taskData.task_tar);
                    getticketsTaskIndex++;
                    break;
                //daily task
                case 2:
                    if (dailyTaskIndex > daily_task_items.Count - 1)
                    {
                        TaskItem newTaskItem = Instantiate(single_daily_task.gameObject, single_daily_task.transform.parent).GetComponent<TaskItem>();
                        daily_task_items.Add(newTaskItem);
                    }
                    daily_task_items[dailyTaskIndex].gameObject.SetActive(true);
                    daily_task_items[dailyTaskIndex].Init(taskData.task_id, taskData.task_title, taskData.task_describe, taskData.taskTargetId, taskData.reward_type, taskData.task_reward, taskData.task_receive, taskData.task_complete, 1, taskData.task_tar);
                    dailyTaskIndex++;
                    break;
                //achievement
                case 3:
                    if (achievementIndex > achievement_task_items.Count - 1)
                    {
                        TaskItem newTaskItem = Instantiate(single_achievement_task.gameObject, single_achievement_task.transform.parent).GetComponent<TaskItem>();
                        achievement_task_items.Add(newTaskItem);
                    }
                    achievement_task_items[achievementIndex].gameObject.SetActive(true);
                    achievement_task_items[achievementIndex].Init(taskData.task_id, taskData.task_title, taskData.task_describe, taskData.taskTargetId, taskData.reward_type, taskData.task_reward, taskData.task_receive, taskData.task_complete, 2, taskData.task_tar);
                    achievementIndex++;
                    break;
            }
        }
        bool hasGetTicketTask = getticketsTaskIndex > 0;
        all_get_tickets_root.SetActive(hasGetTicketTask);
        get_tickets_title.SetActive(hasGetTicketTask);
        bool hasDailyTask = dailyTaskIndex > 0;
        all_daily_task_root.SetActive(hasDailyTask);
        daily_task_title.SetActive(hasDailyTask);
        bool hasAchievementTask = achievementIndex > 0;
        all_achievement_root.SetActive(hasAchievementTask);
        achievement_task_title.SetActive(hasAchievementTask);
        StartCoroutine("DelayRefreshLayout");
    }
    private bool CheckIOSTaskIsShow(PlayerTaskTarget taskTarget)
    {
#if UNITY_IOS
        if (Save.data.isPackB)
            return true;
        switch (taskTarget)
        {
            case PlayerTaskTarget.BuyTicketByRvOnce:
            case PlayerTaskTarget.WinnerOnce:
            case PlayerTaskTarget.InviteAFriend:
            case PlayerTaskTarget.WritePaypalEmail:
            case PlayerTaskTarget.CashoutOnce:
            case PlayerTaskTarget.WatchRvOnce:
            case PlayerTaskTarget.PlayBettingOnce:
                return false;
            case PlayerTaskTarget.EnterSlotsOnce:
            case PlayerTaskTarget.OwnSomeGold:
            case PlayerTaskTarget.GetTicketFromSlotsOnce:
            case PlayerTaskTarget.BuyTicketByGoldOnce:
            default:
                return true;
        }
#else
        return true;
#endif
    }
    private IEnumerator DelayRefreshLayout()
    {
        all_root.enabled = false;
        yield return new WaitForEndOfFrame();
        all_root.enabled = true;
    }
    [Space(15)]
    public Text get_ticketsText;
    public Text tasksText;
    public Text achievementsText;
    public override void SetContent()
    {
        get_ticketsText.text = "    " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.GetTickets);
        tasksText.text = "    " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Tasks);
        achievementsText.text = "    " + Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Achievements);
    }
}
