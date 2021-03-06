﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendInviteRecordItem : MonoBehaviour
{
    public Image head_iconImage;
    public Image starImage;
    public Text nameText;
    public Text idText;
    public Text reward_pt_numText;
    public void Init(int head_icon_id, string name, int rewardPtNum, int distance)
    {
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_icon_id);
        nameText.text = name;
        reward_pt_numText.text = FontContains.getInstance().GetString("lang0145", rewardPtNum.GetTokenShowString());
        starImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Friend, distance == 1 ? "direct_friend" : "indirect_friend");
        starImage.SetNativeSize();
    }
}
