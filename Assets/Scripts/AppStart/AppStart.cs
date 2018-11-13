using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class AppStart : MonoBehaviour
{
    private GameObject roleObj = null;
    [SerializeField]
    private Image img = null;

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
            else {
                UnityEngine.Debug.LogError("img == null ");
            }

        }

    }
}
