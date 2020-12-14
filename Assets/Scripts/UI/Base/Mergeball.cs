using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
namespace HiSpin
{
    public class Mergeball : BaseUI
    {
        public override IEnumerator Show(params int[] args)
        {
            Master.Instance.SetBgState(false);
            yield return null;
        }
        public override IEnumerator Close()
        {
            Master.Instance.SetBgState(true);
            yield return null;
        }
        public override void SetContent()
        {
            GameManager.Instance.UIManager.GetUIPanel(UI_Panel.MenuPanel)?.SetContent();
        }
    }
}
