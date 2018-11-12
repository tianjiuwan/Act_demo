using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using OfficeOpenXml;
using System.IO;
using System.Text;


public class AnimInfo
{
    public string modelName;//模型名称
    public string animName;//动画名称
    public int startIndex;//起始帧
    public int endIndex;//结束帧
    public string nextName;//播放完成指向
    public float nextExitTime = 0;//融合时间
    public bool isLoop = false;
    public float speed = 1;
}

/// <summary>
/// 导入模型和动作
/// 战斗逻辑与显示完全分离
/// 导出的animator不再需要动画帧事件 也不需要过度条件
/// </summary>
public class AnimTool
{
    //需要导出的模型目录
    private const string orgPath = "Assets/Res/Arts/FBX";
    //导出模型 切片 animator的路径
    private const string expPath = "Res/AssetBundle/Prefabs/Model";
    private const string defaultAnim = "idle";

    [MenuItem("Assets/动画工具/导入选中模型", false, 1001)]
    public static void exportFbx()
    {
        //动画切片是否需要程序切？
        //程序切需要配置
        //读取配置lst<animinfo>
        Object obj = Selection.activeObject;
        if (obj == null)
        {
            EditorUtility.DisplayDialog("请选中正确的FBX模型文件1", "请确定", "OK");
            return;
        }
        string path = AssetDatabase.GetAssetPath(obj);
        if (!path.EndsWith(".FBX") && !path.EndsWith(".fbx"))
        {
            EditorUtility.DisplayDialog("请选中正确的FBX模型文件2", "请确定", "OK");
            return;
        }
        //先导入动画切片//同一个模型文件夹下面的命名规范 动画fbx以anim结尾 模型fbx以org结尾
        string animPath = "";
        string orgPath = "";
        string excelPath = "";
        string clipsPath = "";
        string animatorPath = "";
        string modelPath = "";
        //检查导出路径
        string floderName = Path.GetFileName(Path.GetDirectoryName(path));
        string exportModelPath = Path.Combine(expPath, floderName);
        //检查切片导出路径
        clipsPath = Path.Combine(exportModelPath, "clips");
        checkOrCreate(clipsPath);
        clipsPath = Path.Combine("Assets", clipsPath);
        //检查animator导出路径
        animatorPath = Path.Combine(exportModelPath, "animator");
        checkOrCreate(animatorPath);
        animatorPath = Path.Combine("Assets", animatorPath);
        //检查fbx导出路径
        modelPath = Path.Combine(exportModelPath, "model");
        checkOrCreate(modelPath);
        modelPath = Path.Combine("Assets", modelPath);
        string fbxPath = Path.GetDirectoryName(path);
        fbxPath = Path.Combine(Application.dataPath, fbxPath.Replace("Assets/", ""));
        string[] files = Directory.GetFiles(fbxPath);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            if (fileName.EndsWith("anim")) animPath = files[i];
            if (fileName.EndsWith("org")) orgPath = files[i];
            if (fileName.EndsWith("info")) excelPath = files[i];
        }
        //先导出动画切片
        List<AnimInfo> clips = new List<AnimInfo>();
        Dictionary<string, AnimInfo> infos = new Dictionary<string, AnimInfo>();
        getClips(excelPath, ref clips);
        //ModelImporterClipAnimation[] animations = new ModelImporterClipAnimation[clips.Count];
        List<string> usedModelsPath = new List<string>();
        Dictionary<string, List<ModelImporterClipAnimation>> usedClipsMap = new Dictionary<string, List<ModelImporterClipAnimation>>();
        for (int i = 0; i < clips.Count; i++)
        {
            string useModelPath = "";
            string modelName = "";
            if (string.IsNullOrEmpty(clips[i].modelName) || clips[i].modelName == "null")
            {
                useModelPath = getRelativePath(animPath);
                modelName = Path.GetFileNameWithoutExtension(useModelPath);
            }
            else
            {
                string defaultModelPath = getRelativePath(animPath);
                string defaultModelName = Path.GetFileNameWithoutExtension(defaultModelPath);
                useModelPath = defaultModelPath.Replace(defaultModelName, clips[i].modelName);
                modelName = clips[i].modelName;
            }
            var clipModelImporter = AssetImporter.GetAtPath(useModelPath) as ModelImporter;
            if (clipModelImporter == null) continue;
            clipModelImporter.animationType = ModelImporterAnimationType.Generic;
            clipModelImporter.importAnimation = true;
            clipModelImporter.generateAnimations = ModelImporterGenerateAnimations.GenerateAnimations;
            ModelImporterClipAnimation tempClip = new ModelImporterClipAnimation();
            tempClip.name = clips[i].animName;
            tempClip.firstFrame = clips[i].startIndex;
            tempClip.lastFrame = clips[i].endIndex;
            tempClip.loopTime = clips[i].isLoop;
            tempClip.wrapMode = clips[i].isLoop ? WrapMode.Loop : WrapMode.Default;
            if (!usedClipsMap.ContainsKey(modelName)) usedClipsMap.Add(modelName, new List<ModelImporterClipAnimation>());
            usedClipsMap[modelName].Add(tempClip);
            if (!usedModelsPath.Contains(useModelPath)) usedModelsPath.Add(useModelPath);
            infos.Add(clips[i].animName, clips[i]);
        }
        for (int i = 0; i < usedModelsPath.Count; i++)
        {
            string usedModelPath = usedModelsPath[i];
            var clipModelImporter = AssetImporter.GetAtPath(usedModelPath) as ModelImporter;
            if (clipModelImporter == null) continue;
            string modelName = Path.GetFileNameWithoutExtension(usedModelPath);
            clipModelImporter.clipAnimations = usedClipsMap[modelName].ToArray();
            AssetDatabase.ImportAsset(clipModelImporter.assetPath);

            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(usedModelPath);
            for (int k = 0; k < objs.Length; k++)
            {
                if (objs[k] is AnimationClip && !objs[k].name.StartsWith("_") && !objs[k].name.StartsWith("Take"))
                {
                    AnimationClip old = objs[k] as AnimationClip;
                    AnimationClip newClip = new AnimationClip();
                    EditorUtility.CopySerialized(old, newClip);
                    string savaPath = Path.Combine(clipsPath, newClip.name + ".anim");
                    AssetDatabase.CreateAsset(newClip, savaPath);
                }
            }
        }

