using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;
using HiSpin;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public const float ballCircle = 209;
    public const float ballTextHeight = 96;
    public const float ballExplosionTime = 0.2f;
    public const float checkFailDelayTime = 4f;
    public const int canGetCashTimesPerDay = 30;
    public const int startWheelTicket = 10;
    public const int originPropNeedCoinNum = 300;
    public const int propNeedCoinNumIncreaseStep = 50;
    public const int propNeedMaxCoinNum = 1000;
    public const int levelStartTargetBallNum = 128;
    public const int originEnergy = 100;
    public const int maxEnergy = 100;
    public const int addEnergyPerAd = 50;
    public const int buyEnergyMaxTimePerDay = 20;

    public static bool isLoadingEnd = false;
    public AnimationCurve PopPanelScaleAnimation;
    public AnimationCurve PopPanelAlphaAnimation;
    public RectTransform popUIRootRect;
    public RectTransform menuRootRect;
    public GameObject audioRoot;

    [System.NonSerialized]
    public UIManager UIManager;
    public PlayerDataManager PlayerDataManager = null;
    private ConfigManager ConfigManager = null;
    private LevelManager LevelManager = null;
    private AudioManager AudioManager = null;
    [System.NonSerialized]
    public int UpgradeNeedScore = 0;
    [System.NonSerialized]
    public float CurrentLevelProgress = 0;
    [System.NonSerialized]
    public Reward WillBuyProp = Reward.Null;
    [System.NonSerialized]
    public int WillShowSlots = 0;
    [System.NonSerialized]
    public int WillShowGift = 0;
    [System.NonSerialized]
    public int WillShowOpenGoldBall = 0;
    private void Awake()
    {
        Instance = this;
        isLoadingEnd = false;
        UIManager = gameObject.AddComponent<UIManager>();
        LevelManager = gameObject.GetComponent<LevelManager>();
        UIManager.Init(popUIRootRect, menuRootRect, this);
        GetComponent<MainController>().Init();

        PlayerDataManager = new PlayerDataManager();
        ConfigManager = new ConfigManager();
        AudioManager = new AudioManager(audioRoot);

        //UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.LoadingPanel);
        RefreshUpgradeNeedScore();
        CurrentLevelProgress = GetLevel() + GetScore() / (UpgradeNeedScore * 1f);
        LevelManager.SetTargetBallNum(PlayerDataManager.GetLevelTargetBallNum());
    }
    public bool GetIsPackB()
    {
        return HiSpin.Save.data.isPackB;
    }
    public void SetIsPackB()
    {

    }
    public int GetCurrentStageUpgradeNeedScore(int currentStage)
    {
        return ConfigManager.GetStageScoreData(currentStage).upgradeNeedScore;
    }
    public void AddScore(int value)
    {
        PlayerDataManager.playerData.unSendMergeNum++;
        Master.Instance.AddLocalExp();
        StopCoroutine("DealySendMergeNum");
        StartCoroutine("DealySendMergeNum");
        bool isBest = PlayerDataManager.SetScore(GetScore() + value);
        if (PlayerDataManager.GetScore() >= 500)
            PlayerDataManager.playerData.hasUnlockRankAndLottery = true;
        SetCurrentLevelScore(GetCurrentLevelScore() + value);
        if (!GetWhetherRateus() && GetScore() >= 1000)
        {
            bool canShowRateus = true;
#if UNITY_IOS
            if (!GetIsPackB())
                canShowRateus = false;
#endif
            if (canShowRateus)
            {
                SetHasRateus();
                UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.RateusPanel);
            }
        }
        if (GetCurrentLevelScore() >= UpgradeNeedScore)
        {
            SetCurrentLevelScore(0);
            PlayerDataManager.SetLevel(GetLevel() + 1);
            RefreshUpgradeNeedScore();
        }
        CurrentLevelProgress = GetLevel() + GetCurrentLevelScore() / (UpgradeNeedScore * 1f);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshScoreText();
            if (isBest)
                _MenuPanel.RefreshBestScoreText();
        }
    }
    float delaySecond = 2;
    int lastSendMergeNum = 0;
    Coroutine sendCor = null;
    IEnumerator DealySendMergeNum()
    {
        if (sendCor != null)
            StopCoroutine(sendCor);
        float timer = 0;
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;
            if (timer >= delaySecond)
            {
                if (AnimationAutoEnd.IsAnimation)
                    continue;
                if (HiSpin.UI.CheckCurrentPopPanelIs(PopPanel.GetReward))
                    continue;
                if (PlayerDataManager.playerData.unSendMergeNum > 0)
                {
                    lastSendMergeNum = PlayerDataManager.playerData.unSendMergeNum;
                    sendCor = Master.ConnectServerToSendMergeNum(OnSendCallback, lastSendMergeNum);
                    yield break;
                }
            }
        }
    }
    void OnSendCallback()
    {
        PlayerDataManager.playerData.unSendMergeNum -= lastSendMergeNum;
        MainController.Instance.SaveData();
        lastSendMergeNum = 0;
    }
    public void OnLevelUpAndStopSendMergeNum()
    {
        StopCoroutine("DealySendMergeNum");
        if (sendCor != null)
            StopCoroutine(sendCor);
    }
    public int GetScore()
    {
        return PlayerDataManager.GetScore();
    }
    public int GetCurrentLevelScore()
    {
        return PlayerDataManager.playerData.currentLevelScore;
    }
    public void SetCurrentLevelScore(int value)
    {
        PlayerDataManager.playerData.currentLevelScore = value;
    }
    public int GetBestScore()
    {
        return PlayerDataManager.GetBestScore();
    }
    public int GetLevel()
    {
        return PlayerDataManager.GetLevel();
    }
    private void RefreshUpgradeNeedScore()
    {
        UpgradeNeedScore = ConfigManager.GetStageScoreData(GetLevel()).upgradeNeedScore;
    }
    public int GetCoin()
    {
        return Save.data.allData.user_panel.user_gold_live;
    }
    public int GetCash()
    {
        return Save.data.allData.user_panel.user_doller_live;
    }
    public int GetAmazon()
    {
        return PlayerDataManager.GetAmazon();
    }
    public int GetProp1Num()
    {
        return PlayerDataManager.GetPop1Num();
    }
    public int GetProp2Num()
    {
        return PlayerDataManager.GetPop2Num();
    }
    public int GetProp1NeedCoinNum()
    {
        return PlayerDataManager.GetProp1NeedCoinNum();
    }
    public int GetProp2NeedCoinNum()
    {
        return PlayerDataManager.GetProp2NeedCoinNum();
    }
    public int GetTodayCanGetCashTime()
    {
        return PlayerDataManager.GetTodayCanGetCashTime();
    }
    public bool GetTodayHasFreeWheel()
    {
        return PlayerDataManager.GetTodayHasFreeWheel();
    }
    public int GetWheelTicket()
    {
        return PlayerDataManager.GetWheelTicket();
    }
    public void UseFreeWheel()
    {
        PlayerDataManager.UseFreeWheel();
    }
    public int GetTargetLevelBallNum()
    {
        return PlayerDataManager.GetLevelTargetBallNum();
    }
    public void LevelUp(bool isGuide=false)
    {
        MainController.Instance.SetCurrentBallState(false);
        LevelManager.WhenLevelUp();
        if (!isGuide)
            PlayerDataManager.AddLevelTargetBallNum();
    }
    public void RefreshTargetBallNum()
    {
        LevelManager.SetTargetBallNum(PlayerDataManager.GetLevelTargetBallNum());
    }
    public int AddAmazon(int value)
    {
        int endValue = PlayerDataManager.AddAmazon(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            //_MenuPanel.RefreshCashText();
        }
        return endValue;
    }
    public int AddProp1Num(int value)
    {
        int currentPropNum = PlayerDataManager.AddPop1Num(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            if (value < 0)
                _MenuPanel.RefreshProp1();
        }
        return currentPropNum;
    }
    public int AddProp2Num(int value)
    {
        int currentPropNum = PlayerDataManager.AddPop2Num(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            if (value < 0)
                _MenuPanel.RefreshProp2();
        }
        return currentPropNum;
    }
    public int AddWheelTicket(int value)
    {
        return PlayerDataManager.AddWheelTicket(value);
    }
    public int AddBallFallNum(int value = 1)
    {
        PlayerDataManager.playerData.logPerTenBall++;
        if (PlayerDataManager.playerData.logPerTenBall > 0 && PlayerDataManager.playerData.logPerTenBall % 10 == 0)
        {
            SendAdjustPerTenBallEvent();
            MainController.Instance.SpawnNewGoldBall();
        }

        int operationNum= PlayerDataManager.AddFallBallNum(value);
        int targetNum = RandomGiftNeedFallBall();
        if (operationNum >= targetNum)
        {
            ClearBallFallNum();
            SpawnAGiftBall();
        }
        return operationNum;
    }
    public void AddGoldBallx10Time(int value = 1)
    {
        PlayerDataManager.playerData.logGoldBallGetx10Time += value;
    }
    [System.NonSerialized]
    public bool isPropGift = false;
    public void UseProp1()
    {
        SendAdjustPropChangeEvent(1, 0);
        MainController.Instance.UseProp1();
    }
    public bool UseProp2()
    {
        SendAdjustPropChangeEvent(2, 0);
        return MainController.Instance.UseProp2();
    }
    public void ShowUsePropGiftPanel()
    {
        isPropGift = true;
        if (!UIManager.PanelWhetherShowAnyone() && WillShowGift <= 0)
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.GiftPanel);
        else
            WillShowGift++;
    }
    public int IncreaseByProp1NeedCoin()
    {
        int nextNeedCoin = GetProp1NeedCoinNum() + propNeedCoinNumIncreaseStep;
        nextNeedCoin = Mathf.Clamp(nextNeedCoin, 0, propNeedMaxCoinNum);
        PlayerDataManager.SetProp1NeedCoinNum(nextNeedCoin);
        return nextNeedCoin;
    }
    public int IncreaseByProp2NeedCoin()
    {
        int nextNeedCoin = GetProp2NeedCoinNum() + propNeedCoinNumIncreaseStep;
        nextNeedCoin = Mathf.Clamp(nextNeedCoin, 0, propNeedMaxCoinNum);
        PlayerDataManager.SetProp2NeedCoinNum(nextNeedCoin);
        return nextNeedCoin;
    }
    public void ResetPropNeedCoinNum()
    {
        PlayerDataManager.SetProp1NeedCoinNum(originPropNeedCoinNum);
        PlayerDataManager.SetProp2NeedCoinNum(originPropNeedCoinNum);
    }
    public void ClearBallFallNum()
    {
        PlayerDataManager.ClearFallBallNum();
    }
    public int ReduceTodayCanGetCashTime(int value = -1)
    {
        return PlayerDataManager.ReduceTodayCanGetCashTime(value);
    }
    public bool nextSlotsIsUpgradeSlots = false;
    public int RandomSlotsReward()
    {
        SlotsData slotsData = ConfigManager.GetSlotsData(GetTotalCash());
        int total = slotsData.cashWeight + slotsData.coinWeight;
        int result = Random.Range(0, total);
        if (result < slotsData.cashWeight && GetTodayCanGetCashTime() > 0)
            return -Random.Range(slotsData.cashRange.x, slotsData.cashRange.y);
        else
            return Random.Range(slotsData.coinRange.x, slotsData.coinRange.y);
    }
    public Reward ConfirmReward_Type = Reward.Null;
    public int ConfirmRewrad_Num = 0;
    public bool ConfirmReward_Needad = true;
    public bool ConfirmReward_IsWheel = false;
    public void ShowConfirmRewardPanel(Reward type, int num, bool needAd = true, bool isWheel = false)
    {
        ConfirmReward_Type = type;
        ConfirmRewrad_Num = num;
        ConfirmReward_Needad = needAd;
        ConfirmReward_IsWheel = isWheel;
        if (type == Reward.Cash)
        {
            HiSpin.UI.ShowPopPanel(PopPanel.GetCash, (int)GetCashArea.Mergeball, num * 25, isWheel ? 1 : 0);
        }
        else
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.RewardNoCashPanel);
    }
    private struct WheelRandom
    {
        public int index;
        public int maxRange;
    }
    private List<WheelData> allWheelDatas = null;
    private readonly List<WheelRandom> wheelRandomDatas = new List<WheelRandom>();
    public int RandomWheelReward()
    {
        int playTime = PlayerDataManager.GetWheelSpinTimeTotal() + 1;
        PlayerDataManager.AddSpinWheelTimeTotal();
        if(allWheelDatas is null)
            allWheelDatas = ConfigManager.GetWheelDatas();
        wheelRandomDatas.Clear();
        int total = 0;
        int count = allWheelDatas.Count;
        for(int i = 0; i < count; i++)
        {
            //List<int> blackBox = allWheelDatas[i].blackbox;
            //foreach(int time in blackBox)
            //{
            //    if (time == playTime)
            //        return i;
            //}
            if (allWheelDatas[i].limitCash < 0 || (GetTotalCash() < allWheelDatas[i].limitCash && GetTodayCanGetCashTime() > 0))
            {
                total += allWheelDatas[i].weight;
                WheelRandom random = new WheelRandom
                {
                    index = i,
                    maxRange = total
                };
                wheelRandomDatas.Add(random);
            }
        }

        int result = Random.Range(0, total);
        int randomCount = wheelRandomDatas.Count;
        for(int i = 0; i < randomCount; i++)
        {
            if (result < wheelRandomDatas[i].maxRange)
                return wheelRandomDatas[i].index;
        }
        Debug.LogError("随机转盘奖励错误");
        return -1;
    }
    public void SaveBallData(List<Vector2> ballPos,List<int> ballNum , int currentBallNum)
    {
        PlayerDataManager.SaveBallData(ballPos, ballNum, currentBallNum);
    }
    public List<Vector2> GetBallData(out List<int> ballNum,out int currentBallNum)
    {
        return PlayerDataManager.GetBallData(out ballNum, out currentBallNum);
    }
    public int RandomGiftNeedFallBall()
    {
        bool isPackB = true;
        if (isPackB)
        {
            GiftDataB giftDataB = ConfigManager.GetGiftBData(GetTotalCash());
            return Random.Range(giftDataB.fallBallNumRange.x, giftDataB.fallBallNumRange.y);
        }
        else
        {
            GiftDataA giftDataA = ConfigManager.GetGiftAData(GetLevel());
            return Random.Range(giftDataA.fallBallNumRange.x, giftDataA.fallBallNumRange.y);
        }
    }
    private int GetTotalCash()
    {
        return Save.data.allData.user_panel.lucky_total_cash / 25;
    }
    public Reward RandomGiftReward(out int rewardNum)
    {
        bool isPackB = true;
        if (isPackB)
        {
            GiftDataB giftDataB = ConfigManager.GetGiftBData(GetTotalCash());
            if (GetTodayCanGetCashTime() <= 0)
            {
                rewardNum = Random.Range(giftDataB.rewardCoinNumRange.x, giftDataB.rewardCoinNumRange.y);
                return Reward.Coin;
            }
            int total = giftDataB.cashWeight + giftDataB.coinWeight;
            int result = Random.Range(0, total);
            if (result < giftDataB.cashWeight)
            {
                rewardNum = Random.Range(giftDataB.rewardCashNumRange.x, giftDataB.rewardCashNumRange.y);
                return Reward.Cash;
            }
            else
            {
                rewardNum = Random.Range(giftDataB.rewardCoinNumRange.x, giftDataB.rewardCoinNumRange.y);
                return Reward.Coin;
            }
        }
        else
        {
            GiftDataA giftDataA = ConfigManager.GetGiftAData(GetLevel());
            rewardNum = Random.Range(giftDataA.rewardCoinNumRange.x, giftDataA.rewardCoinNumRange.y);
            return Reward.Coin;
        }
    }
    public void ContinueGame()
    {
        MainController.Instance.OnContinueGame();
        SendAdjustGameOverEvent(true);
    }
    public void RestartGame()
    {
        ClearUnShowPanel();
        PlayerDataManager.playerData.logRestartTime++;
        MainController.Instance.OnRestartGame();
        PlayerDataManager.SetScore(0);
        PlayerDataManager.SetLevel(0);
        RefreshUpgradeNeedScore();
        PlayerDataManager.SetProp1NeedCoinNum(originPropNeedCoinNum);
        PlayerDataManager.SetProp2NeedCoinNum(originPropNeedCoinNum);
        PlayerDataManager.ReSetLevelTargetBallNum();
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshScoreText();
            _MenuPanel.ResetStageProgress();
            _MenuPanel.SetStageInfo();
            _MenuPanel.RefreshProp1();
            _MenuPanel.RefreshProp2();
        }
        LevelManager.SetTargetBallNum(PlayerDataManager.GetLevelTargetBallNum());
        MainController.Instance.SaveData();
    }
    public void WhenLevelUpAnimationEnd()
    {

    }
    public void WhenLoadingGameEnd()
    {
        isLoadingEnd = true;
        UIManager.ShowPopPanelByType(UI_Panel.MenuPanel);
        MainController.Instance.LoadSaveData();
        stopGuideGame = false;
        CheckGuideGameAndShow();
    }
    public void WhenGetGfitBall()
    {
        if (!UIManager.PanelWhetherShowAnyone() && WillShowGift <= 0)
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.GiftPanel);
        else
            WillShowGift++;
    }
    public int OpenGoldBallReward_Num = 0;
    public void WhenGetGoldBall()
    {
        OpenGoldBallReward_Num = Random.Range(30, 51);
        if (!UIManager.PanelWhetherShowAnyone() && WillShowOpenGoldBall <= 0)
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.OpenGoldBallPanel);
        else
            WillShowOpenGoldBall++;
    }
    public void WhenGetTicketBall()
    {
        OpenGoldBallReward_Num = -Random.Range(20, 31);
        if (!UIManager.PanelWhetherShowAnyone() && WillShowOpenGoldBall <= 0)
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.OpenGoldBallPanel);
        else
            WillShowOpenGoldBall++;
    }
    public void ShowNextPanel()
    {
        if (WillShowGift > 0)
        {
            WillShowGift--;
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.GiftPanel);
        }
        else if (WillShowOpenGoldBall > 0)
        {
            WillShowOpenGoldBall--;
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.OpenGoldBallPanel);
        }
        else if (WillShowSlots > 0)
        {
            WillShowSlots--;
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.SlotsPanel);
        }
    }
    public void ClearUnShowPanel()
    {
        WillShowGift = 0;
        WillShowOpenGoldBall = 0;
        WillShowSlots = 0;
    }
    public void SpawnAGiftBall()
    {
        MainController.Instance.SpawnNewGiftBall();
    }
    public bool GetWhetherFirstPlay()
    {
        return PlayerDataManager.GetWhetherFirstPlay();
    }
    public void SetFirstPlayFalse()
    {
        PlayerDataManager.SetFirstPlayFalse();
    }
    public bool GetWhetherRateus()
    {
        return PlayerDataManager.GetWhetherRateus();
    }
    public void SetHasRateus()
    {
        PlayerDataManager.SetHasRateus();
    }
    public bool GetHasGetFreeGift()
    {
        return PlayerDataManager.playerData.hasGetFreeGift;
    }
    public void SetHasGetFreeGift()
    {
        PlayerDataManager.playerData.hasGetFreeGift = true;
    }
    public int GetCurrentEnergy()
    {
        return PlayerDataManager.playerData.energy;
    }
    public int AddEnergy(int value)
    {
        if (value > 0)
        {
            if (PlayerDataManager.playerData.energy < maxEnergy)
            {
                PlayerDataManager.playerData.energy += value;
                PlayerDataManager.playerData.energy = Mathf.Clamp(PlayerDataManager.playerData.energy, 0, maxEnergy);
            }
        }
        else
            PlayerDataManager.playerData.energy += value;
        if (value == 1)
            PlayerDataManager.playerData.lastGetNaturalEnergyTime = System.DateTime.Now.ToString();
        MainController.Instance.RefreshEnergyText();
        if (HiSpin.UI.MenuPanel != null)
            HiSpin.UI.MenuPanel.UpdateEnergyNumText();
        return PlayerDataManager.playerData.energy;
    }
    public void AddBuyEnergyTime(int value = 1)
    {
        PlayerDataManager.playerData.todayBuyEnergyTime += value;
        PlayerDataManager.playerData.lastBuyEnergyTime = System.DateTime.Now.ToString();
    }
    public bool CheckHasBuyEnergyTime()
    {
        return PlayerDataManager.playerData.todayBuyEnergyTime < buyEnergyMaxTimePerDay;
    }
    public void AddGiftBallAppearTime(int num = 1)
    {
        PlayerDataManager.playerData.logGiftBallAppearTime += num;
    }
    public void AddOpenGiftBallNum(int num = 1)
    {
        PlayerDataManager.playerData.logOpenGiftBallTime += num;
    }
    public void AddSpinWheelTime(int num = 1)
    {
        PlayerDataManager.playerData.logSpinWheelTime += num;
    }
    public void AddSpinSlotsTime(int num = 1)
    {
        PlayerDataManager.playerData.logSpinSlotsTime += num;
    }

    public RectTransform hand;
    private bool stopGuideGame = false;
    private void CheckGuideGameAndShow()
    {
        if (PlayerDataManager.playerData.hasGuideGame) return;
        StartCoroutine("ShakeGuidegameHand");
    }
    public void StopGuideGame()
    {
        stopGuideGame = true;
        PlayerDataManager.playerData.hasGuideGame = true;
    }
    IEnumerator ShakeGuidegameHand()
    {
        hand.gameObject.SetActive(true);
        bool isRight = false;
        float leftX = -144;
        float rightX = 144;
        float y = hand.localPosition.y;
        hand.localPosition = new Vector3(leftX, y, 0);
        while (!stopGuideGame)
        {
            yield return null;
            float offset = Time.deltaTime*20;
            hand.Translate(new Vector3(isRight ? -offset : offset, 0, 0));
            if (hand.localPosition.x >= rightX)
                isRight = true;
            else if (hand.localPosition.x <= leftX)
                isRight = false;
        }
        hand.gameObject.SetActive(false);
    }
    public bool CheckWhetherGuideHowtoplay()
    {
        return PlayerDataManager.playerData.hasGuideHowtoplay;
    }
    public void SetHasGuideHowtoplay()
    {
        PlayerDataManager.playerData.hasGuideHowtoplay = true;
    }


    public bool GetMusicOn()
    {
        return PlayerDataManager.playerData.musicOn;
    }
    public void SetSaveMusicState(bool isOn)
    {
        PlayerDataManager.playerData.musicOn = isOn;
        AudioManager.SetMusicState(isOn);
    }
    public bool GetSoundOn()
    {
        return PlayerDataManager.playerData.soundOn;
    }
    public void SetSaveSoundState(bool isOn)
    {
        PlayerDataManager.playerData.soundOn = isOn;
        AudioManager.SetSoundState(isOn);
    }
    public void PlayButtonClickSound()
    {
        AudioManager.PlayOneShot(AudioPlayArea.Button);
    }
    public AudioSource PlaySpinSound()
    {
        return AudioManager.PlayLoop(AudioPlayArea.Spin);
    }
    public void PlayMergeBallCombeSound(int combe)
    {
        switch (combe)
        {
            case 1:
                AudioManager.PlayOneShot(AudioPlayArea.Combo1);
                break;
            case 2:
                AudioManager.PlayOneShot(AudioPlayArea.Combo2);
                break;
            case 3:
                AudioManager.PlayOneShot(AudioPlayArea.Combo3);
                break;
            default:
                AudioManager.PlayOneShot(AudioPlayArea.Combo3);
                break;
        }
    }
    public void PlayFlyOverSound()
    {
        AudioManager.PlayOneShot(AudioPlayArea.FlyOver);
    }
    public void PlayRV(System.Action callback, int clickTime, string des, System.Action failCallback = null)
    {
        Ads._instance.ShowRewardVideo(callback, clickTime, des,failCallback);
    }
    public void PlayIV(string des,System.Action callback = null)
    {
        Ads._instance.ShowInterstialAd(callback, des);
    }

    public void SendAdjustPerTenBallEvent()
    {
        Master.Instance.SendAdjustPerTenBallEvent(PlayerDataManager.playerData.logPerTenBall / 10, MainController.Instance.BallMaxNum);
    }
    public void SendAdjustGameOverEvent(bool passive)
    {
        Master.Instance.SendAdjustGameOverEvent(PlayerDataManager.playerData.logRestartTime, PlayerDataManager.GetLevel(), passive, PlayerDataManager.GetLevelTargetBallNum());
    }
    public void SendAdjustPropChangeEvent(int propID, int propChangeType)
    {
        string value;
        if (propChangeType == 0)
            value = "-1";
        else
            value = "+1";
        Master.Instance.SendAdjustPropChangeEvent(propID, propChangeType, PlayerDataManager.GetLevelTargetBallNum(), value);
    }
    public void SendAdjustSpawnGiftballEvent()
    {
        Master.Instance.SendAdjustSpawnGiftballEvent(PlayerDataManager.playerData.logGiftBallAppearTime, PlayerDataManager.playerData.logOpenGiftBallTime);
    }
    public void SendAdjustSpinWheelEvent()
    {
        Master.Instance.SendAdjustSpinWheelEvent(PlayerDataManager.playerData.logSpinWheelTime);
    }
    public void SendAdjustSpinSlotsEvent()
    {
        Master.Instance.SendAdjustSpinSlotsEvent(PlayerDataManager.playerData.logSpinSlotsTime);
    }
    public void SendAdjustSpawnGoldBallEvent()
    {
        Master.Instance.SendAdjustSpawnGoldBallEvent(PlayerDataManager.playerData.logGoldBallAppearTime, PlayerDataManager.playerData.logGoldBallGetx10Time);
    }
    public void SendAdjustGuideEvent(int step)
    {
        Master.Instance.SendAdjustGuideEvent(step, true);
    }
    public void SendAdjustSpawnTicketBallEvent()
    {
        Master.Instance.SendAdjustSpawnTicketBallEvent(PlayerDataManager.playerData.logTicketBallAppearTime);
    }
}
public enum Reward
{
    Null,
    Prop1,
    Prop2,
    Cash,
    Coin,
    Amazon,
    WheelTicket,
    Energy,
    Ticket
}
