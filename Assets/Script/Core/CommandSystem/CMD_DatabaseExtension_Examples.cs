using System;
using System.Collections;
using UnityEngine;

public class CMD_DatabaseExtension_Examples : CMD_DatabaseExtension
{
    public new static void Extend(CommandDataBase database)
    {
        //添加不带参数的Action
        database.AddCommand("print", new Action(PrintDefaultMessage));
        database.AddCommand("print_1p", new Action<string>(PrintUserMessage));
        database.AddCommand("print_mp", new Action<string[]>(PrintLines));
        //添加lambda -不带参数
        database.AddCommand("lambda", new Action(() => { "lambda".Log(); }));
        database.AddCommand("lambda_1p", new Action<string>(str => { $"lambda1p {str}".Log(); }));
        database.AddCommand("lambda_mp", new Action<string[]>(strArray => { $"lambda1p {string.Join(",", strArray)}".Log(); }));
        //添加无参数的协程
        database.AddCommand("process", new Func<IEnumerator>(SimpleProcess));
        database.AddCommand("process_1p", new Func<string,IEnumerator>(LineProcess));
        database.AddCommand("process_mp", new Func<string[],IEnumerator>(MultilineProcess));

        database.AddCommand("moveCharDemo", new Func<string, IEnumerator>(MoveCharacter));
    }

   

    private static void PrintDefaultMessage()
    {
        "将默认消息打印到控制台.".Log();
    }

    private static void PrintUserMessage(string message)
    {
        $"角色消息{message}".Log();
    }

    private static void PrintLines(params string[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            $"{i} : {messages[i]}.".Log();
        }
    }


    private static IEnumerator SimpleProcess()
    {
        for (int i = 1; i < 5; i++)
        {
            $"Process Running...[{i}]".Log();
            yield return new WaitForSeconds(1);
        }
    }

    private static IEnumerator LineProcess(string data)
    {
        if (int.TryParse(data,out int num))
        {
            for (int i = 1; i < num; i++)
            {
                $"precess Message '{num}'".Log();
                yield return new WaitForSeconds(1);
            }
            
        }
    }
    
    private static IEnumerator MultilineProcess(params string[] lines)
    {
        foreach (string line in lines)
        {
            $"precess Message '{line}'".Log();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private static IEnumerator MoveCharacter(string direction)
    {
        bool left = direction.ToLower() == "left";
        //Get the variables-I.need.This-would-be-defined somewhere else.
        Transform character = GameObject.Find("Image").transform;
        float moveSpeed = 15;
        // Calculate the target position-for the image
        float targetX = left ? -8 : 8;
        //Calculate the current position of the image
        float currentX = character.position.x;
        // Move the image gradually towards the target position
        while (Mathf.Abs(targetX - currentX) > 0.1f)
        {
            //$"Moving·character to{(left ? "left" : "right")}[{currentX}/{targetX}]".Log();
            currentX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
            character.position = new Vector3(currentX, character.position.y, character.position.z);
            yield return null;
        }
    }
}