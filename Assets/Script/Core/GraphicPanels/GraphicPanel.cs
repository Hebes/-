using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GraphicPanel
{
    public string panelName;
    public GameObject rootPanel;
    public  List<GraphicLayer> Layers = new List<GraphicLayer>();
    public bool isClear => Layers == null || Layers.Count == 0 || Layers.All(layer => layer.currentGraphic == null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="layerDepth"></param>
    /// <param name="createIfDoesNotExist"></param>
    /// <returns></returns>
    public GraphicLayer GetLayer(int layerDepth, bool createIfDoesNotExist = false)
    {
        for (int i = 0; i < Layers.Count; i++)
        {
            if (Layers[i].layerDepth == layerDepth)
                return Layers[i];
        }

        return createIfDoesNotExist ? CreateLayer(layerDepth) : null;
    }

    private GraphicLayer CreateLayer(int layerDepth)
    {
        GraphicLayer layer = new GraphicLayer();
        GameObject panel = new GameObject(string.Format(GraphicLayer.LAYER_OBJECT_NAME_FORMAT, layerDepth));
        RectTransform rect = panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasGroup>();
        panel.transform.SetParent(rootPanel.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;
        layer.panel = panel.transform;
        layer.layerDepth = layerDepth;
        int index = Layers.FindIndex(l => l.layerDepth > layerDepth);
        if (index == -1)
            Layers.Add(layer);
        else
            Layers.Insert(index, layer);

        for (int i = 0; i < Layers.Count; i++)
            Layers[i].panel.SetSiblingIndex(Layers[i].layerDepth);
        return layer;
    }

    public void Clear(float transitionSpeed = 1, Texture blendTexture = null, bool immediate = false)
    {
        foreach (var layer in Layers)
            layer.Clear(transitionSpeed, blendTexture,immediate);
    }
}