using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// 切换边界
/// </summary>
public class SwitchBounds : MonoBehaviour
{
    //TODO:切换场景后更改调用
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SwitchConfinerShape;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchConfinerShape;
    }
    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        Debug.Log(confinerShape.name);
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        Debug.Log(confiner.name);

        confiner.m_BoundingShape2D = confinerShape;

        Debug.Log(confiner.m_BoundingShape2D.name);
        //清除缓存
        confiner.InvalidatePathCache();
    }
}
