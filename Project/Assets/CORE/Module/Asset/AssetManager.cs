﻿/****************************************************************
 * File			: Assets\CORE\Module\Asset\AssetManager.cs
 * Author		: www.loywong.com
 * COPYRIGHT	: (C)
 * Date			: 2019/08/02
 * Description	: 加载所有资源类型（全部采用异步的方式！）
                1,复合资源：Prefab
                2,原始资源：比如：场景,贴图,动画,音效
 * Version		: 1.0
 * Maintain		: //[date] desc
 ****************************************************************/

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public enum Enum_AssetBundle {
    Lua,
    Panel,
    Model,
    Effect,
    Sound,
    Misc,
    // Font,
    // Atlas
}

public enum Enum_Asset_Prefab {
    Panel,
    Model,
    Effect,
}

public class AssetManager : ManagerBase<AssetManager> {
    private Dictionary<string, AssetBundle> luaAssets = new Dictionary<string, AssetBundle> ();

    // private Dictionary<string, AssetBundle> BundleCacheDict = new Dictionary<string, AssetBundle> ();
    /// lua bundle 索引,用于lua框架初始化脚本加载路径
    [HideInInspector]
    public List<string> luaBundleList = new List<string> ();

    ///应⽤程序内部资源路径
    public static string InAppAssetPath { get { return Application.streamingAssetsPath + "/"; } }
    ///应⽤程序外部可读写资源路径
    public static string OutAppAssetPath {
        get {
            if (Application.isMobilePlatform)
                return Application.persistentDataPath + "/Assets/";
            else
                return Application.dataPath.Replace ("Assets", "Assets_Persistent DataPath") + "/";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="abName"> Bundle文件名 lua.unity3d 需要带相对路径？？？</param>
    /// <param name="assetName">lua文件名称 需要带相对路径？？？</param>
    /// <returns></returns>
    public byte[] LoadLua (string scene, string abName, string assetName) {
        AssetBundle ab = null;
        luaAssets.TryGetValue (scene, out ab);
        if (ab == null) {
            // string toLuaScene = "Comn";
            ab = BundleLoader.ReadFile4Lua (scene, abName); //, assetName
            luaAssets.Add (scene, ab);
        }

        // 不需要从Temp目录里直接读.bytes文件！
        Log.Error ("[Asset] assetName: " + assetName);
        Debug.Log (ab);
        return ab.LoadAsset<TextAsset> (assetName).bytes;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 加载.prefab文件（异步）
    /// TODO 考虑 子类型：UI面板同步加载 其他子类型（model和effect）异步加载
    /// </summary>
    /// <param name="scene">根据场景加载相应的prefab,找不到,则从场景通用目录查找（Comn）,如果再找不到,则提示资源加载异常</param>
    /// <param name="prefabName"></param>
    /// <param name="prefabType"></param>
    /// <param name="cb"></param>
    public void LoadPrefab (string scene, string prefabName, Enum_Asset_Prefab prefabType, System.Action<GameObject> cb) {
        string filepath = "";
        // 编辑器状态运行时,不受文件名大小写的影响
        string folderName = "";

        if (!GameSetting.isBundle) {
            folderName = prefabType.ToString ();

            // 矫正没有.prefab后缀的Prefab文件
            if (!prefabName.EndsWith (".prefab"))
                prefabName += ".prefab";

            Log.Blue ("asset", "moduleName: " + scene);
            Log.Blue ("asset", "folderName: " + folderName);
            Log.Blue ("asset", "prefabName: " + prefabName);
            filepath = string.Format ("Assets/BIZ_Res/{0}/{1}/{2}", scene, folderName, prefabName);
            Log.Gray ("asset", "AssetManager LoadPrefab path is: " + filepath);
            GameObject go = null;
#if UNITY_EDITOR
            go = AssetDatabase.LoadAssetAtPath<GameObject> (filepath);
#endif
            if (cb != null)
                cb (go);
            return;
        }

        folderName = prefabType.ToString ().ToLower ();
        filepath = string.Format ("Assets/BIZ_Res/{0}/{1}/{2}", scene, folderName, prefabName + ".prefab"); //, AssetSetting.PrefabExtName

        // 和创建bundle时的规则一样！！！
        string abName = prefabName.ToLower () + ".unity3d";
        Debug.LogError ("@@@@@@@@@@ LoadPrefab abName: " + abName);

        BundleLoader.ReadFileAsync<GameObject> (scene, filepath, abName, cb);
    }

    /// <summary>
    /// 加载音频文件
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="filename"></param>
    /// <param name="cb"></param>
    public void LoadAudio (string scene, string filename, System.Action<AudioClip> cb) {
        string filepath = "";
        string folderName = "Sound";

        if (!GameSetting.isBundle) {
            filepath = string.Format ("Assets/BIZ_Res/{0}/{1}/{2}", scene, folderName, filename);
            AudioClip ac = null;
#if UNITY_EDITOR
            ac = AssetDatabase.LoadAssetAtPath<AudioClip> (filepath);
#endif
            if (cb != null)
                cb (ac);
            return;
        }

        string abName = filename.ToLower () + ".unity3d";
        filepath = string.Format ("Assets/BIZ_Res/{0}/{1}/{2}", scene, folderName, filename);
        BundleLoader.ReadFileAsync<AudioClip> (scene, filepath, abName, cb);
    }

    public static string CombineABName (string abName, int assetType) {
        return CombineAssetPartName (assetType) + "_" + abName + GameSetting.ExtName;
    }

    public static string CombineAssetPartName (int assetType) {
        return "asset_" + assetType.ToString ();
    }

    public T LoadAsset<T> (string abName, string assetName, int assetType = 0) where T : UnityEngine.Object {
        // Log.Gray ("asset", "LoadAsset isGameHideOrShow：" + Entry.Instance.isGameHideOrShow + " run In Background：" + Application.runInBackground);
        Log.Gray ("asset", "LoadAsset" + assetName);
        abName = AssetManager.CombineABName (abName, assetType).ToLower ();

        T loadedResource = null;
#if UNITY_EDITOR
        //???(在正式使⽤美术规范之前.这⾥会读到同⼀路径下的同名的资源)
        //在Editor的开发环境下这⾥⼀定会读取到资源(Resources资源除外)
        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (abName, assetName);
        foreach (var assetPathItem in assetPaths) {
            if (loadedResource == null) {
                loadedResource = AssetDatabase.LoadAssetAtPath<T> (assetPathItem);
            } else {
                break;
            }
        }
#endif

        if (loadedResource == null) {
            AssetBundle bundle = LoadAssetBundle (abName);
            if (bundle && bundle.Contains (assetName))
                loadedResource = bundle.LoadAsset<T> (assetName);
        }

        //加载不到资源尝试使⽤Resources.Load
        if (loadedResource == null)
            loadedResource = Resources.Load<T> (assetName);

        if (loadedResource == null) {
            if (typeof (T).Name == "Audio clip")
                Debug.LogWarning (string.Format ("资源加载失败：{0} /{1} /{2} ", assetType, abName, assetName));
            else {
#if UNITY_EDITOR
                Debug.LogError (string.Format ("资源加载失败：{0} /{1} /{2} ", assetType, abName, assetName));
#else
                Debug.LogWarning (string.Format ("资源加载夫败：{0} /{1} /{2} ", assetType, abName, assetName));
#endif
            }
        }

        return loadedResource;
    }

    // TODO
    public AssetBundle LoadAssetBundle (string abName) {
        abName = abName.ToLower ();
        AssetBundle bundle = null;
        //         string bundlePath = OutAppAssetPath + abName;

        //         if (File.Exists (bundlePath)) {
        //             if (BundleCacheDict.ContainsKey (abName))
        //                 BundleCacheDict.TryGetValue (abName, out bundle);
        //             else {
        //                 LoadDependencies (abName);
        //                 bundle = AssetBundle.LoadFromFile (bundlePath);
        //                 if (bundle)
        //                     BundleCacheDict.Add (abName, bundle);
        //                 //Debug.Log Warning("AssetManager{} Bundle缓存数量Bundle cacheD ict.count： "+Bundle CacheD ict.Count) ;
        //             }
        //         } else {
        // #if UNITY_EDITOR
        //             Debug.Log ("(可能是Resource下的资源.否则就是该资源没有更新AB名设置) 加载bundle失败.可忽略.资源加载失败会有红⾊⽇志：" + bundlePath);
        // #else
        //             Debug.Log("加载bundle失败.可忽略.资源加载失败会有红⾊⽇ 志：" + bundlePath);
        // #endif
        //         }

        return bundle;
    }
}