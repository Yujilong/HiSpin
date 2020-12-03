using System;
using System.Collections.Generic;
using UnityEngine;

public class FontContains 
{
    protected Dictionary<string, string> pFonts = new Dictionary<string, string>();
    private static FontContains _instance = null;
   
    public static FontContains getInstance()
    {
        if(FontContains._instance == null)
        {
            FontContains._instance = new FontContains();
            FontContains._instance.Init();
        }
        return FontContains._instance;
    }
  
    public string GetString(string key, params object[] args)
    {
        try
        {
            return String.Format(this.GetFormatStr(key), args);
        }
        catch
        {
            Debug.LogError(key);
            Debug.LogError(args);
        }
        return "";
    }

    public string GetFormatStr(string key)
    {
        string value = "";
        if (this.pFonts.TryGetValue(key, out value) && value != "")
        {
            return value ;
        }
        Debug.LogWarning(String.Format("{0} is empty", key));
        return "404: " + key;
    }


    private void Init()
    {
        string sysLanguage =this.GetLanguageShortName(Application.systemLanguage);
        try
        {
            TextAsset text = Resources.Load<TextAsset>("Json/Font");
            Dictionary<string, Dictionary<string, string>> fonts = LitJson.JsonMapper.ToObject<Dictionary<string, Dictionary<string, string>>>(new LitJson.JsonReader(text.text));
            if (fonts.ContainsKey(sysLanguage))
            {
                this.pFonts = fonts[sysLanguage];
            }
            else
            {
                this.pFonts = fonts["ru"];
            }
        }
        catch
        {
            this.pFonts = new Dictionary<string, string>();
        }
    }
    private string GetLanguageShortName(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.English:
                return "en";
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                return "cn";
            case SystemLanguage.Russian:
                return "ru";
        }
        return "en";
    }
}
