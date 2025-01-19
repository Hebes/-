using System.Collections;
using UnityEngine;

public class AudioTest : BaseBehaviour
{
    private void Start()
    {
        StartCoroutine(Running());
    }

    private Character CreateCharacter(string name) => R.CharacterSystem.CreateCharacter(name);

    private IEnumerator Running()
    {
        Character_Sprite Haruka = CreateCharacter("Haruka") as Character_Sprite;
        Character me = CreateCharacter("Me");
        Haruka.Show();

        R.GraphicPanelSystem.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BGImages/villagenight");
        R.AudioSystem.PlayTrack("Audio/Ambience/RainyMood", 0);
        R.AudioSystem.PlayTrack("Audio/Music/Calm", 1, pitch: 0.7f);
        yield return Haruka.Say("We can have multiple channels for playing ambience as well as music!");
        yield break;
        //
        // yield return new WaitForSeconds(0.5f);
        // R.AudioSystem.PlaySoundEffect("Audio/SFX/RadioStatic", loop: true);
        //
        //
        // yield return me.Say("开启音乐");
        // yield return new WaitForSeconds(1f);
        //
        // R.AudioSystem.StopSoundEffect("RadioStatic");
        // R.AudioSystem.PlayVoice("Audio/Voices/wakeup");
        // Haruka.Say("关闭音乐");
        // yield return new WaitForSeconds(1f);
        // Haruka.Animate("Hop");
        // Haruka.TransitionSprite(Haruka.GetSprite("cry"));
        // Haruka.TransitionSprite(Haruka.GetSprite("mad_blink"), 1);
        // Haruka.Say("Yikes!");

        //
        // yield return new WaitForSeconds(1);
        // Character_Sprite Haruka = CreateCharacter("Haruka") as Character_Sprite;
        // Haruka.Show();
        // yield return R.DialogueSystem.Say("Narrator", "Can we see your ship?");
        // R.GraphicPanelSystem.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/A5");
        // R.AudioSystem.PlayTrack("Audio/Music/Comedy", startingVolume: 0.5f);
        // R.AudioSystem.PlayVoice("Audio/Voices/wakeup");
        // Haruka.SetSprite(Haruka.GetSprite("cry"), 0);
        // Haruka.SetSprite(Haruka.GetSprite("mad_blink"), 1);
        // Haruka.MoveToPosition(new Vector2(0.7f, 0), speed: 0.5f);
        // yield return Haruka.Say("Yes, of course!");
        //
        // yield return Haruka.Say("Let me show you the engine room.");
        // R.GraphicPanelSystem.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/EngineRoom");
        // R.AudioSystem.PlayTrack(ConfigMusic.Calm, volumeCap: 0.8f);
        //
        // yield return null;
    }
}