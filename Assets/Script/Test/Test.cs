using System;
using System.Collections;
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

        Character Haruka = R.CharacterSystem.CreateCharacter("Haruka");

        StartCoroutine(TestMethod());
    }

    private IEnumerator TestMethod()
    {
        yield return null;
    }

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
    }
}