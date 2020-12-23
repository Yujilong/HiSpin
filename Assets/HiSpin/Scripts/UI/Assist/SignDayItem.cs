using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class SignDayItem : MonoBehaviour
    {
        public Image bgImage;
        public Image reward_iconImage;
        public Text day_numText;
        public Image has_signImage;
        public void Init(int day,int today, Reward rewardType,bool hasSign)
        {
            bgImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Sign, day == today ? "day_today" : "day_normal");
            reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Sign, "reward_" + rewardType);
            day_numText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Sign_Day), day);
            if (day < today || (day == today && hasSign))
            {
                has_signImage.gameObject.SetActive(true);
                has_signImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Sign, hasSign ? "yes" : "no");
            }
            else
                has_signImage.gameObject.SetActive(false);
        }
    }
}
