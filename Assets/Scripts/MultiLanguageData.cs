using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "MultiLanguageData", menuName = "Create MultiLanguageData")]
public class MultiLanguageData : ScriptableObject
{
    //[System.NonSerialized]
    public List<MultiLanguageUnit> languageUnits = new List<MultiLanguageUnit>();
}
[System.Serializable]
public struct MultiLanguageUnit
{
    public LanguageCountryEnum country;
    public List<MultiLanguageUnitArea> value_allAreas;
}
[System.Serializable]
public struct MultiLanguageUnitArea
{
    public LanguageAreaEnum area;
    public string value;
}
