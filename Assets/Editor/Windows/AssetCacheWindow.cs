using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class AssetCacheWindow : EditorWindow
{

    private static AssetCacheWindow _instance = null;
    private bool isInit = false;

    [MenuItem("ToolsWindow/查看AB缓存")]
    public static void showWindow()
    {
        if (_instance == null)
        {
            _instance = (AssetCacheWindow)EditorWindow.GetWindow(typeof(AssetCacheWindow), false, "查看AB缓存", true);
            _instance.maxSize = new Vector2(1280, 720);
            GUIContent content = new GUIContent();
            content.text = "查看AB缓存";
            _instance.titleContent = content;
            _instance.isInit = true;
        }
    }

    private Vector2 scrollPos = Vector2.zero;
    private Vector2 poolPos = Vector3.zero;
    private List<GameObject> testLoadObjs = new List<GameObject>();
    private GameObject uiRoot;
    private int roleIndex = 100000000;
    private int startZ = 2;
    private void OnGUI()
    {
        if (!isInit) return;
        if (!Application.isPlaying)
        {
            testLoadObjs.Clear();
            return;
        }
        GUILayout.BeginVertical();
        GUILayout.Label("Time: " + System.DateTime.Now);
        //绘制2个滑动框
        GUILayout.BeginHorizontal();
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(1000), GUILayout.Height(600));
        List<PackAsset> lst = new List<PackAsset>();
        Dictionary<string, PackAsset> map = AssetMgr.Instance.getAll();
        lst.AddRange(map.Values);
        for (int i = 0; i < lst.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            StringBuilder builder = new StringBuilder();
            builder.Append("Name: ");
            builder.Append(lst[i].Name);
            GUIContent content = new GUIContent();
            content.text = builder.ToString();
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            GUILayout.Box(content, style, GUILayout.Width(600), GUILayout.Height(40));
            style.fontSize = 16;
            content.text = "RefCount: " + lst[i].RefCount;
            GUILayout.Box(content, style, GUILayout.Width(100), GUILayout.Height(40));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        //poolPos = GUILayout.BeginScrollView(poolPos, false, true, GUILayout.Width(600), GUILayout.Height(600));
        //List<BasePool> pools = PoolMgr.Instance.getPools();
        //for (int i = 0; i < pools.Count; i++)
        //{
        //    BasePool bp = pools[i];
        //    GUILayout.BeginHorizontal();
        //    GUILayout.Label("PoolName: " + bp.Name, GUILayout.Width(160), GUILayout.Height(30));
        //    GUILayout.Label("PoolType: " + bp.PoolType, GUILayout.Width(100), GUILayout.Height(30));
        //    GUILayout.Label("CacheCount: " + bp.CacheCount, GUILayout.Width(100), GUILayout.Height(30));
        //    GUILayout.Label("HandlerCount: " + bp.HandlerCount, GUILayout.Width(100), GUILayout.Height(30));
        //    GUILayout.EndHorizontal();
        //}
        // GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        //结束绘制2个滑动框
        GUILayout.EndVertical();
    }
}
