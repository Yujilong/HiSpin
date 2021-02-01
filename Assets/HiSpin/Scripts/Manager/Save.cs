using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HiSpin
{
    public class Save
    {
        public static PlayerLocalData data;
        public Save()
        {
            string dataString = PlayerPrefs.GetString("local_Data", "");
            if (string.IsNullOrEmpty(dataString))
            {
                data = new PlayerLocalData()
                {
                    allData = null,
                    sound_on = true,
                    music_on = true,
                    input_eamil_time = 0,
                    hasRateus = false,
                    isPackB = false,
                    head_icon_hasCheck = new List<bool>(),
                    lastClickFriendTime = System.DateTime.Now.AddDays(-1),
                    uuid = string.Empty,
                    hasSendToThoundsEvent = false,
                    hasWatchThreeCardGuide = false,
                    todayHasClickCashBubble = false,
                    lastLoginDate = System.DateTime.Now,
                    totalAdTimes = 0,
                    activeTimes = 0,
                    hasUnlockCashout = false,
                };
                PlayerPrefs.SetString("local_Data", JsonMapper.ToJson(data));
                PlayerPrefs.Save();
            }
            else
                data = JsonMapper.ToObject<PlayerLocalData>(dataString);
            if (data.lastClickFriendTime == null)
                data.lastClickFriendTime = System.DateTime.Now.AddDays(-1);
            System.DateTime now = System.DateTime.Now;
            if (CheckTomorrow(data.lastLoginDate, now))
            {
                data.todayHasClickCashBubble = false;
                data.activeTimes++;
            }
            if (data.activeTimes == 0)
                data.activeTimes = 1;
            data.lastLoginDate = now;
#if UNITY_EDITOR
            data.activeTimes = 9;
#endif
        }
        public static void SaveLocalData()
        {
            PlayerPrefs.SetString("local_Data", JsonMapper.ToJson(data));
            PlayerPrefs.Save();
        }
        public static bool CheckTomorrow(System.DateTime last, System.DateTime now)
        {
            bool isTomorrow = false;
            if (last.Year == now.Year)
            {
                if (last.Month == now.Month)
                {
                    if (last.Day < now.Day)
                        isTomorrow = true;
                }
                else if (last.Month < now.Month)
                    isTomorrow = true;
            }
            else if (last.Year < now.Year)
                isTomorrow = true;
            return isTomorrow;
        }
    }
    public class PlayerLocalData
    {
        public AllData allData;
        public bool sound_on;
        public bool music_on;
        public int input_eamil_time;
        public bool hasRateus;
        public bool isPackB;
        public List<bool> head_icon_hasCheck;
        public System.DateTime lastClickFriendTime;
        public string uuid;
        public string adid;
        public bool hasSendToThoundsEvent;
        public bool hasWatchThreeCardGuide;
        public System.DateTime lastLoginDate;
        public bool todayHasClickCashBubble;
        public int totalAdTimes;
        public int activeTimes;
        public bool hasUnlockCashout;
    }
}
