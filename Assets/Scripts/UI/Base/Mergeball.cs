using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }
}
