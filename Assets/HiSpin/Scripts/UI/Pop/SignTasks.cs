using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class SignTasks : PopUI
    {
        public Button closeButton;
        public Text timedownText;
        public SignTaskItem single_sign_taskItem;
        private readonly List<SignTaskItem> all_sign_tasks = new List<SignTaskItem>();
        protected override void Awake()
        {
            base.Awake();
            all_sign_tasks.Add(single_sign_taskItem);
            closeButton.AddClickEvent(OnCloseClick);
            timedownText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.SignTask_Endin), "23:59:59");
        }
        private void OnCloseClick()
        {
            UI.ClosePopPanel(this);
        }
        public void UpdateTimeDown(string time)
        {
            timedownText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.SignTask_Endin), time);
        }
        protected override void BeforeShowAnimation(params int[] args)
        {
            foreach (var task in all_sign_tasks)
                task.gameObject.SetActive(false);

            List<AllData_SignTaskData> _SignTaskDatas = Save.data.allData.check_task.tar_task;
            int taskCount = _SignTaskDatas.Count;
            for(int i = 0; i < taskCount; i++)
            {
                if (i > all_sign_tasks.Count - 1)
                {
                    SignTaskItem newTask = Instantiate(single_sign_taskItem.gameObject, single_sign_taskItem.transform.parent).GetComponent<SignTaskItem>();
                    all_sign_tasks.Add(newTask);
                }
                all_sign_tasks[i].gameObject.SetActive(true);
                AllData_SignTaskData signTaskData = _SignTaskDatas[i];
                all_sign_tasks[i].Init(signTaskData.cur_num, signTaskData.tar_num, signTaskData.tar_id);
            }
        }
        [Space(15)]
        public Text titleText;
        public Text tipText;
        public override void SetContent()
        {
            titleText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.SignTask_Title);
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.SignTask_Tip);
        }
        protected override void AfterCloseAnimation()
        {
            UI.ShowPopPanel(PopPanel.Sign);
        }
    }
}