        //创建Animator
        string animatorName = floderName + "_Controller.controller";
        string acSavePath = Path.Combine(animatorPath, animatorName);
        AnimatorController ac = AnimatorController.CreateAnimatorControllerAtPath(acSavePath);
        AnimatorControllerLayer layer = ac.layers[0];
        //添加clip        
        AnimatorStateMachine stateMachine = layer.stateMachine;
        string[] expClips = Directory.GetFiles(getFullPath(clipsPath), "*anim");
        Dictionary<string, Motion> dictClips = new Dictionary<string, Motion>();
        for (int i = 0; i < expClips.Length; i++)
        {
            Motion cp = AssetDatabase.LoadAssetAtPath<Motion>(getRelativePath(expClips[i]));
            if (cp != null)
            {
                dictClips.Add(cp.name, cp);
            }
        }
        int startY = 1;
        int startX = 300;
        AnimatorState animState = new AnimatorState();
        if (!dictClips.ContainsKey(defaultAnim))
        {
            Debug.LogError("没有配置idle动画 请检查 " + excelPath);
        }
        foreach (var item in dictClips)
        {
            int y = item.Key == defaultAnim ? 200 : startY * 80;
            int x = item.Key == defaultAnim ? 0 : startX;
            animState = stateMachine.AddState(item.Key, new Vector2(x, y));
            animState.motion = item.Value;
            if (infos.ContainsKey(item.Key))
            {
                animState.speed = infos[item.Key].speed;
            }
            if (item.Key == defaultAnim)
            {
                stateMachine.defaultState = animState;
            }
            startY = item.Key == defaultAnim ? startY : startY + 1;
        }
        //动画全部加入再连线
        ChildAnimatorState[] states = stateMachine.states;
        Dictionary<string, AnimatorState> stateMap = new Dictionary<string, AnimatorState>();
        for (int i = 0; i < states.Length; i++)
        {
            stateMap.Add(states[i].state.name, states[i].state);
        }
        for (int i = 0; i < states.Length; i++)
        {
            ChildAnimatorState currState = states[i];
            string animName = currState.state.name;
            if (infos.ContainsKey(animName) && !string.IsNullOrEmpty(infos[animName].nextName))
            {
                string nextName = infos[animName].nextName;
                float exitTime = infos[animName].nextExitTime;
                if (!stateMap.ContainsKey(nextName)) continue;
                AnimatorState nextState = stateMap[nextName];
                AnimatorStateTransition cond = currState.state.AddTransition(nextState);
                cond.hasExitTime = true;
                cond.exitTime = exitTime;
            }
        }
        ac.name = animatorName;
        //导出prefab赋值Animator
        GameObject fbxObj = AssetDatabase.LoadAssetAtPath(getRelativePath(orgPath), typeof(UnityEngine.GameObject)) as GameObject;
        if (fbxObj == null)
        {
            Debug.LogError("模型预设资源不存在: " + orgPath);
            return;
        }
        AnimatorController sac = AssetDatabase.LoadAssetAtPath<AnimatorController>(acSavePath);
        fbxObj.GetComponent<Animator>().runtimeAnimatorController = sac;
        string name = string.Format("{0}.prefab", floderName);
        string savePath = Path.Combine(modelPath, name);
        savePath = savePath.Replace("\\", "/");
        UnityEngine.Object pref = PrefabUtility.CreateEmptyPrefab(savePath);
        GameObject go = PrefabUtility.ReplacePrefab(fbxObj, pref);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void checkExportPath(string floderName)
    {
        string path = Path.Combine(expPath, floderName);
        checkOrCreate(path);
        //检查切片导出路径
        string clipPath = Path.Combine(path, "clips");
        checkOrCreate(clipPath);
        //检查animator导出路径
        string animPath = Path.Combine(path, "animator");
        checkOrCreate(animPath);
        //检查fbx导出路径
        string fbxPath = Path.Combine(path, "model");
        checkOrCreate(fbxPath);
    }

