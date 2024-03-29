﻿/****************************************************************
 * File			: Assets\CORE\Entry.cs
 * Author		: www.loywong.com
 * COPYRIGHT	: (C)
 * Date			: 2019/07/29
 * Description	: 程序逻辑入口
                1，加载所有数据
                2，设置程序运行环境 
                    固定配置：比如log是否开启，运行帧率，是否休眠==
                    Lua虚拟机
                3，VI Util Module Feature
                4，热更检测
                    更新：开始更新逻辑，等待更新完成，重新走启动流程！！！
                    不更新：调用Lua脚本开始逻辑
 * Version		: 2.0
 * Maintain		: [2020/04] 初始化运行程序的流程
                0，Env/INI 环境，框架
                1，Util
                2，Module
                3，Feature
                BIZ
                1，Data
                2，Start Logic
                    0，检测热更
                    1，有热更则热更，热更结束重新调用Entry的 Start方法
                        1 Data
                        2 Start Ingame（Lua）
                    2，Start Ingame（Lua）
 ****************************************************************/

using UnityEngine;

public class Entry : MonoBehaviour {
    public static Entry Instance { get; private set; }

#if UNITY_EDITOR
    // 走正式流程，需要首先检测热更新，否则开发状态下一旦热更功能被验证，则后续开发时可省略次步骤
    // 也就是说，发布模式下此属性值必然设置为True
    [SerializeField]
    private bool isHotUpdate = false;
    // 是否采用wss通信模式
    [SerializeField]
    private bool isWss = false;
    // 模拟后台执行，一般来说：真实移动设备上 都是默认 后台运行的，这里如果设置为false，是为了模拟息屏和后台停止运行状态！！！
    [SerializeField]
    private bool runInBackground = true;
#endif

    void Awake () {
        // 设置为切换场景不被销毁的属性
        GameObject.DontDestroyOnLoad (gameObject);

        Instance = this;

#if UNITY_EDITOR
        GameSetting.isWss = isWss;
#endif
    }

    /// <summary>
    /// 如果热更检测有效，热更之后需要重新调用此接口 重新初始化
    /// </summary>
    /// <param name="isHotUpdate">正常流程时 False，热更之后重新初始化的流程 True</param>
    void Start () {
        // 0 Env
        Debug.Log (">>>>>> Init Local Config");
        GameSetting.OnInitEngine ();

        // 启动游戏一定会经过一轮初始化
        Init_Module ();

        // 1 config
        GameSetting.OnInitData (AssetManager.Instance.LoadAsset<TextAsset> ("Configs", "GameConfig"));
        // LocaleManager.Instance.OnInit();
        GameSetting.OnInitDebug ();

#if UNITY_EDITOR
        Debug.Log ("Entry{} Start() EDIT Mode isHotUpdate == false");
#else
        // TODO 可能发布非热更模式包用于临时测试
        isHotUpdate = true;
#endif

        if (isHotUpdate) {
            AssetUpdate.Instance.OnStart (
                (progress) => {
                    // 更新进度中。。。
                },
                (hasValidUpdate) => {
                    if (hasValidUpdate) {
                        Debug.Log (">>>>>> Init Local Config Start");
                        // 热更之后，再一次初始化游戏设置配置信息！！！
                        GameSetting.OnInitData (AssetManager.Instance.LoadAsset<TextAsset> ("Configs", "GameConfig"));
                        // LocaleManager.Instance.OnInit();
                        GameSetting.OnInitDebug ();
                    }

                    Debug.Log ("Entry{} StartIngame() when hotupdate is over!");
                    StartIngame ();
                }
            );
        } else {
            Debug.Log ("Entry{} hotupdate disabled!");
            StartIngame ();
        }
    }

    private void Init_Module () {
        // AppFacade.Instance.OnInit();
        LuaManager.Instance.OnInit ();

        NetManager.Instance.OnInit ();
        InputManager.Instance.OnInit ();
        AssetUpdate.Instance.OnInit ();
    }

    // 不管哪种流程，此函数只执行一次
    private void StartIngame () {
        LuaManager.Instance.OnStart ();
        LuaManager.Instance.OnStart ("entry.lua");
        LuaManager.Instance.CallFunction ("entry.OnStart");

        // hasLuaStarted = true;
    }

#if UNITY_EDITOR
    void Update () {
        // TEST
        // if (Input.GetKeyDown (KeyCode.G)) {
        //     if (hasLuaStarted)
        //         LuaManager.Instance.CallFunction ("gamecontroller.KeyCode_G");
        // }
    }
#endif

    void OnApplicationFocus () {
        Debug.Log ("OnApplicationFocus");
    }
    void OnApplicationPause () {
        Debug.Log ("OnApplicationPause");
    }

    void OnApplicationQuit () {
        Debug.Log ("@@@@@@@@@@@ Quit Game @@@@@@@@@@@@@");
    }
}