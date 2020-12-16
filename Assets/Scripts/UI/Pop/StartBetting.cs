﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class StartBetting : PopUI
    {
        public GameObject allFlyticketGo;
        public Text yesterday_ticket_numText;
        public RectTransform lightRect;
        public RectTransform all_card_rootRect;
        public List<RectTransform> all_fly_cards;
        public CardItem single_card_item;
        public RectTransform fly_targetRect;
        public Button getButton;
        protected override void Awake()
        {
            base.Awake();
            getButton.AddClickEvent(OnGetButtonClick);
            endPos = new Vector2(-473.27f - 3, all_card_rootRect.localPosition.y);
            if (Master.IsBigScreen)
            {
                yesterday_ticket_numText.transform.parent.localPosition -= new Vector3(0, 100, 0);
                foreach (var rect in all_fly_cards)
                    rect.localPosition -= new Vector3(0, 100, 0);
            }
        }
        private void OnGetButtonClick()
        {
            //Server.Instance.OperationData_OpenBettingPrize(OnRequstDataCallback, null);
            Server_New.Instance.ConnectToServer_OpenBettingPrize(OnRequstDataCallback, null, null, true);
        }
        private void OnRequstDataCallback()
        {
            UI.ClosePopPanel(this);
        }
        Vector3 endPosOffset = new Vector2(-473.27f, 0);
        Vector3 endPos = new Vector2(-473.27f, 0);
        IEnumerator AutoSpinCards()
        {
            int yesterdayTicket = Save.data.allData.award_ranking.ysterday_tickets;
            yield return new WaitForSeconds(1);
            if (yesterdayTicket >= Save.data.allData.award_ranking.ticktes_flag)
            {
                Vector3 flyEndPos = fly_targetRect.position;
                Vector3 startPos = yesterday_ticket_numText.transform.position;
                int cardCount = all_fly_cards.Count;
                int startCardIndex = 0;
                int endCardIndex = 0;
                float flyInterval = 0.3f;
                float flyTimer = 0;
                float nextFlyTime = 0;
                while (endCardIndex < cardCount)
                {
                    flyTimer += Time.deltaTime;
                    yesterday_ticket_numText.text = ((int)(yesterdayTicket * Mathf.Clamp(((1.8f - flyTimer) / 1.8f), 0, 1))).GetTokenShowString();
                    if (flyTimer >= nextFlyTime)
                    {
                        startCardIndex++;
                        if (startCardIndex > cardCount)
                            startCardIndex = cardCount;
                        nextFlyTime += flyInterval;
                    }
                    for (int i = endCardIndex; i < startCardIndex; i++)
                    {
                        all_fly_cards[i].Rotate(new Vector3(0, 0, Time.deltaTime * 100));
                        float progress = Mathf.Clamp(flyTimer - flyInterval * i, 0, 1);
                        all_fly_cards[i].transform.position = Vector3.Lerp(startPos, flyEndPos, progress);
                        if (progress == 1)
                        {
                            all_fly_cards[i].gameObject.SetActive(false);
                            Audio.PlayOneShot(AudioPlayArea.FlyOver);
                            endCardIndex++;
                        }
                    }
                    yield return null;
                }
            }
            float timer = 0;
            float spinTime = 2;
            bool hasBack = false;
            float forwardSpeed = 0;
            float backSpeed = 0.015f;
            float backTimer = 0;

            float maxSpeed = 6000;

            all_card_rootRect.localPosition = new Vector3(0, all_card_rootRect.localPosition.y);
            while (true)
            {
                yield return null;
                timer += Time.deltaTime;
                if (timer < spinTime)
                {
                    forwardSpeed += 50;
                    if (forwardSpeed > maxSpeed)
                        forwardSpeed = maxSpeed;
                    all_card_rootRect.localPosition += new Vector3(-Time.deltaTime * forwardSpeed, 0);
                    if (all_card_rootRect.localPosition.x <= endPosOffset.x)
                    {
                        all_card_rootRect.localPosition -= 2 * endPosOffset;
                    }
                }
                else
                {
                    if (!hasBack)
                    {
                        backSpeed -= 0.0005f;
                        backTimer += backSpeed;
                        all_card_rootRect.localPosition = new Vector3(endPosOffset.x * (1 + backTimer), endPos.y);
                        if (backSpeed <= 0)
                            hasBack = true;
                    }
                    else
                    {
                        backSpeed -= 0.002f;
                        backTimer += backSpeed;
                        all_card_rootRect.localPosition = new Vector3(endPosOffset.x * (1 + backTimer), endPos.y);
                        if (backTimer <= 0)
                        {
                            all_card_rootRect.localPosition = endPos;
                            break;
                        }
                    }
                }
            }
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.OpenPrize_Tip1);
            get_button_contentText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.OpenPrize_Button1);
            List<AllData_BettingWinnerData_Winner> bettingWinners = Save.data.allData.award_ranking.ranking;
            string selfId = Save.data.allData.user_panel.user_id;
            AllData_BettingWinnerData_Winner willShow = bettingWinners[0];
            foreach (var winner in bettingWinners)
            {
                if (winner.user_id.Equals(selfId))
                {
                    willShow = winner;
                    tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.OpenPrize_Tip2);
                    get_button_contentText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.OpenPrize_Button2);
                    TaskAgent.TriggerTaskEvent(PlayerTaskTarget.WinnerOnce, 1);
                    break;
                }
            }
            single_card_item.Init(willShow.user_title, willShow.user_id, willShow.user_num);
            yield return new WaitForSeconds(1);
            getButton.gameObject.SetActive(true);
        }
        IEnumerator AutoRatateLight()
        {
            while (true)
            {
                lightRect.Rotate(new Vector3(0, 0, Time.deltaTime * 10));
                yield return null;
            }
        }
        protected override void BeforeShowAnimation(params int[] args)
        {
            single_card_item.SetOff();
            int yesterdayTicket = Save.data.allData.award_ranking.ysterday_tickets;
            yesterday_ticket_numText.text = yesterdayTicket.GetTokenShowString();
            tipText.text = "";
            allFlyticketGo.SetActive(yesterdayTicket >= Save.data.allData.award_ranking.ticktes_flag);
            yesterday_ticket_numText.transform.parent.gameObject.SetActive(allFlyticketGo.activeSelf);
            getButton.gameObject.SetActive(false);
            StartCoroutine(AutoRatateLight());
        }
        protected override void AfterShowAnimation(params int[] args)
        {
            StartCoroutine(AutoSpinCards());
        }
        [Space(15)]
        public Text titleText;
        public Text tipText;
        public Text get_button_contentText;
        public override void SetContent()
        {
            tipText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.OpenPrize_Title);


        }
    }
}
