using System;
using System.Collections;
using UnityEngine;

public class GraphicLayerTesting : BaseBehaviour
{
    public Texture Texture;

    private void Start()
    {
        //StartCoroutine(Running());
        StartCoroutine(RunningLayers());
    }

    private IEnumerator RunningLayers()
    {
        GraphicPanel panel = R.GraphicPanelSystem.GetPanel("Background");
        var layer0 = panel.GetLayer(0, true);
        var layer1 = panel.GetLayer(1, true);
        layer0.SetVideo(ConfigVideo.Nebula);
        layer1.SetTexture("Graphics/BG Images/SpaceshipInterior");
        yield return new WaitForSeconds(2);

        GraphicPanel cinematic = R.GraphicPanelSystem.GetPanel("Cinematic");
        GraphicLayer cinLayer = cinematic.GetLayer(0, true);
        Character Haruka = R.CharacterSystem.CreateCharacter("Haruka", true);
        yield return Haruka.Say("Let's take a 1ook at a picture on the cinematic layer.");
        cinLayer.SetTexture("Graphics/Gallery/pup");
        yield return R.DialogueSystem.Say("Narrator", "We truly don");
        
        cinLayer.Clear();
        yield return new WaitForSeconds(1);
        panel.Clear();
    }

    private IEnumerator Running()
    {
        GraphicPanel panel = R.GraphicPanelSystem.GetPanel("Background");
        GraphicLayer layer = panel.GetLayer(0, true);
        yield return new WaitForSeconds(1);
        //layer.SetTexture("Graphics/BG Images/A2", blendingTexture: Texture);
        layer.SetVideo(ConfigVideo.FantasyCity, blendingTexture: Texture);
        yield return new WaitForSeconds(1);
        layer.SetVideo(ConfigVideo.Nebula, blendingTexture: Texture);
        yield return new WaitForSeconds(3);
        layer.currentGraphic.FadeOut();
    }
}