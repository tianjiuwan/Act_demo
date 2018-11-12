using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OfficeOpenXml;
using System.IO;
using System;
using System.Text;

/// <summary>
/// Excel导出Lua工具
/// Excel配置规则(只读取sheet1,Sheet1的命名是导出Lua table的命名)
/// 第一行描述 
/// 第二行标记是否导出Lua字段或是sql 
/// 第三行数据类型(string,int,float,bool,int[]) 
/// 第四行字段名称(如果读取excel第n列发现第4行字段为空则不导出)
/// 第五行正式数据配置...
/// </summary>
public class ExcelExportTool
{
    private const int dataRowIndex = 5;
    private const string path = "Assets/Res/Arts/ExcelConfig";

    [MenuItem("Assets/UI工具/导出Excel", false, 5000)]
    public static void exportExcel()
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("excel路径不存在" + path);
            return;
        }
        UnityEngine.Object obj = Selection.activeObject;
        if (File.Exists(Path.Combine(path, obj.name + ".xlsx")))
        {
            exportExcel(Path.Combine(path, obj.name + ".xlsx"));
        }
        else
        {
            string[] files = Directory.GetFiles(path, "*xlsx");
            for (int i = 0; i < files.Length; i++)
            {
                exportExcel(files[i]);
            }
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("导出完成", "导出Excel完成", "OK");
    }
    //读取excel
    public static void exportExcel(string path)
    {
        using (ExcelPackage package = new ExcelPackage(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
        {
            Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            int maxRow = sheet.Dimension.End.Row;
            if (maxRow >= dataRowIndex)
            {
                for (int i = 1; i <= maxRow; i++)
                {
                    //如果当前行 第一个元素为空 continue                    
                    object val = sheet.GetValue(i, 1);
                    if (i >= dataRowIndex && (val == null || string.IsNullOrEmpty(val.ToString()))) continue;
                    List<string> lst = new List<string>();
                    readRow(sheet, i, ref lst);
                    dict.Add(i, lst);
                }
                writeTest(dict, sheet.Name);
            }
            else
            {
                Debug.LogError("配置表格式有问题 ");
            }
        }
    }

    //读取一行
    public static void readRow(ExcelWorksheet sheet, int index, ref List<string> lst)
    {
        int maxCol = sheet.Dimension.End.Column;
        for (int i = 1; i <= maxCol; i++)
        {
            //拿当前列 第4行 字段为空 跳过
            object proVal = sheet.GetValue(4, i);
            if (proVal == null || string.IsNullOrEmpty(proVal.ToString())) continue;

            object val = sheet.GetValue(index, i);
            lst.Add(val != null ? val.ToString() : "");
        }
    }
    //写入一整张表
    private static string expPath = Path.Combine(Application.dataPath, "LuaScripts/Scripts/Config");
    public static void writeTest(Dictionary<int, List<string>> dict, string tableName)
    {
        if (!Directory.Exists(expPath))
            Directory.CreateDirectory(expPath);
        string exp = Path.Combine(expPath, tableName + ".lua");
        if (File.Exists(exp)) File.Delete(exp);

        FileStream fs = new FileStream(exp, FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        string calss = tableName + " = { }\n";
        sw.Write(calss);
        for (int i = dataRowIndex; i <= dict.Count; i++)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(tableName + "[" + dict[i][0] + "]={");
            List<string> lst = dict[i];
            for (int j = 0; j < lst.Count; j++)
            {
                string dataClass = dict[3][j];
                if (dataClass == "int" || dataClass == "float" || dataClass == "long")
                {
                    lst[j] = string.IsNullOrEmpty(lst[j]) ? "0" : lst[j];
                }
                else if (dataClass == "string")
                {
                    lst[j] = "'" + lst[j] + "'";
                }
                else if (dataClass == "bool")
                {
                    int boolVal = 0;//bool变量 0false 1true 
                    if (int.TryParse(lst[j], out boolVal))
                    {
                        lst[j] = boolVal == 0 ? "false" : "true";
                    }
                    else
                    {
                        StringBuilder bbuilder = new StringBuilder();
                        bbuilder.Append("'");
                        bbuilder.Append(lst[j]);
                        bbuilder.Append("'");
                        lst[j] = bbuilder.ToString();
                    }
                }
                else if (dataClass == "int[]" || dataClass == "float[]")
                {
                    string[] spLst = lst[j].Split(',');
                    StringBuilder spBuilder = new StringBuilder();
                    spBuilder.Append("{");
                    for (int k = 0; k < spLst.Length; k++)
                    {
                        if (k > 0) spBuilder.Append(',');
                        spBuilder.Append(spLst[k]);
                    }
                    spBuilder.Append("}");
                    lst[j] = spBuilder.ToString();
                }
                builder.Append(dict[4][j] + "=" + lst[j]);
                if (j < lst.Count - 1)
                {
                    builder.Append(",");
                }
            }
            builder.Append("}");
            sw.Write(builder.ToString() + '\n');
        }
        sw.Flush();
        sw.Close();
        fs.Close();
    }


}