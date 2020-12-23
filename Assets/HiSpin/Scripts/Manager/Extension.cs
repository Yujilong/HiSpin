using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace HiSpin
{
    public static class Extension
    {
        public static void AddClickEvent(this Button button, UnityAction call)
        {
            button.onClick.AddListener(call);
            button.onClick.AddListener(Master.PlayButtonClickSound);
        }
        public static void RemoveAllClickEvent(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }
        public static void RemoveClickEvent(this Button button, UnityAction call)
        {
            button.onClick.RemoveListener(call);
        }
        public static string GetCashShowString(this int cashNum)
        {
            string str = cashNum.ToString();
            if (str.Length == 1)
                return "0.0" + str;
            else if (str.Length == 2)
                return "0." + str;
            else
            {
                str = str.Insert(str.Length - 2, ".");
                int length = str.Length;
                int pos = 0;
                for (int i = length - 4; i > 0; i--)
                {
                    pos++;
                    if (pos % 3 == 0)
                        str = str.Insert(i, ",");
                }
                return str;
            }
        }
        public static string GetTokenShowString(this int tokenNum)
        {
            string str = tokenNum.ToString();
            int length = str.Length;
            int pos = 0;
            for (int i = length - 1; i > 0; i--)
            {
                pos++;
                if (pos % 3 == 0)
                    str = str.Insert(i, ",");
            }
            return str;
        }
        public static string GetTicketMultipleString(this int multiple)
        {
            string mulStr = multiple.ToString();
            if (mulStr.Length == 1)
                return "0." + mulStr;
            else
                return mulStr.Insert(mulStr.Length - 1, ".");
        }
        public static string CheckName(this string str)
        {
            if (str == null || str.Length == 0) { return ""; }

            StringBuilder @string = new StringBuilder();
            int charCount = 0;
            int stringIndex = 0;
            int stringLength = str.Length;
            while (stringIndex < stringLength)
            {
                @string.Append(str[stringIndex]);
                charCount++;
                if (str[stringIndex] > 128)
                    charCount++;
                if (charCount >= 16)
                    break;
                stringIndex++;
            }
            return @string.ToString();
        }
        public static string[] ToStringArray(this Reward[] rewards)
        {
            if (rewards == null)
                return null;
            int length = rewards.Length;
            string[] result = new string[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = rewards[i].ToString();
            }
            return result;
        }
        public static string GetBigTokenString(this int num)
        {
            if (num < 1000)
                return num.ToString();
            string numStr = num.ToString();
            int frontNum = numStr.Length % 3;
            if (frontNum == 0)
            {
                frontNum = 3;
            }
            return numStr.Substring(0, frontNum + 1).Insert(frontNum, ".") + GetNumAbb((numStr.Length - 1) / 3);
        }
        private static string GetNumAbb(int numDevideThree)
        {
            switch (numDevideThree)
            {
                case 1:
                    return "K";
                case 2:
                    return "M";
                case 3:
                    return "B";
                case 4:
                    return "T";
                case 5:
                    return "Q";
                case 6:
                    return "S";
                case 7:
                    return "O";
                case 8:
                    return "N";
                default:
                    return "E+" + numDevideThree * 3;
            }
        }
        public static string TotalSecondsTo24hTime(this int leftSeconds)
        {
            int second = leftSeconds % 60;
            int minute = leftSeconds % 3600 / 60;
            int hour = leftSeconds / 3600;
            string time = (hour < 10 ? "0" + hour : hour.ToString()) + ":" + (minute < 10 ? "0" + minute : minute.ToString()) + ":" + (second < 10 ? "0" + second : second.ToString());
            return time;
        }
    }
}
