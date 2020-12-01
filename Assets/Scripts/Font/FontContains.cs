using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
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
        string sysLanguage = Application.systemLanguage.ToString();
        DataSet dataSet = ReadExcel(Application.dataPath + "/Excel/" + "Font.xlsx");
        if (dataSet == null)
        {
            return;
        }
        for (int i = 0; i < dataSet.Tables.Count; ++i)
        {
            DataTable tabel = dataSet.Tables[0];
            if(tabel == null)
            {
                continue;
            }
            // 根据系统语言初始化 col
            int col = this.AnalysisColByLanguage(tabel, sysLanguage);
            this.ReadFont(tabel, col, this.pFonts);
        }
    }


    private int AnalysisColByLanguage(DataTable tabel,string language)
    {
        for (int i = 1; i < tabel.Columns.Count; ++i)
        {
            object data = this.ReadDataByRowCol(tabel, 0, i);
            if (data != null && language == data.ToString())
            {
                return i;
            }
        }
        return 1;
    }

    private void ReadFont(DataTable tabel,int col, Dictionary<string, string> font)
    {
        for(int i = 1; i < tabel.Rows.Count; ++i)
        {
            object key = this.ReadDataByRowCol(tabel, i,0);
            object value = this.ReadDataByRowCol(tabel, i, col);
            if(key!=null && value != null)
            {
                font[key.ToString()] = value.ToString();
            }
        }       
    }


    private object ReadDataByRowCol(DataTable table, int row , int col)
    {
        DataRow dataRow = table.Rows[row];
        if (dataRow.IsNull(0))
        {
            return null;
        }
        return dataRow[col];

    }

    private DataSet ReadExcel(string filePath)
    {
        try
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            return result;
        }
        catch
        {
            return null;
        }
    }

}
