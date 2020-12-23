using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HiSpin
{
    public class TaskAgent
    {
        public static void TriggerTaskEvent(PlayerTaskTarget taskTarget, int change_num)
        {
            int hasFinished = 0;
            List<AllData_Task> allTask = Save.data.allData.lucky_schedule.user_task;
            int taskCount = allTask.Count;
            for (int i = 0; i < taskCount; i++)
            {
                AllData_Task task = allTask[i];
                if (task.taskTargetId == taskTarget)
                {
                    if (task.taskTargetId == PlayerTaskTarget.InviteAFriend)
                        continue;
                    task.task_cur += change_num;
                    if (!string.IsNullOrEmpty(task.task_describe))
                        task.task_describe = task.task_cur + "/" + task.task_tar;
                    if (task.task_cur >= task.task_tar && !task.task_receive)
                    {
                        task.task_complete = true;
                        hasFinished++;
                    }
                    else
                        task.task_describe = "";
                }
            }
            List<AllData_SignTaskData> _SignTaskDatas = Save.data.allData.check_task.tar_task;
            foreach (var task in _SignTaskDatas)
                if (task.tar_id == taskTarget)
                    task.cur_num += change_num;
            UI.OnHasTaskFinished(hasFinished > 0);
        }
    }
}
