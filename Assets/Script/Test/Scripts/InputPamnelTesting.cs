// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using CHARACTERS;
//
// public class InputPamnelTesting : MonoBehaviour
// {
//     public InputPanel inputPanel;
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         StartCoroutine(Running());
//     }
//
//     IEnumerator Running()
//     {
//         Character Stella = CharacterManager.instance.CreateCharacter("Stella", revealAfterCreation: true);
//
//         yield return Stella.Say("Hi! What's your name?");
//
//         inputPanel.Show("What Is Your Name?");
//
//         while (inputPanel.isWaitingOnUserInput)
//             yield return null;
//
//         string characterName = inputPanel.lastInput;
//
//         yield return Stella.Say($"It's very nice to meet you, {characterName}!");
//     }
// }
