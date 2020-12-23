using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class SignTaskItem : MonoBehaviour
    {
        public Text desText;
        public Text progressText;
        public GameObject completeGo;
        public void Init(int task_cur,int task_tar,PlayerTaskTarget taskTarget)
        {
            desText.text = Tools.GetTaskDesMultiLanguage(taskTarget, task_tar);
            task_tar %= 100000;
            progressText.text = task_cur + "/" + task_tar;
            progressText.gameObject.SetActive(task_cur < task_tar);
            completeGo.gameObject.SetActive(task_cur >= task_tar);
        }
    }
}
