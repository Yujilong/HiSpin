using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiSpin
{
    public class EventFriendItem : MonoBehaviour
    {
        public Image headImage;
        public Text nameText;
        public Text currentText;
        public Text expectText;
        public void Init(int head_id,string name,int current,int expect)
        {
            headImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_id);
            nameText.text = name;
            currentText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + current.GetCashShowString();
            expectText.text = Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar) + expect.GetCashShowString();
        }
    }
}
