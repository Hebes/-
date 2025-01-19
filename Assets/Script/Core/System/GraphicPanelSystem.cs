using System;
using UnityEngine;

/// <summary>
/// 图形面板系统
/// </summary>
public class GraphicPanelSystem : SM<GraphicPanelSystem>
{
    public const float DEFAULT_TRANSITION_SPEED = 3f;
    [SerializeField] public GraphicPanel[] allPanels;

    private void Awake()
    {
        GameObject layersGo = GameObject.Find("Layers");
        allPanels = new GraphicPanel[layersGo.transform.childCount];
        allPanels[0] = new GraphicPanel
        {
            panelName = "Background",
            rootPanel = layersGo.transform.FindChildByName("01Background").gameObject,
        };
        allPanels[1] = new GraphicPanel
        {
            panelName = "Cinematic",
            rootPanel = layersGo.transform.FindChildByName("03Cinematic").gameObject,
        };
        allPanels[2] = new GraphicPanel
        {
            panelName = "Foreground",
            rootPanel = layersGo.transform.FindChildByName("05Foreground").gameObject,
        };
    }

    public GraphicPanel GetPanel(string name)
    {
        name = name.ToLower();
        foreach (var panel in allPanels)
        {
            if (panel.panelName.ToLower() == name)
                return panel;
        }

        return null;
    }
}