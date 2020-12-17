using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System.Text;
namespace HiSpin
{
    public class ReadMultiLanguage : Editor
    {
        private static string xlsxPath;
        [MenuItem("Excel/ReadMultiLanguage")]
        public static void ReadExcel()
        {
            xlsxPath = Application.dataPath + "HiSpin/Language.xlsx";
            if (!File.Exists(xlsxPath))
            {
                Debug.LogError("文件不存在");
                return;
            }
            FileStream fs = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
            DataSet dataSet = reader.AsDataSet();
            reader.Dispose();
            if (dataSet == null)
            {
                Debug.LogError("文件为空!");
                return;
            }
            string areaEnumFilePath = Application.dataPath + "/Scripts/LanguageAreaEnum.cs";
            string countryEnumFilePath = Application.dataPath + "/Scripts/LanguageCountryEnum.cs";
            StringBuilder areaString = new StringBuilder("public enum LanguageAreaEnum\n{");
            StringBuilder countryString = new StringBuilder("public enum LanguageCountryEnum\n{");
            string endContent = "\n}";
            DataTable dataTable1 = dataSet.Tables[0];
            int rowCount = dataTable1.Rows.Count;
            int columnsCount = dataTable1.Columns.Count;
            for (int i = 3; i < rowCount; i++)
            {
                string value = dataTable1.Rows[i][0].ToString();
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    continue;
                areaString.Append("\n    " + value + ",");
            }
            for (int i = 1; i < columnsCount; i++)
            {
                string value = dataTable1.Rows[0][i].ToString();
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    continue;
                countryString.Append("\n    " + value + ",");
            }
            countryString.Append("\n    LanguageTypeNum");



            areaString.Append(endContent);
            countryString.Append(endContent);
            if (File.Exists(areaEnumFilePath))
                File.Delete(areaEnumFilePath);
            FileStream areaFs = new FileStream(areaEnumFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            byte[] willWriteAreaBytes = Encoding.UTF8.GetBytes(areaString.ToString());
            areaFs.Write(willWriteAreaBytes, 0, willWriteAreaBytes.Length);
            areaFs.Dispose();
            if (File.Exists(countryEnumFilePath))
                File.Delete(countryEnumFilePath);
            FileStream countryFs = new FileStream(countryEnumFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            byte[] willWriteCountryBytes = Encoding.UTF8.GetBytes(countryString.ToString());
            countryFs.Write(willWriteCountryBytes, 0, willWriteCountryBytes.Length);
            countryFs.Dispose();
            isCompileLanguageEnum = true;
            AssetDatabase.Refresh();
            if (!EditorApplication.isCompiling)
            {
                OnCompileComplete();
            }
        }
        public static bool isCompileLanguageEnum = false;
        public static void OnCompileComplete()
        {
            isCompileLanguageEnum = false;
            MultiLanguageData data = Resources.Load<MultiLanguageData>("MultiLanguageData");
            data.languageUnits.Clear();

            xlsxPath = Application.dataPath + "HiSpin/Language.xlsx";
            if (!File.Exists(xlsxPath))
            {
                Debug.LogError("文件不存在");
                return;
            }
            FileStream fs = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
            DataSet dataSet = reader.AsDataSet();
            reader.Dispose();
            if (dataSet == null)
            {
                Debug.LogError("文件为空!");
                return;
            }
            DataTable dataTable1 = dataSet.Tables[0];
            int rowCount = dataTable1.Rows.Count;
            int columnsCount = dataTable1.Columns.Count;
            for (int columnIndex = 1; columnIndex < columnsCount; columnIndex++)
            {
                if (string.IsNullOrEmpty(dataTable1.Rows[0][columnIndex].ToString()))
                    continue;
                LanguageCountryEnum languageCountry = (LanguageCountryEnum)System.Enum.Parse(typeof(LanguageCountryEnum), dataTable1.Rows[0][columnIndex].ToString());
                MultiLanguageUnit languageUnit = new MultiLanguageUnit()
                {
                    country = languageCountry,
                    value_allAreas = new List<MultiLanguageUnitArea>()
                };
                for (int rowIndex = 3; rowIndex < rowCount; rowIndex++)
                {
                    if (string.IsNullOrEmpty(dataTable1.Rows[rowIndex][0].ToString()))
                        continue;
                    LanguageAreaEnum languageArea = (LanguageAreaEnum)System.Enum.Parse(typeof(LanguageAreaEnum), dataTable1.Rows[rowIndex][0].ToString());
                    MultiLanguageUnitArea languageUnitArea = new MultiLanguageUnitArea()
                    {
                        area = languageArea,
                        value = dataTable1.Rows[rowIndex][columnIndex].ToString()
                    };
                    languageUnit.value_allAreas.Add(languageUnitArea);
                }
                data.languageUnits.Add(languageUnit);
            }
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    [InitializeOnLoad]
    public class UnityScriptCompiling : AssetPostprocessor
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void AllScriptsReloaded()
        {
            if (ReadMultiLanguage.isCompileLanguageEnum)
            {
                ReadMultiLanguage.OnCompileComplete();
            }
        }
    }
}
