using System.Collections.Generic;
using UnityEngine;
using SFarm.Save;

public class NPCManager : Singleton<NPCManager>
{
    [Header("场景路线配置文件")]
    public SceneRouteDataList_SO sceneRouteData;

    [Header("NPC初始位置配置列表")]
    public List<NPCPosition> nPCPositionList;

    /// <summary>
    /// 场景路线缓存字典 key:from+goto value:SceneRoute
    /// </summary>
    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();


    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }

    private void OnEnable()
    {
        // 注册新游戏开始事件
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        // 取消事件注册（防止内存泄漏）
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    /// <summary>
    /// 新游戏开始：重置所有NPC位置和初始场景
    /// </summary>
    private void OnStartNewGameEvent(int gameIndex)
    {
        // 空值判断
        if (nPCPositionList == null || nPCPositionList.Count == 0)
        {
            Debug.LogWarning("NPC位置列表为空，请在Inspector面板配置！");
            return;
        }

        foreach (var character in nPCPositionList)
        {
            if (character.npc == null)
            {
                Debug.LogWarning("NPC对象未赋值，请检查配置！");
                continue;
            }

            // 重置位置
            character.npc.position = character.position;

            // 重置初始场景
            NPCMovement movement = character.npc.GetComponent<NPCMovement>();
            if (movement != null)
            {
                movement.StartScene = character.startScene;
            }
        }

        Debug.Log("所有NPC已重置到初始位置");
    }

    /// <summary>
    /// 初始化场景路线字典
    /// </summary>
    private void InitSceneRouteDict()
    {
        // 空值校验
        if (sceneRouteData == null || sceneRouteData.sceneRouteList == null || sceneRouteData.sceneRouteList.Count == 0)
        {
            Debug.LogError("场景路线数据未配置，请赋值SceneRouteDataList_SO！");
            return;
        }

        // 清空字典，避免重复加载
        sceneRouteDict.Clear();

        foreach (SceneRoute route in sceneRouteData.sceneRouteList)
        {
            string key = route.fromSceneName + route.gotoSceneName;

            // 去重添加
            if (!sceneRouteDict.ContainsKey(key))
            {
                sceneRouteDict.Add(key, route);
            }
        }

        Debug.Log($"场景路线字典初始化完成，共加载 {sceneRouteDict.Count} 条路线");
    }

    /// <summary>
    /// 安全获取两个场景之间的路线
    /// </summary>
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        string key = fromSceneName + gotoSceneName;

        // 安全查询，不存在则返回null，不报错
        if (sceneRouteDict.TryGetValue(key, out SceneRoute route))
        {
            return route;
        }
        else
        {
            Debug.LogError($"未找到场景路线：{fromSceneName} -> {gotoSceneName}");
            return null;
        }
    }
}