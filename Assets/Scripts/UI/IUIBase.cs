using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HiSpin
{
    public interface IUIBase : ILanguage
    {
        IEnumerator Show(params int[] args);
        void Pause();
        void Resume();
        IEnumerator Close();
    }
}
