﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HiSpin
{
    public class RankItem : MonoBehaviour
    {
        public Image head_iconImage;
        public Text nameText;
        public Text rankText;
        public Text numText;
        public void Init(int user_head_id, string id, int rank, int token)
        {
            head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + user_head_id);
            nameText.text = id;
            if (rankText != null)
                rankText.text = "No." + (rank > 0 ? rank.ToString() : "25+");
            numText.text = token.GetTokenShowString();
        }
    }
}
