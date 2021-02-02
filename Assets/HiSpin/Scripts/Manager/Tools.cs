using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace HiSpin
{
    public class Tools
    {
        public static List<string> GetTextFlexiableContent(Text text, string content)
        {
            Font myFont = text.font;
            myFont.RequestCharactersInTexture(content, text.fontSize, text.fontStyle);

            List<string> valuePerModule = new List<string>();
            int contentLength = content.Length;
            string matchString = "\n";
            int startIndex = 0;
            while (startIndex < contentLength)
            {
                int nextModuleIndex = content.IndexOf(matchString, startIndex);
                if (nextModuleIndex < 0)
                    nextModuleIndex = contentLength;
                valuePerModule.Add(content.Substring(startIndex, nextModuleIndex - startIndex));
                startIndex = nextModuleIndex + 1;
            }
            return valuePerModule;
        }
        public static List<string> GetTextMiddleCenterContent(Text text, List<string> content)
        {
            float maxSizeX = text.GetComponent<RectTransform>().rect.width;
            Font myFont = text.font;
            float height = myFont.lineHeight * text.lineSpacing;
            int textSize = text.fontSize;
            int maxLength = 0;
            CharacterInfo characterInfo;
            int strCount = content.Count;
            List<int> totalLengths = new List<int>();
            for (int i = 0; i < strCount; i++)
            {
                myFont.RequestCharactersInTexture(content[i], text.fontSize, text.fontStyle);
                char[] charArr = content[i].ToCharArray();
                int totalCharWidth = 0;
                foreach (char ch in charArr)
                {
                    myFont.GetCharacterInfo(ch, out characterInfo, textSize);
                    totalCharWidth += characterInfo.advance;
                }
                totalLengths.Add(totalCharWidth);
                if (totalCharWidth > maxLength)
                    maxLength = totalCharWidth;
            }
            myFont.GetCharacterInfo(' ', out characterInfo, textSize);
            int spaceLength = characterInfo.advance;
            maxLength += spaceLength * 4;
            for (int i = 0; i < strCount; i++)
            {
                float offsetWidth = maxLength - totalLengths[i];
                int addSpaceCount = Mathf.CeilToInt(offsetWidth / spaceLength);
                int insertIndex;
                if (Language_M.isJapanese)
                {
                    insertIndex = content[i].LastIndexOf("の") + 1;
                    if (insertIndex == 0)
                        insertIndex = content[i].LastIndexOf("位") + 1;
                }
                else
                    insertIndex = content[i].IndexOf("$");
                insertIndex = Mathf.Clamp(insertIndex, 0, content[i].Length - 1);
                for (int j = 0; j < addSpaceCount; j++)
                    if (j < 4 + (j - 4) / 2)
                        content[i] = content[i].Insert(insertIndex, " ");
                    else
                        content[i] = content[i].Insert(0, " ");
            }
            return content;
        }
        public static string GetTaskDesMultiLanguage(PlayerTaskTarget taskTarget,int task_tar)
        {
            string result;
            switch (taskTarget)
            {
                case PlayerTaskTarget.EnterSlotsOnce:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_EnterSlotsOnce), task_tar);
                    break;
                case PlayerTaskTarget.PlayBettingOnce:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_PlayBettingOnce), task_tar);
                    break;
                case PlayerTaskTarget.WatchRvOnce:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_WatchRvOnce), task_tar);
                    break;
                case PlayerTaskTarget.CashoutOnce:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_CashoutOnce), task_tar);
                    break;
                case PlayerTaskTarget.WritePaypalEmail:
                    result = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_WritePaypalEmail);
                    break;
                case PlayerTaskTarget.OwnSomeGold:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_OwnSomeGold), task_tar);
                    break;
                case PlayerTaskTarget.WinnerOnce:
                    result = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_WinnerOnce);
                    break;
                case PlayerTaskTarget.InviteAFriend:
                    result = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.InviteFriends);
                    break;
                case PlayerTaskTarget.GetTicketFromSlotsOnce:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_GetTicketFromSlotsOnce), task_tar);
                    break;
                case PlayerTaskTarget.BuyTicketByGoldOnce:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_BuyTicketByGoldOnce), Save.data.allData.lucky_schedule.coin_ticket);
                    break;
                case PlayerTaskTarget.BuyTicketByRvOnce:
                    result = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_BuyTicketByRvOnce);
                    break;
                case PlayerTaskTarget.OwnSomeFriend:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_OwnSomeFriend), task_tar);
                    break;
                case PlayerTaskTarget.OwnFriendAndAllReachLv:
                    result = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Task_Des_OwnFriendAndAllReachLv), task_tar % 100000, task_tar / 100000);
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
        }
        public static void SetTwoUICenterInParent(RectTransform leftRect,RectTransform rightRect)
        {
            float totalWidth = leftRect.sizeDelta.x + rightRect.sizeDelta.x + 10;
            float x = totalWidth / 2;
            rightRect.localPosition = new Vector3(x - rightRect.sizeDelta.x / 2, rightRect.localPosition.y);
            leftRect.localPosition = new Vector3(-(x - leftRect.sizeDelta.x / 2), leftRect.localPosition.y);
        }
        public static List<string> TextToSameLengthByFillSpaceAsBehind(Text text, List<string> content)
        {
            Font myFont = text.font;
            int textSize = text.fontSize;
            int maxLength = 0;
            CharacterInfo characterInfo;
            int strCount = content.Count;
            List<int> totalLengths = new List<int>();
            for (int i = 0; i < strCount; i++)
            {
                myFont.RequestCharactersInTexture(content[i], text.fontSize, text.fontStyle);
                char[] charArr = content[i].ToCharArray();
                int totalCharWidth = 0;
                foreach (char ch in charArr)
                {
                    myFont.GetCharacterInfo(ch, out characterInfo, textSize);
                    totalCharWidth += characterInfo.advance;
                }
                totalLengths.Add(totalCharWidth);
                if (totalCharWidth > maxLength)
                    maxLength = totalCharWidth;
            }
            myFont.GetCharacterInfo(' ', out characterInfo, textSize);
            int spaceLength = characterInfo.advance;
            for (int i = 0; i < strCount; i++)
            {
                float offsetWidth = maxLength - totalLengths[i];
                int addSpaceCount = Mathf.CeilToInt(offsetWidth / spaceLength);
                for (int j = 0; j < addSpaceCount; j++)
                    content[i] = content[i] + " ";
            }
            return content;
        }
    }
    public class Range
    {
        private int min;
        private int max;
        public int Min { get { return min; } }
        public int Max { get { return max; } }
        public Range(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
        public int RandomIncludeMax()
        {
            return Random.Range(min, max + 1);
        }
        public int RandomExculdeMax()
        {
            return Random.Range(min, max);
        }
    }
}
