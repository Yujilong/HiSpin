using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetNewPlayerReward : PopUI
{
    public CanvasGroup get_newplayer_rewardCg;
    public Text cashnumText;
    public Button cashoutButton;
    [Space(15)]
    public CanvasGroup input_paypal_emailCg;
    public Button skipButton;
    public Text card_cashnumText;
    public InputField emailInputfield;
    public Button claimButton;
    public Button agreeButton;
    public Image agreeIcon;
    public Button termsButton;
    [Space(15)]
    public CanvasGroup cashout_recordCg;
    public Text cashout_cashnumText;
    public Text stateText;
    public Text timeValueText;
    public Text orderidValueText;
    public Text accountValueText;
    public Button nextButton;
    protected override void Awake()
    {
        base.Awake();
        cashoutButton.AddClickEvent(OnCashouButtonClick);
        skipButton.AddClickEvent(OnSkipButtonClick);
        claimButton.AddClickEvent(OnClaimButtonClick);
        agreeButton.AddClickEvent(OnAgreeButtonClick);
        termsButton.AddClickEvent(OnTermsButtonClick);
        nextButton.AddClickEvent(OnNextButtonClick);
    }
    private void Start()
    {
        OriginInputFieldLocalPos = emailInputfield.transform.localPosition;
        OriginAgreeLocalPos = agreeButton.transform.parent.localPosition;
    }
    private void OnCashouButtonClick()
    {
        get_newplayer_rewardCg.alpha = 0;
        get_newplayer_rewardCg.blocksRaycasts = false;
        input_paypal_emailCg.alpha = 1;
        input_paypal_emailCg.blocksRaycasts = true;
    }
    private void OnSkipButtonClick()
    {
        UI.ClosePopPanel(this);
    }
    bool hasAgreeRule = false;
    Vector3 OriginInputFieldLocalPos;
    Vector3 OriginAgreeLocalPos;
    private void OnClaimButtonClick()
    {
        if (string.IsNullOrEmpty(emailInputfield.text) || string.IsNullOrWhiteSpace(emailInputfield.text))
        {
            StopCoroutine("ShakeSomething");
            emailInputfield.transform.localPosition = OriginInputFieldLocalPos;
            agreeButton.transform.parent.localPosition = OriginAgreeLocalPos;
            StartCoroutine("ShakeSomething", emailInputfield.transform);
        }
        else if (!hasAgreeRule)
        {
            StopCoroutine("ShakeSomething");
            emailInputfield.transform.localPosition = OriginInputFieldLocalPos;
            agreeButton.transform.parent.localPosition = OriginAgreeLocalPos;
            StartCoroutine("ShakeSomething", agreeButton.transform.parent);
        }
        else
            //Server_New.Instance.ConnectToServer_BindPaypal(OnBindPaypalCallback, null, null, true, emailInputfield.text, " ", " ");
            OnBindPaypalCallback();
    }
    IEnumerator ShakeSomething(Transform targetTrans)
    {
        float shakeOffsetX = 15;
        float shakeOffsetY = 5;
        float startLocalX = targetTrans.localPosition.x - shakeOffsetX;
        float endLocalX = targetTrans.localPosition.x + shakeOffsetX;
        float startLocalY = targetTrans.localPosition.y - shakeOffsetY;
        float endLocalY = targetTrans.localPosition.y + shakeOffsetY;
        float progressX = 0.5f;
        float progressY = 0.5f;
        bool isUp = true;
        bool isRight = true;
        bool xEnd = false;
        bool yEnd = false;
        float speedX = 20;
        float speedY = 20;
        int turnX = 4;
        int turnY = 4;
        int turnIndexX = 0;
        int turnIndexY= 0;
        while (true)
        {
            if (!xEnd)
            {
                progressX += isRight ? Time.deltaTime * speedX : -Time.deltaTime * speedX;
                progressX = Mathf.Clamp(progressX, 0, 1);
            }
            if (!yEnd)
            {
                progressY += isRight ? Time.deltaTime * speedY : -Time.deltaTime * speedY;
                progressY = Mathf.Clamp(progressY, 0, 1);
            }
            if (turnIndexX >= turnX)
            {
                if (turnX % 2 == 1)
                    progressX = Mathf.Clamp(progressX, 0.5f, 1);
                else
                    progressX = Mathf.Clamp(progressX, 0, 0.5f);
                if (progressX == 0.5f)
                {
                    xEnd = true;
                }
            }
            if (turnIndexY >= turnY)
            {
                if (turnY % 2 == 1)
                    progressY = Mathf.Clamp(progressY, 0.5f, 1);
                else
                    progressY = Mathf.Clamp(progressY, 0, 0.5f);
                if (progressY == 0.5f)
                {
                    xEnd = true;
                }
            }
            targetTrans.localPosition = new Vector3(Mathf.Lerp(startLocalX, endLocalX, progressX), Mathf.Lerp(startLocalY, endLocalY, progressY), 0);
            if (isRight && progressX >= 1)
            {
                turnIndexX++;
                isRight = false;
            }
            else if (!isRight && progressX <= 0)
            {
                turnIndexX++;
                isRight = true;
            }
            if (isRight && progressY >= 1)
            {
                turnIndexY++;
                isUp = false;
            }
            else if (!isRight && progressY <= 0)
            {
                turnIndexY++;
                isUp = true;
            }
            yield return null;
            if (xEnd && yEnd)
                break;
        }
    }
    private void OnBindPaypalCallback()
    {
        input_paypal_emailCg.alpha = 0;
        input_paypal_emailCg.blocksRaycasts = false;
        cashout_recordCg.alpha = 1;
        cashout_recordCg.blocksRaycasts = true;
    }
    private void OnAgreeButtonClick()
    {
        hasAgreeRule = !hasAgreeRule;
        agreeIcon.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetNewPlayerReward, hasAgreeRule ? "agree" : "disagree");
    }
    private void OnTermsButtonClick()
    {
        Application.OpenURL("");
    }
    private void OnNextButtonClick()
    {
        UI.ClosePopPanel(this);
    }
    int step = -1;
    protected override void BeforeShowAnimation(params int[] args)
    {
        step = args[0];
        switch (step)
        {
            case 0:
                get_newplayer_rewardCg.alpha = 1;
                get_newplayer_rewardCg.blocksRaycasts = true;
                input_paypal_emailCg.alpha = 0;
                input_paypal_emailCg.blocksRaycasts = false;
                cashout_recordCg.alpha = 0;
                cashout_recordCg.blocksRaycasts = false;
                break;
            case 1:
                get_newplayer_rewardCg.alpha = 0;
                get_newplayer_rewardCg.blocksRaycasts = false;
                input_paypal_emailCg.alpha = 1;
                input_paypal_emailCg.blocksRaycasts = true;
                cashout_recordCg.alpha = 0;
                cashout_recordCg.blocksRaycasts = false;
                break;
        }
        agreeIcon.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetNewPlayerReward, hasAgreeRule ? "agree" : "disagree");
    }
}
