using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class Sign : PopUI
    {
        public Button closeButton;
        public Button helpButton;
        public Text bonus_numText;
        public SignDayItem single_signdayItem;
        private List<SignDayItem> all_signdayItems = new List<SignDayItem>();
        public Button sign_inButton;
        protected override void Awake()
        {
            base.Awake();
            all_signdayItems.Add(single_signdayItem);
            closeButton.AddClickEvent(OnCloseButtonClick);
            helpButton.AddClickEvent(OnHelpButtonClick);
            sign_inButton.AddClickEvent(OnSigninButtonClick);
        }
        private void OnCloseButtonClick()
        {
            UI.ClosePopPanel(this);
        }
        private void OnHelpButtonClick()
        {
            UI.ShowPopPanel(PopPanel.Rules);
        }
        private void OnSigninButtonClick()
        {
            if (!canSign) return;

        }
        private bool canSign = false;
        protected override void BeforeShowAnimation(params int[] args)
        {
            foreach (var dayitem in all_signdayItems)
                dayitem.gameObject.SetActive(false);
            List<int> dayStates = Save.data.allData.check_task.task_list;
            int dayCount = dayStates.Count;
            int today = Save.data.allData.check_task.cur_day;
            canSign = !Save.data.allData.check_task.flag_task;
            for(int i = 0; i < dayCount; i++)
            {
                if (i > all_signdayItems.Count - 1)
                {
                    SignDayItem newDayItem = Instantiate(single_signdayItem.gameObject, single_signdayItem.transform.parent).GetComponent<SignDayItem>();
                    all_signdayItems.Add(newDayItem);
                }
                all_signdayItems[i].gameObject.SetActive(true);
                all_signdayItems[i].Init(i + 1, today + 1, Reward.Cash, dayStates[i] == 0);
            }
            sign_inButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Sign, canSign ? "signin_on" : "signin_off");
        }
    }
}
