#if UNITY_EDITOR

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
// using LitJson;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
// using ExcelDataReader;
// using OfficeOpenXml;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// 自定义编辑通用方法
/// </summary>
public static class CommonEditorMethod
{
    public static void Save(this object obj, string value = default)
    {
        //if (!EditorWindow.HasOpenInstances<DataTools>()) return;
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo[] fieldsValue = obj.GetType().GetFields(bindingFlags);
        foreach (FieldInfo data in fieldsValue)
        {
            if (data.IsLiteral) continue; //反射判断这个字段是否是const字段
            if (data.IsInitOnly) continue; //反射判断这个字段是否是readonly字段
            if (data.FieldType == typeof(string))
                PlayerPrefs.SetString($"{Application.productName}{value}{data.Name}Save", (string)data.GetValue(obj));
            else if (data.FieldType == typeof(int))
                PlayerPrefs.SetInt($"{Application.productName}{value}{data.Name}Save", (int)data.GetValue(obj));
        }

        UnityEngine.Debug.Log("保存成功");
    }

    public static void Load(this object obj, string value = default)
    {
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo[] fieldsValue = obj.GetType().GetFields(bindingFlags);
        foreach (var data in fieldsValue)
        {
            if (data.IsLiteral) continue; //反射判断这个字段是否是const字段
            if (data.IsInitOnly) continue; //反射判断这个字段是否是readonly字段

            if (data.FieldType == typeof(string))
                data.SetValue(obj, PlayerPrefs.GetString($"{Application.productName}{value}{data.Name}Save"));
            if (data.FieldType == typeof(int))
                data.SetValue(obj, PlayerPrefs.GetInt($"{Application.productName}{value}{data.Name}Save"));
        }

        UnityEngine.Debug.Log("加载成功");
    }

    public static void ShowUI<T>(this string title) where T : EditorWindow
    {
        if (!EditorWindow.HasOpenInstances<T>())
            EditorWindow.GetWindow(typeof(T), false, title).Show();
        else
            EditorWindow.GetWindow(typeof(T)).Close();
    }

    /// <summary>
    /// 复制
    /// </summary>
    /// <param name="str"></param>
    public static void Copy(this string str)
    {
        TextEditor te = new TextEditor { text = str };
        te.SelectAll();
        te.Copy();
    }

    // /// <summary>
    // /// 读取Excel数据并保存为字符串锯齿数组
    // /// </summary>
    // /// <param name="filePath"></param>
    // /// <param name="tabelNum"></param>
    // /// <returns></returns>
    // public static string[][] ReadExcel(this string filePath, int tabelNum)
    // {
    //     FileInfo fileInfo = new FileInfo(filePath);
    //     using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    //     DataSet dataSet = fileInfo.Extension == ".xlsx" ? ExcelReaderFactory.CreateOpenXmlReader(stream).AsDataSet() : ExcelReaderFactory.CreateBinaryReader(stream).AsDataSet();
    //     DataRowCollection rows = dataSet.Tables[tabelNum].Rows;
    //     string[][] data = new string[rows.Count][];
    //     for (int i = 0; i < rows.Count; ++i)
    //     {
    //         var columnCount = rows[i].ItemArray.Length;
    //         var columnArray = new string[columnCount];
    //         for (var j = 0; j < columnArray.Length; ++j)
    //             columnArray[j] = rows[i].ItemArray[j].ToString();
    //         data[i] = columnArray;
    //     }
    //
    //     return data;
    // }
    //
    // /// <summary>
    // /// 写入Excel
    // /// </summary>
    // /// <param name="excelPath"></param>
    // /// <param name="data"></param>
    // /// <param name="worksheetsNum">第几个表</param>
    // public static void WriteToExcel(this string excelPath, string[][] data, int worksheetsNum)
    // {
    //     FileInfo excelName = new FileInfo(excelPath);
    //     using ExcelPackage package = new ExcelPackage(excelName); //通过ExcelPackage打开文件
    //     ExcelWorksheet worksheet = package.Workbook.Worksheets[worksheetsNum]; //1表示第一个表
    //     for (int i = 0; i < data.Length; i++)
    //     {
    //         string[] dataTemp = data[i];
    //         for (int j = 0; j < dataTemp.Length; j++)
    //             worksheet.SetValue(i + 1, j + 1, dataTemp[j]);
    //     }
    //
    //     package.Save(); //储存
    // }
    //
    // private static void WriteOneDataToExcel(string[][] data, int row, int col, string excelPath, string content)
    // {
    //     data[row][col] = content;
    //     FileInfo excelName = new FileInfo(excelPath);
    //     using var package = new ExcelPackage(excelName); //通过ExcelPackage打开文件
    //     ExcelWorksheet worksheet = package.Workbook.Worksheets[1]; //1表示第一个表
    //     worksheet.SetValue(row, col, content);
    //     package.Save(); //储存
    // }
}

/// <summary>
/// 每行类型
/// </summary>
public enum RowType : byte
{
    ConfigIndex = 0, //配置
    BeginIndex = 1, //开始行数
    FieldIndex = 1, //字段
    TypeIndex = 2, //类型
    DesIndex = 3, //中文名称
    DataIndex = 4, //数据行数
}

/// <summary>
/// 列类型
/// </summary>
public enum ColType : byte
{
    ClassName = 0, //Class
    ScriptableObjectName = 1, //ScriptableObject
    Chinese = 2, //中文
    // English = 1, //英文
}
#endif