    private static void checkOrCreate(string path)
    {
        path = Path.Combine(Application.dataPath, path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    //获取切片信息
    private static void getClips(string excelPath, ref List<AnimInfo> clips)
    {
        Dictionary<int, List<string>> excelMap = new Dictionary<int, List<string>>();
        getExcelByPath(excelPath, ref excelMap);
        //返回的字典key = 2起
        foreach (var item in excelMap)
        {
            List<string> clipInfo = item.Value;
            AnimInfo info = new AnimInfo();
            info.animName = clipInfo[0].ToString();
            info.startIndex = int.Parse(clipInfo[1]);
            info.endIndex = int.Parse(clipInfo[2]);
            info.isLoop = int.Parse(clipInfo[3]) == 1;
            info.speed = clipInfo[4] == null ? 1 : float.Parse(clipInfo[4]);
            info.nextName = clipInfo.Count <= 5 ? "" : clipInfo[5].ToString();
            info.nextExitTime = clipInfo.Count <= 6 ? 0 : float.Parse(clipInfo[6]);
            info.modelName = clipInfo.Count <= 7 ? "" : clipInfo[7].ToString();
            clips.Add(info);
        }
    }


    #region   以下获取配置方法
    /// <summary>
    /// 以下获取配置方法
    /// </summary>
    private const int dataRowIndex = 2;//所有excel配置从第三行读取(前两行备注)
                                       //读取excel by path
                                       //返回 dict <  id,li  >
    public static void getExcelByPath(string path, ref Dictionary<int, List<string>> dict)
    {
        using (ExcelPackage package = new ExcelPackage(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            int maxRow = sheet.Dimension.End.Row;
            if (maxRow >= dataRowIndex)
            {
                for (int i = dataRowIndex; i <= maxRow; i++)
                {
                    //如果当前行 第一个元素为空 continue                    
                    object val = sheet.GetValue(i, 1);
                    if (i >= dataRowIndex && (val == null || string.IsNullOrEmpty(val.ToString()))) continue;
                    List<string> lst = new List<string>();
                    readRow(sheet, i, ref lst);
                    dict.Add(i, lst);
                }
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
            object val = sheet.GetValue(index, i);
            lst.Add(val != null ? val.ToString() : "null");
        }
    }
    #endregion


    //根据完整路径获取相对路径
    private static string getRelativePath(string full)
    {
        int index = full.IndexOf("Assets");
        string path = full.Substring(index);
        return path;
    }
    //get full path
    private static string getFullPath(string path)
    {
        path = path.Replace("Assets/", "");
        path = path.Replace(@"Assets\", "");
        path = Path.Combine(Application.dataPath, path);
        return path;
    }

}
