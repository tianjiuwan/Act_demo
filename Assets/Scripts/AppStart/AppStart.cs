﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class AppStart : MonoBehaviour
{
    private GameObject roleObj = null;
    [SerializeField]
    private Image img = null;

    private int preNum = 0;
    private int loadedNum = 0;
    private Dictionary<E_PoolType, List<string>> preLoads = new Dictionary<E_PoolType, List<string>>();
    Stopwatch watch = new Stopwatch();

    // Use this for initialization
    void Start()
    {
        preLoads.Add(E_PoolType.Atlas, new List<string>());
        preLoads[E_PoolType.Atlas].Add("assetbundle/atlas/commonatlas");
        preLoads[E_PoolType.Atlas].Add("assetbundle/atlas/facebookatlas");
        preLoads[E_PoolType.Atlas].Add("assetbundle/atlas/teamatlas");
        preLoads.Add(E_PoolType.Model, new List<string>());
        preLoads[E_PoolType.Model].Add("assetbundle/prefabs/model/role_ueman/model/role_ueman");
        preLoads[E_PoolType.Model].Add("assetbundle/prefabs/model/role_fistgirl/model/role_fistgirl");
        preLoads[E_PoolType.Model].Add("assetbundle/prefabs/model/role_superman/model/role_superman");

        ManifestMgr.Init();
    }


    private void onLoadUEManFinish(GameObject go)
    {
        roleObj = go;
        roleObj.transform.position = Vector3.zero;
        //watch.Stop();
        UnityEngine.Debug.LogError("实例化role_ueman成功");
    }

    private void testUpdate()
    {
        //创建一个模型
        if (Input.GetKeyDown(KeyCode.P))
        {
            //watch.Start();
            string role = "AssetBundle/Prefabs/model/role_ueman/model/role_ueman";
            UnityEngine.Debug.LogError("加载role_ueman");
            PoolMgr.Instance.getObj(role, onLoadUEManFinish);
            UnityEngine.Debug.LogError("取消加载role_ueman");
            PoolMgr.Instance.unLoad(role, onLoadUEManFinish);
        }
        //回收一个模型
        if (Input.GetKeyDown(KeyCode.R))
        {
            PoolMgr.Instance.recyleObj(roleObj);
        }

        //设置img的sprite
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (img != null)
            {
                string spName = "chosenKuang".ToLower();
                PoolMgr.Instance.getObj(spName, (sp) =>
                {
                    img.sprite = sp;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("img == null ");
            }

        }
        //预加载模型
        if (Input.GetKeyDown(KeyCode.K))
        {
            watch.Start();
            foreach (var item in preLoads)
            {
                E_PoolType pType = item.Key;
                int cacheCount = pType == E_PoolType.Model ? 3 : 0;
                for (int i = 0; i < item.Value.Count; i++)
                {
                    preNum++;
                    PoolMgr.Instance.preLoad(item.Value[i], preLoadCount, E_PoolMode.Overall, pType, cacheCount);
                }
            }
        }

    }


    private void OnGUI()
    {
        //释放所有池子
        if (GUILayout.Button("释放所有池子"))
        {
            PoolMgr.Instance.onDispose();
        }

        //释放一个池子
        if (GUILayout.Button("释放一个池子"))
        {
            if (roleObj != null)
            {
                string resName = roleObj.GetComponent<PoolObj>().resName;
                PoolMgr.Instance.disposePool(resName);
                UnityEngine.Debug.LogError("释放池子 "+ resName);
            }
        }

        //创建一个模型
        if (GUILayout.Button("创建一个模型"))
        {
            //watch.Start();
            string role = "AssetBundle/Prefabs/model/role_ueman/model/role_ueman";
            UnityEngine.Debug.LogError("加载role_ueman");
            PoolMgr.Instance.getObj(role, onLoadUEManFinish);
            //UnityEngine.Debug.LogError("取消加载role_ueman");
            //PoolMgr.Instance.unLoad(role, onLoadUEManFinish);
        }
        //回收一个模型
        if (GUILayout.Button("回收一个模型"))
        {
            PoolMgr.Instance.recyleObj(roleObj);
        }

        //设置img的sprite
        if (GUILayout.Button("设置img的sprite"))
        {
            if (img != null)
            {
                string spName = "chosenKuang".ToLower();
                PoolMgr.Instance.getObj(spName, (sp) =>
                {
                    img.sprite = sp;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("img == null ");
            }

        }
        //预加载模型
        if (GUILayout.Button("预加载"))
        {
            watch.Start();
            foreach (var item in preLoads)
            {
                E_PoolType pType = item.Key;
                int cacheCount = pType == E_PoolType.Model ? 3 : 0;
                for (int i = 0; i < item.Value.Count; i++)
                {
                    preNum++;
                    PoolMgr.Instance.preLoad(item.Value[i], preLoadCount, E_PoolMode.Overall, pType, cacheCount);
                }
            }
        }
    }

    private void preLoadCount(string name)
    {
        loadedNum++;
        if (loadedNum >= preNum)
        {
            watch.Stop();
            UnityEngine.Debug.LogError("预加载完成  耗时： " + watch.ElapsedMilliseconds.ToString());
        }
    }

}
