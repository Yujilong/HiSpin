using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class I18nFont : MonoBehaviour
{   
    public string eTid = null;
    public string[] args;
    private Text eText = null;
    private void Awake()
    {
        this.eText = this.gameObject.GetComponent<Text>();
        if (this.eTid != null) FontContains.getInstance().GetString(this.eTid, this.args);
    }
}
