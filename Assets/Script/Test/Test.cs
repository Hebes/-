using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : BaseBehaviour
{
    private void Awake()
    {
        // CommandSystem.I.Extend("print");
        // CommandSystem.I.Extend("print_1p", "print_1p1");
        // CommandSystem.I.Extend("print_mp", "print_mp1", "print_mp2", "print_mp3");
        //
        // CommandSystem.I.Extend("lambda");
        // CommandSystem.I.Extend("lambda_1p", "lambda_1p1");
        // CommandSystem.I.Extend("lambda_mp", "lambda_mp1", "lambda_mp2", "lambda_mp3");
        //
        // CommandSystem.I.Extend("process");
        // CommandSystem.I.Extend("process_1p", "process_1p1");
        // CommandSystem.I.Extend("process_mp", "process_mp1", "process_mp2", "process_mp3");
    }

    private void Start()
    {
        StartCoroutine(Test1());
    }

    private IEnumerator Test1()
    {
        List<string> lines = new List<string>
        {
            "This is line 1 from the original conversation.",
            "This is line 2 from the original conversation.",
            "This is line 3 from·the·original conversation",
        };
        yield return R. DialogueSystem.Say(lines);
        R. DialogueSystem.Hide();
    }

    private IEnumerator ChoicePanel()
    {
        string[] choices = new string[]
        {
            "Witness? Is that camera on?",
            "Oh, nah!",
            "I·didn't see nothin'!",
            "Matta' Fact-·I'm blind in·my·left eye and 43% blind·in·my right eye",
        };
        R.ChoicePanelSystem.Show("Did You Witness Anything Strange?", choices);
        yield return null;
    }

    private IEnumerator InputPamme()
    {
        Character Haruka = R.CharacterSystem.CreateCharacter("Haruka", revealAfterCreation: true);
        yield return Haruka.Say("Hi!-What's·your name?");
        R.InputPanelSystem.Show("What Is Your Name?");
        while (R.InputPanelSystem.IsWaitingonUserInput)
            yield return null;
        string characterName = R.InputPanelSystem.LastInput;
        yield return Haruka.Say($"It's very-nice·to meet you!{characterName}");
    }

    private IEnumerator TestMethod()
    {
        Character_Sprite guard1 = CreateCharacter("Haruka") as Character_Sprite;
        Character_Sprite guard2 = CreateCharacter("Guard as Generic") as Character_Sprite;
        Character_Sprite guard3 = CreateCharacter("Raelin") as Character_Sprite;
        guard1.SetPosition(new Vector2(0.3f, 0));
        guard2.SetPosition(new Vector2(0.5f, 0));
        guard3.SetPosition(new Vector2(0.3f, 0));


        // guard2.SetColor(Color.red);
        //
        // guard1.SetPriority(1000);
        // guard2.SetPriority(15);
        // guard3.SetPriority(8);
        // yield return new WaitForSeconds(1f);
        // guard1.SetPriority(1);


        // R.CharacterSystem.SortCharacters("Raelin", "Haruka");
        //
        // yield return new WaitForSeconds(3f);
        // R.CharacterSystem.SortCharacters("Haruka", "Raelin");
        // R.CharacterSystem.SortCharacters();


        guard1.Animate("Hop", true);
        guard3.Animate("Shiver", false);


        // yield return new WaitForSeconds(1f);
        // yield return guard1.UnHighlight();
        // yield return new WaitForSeconds(1f);
        // yield return guard1.TransitionColor(Color.red, speed: 0.3f);
        // yield return new WaitForSeconds(1f);
        // yield return guard1.Highlight();
        // yield return new WaitForSeconds(1f);
        // yield return guard1.TransitionColor(Color.white, speed: 0.3f);
        //
        // yield return new WaitForSeconds(1f);
        // guard1.Flip();
        // yield return guard1.FaceRight();
        // yield return guard1.FaceLeft();

        //Character_Sprite guard1 = CreateCharacter("Guard as Generic") as Character_Sprite;
        // Character guard3 = CreateCharacter("Guard3 as Generic");
        //guard1.SetPosition(Vector2.zero);
        // guard2.SetPosition(new Vector2(0.5f,0.5f));
        // guard3.SetPosition(Vector2.one); 
        //guard1.Show();
        //guard1.isVisible = false;
        //Sprite body = guard1.GetSprite("cry");
        // guard1.TransitionSprite(body);
        // yield return guard1.TransitionColor(Color.red, speed:0.3f);
        // yield return guard1.TransitionColor(Color.blue, speed:0.3f);
        // yield return guard1.TransitionColor(Color.yellow, speed:0.3f);
        // yield return guard1.TransitionColor(Color.white, speed:0.3f);

        // yield return new WaitForSeconds(5f);
        // Sprite face = guard1.GetSprite("smile_blink");
        // // Sprite face = guard1.GetSprite("mad_blink");
        //  yield return guard1.TransitionSprite(face,1);
        // yield return new WaitForSeconds(5f);
        //  Sprite face1 = guard1.GetSprite("mad_blink");
        // yield return guard1.TransitionSprite(face1,1);
        // yield return guard1.MoveToPosition(Vector2.one, smooth: true);
        // yield return guard1.MoveToPosition(Vector2.zero, smooth: true);

        yield return null;
    }

    private Character CreateCharacter(string name) => R.CharacterSystem.CreateCharacter(name);

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     CommandSystem.I.Extend("moveCharDemo","left");
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     CommandSystem.I.Extend("moveCharDemo","right");
        // }


        if (Input.GetKeyDown(KeyCode.A))
            R.UISystem.UIDialogue.Hide();
        else if (Input.GetKeyDown(KeyCode.S))
            R.UISystem.UIDialogue.Show();
    }
}