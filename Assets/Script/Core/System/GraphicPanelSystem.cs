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
        Transform layersGo = R.UITotalRoot.FindComponent("Layers");
        allPanels = new GraphicPanel[3];
        allPanels[0] = new GraphicPanel
        {
            panelName = "Background",
            rootPanel = layersGo.FindComponent("01Background").gameObject,
        };
        allPanels[1] = new GraphicPanel
        {
            panelName = "Cinematic",
            rootPanel = layersGo.FindComponent("03Cinematic").gameObject,
        };
        allPanels[2] = new GraphicPanel
        {
            panelName = "Foreground",
            rootPanel = layersGo.FindComponent("05Foreground").gameObject,
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