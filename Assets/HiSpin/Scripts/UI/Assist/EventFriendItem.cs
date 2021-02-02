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
        public GameObject coin1Go;
        public Text expectText;
        public GameObject coin2Go;
        public void Init(int head_id,string name,int current,int expect)
        {
            headImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_id);
            nameText.text = name;
            currentText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), current.GetCashShowString());
            expectText.text = string.Format(Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Dollar), expect.GetCashShowString());
            bool isPackB = Save.data.isPackB;
            coin1Go.SetActive(isPackB);
            coin2Go.SetActive(isPackB);
            currentText.transform.localPosition = new Vector3(isPackB ? 25.63f : 0, 2.05f);
            expectText.transform.localPosition = new Vector3(isPackB ? 25.63f : 0, 2.05f);
        }
    }
}
