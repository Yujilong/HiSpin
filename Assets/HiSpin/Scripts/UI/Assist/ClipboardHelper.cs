using UnityEngine;

public class ClipboardHelper
{
    public static void Copy(string text)
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        //For Android
        //For Android
        AndroidJavaObject javaClipboardHelper = new AndroidJavaObject("com.game.ClipboardHelper");     
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        if (activity != null)
        {
            javaClipboardHelper.Call("copyTextToClipboard", activity, text);
        }

#elif UNITY_IOS && !UNITY_EDITOR

        //For iOS
        _copyTextToClipboard(text);

#else

        //方法一
        var textEditor = new TextEditor();
        textEditor.text = text;
        textEditor.OnFocus();
        textEditor.SelectAll();
        textEditor.Copy();
        //方法二
        GUIUtility.systemCopyBuffer = text;

#endif
        HiSpin.Master.Instance.ShowTip(HiSpin.Language_M.GetMultiLanguageByArea(LanguageAreaEnum.Tips_CopySuccess));
    }

#if UNITY_IOS
    [DllImport ("__Internal")]
    private static extern void _copyTextToClipboard(string text);
#endif
}
