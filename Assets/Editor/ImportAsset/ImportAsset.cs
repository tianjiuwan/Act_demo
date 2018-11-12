using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 资源导入
/// 修改导入的资源
/// </summary>
public class ImportAsset : AssetPostprocessor
{
    private const string atlasPath = "Assets/Res/AssetBundle/Atlas";
    private const string texturesPath = "Assets/Res/AssetBundle/Textures";

    /// <summary>
    /// 当导入一个texture
    /// 如果导入的是atlas里面的icon
    /// 如果导入的是textures里面的tex
    /// </summary>
    void OnPreprocessTexture()
    {
        if (this.assetPath.StartsWith(atlasPath))
        {
            //texImpoter是图片的Import Settings对象
            //AssetImporter是TextureImporter的基类
            TextureImporter texImpoter = assetImporter as TextureImporter;
            //TextureImporterType是结构体，包含所有Texture Type
            texImpoter.textureType = TextureImporterType.Sprite;
            texImpoter.spriteImportMode = SpriteImportMode.Single;
            texImpoter.spritePackingTag = getAtlasTag(this.assetPath);
        }
        if (this.assetPath.StartsWith(texturesPath))
        {
            TextureImporter texImpoter = assetImporter as TextureImporter;
            texImpoter.textureType = TextureImporterType.Sprite;
            texImpoter.spriteImportMode = SpriteImportMode.Single;
            //texImpoter.spritePackingTag = "";
        }
    }

    /*
    //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的
    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        Debug.Log("OnPostprocessAllAssets");
        foreach (string str in importedAsset)
        {
            Debug.Log("importedAsset = " + str);
        }
        foreach (string str in deletedAssets)
        {
            Debug.Log("deletedAssets = " + str);
        }
        foreach (string str in movedAssets)
        {
            Debug.Log("movedAssets = " + str);
        }
        foreach (string str in movedFromAssetPaths)
        {
            Debug.Log("movedFromAssetPaths = " + str);
        }
    }
    */

    //获取导入资源名称
    private string getNameByPath(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            string[] lst = path.Split('/');
            return lst[lst.Length - 1];
        }
        return "";
    }
    //获取图集tag
    private string getAtlasTag(string path)
    {
        string tag = "";
        string[] lst = path.Split('/');
        tag = lst[lst.Length - 2];
        return tag;
    }

}
