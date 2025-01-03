using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 指令管理器
/// </summary>
public class CommandSystem : SingletonMono<CommandSystem>
{
    private readonly CommandDataBase _dataBase = new CommandDataBase();
    private static Coroutine _process;
    public static bool IsRunningProcess => _process != null;


    private void Awake()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] extensionTypes = assembly.GetTypes().Where(t =>
            t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();
        foreach (Type extension in extensionTypes)
        {
            MethodInfo extendMethod = extension.GetMethod("Extend");
            extendMethod?.Invoke(null, new object[] { _dataBase });
        }
    }

    public Coroutine Extend(string commandName, params string[] messages)
    {
        Delegate command = _dataBase.GetCommand(commandName);
        if (command == null) return null;
        _process = StartCoroutine(StartProcess(command, messages));
        return _process;
    }

    private IEnumerator StartProcess(Delegate command, params string[] messages)
    {
        StopCurrentProcess();
        switch (command)
        {
            case Action:
                command.DynamicInvoke();
                break;
            case Action<string>:
                command.DynamicInvoke(messages[0]);
                break;
            case Action<string[]>:
                command.DynamicInvoke((object)messages);
                break;
            case Func<IEnumerator> func:
                yield return func();
                break;
            case Func<string, IEnumerator> func:
                yield return func(messages[0]);
                break;
            case Func<string[], IEnumerator> func:
                yield return func(messages);
                break;
        }

        _process = null;
    }


    private void StopCurrentProcess()
    {
        if (_process != null)
            StopCoroutine(_process);
        _process = null;
    }
}