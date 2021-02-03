﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HiSpin
{
    public sealed class UI
    {
        static MonoBehaviour TaskEngine = null;
        static Transform BaseRoot = null;
        static Transform MenuRoot = null;
        static Transform PopRoot = null;
        public UI(MonoBehaviour TaskEngine, Transform BaseRoot, Transform MenuRoot, Transform PopRoot)
        {
            UI.TaskEngine = TaskEngine;
            UI.BaseRoot = BaseRoot;
            UI.MenuRoot = MenuRoot;
            UI.PopRoot = PopRoot;
        }
        const string menuPanelPath = "Prefabs/UI/Menu";
        static readonly Dictionary<int, string> panelPathDic = new Dictionary<int, string>()
    {
        { (int)BasePanel.Offerwall,"Prefabs/UI/Base_Offerwall" },
        { (int)BasePanel.Rank,"Prefabs/UI/Base_Rank" },
        { (int)BasePanel.Friend,"Prefabs/UI/Base_Friend" },
        { (int)BasePanel.Slots,"Prefabs/UI/Base_Slots" },
        { (int)BasePanel.Betting,"Prefabs/UI/Base_Betting" },

        { (int)BasePanel.Cashout_Gold,"Prefabs/UI/Base_Cashout_Gold" },
        { (int)BasePanel.Cashout_Cash,"Prefabs/UI/Base_Cashout_Cash" },
        { (int)BasePanel.CashoutRecord,"Prefabs/UI/Base_CashoutRecord" },
        { (int)BasePanel.Task,"Prefabs/UI/Base_Task&Achievement" },
        { (int)BasePanel.PlaySlots,"Prefabs/UI/Base_PlaySlots" },
        { (int)BasePanel.Me,"Prefabs/UI/Base_Me" },
        { (int)BasePanel.FriendList,"Prefabs/UI/Base_FriendList" },
        { (int)BasePanel.FriendEvent,"Prefabs/UI/Base_FriendEvent" },

        { (int)PopPanel.Loading,"Prefabs/UI/Pop_Loading" },
        { (int)PopPanel.Setting,"Prefabs/UI/Pop_Setting" },
        { (int)PopPanel.GetReward,"Prefabs/UI/Pop_GetReward" },
        { (int)PopPanel.Rules,"Prefabs/UI/Pop_Rules" },
        { (int)PopPanel.CashoutPop,"Prefabs/UI/Pop_AsCashout" },
        { (int)PopPanel.InviteOk,"Prefabs/UI/Pop_InviteOk" },
        { (int)PopPanel.StartBetting,"Prefabs/UI/Pop_StartBetting" },
        { (int)PopPanel.GetCash,"Prefabs/UI/Pop_GetCash" },
        { (int)PopPanel.Guide,"Prefabs/UI/Pop_Guide" },
        { (int)PopPanel.InputPaypalEmail,"Prefabs/UI/Pop_InputPaypalEmail" },
        { (int)PopPanel.GetNewPlayerReward,"Prefabs/UI/Pop_NewPlayerReward" },
        { (int)PopPanel.Sign,"Prefabs/UI/Pop_Sign" },
        { (int)PopPanel.SignTasks,"Prefabs/UI/Pop_SignTask" },
        { (int)PopPanel.InputInviteCode,"Prefabs/UI/Pop_InputInviteCode" },
        { (int)PopPanel.QuitPlaySlots,"Prefabs/UI/Pop_QuitPlaySlots" },
        { (int)PopPanel.EnterCashoutTask,"Prefabs/UI/Pop_EnterCashoutTask" },
    };
        //除菜单外所有面板已经加载的资源
        static readonly Dictionary<int, GameObject> loadedpanelPrefabDic = new Dictionary<int, GameObject>();
        //除菜单外所有面板的动作
        static readonly Dictionary<int, IUIBase> allPanelDic = new Dictionary<int, IUIBase>();
        //除菜单外所有面板的实例物体
        static readonly Dictionary<int, GameObject> allPanelGoDic = new Dictionary<int, GameObject>();
        //根据优先度排序的弹窗任务
        static readonly List<PopTask> allPopTaskOrderList = new List<PopTask>()
    {
        new PopTask() { panelType = PopPanel.Loading, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.Setting, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.GetReward, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.Rules, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.CashoutPop, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.InviteOk, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.StartBetting, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.GetCash, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.Guide, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.InputPaypalEmail, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.GetNewPlayerReward, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.Sign, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.SignTasks, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.InputInviteCode, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.QuitPlaySlots, taskQueue = new Queue<int[]>() },
        new PopTask() { panelType = PopPanel.EnterCashoutTask, taskQueue = new Queue<int[]>() },
    };
        class PopTask
        {
            public PopPanel panelType;
            public Queue<int[]> taskQueue = new Queue<int[]>();
        }
        //底层面板的动画队列
        static readonly Queue<int> BasePanelTasks = new Queue<int>();
        static readonly Queue<int[]> BasePanelTaskParams = new Queue<int[]>();
        //底层面板的历史记录
        static readonly Stack<int> BasePanelHistoryRecord = new Stack<int>();
        static readonly Stack<int[]> BasePanelHistoryRecordArgs = new Stack<int[]>();
        public static IUIBase CurrentBasePanel = null;
        public static IUIBase CurrentRulePanel = null;
        static IUIBase CurrentPopPanel = null;
        public static Menu MenuPanel = null;
        static GameObject MenuObj = null;
        public static void ShowMenuPanel()
        {
            GameObject menuPrefab = Resources.Load<GameObject>(menuPanelPath);
            MenuObj = GameObject.Instantiate(menuPrefab, MenuRoot);
            MenuPanel = MenuObj.GetComponent<Menu>();
            MenuPanel.SetContent();
            TaskEngine.StartCoroutine(MenuPanel.Show());
        }
        static Coroutine _baseAnimationCor = null;
        public static void ShowBasePanel(BasePanel basePanel, params int[] args)
        {
            int panelIndex = (int)basePanel;
            ShowBasePanel(panelIndex, args);
        }
        private static void ShowBasePanel(int panelIndex, params int[] args)
        {
            if (panelIndex == (int)BasePanel.Cashout_Cash)
                if (Save.data.allData.pre_cash_task && !Save.data.hasUnlockCashout)
                {
                    ShowPopPanel(PopPanel.EnterCashoutTask);
                    return;
                }
            if (BasePanelHistoryRecord.Count > 0)
            {
                if (BasePanelHistoryRecord.Peek() == panelIndex)
                {
                    Debug.LogWarning("已经显示当前面板，面板类型ID：" + panelIndex);
                    return;
                }
            }
            BasePanelTasks.Enqueue(panelIndex);
            BasePanelTaskParams.Enqueue(args);
            if (panelIndex <= (int)BasePanel.Betting)
            {
                BasePanelHistoryRecord.Clear();
                BasePanelHistoryRecordArgs.Clear();
            }
            BasePanelHistoryRecord.Push(panelIndex);
            BasePanelHistoryRecordArgs.Push(args);
            if (_baseAnimationCor is null)
                _baseAnimationCor = TaskEngine.StartCoroutine(ExcuteBasePanelShowTask());
        }
        public static void CloseCurrentBasePanel(bool isPhoneBack = false, bool isForce = false)
        {
            if (!isForce)
                if (CurrentPopPanel != null) return;
            int panelIndex = BasePanelHistoryRecord.Peek();
            if (isPhoneBack)
                if (panelIndex == (int)BasePanel.PlaySlots)
                {
                    //ShowPopPanel(PopPanel.QuitPlaySlots);
                    return;
                }
            if (panelIndex > (int)BasePanel.Betting)
            {
                BasePanelHistoryRecord.Pop();
                BasePanelHistoryRecordArgs.Pop();
                int lastPanelIndex = BasePanelHistoryRecord.Pop();
                int[] lastPanelArgs = BasePanelHistoryRecordArgs.Pop();
                ShowBasePanel(lastPanelIndex, lastPanelArgs);
            }
            else
                Debug.LogError("关闭了错误的底层面板，面板ID：" + panelIndex);
        }
        static Coroutine _popAnimationCor = null;
        static Coroutine _ruleAnimationCor = null;
        public static void ShowPopPanel(PopPanel popPanel, params int[] args)
        {
            bool hasAddTask = false;
            foreach (var task in allPopTaskOrderList)
            {
                if (task.panelType == popPanel)
                {
                    if (popPanel == PopPanel.QuitPlaySlots)
                        if (CheckCurrentPopPanelIs(PopPanel.QuitPlaySlots))
                            return;
                    task.taskQueue.Enqueue(args);
                    hasAddTask = true;
                    break;
                }
            }
            if (!hasAddTask)
            {
                Debug.LogError("没有配置面板的优先级，面板类型：" + popPanel);
                return;
            }
            if (popPanel == PopPanel.Rules)
            {
                if (_ruleAnimationCor is null)
                    _ruleAnimationCor = TaskEngine.StartCoroutine(ExcutePopRulesPanelShowTask());
            }
            else
            {
                if (_popAnimationCor is null)
                    _popAnimationCor = TaskEngine.StartCoroutine(ExcutePopPanelShowTaskExceptRules());
            }
        }
        static Coroutine _popCloseCor = null;
        static Coroutine _popRuleCloseCor = null;
        public static void ClosePopPanel(IUIBase panel)
        {
            if (panel == CurrentPopPanel && _popCloseCor == null)
                _popCloseCor = TaskEngine.StartCoroutine(ExcutePopPanelCloseTaskExceptRule());
            else if(panel==CurrentRulePanel&&_popRuleCloseCor==null)
                _popRuleCloseCor = TaskEngine.StartCoroutine(ExcutePopRulesPanelCloseTask());
        }
        private static IEnumerator ExcuteBasePanelShowTask()
        {
            while (BasePanelTasks.Count > 0)
            {
                int panelIndex = BasePanelTasks.Dequeue();
                int[] args = BasePanelTaskParams.Dequeue();
                if (allPanelDic.TryGetValue(panelIndex, out IUIBase tempUI))
                {
                    if (tempUI is null)
                    {
                        Debug.LogError("保存的接口为空，面板类型ID：" + panelIndex);
                        allPanelDic.Remove(panelIndex);
                        continue;
                    }
                    if (CurrentBasePanel is object)
                    {
                        if (tempUI == CurrentBasePanel)
                        {
                            Debug.LogError("已经显示当前面板，面板类型ID：" + panelIndex);
                            continue;
                        }
                        yield return CurrentBasePanel.Close();
                    }
                    CurrentBasePanel = tempUI;
                    MenuPanel.OnBasePanelShow(panelIndex);
                    CurrentBasePanel.SetContent();
                    yield return CurrentBasePanel.Show(args);
                }
                else
                {
                    if (loadedpanelPrefabDic.TryGetValue(panelIndex, out GameObject tempPrefab))
                    {
                        if (tempPrefab is null)
                        {
                            Debug.LogError("加载预制体的资源为空，面板类型ID：" + panelIndex);
                            loadedpanelPrefabDic.Remove(panelIndex);
                            continue;
                        }
                        GameObject tempUIGo = GameObject.Instantiate(tempPrefab, BaseRoot);
                        tempUI = tempUIGo.GetComponent<IUIBase>();
                        allPanelDic.Add(panelIndex, tempUI);
                        allPanelGoDic.Add(panelIndex, tempUIGo);
                        if (CurrentBasePanel is object)
                        {
                            if (tempUI == CurrentBasePanel)
                            {
                                Debug.LogWarning("已经显示当前面板，面板类型ID：" + panelIndex);
                                continue;
                            }
                            yield return CurrentBasePanel.Close();
                        }
                        CurrentBasePanel = tempUI;
                        MenuPanel.OnBasePanelShow(panelIndex);
                        CurrentBasePanel.SetContent();
                        yield return CurrentBasePanel.Show(args);
                    }
                    else
                    {
                        if (panelPathDic.TryGetValue(panelIndex, out string tempUIPath))
                        {
                            tempPrefab = Resources.Load<GameObject>(tempUIPath);
                            if (tempPrefab is null)
                            {
                                Debug.LogError("加载预制体的资源为空，面板类型ID：" + panelIndex);
                                loadedpanelPrefabDic.Remove(panelIndex);
                                continue;
                            }
                            loadedpanelPrefabDic.Add(panelIndex, tempPrefab);

                            GameObject tempUIGo = GameObject.Instantiate(tempPrefab, BaseRoot);
                            tempUI = tempUIGo.GetComponent<IUIBase>();
                            allPanelDic.Add(panelIndex, tempUI);
                            allPanelGoDic.Add(panelIndex, tempUIGo);
                            if (CurrentBasePanel is object)
                            {
                                if (tempUI == CurrentBasePanel)
                                {
                                    Debug.LogWarning("已经显示当前面板，面板类型ID：" + panelIndex);
                                    continue;
                                }
                                yield return CurrentBasePanel.Close();
                            }
                            CurrentBasePanel = tempUI;
                            MenuPanel.OnBasePanelShow(panelIndex);
                            CurrentBasePanel.SetContent();
                            yield return CurrentBasePanel.Show(args);
                        }
                        else
                        {
                            Debug.LogError("没有配置面板预制体资源路径，面板类型ID：" + panelIndex);
                        }
                    }
                }
            }
            _baseAnimationCor = null;
        }
        private static IEnumerator ExcutePopPanelShowTaskExceptRules()
        {
            while (true)
            {
                if (CurrentPopPanel is object || CurrentRulePanel is object)
                {
                    yield return null;
                    continue;
                }
                PopTask nextTask = null;
                int popOrderCount = allPopTaskOrderList.Count;
                for (int i = 0; i < popOrderCount; i++)
                {
                    if (allPopTaskOrderList[i].panelType == PopPanel.Rules)
                        continue;
                    if (allPopTaskOrderList[i].taskQueue.Count > 0)
                    {
                        nextTask = allPopTaskOrderList[i];
                        break;
                    }
                }
                if (nextTask is null)
                {
                    if (CurrentBasePanel != null)
                        CurrentBasePanel.Resume();
                    if (MenuPanel != null)
                        MenuPanel.Resume();
                    yield return null;
                    continue;
                }

                int panelIndex = (int)nextTask.panelType;
                int[] args = nextTask.taskQueue.Dequeue();
                if (allPanelDic.TryGetValue(panelIndex, out IUIBase tempUI))
                {
                    if (tempUI is null)
                    {
                        Debug.LogError("保存的接口为空，面板类型ID：" + panelIndex);
                        allPanelDic.Remove(panelIndex);
                        continue;
                    }
                    CurrentPopPanel = tempUI;
                    CurrentBasePanel.Pause();
                    MenuPanel.Pause();
                    CurrentPopPanel.SetContent();
                    yield return CurrentPopPanel.Show(args);
                }
                else
                {
                    if (loadedpanelPrefabDic.TryGetValue(panelIndex, out GameObject tempPrefab))
                    {
                        if (tempPrefab is null)
                        {
                            Debug.LogError("加载预制体的资源为空，面板类型ID：" + panelIndex);
                            loadedpanelPrefabDic.Remove(panelIndex);
                            continue;
                        }
                        GameObject tempUIGo = GameObject.Instantiate(tempPrefab, PopRoot);
                        tempUI = tempUIGo.GetComponent<IUIBase>();
                        allPanelDic.Add(panelIndex, tempUI);
                        allPanelGoDic.Add(panelIndex, tempUIGo);
                        CurrentPopPanel = tempUI;
                        CurrentBasePanel.Pause();
                        MenuPanel.Pause();
                        CurrentPopPanel.SetContent();
                        yield return CurrentPopPanel.Show(args);
                    }
                    else
                    {
                        if (panelPathDic.TryGetValue(panelIndex, out string tempUIPath))
                        {
                            tempPrefab = Resources.Load<GameObject>(tempUIPath);
                            if (tempPrefab is null)
                            {
                                Debug.LogError("加载预制体的资源为空，面板类型ID：" + panelIndex);
                                loadedpanelPrefabDic.Remove(panelIndex);
                                continue;
                            }
                            loadedpanelPrefabDic.Add(panelIndex, tempPrefab);

                            GameObject tempUIGo = GameObject.Instantiate(tempPrefab, PopRoot);
                            tempUI = tempUIGo.GetComponent<IUIBase>();
                            allPanelDic.Add(panelIndex, tempUI);
                            allPanelGoDic.Add(panelIndex, tempUIGo);
                            CurrentPopPanel = tempUI;
                            if (CurrentBasePanel is object)
                                CurrentBasePanel.Pause();
                            if (MenuPanel is object)
                                MenuPanel.Pause();
                            CurrentPopPanel.SetContent();
                            yield return CurrentPopPanel.Show(args);
                        }
                        else
                        {
                            Debug.LogError("没有配置面板预制体资源路径，面板类型ID：" + panelIndex);
                        }
                    }
                }
            }
        }
        private static IEnumerator ExcutePopRulesPanelShowTask()
        {
            while (true)
            {
                if (CurrentRulePanel is object)
                {
                    yield return null;
                    continue;
                }
                PopTask nextTask = null;
                int popOrderCount = allPopTaskOrderList.Count;
                for (int i = 0; i < popOrderCount; i++)
                {
                    if (allPopTaskOrderList[i].panelType == PopPanel.Rules)
                    {
                        if (allPopTaskOrderList[i].taskQueue.Count > 0)
                            nextTask = allPopTaskOrderList[i];
                        break;
                    }
                }
                if (nextTask is null)
                {
                    if (CurrentBasePanel != null)
                        CurrentBasePanel.Resume();
                    if (MenuPanel != null)
                        MenuPanel.Resume();
                    yield return null;
                    continue;
                }

                int panelIndex = (int)nextTask.panelType;
                int[] args = nextTask.taskQueue.Dequeue();
                if (allPanelDic.TryGetValue(panelIndex, out IUIBase tempUI))
                {
                    if (tempUI is null)
                    {
                        Debug.LogError("保存的接口为空，面板类型ID：" + panelIndex);
                        allPanelDic.Remove(panelIndex);
                        continue;
                    }
                    CurrentRulePanel = tempUI;
                    CurrentBasePanel.Pause();
                    MenuPanel.Pause();
                    CurrentRulePanel.SetContent();
                    yield return CurrentRulePanel.Show(args);
                }
                else
                {
                    if (loadedpanelPrefabDic.TryGetValue(panelIndex, out GameObject tempPrefab))
                    {
                        if (tempPrefab is null)
                        {
                            Debug.LogError("加载预制体的资源为空，面板类型ID：" + panelIndex);
                            loadedpanelPrefabDic.Remove(panelIndex);
                            continue;
                        }
                        GameObject tempUIGo = GameObject.Instantiate(tempPrefab, PopRoot);
                        tempUI = tempUIGo.GetComponent<IUIBase>();
                        allPanelDic.Add(panelIndex, tempUI);
                        allPanelGoDic.Add(panelIndex, tempUIGo);
                        CurrentRulePanel = tempUI;
                        CurrentBasePanel.Pause();
                        MenuPanel.Pause();
                        CurrentRulePanel.SetContent();
                        yield return CurrentRulePanel.Show(args);
                    }
                    else
                    {
                        if (panelPathDic.TryGetValue(panelIndex, out string tempUIPath))
                        {
                            tempPrefab = Resources.Load<GameObject>(tempUIPath);
                            if (tempPrefab is null)
                            {
                                Debug.LogError("加载预制体的资源为空，面板类型ID：" + panelIndex);
                                loadedpanelPrefabDic.Remove(panelIndex);
                                continue;
                            }
                            loadedpanelPrefabDic.Add(panelIndex, tempPrefab);

                            GameObject tempUIGo = GameObject.Instantiate(tempPrefab, PopRoot);
                            tempUI = tempUIGo.GetComponent<IUIBase>();
                            allPanelDic.Add(panelIndex, tempUI);
                            allPanelGoDic.Add(panelIndex, tempUIGo);
                            CurrentRulePanel = tempUI;
                            if (CurrentBasePanel is object)
                                CurrentBasePanel.Pause();
                            if (MenuPanel is object)
                                MenuPanel.Pause();
                            CurrentRulePanel.SetContent();
                            yield return CurrentRulePanel.Show(args);
                        }
                        else
                        {
                            Debug.LogError("没有配置面板预制体资源路径，面板类型ID：" + panelIndex);
                        }
                    }
                }
            }
        }
        private static IEnumerator ExcutePopPanelCloseTaskExceptRule()
        {
            yield return CurrentPopPanel.Close();
            CurrentPopPanel = null;
            _popCloseCor = null;
        }
        private static IEnumerator ExcutePopRulesPanelCloseTask()
        {
            yield return CurrentRulePanel.Close();
            CurrentRulePanel = null;
            _popRuleCloseCor = null;
        }
        public static void FlyReward(Reward type, int num, Vector3 startWorldPos)
        {
            MenuPanel.FlyReward_GetTargetPosAndCallback_ThenFly(type, num, startWorldPos);
        }
        private static IUIBase GetUI(int panelIndex)
        {
            allPanelDic.TryGetValue(panelIndex, out IUIBase ui);
            return ui;
        }
        public static IUIBase GetUI(BasePanel basePanel)
        {
            return GetUI((int)basePanel);
        }
        public static IUIBase GetUI(PopPanel popPanel)
        {
            return GetUI((int)popPanel);
        }
        public static void OnHasTaskFinished(bool hasFinished)
        {
            MenuPanel.OnTaskFinishChange(hasFinished);
            Setting setting = GetUI(PopPanel.Setting) as Setting;
            if (setting != null)
                setting.OnTaskFinishChange(hasFinished);
        }
        public static bool CheckCurrentPopPanelIs(PopPanel popPanel)
        {
            if (allPanelDic.TryGetValue((int)popPanel, out IUIBase pop))
            {
                return CurrentPopPanel == pop;
            }
            return false;
        }
        public static bool CheckCurrentBasePanelIs(BasePanel basePanel)
        {
            if (allPanelDic.TryGetValue((int)basePanel, out IUIBase pop))
            {
                return CurrentPopPanel == pop;
            }
            return false;
        }
        public static float GetMenuTopHeight()
        {
            return MenuPanel.all_topGo.GetComponent<RectTransform>().sizeDelta.y - 27;
        }
        public static void OnWebView(bool show)
        {
            if (show)
                MenuPanel.OnWebViewShow();
            else
                MenuPanel.OnWebViewHide();
        }
    }
    public enum BasePanel
    {
        Offerwall = 0,
        Rank = 1,
        Slots = 2,
        Friend = 3,
        Betting = 4,

        Cashout_Gold = 5,
        Cashout_Cash = 6,
        CashoutRecord = 7,
        Task = 8,
        PlaySlots = 9,
        Me = 10,
        FriendList = 11,
        FriendEvent = 12,
    }
    public enum PopPanel
    {
        Loading = 27,
        Setting = 13,
        GetReward = 14,
        Rules = 15,
        CashoutPop = 16,
        InviteOk = 17,
        StartBetting = 18,
        GetCash = 19,
        Guide = 20,
        InputPaypalEmail = 21,
        GetNewPlayerReward = 22,
        Sign = 23,
        SignTasks = 24,
        InputInviteCode = 25,
        QuitPlaySlots = 26,
        EnterCashoutTask = 28,
    }
}