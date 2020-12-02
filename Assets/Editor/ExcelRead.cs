using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using UnityEditor;
using UnityEngine;

public class ExcelRead : Editor
{
    public static DataSet Read(string filePath)
    {
        try
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            excelReader.Close();
            stream.Close();
            return result;
        }
        catch
        {
            return null;
        }
    }

    public static object ReadDataByRowCol(DataTable table, int row, int col)
    {
        DataRow dataRow = table.Rows[row];
        if (dataRow.IsNull(col))
        {
            return null;
        }
        return dataRow[col];

    }
}
