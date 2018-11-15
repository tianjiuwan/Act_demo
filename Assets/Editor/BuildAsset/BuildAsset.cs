using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class BuildAsset
{
    //资源路径
    private const string assetPath = "Res/AssetBundle";
    //ab导出路径
    private const string exportABPath = "Assets/Res/AssetBundleExport";
    //图集文件夹 图集文件夹下面的每个文件夹打一个ab
    private const string atlasPathSuff = "Atlas";

    [MenuItem("Tools/打包相关/BuildAsset", false, 1001)]
    public static void build()
    {
        checkPath();
        markAsset();
        //一键打包
        BuildPipeline.BuildAssetBundles(exportABPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    private static void checkPath()
    {
        string path = exportABPath.Replace("Assets/", "");
        if (!Directory.Exists(Path.Combine(Application.dataPath, path)))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, path));
        }
        AssetDatabase.Refresh();
    }


    //标记所有需要打包的资源
    //AssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform);
    private static void markAsset()
    {
        string fullPath = getFullPath(assetPath);
        List<string> flst = FileUtils.getAllFolder(fullPath); Directory.GetDirectories(fullPath);
        for (int i = 0; i < flst.Count; i++)
        {
            doMark(flst[i]);
        }
        AssetDatabase.Refresh();
    }
    private static void doMark(string aPath)
    {
        //如果是图集 打包ab比较特殊 需要打包整个文件夹
        string abName = "";
        string[] files = Directory.GetFiles(aPath);
        for (int k = 0; k < files.Length; k++)
        {
            if (files[k].EndsWith(".meta")) continue;
            string assetRpath = getRelativePath(files[k]);
            AssetImporter imp = AssetImporter.GetAtPath(assetRpath);
            if (imp != null)
            {
                abName = aPath.EndsWith(atlasPathSuff) ? getAbName(aPath) : getAbName(files[k]);
                abName = abName.Replace(@"\", "/");
                imp.assetBundleName = abName;
                EditorUtility.SetDirty(imp);
            }
            else
            {
                Debug.Log(assetRpath);
            }
        }
    }


    [MenuItem("Assets/打包相关/清理ab标记", false, 5001)]
    public static void clearMark()
    {
        bool isClear = EditorUtility.DisplayDialog("是否清理所有ab标记？", "请确认", "清理", "取消");
        if (isClear)
        {
            UnityEngine.Object[] floderName = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets);
            if (floderName.Length > 0)
            {
                string selectPath = AssetDatabase.GetAssetPath(floderName[0]);
                string full = getFullPath(selectPath);
                List<string> floders = FileUtils.getAllFolder(full);
                for (int i = 0; i < floders.Count; i++)
                {
                    string[] files = Directory.GetFiles(floders[i]);
                    for (int k = 0; k < files.Length; k++)
                    {
                        string assetRpath = getRelativePath(files[k]);
                        AssetImporter imp = AssetImporter.GetAtPath(assetRpath);
                        if (imp != null)
                        {
                            imp.assetBundleName = "";
                            EditorUtility.SetDirty(imp);
                        }
                    }
                }
                AssetDatabase.Refresh();
            }
        }
    }
    [MenuItem("Tools/打包相关/清理所有ab", false, 1002)]
    public static void clearAssetBundle()
    {
        bool isClear = EditorUtility.DisplayDialog("确认清理所有AB？", "请确认", "清理", "取消");
        if (isClear)
        {
            if (Directory.Exists(Path.Combine(Application.dataPath, exportABPath)))
            {
                Directory.Delete(Path.Combine(Application.dataPath, exportABPath));
            }
            AssetDatabase.Refresh();
        }
    }


    //获取完整路径
    private static string getFullPath(string suff)
    {
        if (suff.StartsWith("Assets/"))
        {
            suff = suff.Replace("Assets/", "");
        }
        return Path.Combine(Application.dataPath, suff);
    }
    //根据完整路径获取相对路径
    private static string getRelativePath(string full)
    {
        int index = full.IndexOf("Assets");
        string path = full.Substring(index);
        return path;
    }
    //Atlas abName
    public static string getAbName(string path)
    {
        int index = path.IndexOf("AssetBundle");
        string name = path.Substring(index);
        int suffIndex = name.LastIndexOf(".");
        if (suffIndex > 0)
        {
            name = name.Substring(0, suffIndex);
        }
        return name;
    }
    //获取文件名称
    public static string getOtherAbName(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath);
    }


    //atals资源路径
    private const string atlasPath = "Res/AssetBundle/Atlas";
    private const string texturesPath = "Res/AssetBundle/Textures";
    private const string atlasBundlePre = "AssetBundle/Atlas/";
    private const string atlasCfgName = "Res/AssetBundle/cfgs/atlasCfg.txt";
    [MenuItem("Assets/打包相关/导出图集配置", false, 6001)]
    public static void exportAtlasCfg()
    {
        Dictionary<string, string> map = new Dictionary<string, string>();
        //遍历atlas下面的所有文件夹
        List<string> floders = FileUtils.getAllFolder(Path.Combine(Application.dataPath, atlasPath));
        //存一个字典 key = iconName val = path(assetBundleName)
        for (int i = 0; i < floders.Count; i++)
        {
            string[] files = Directory.GetFiles(floders[i]);
            for (int k = 0; k < files.Length; k++)
            {
                if (files[k].EndsWith(".meta")) continue;
                if (files[k].EndsWith(".txt")) continue;
                string iconName = Path.GetFileNameWithoutExtension(files[k]);
                int index = floders[i].IndexOf("AssetBundle");
                string bundleName = floders[i].Substring(index);
                bundleName = bundleName.Replace("\\", "/").ToLower();
                map.Add(iconName, bundleName);
            }
        }
        //删除一下文件
        string cfg = Path.Combine(Application.dataPath, atlasCfgName);
        if (File.Exists(cfg))
        {
            File.Delete(cfg);
        }
        //开始写入  F:\GithubPro\demo\Assets\Res\AssetBundle\Atlas/common
        FileStream fs = File.Create(cfg);
        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
        {
            foreach (var item in map)
            {
                StringBuilder buder = new StringBuilder();
                buder.Append(item.Key);
                buder.Append(':');
                buder.Append(item.Value);
                sw.WriteLine(buder.ToString());
            }
        }
        fs.Close();
        fs.Dispose();
        AssetDatabase.Refresh();
    }

    private const string atlasInfoPath = "Res/AssetBundle/cfgs/atlasInfo.asset";
    [MenuItem("Assets/打包相关/导出图集配置ScriptTable", false, 6002)]
    public static void exportAtlasCfg2()
    {
        Dictionary<string, string> map = new Dictionary<string, string>();
        //遍历atlas下面的所有文件夹
        List<string> floders = FileUtils.getAllFolder(Path.Combine(Application.dataPath, atlasPath));
        addSpriteToMap(floders, map);
        //还有textures下面的散图
        List<string> flodersTextures = FileUtils.getAllFolder(Path.Combine(Application.dataPath, texturesPath));
        addSpriteToMap(flodersTextures, map, false);

        //删除一下文件
        string cfg = Path.Combine(Application.dataPath, atlasInfoPath);
        if (File.Exists(cfg))
        {
            File.Delete(cfg);
        }
        AtlasInfo info = ScriptableObject.CreateInstance<AtlasInfo>();
        info.keys.AddRange(map.Keys);
        info.abs.AddRange(map.Values);
        string path = Path.Combine("Assets", atlasInfoPath);
        AssetDatabase.CreateAsset(info, path);
        AssetDatabase.Refresh();
    }

    private static void addSpriteToMap(List<string> floders, Dictionary<string, string> map, bool isAtlas = true)
    {
        //存一个字典 key = iconName val = path(assetBundleName)
        for (int i = 0; i < floders.Count; i++)
        {
            string[] files = Directory.GetFiles(floders[i]);
            for (int k = 0; k < files.Length; k++)
            {
                if (files[k].EndsWith(".meta")) continue;
                if (files[k].EndsWith(".txt")) continue;
                string iconName = Path.GetFileNameWithoutExtension(files[k]);
                iconName = iconName.ToLower();
                int index = floders[i].IndexOf("AssetBundle");
                string bundleName = floders[i].Substring(index);
                bundleName = bundleName.Replace("\\", "/").ToLower();
                if (!isAtlas)
                {
                    bundleName = bundleName + "/" + iconName;
                }
                map.Add(iconName, bundleName);
            }
        }
    }

}

