using System;
using System.Collections.Generic;
using System.IO;

public class FileUtils
{
    //获取这个文件夹下面的所有文件夹
    public static List<string> getAllFolder(string path) {
        List<string> pathLst = new List<string>();
        pathLst.Add(path);
        getAllPath(path, ref pathLst);
        return pathLst;
    }
    private static void getAllPath(string path,ref List<string> pathLst) {
        string[] arr = Directory.GetDirectories(path);
        for (int i = 0; i < arr.Length; i++)
        {
            getAllPath(arr[i], ref pathLst);
        }
        pathLst.AddRange(arr);
    }
    //获取这个文件夹下面所有的文件
    //public static List<string> getAllFiles() {
    //    Directory.GetFiles
    //}

}

