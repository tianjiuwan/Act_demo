using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AppStart : MonoBehaviour
{
    private GameObject roleObj = null;
    // Use this for initialization
    void Start()
    {
        ManifestMgr.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string role = "AssetBundle/Prefabs/model/role_ueman/model/role_ueman";
            PoolMgr.Instance.getObj(role, (go) =>
            {
                roleObj = go;
                roleObj.transform.position = Vector3.zero;
                watch.Stop();
                //Debug.log("watch:" + totleTime.ElapsedMilliseconds.ToString());
                UnityEngine.Debug.LogError("耗时： " + watch.ElapsedMilliseconds.ToString());
            });
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            PoolMgr.Instance.recyleObj(roleObj);
        }
    }
}
