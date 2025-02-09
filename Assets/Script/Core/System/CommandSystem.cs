using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 指令管理器
/// </summary>
[NoDontDestroyOnLoad]
public class CommandSystem : SM<CommandSystem>
{
    private const char SUB_COMMAND_IDENTIFIER = '.';
    public const string DATABASE_CHARACTERS_BASE = "characters";
    public const string DATABASE_CHARACTERS_SPRITE = "characters_sprite";
    public const string DATABASE_CHARACTERS_LIVE2D = "characters_live2D";
    public const string DATABASE_CHARACTERS_Model3D = "characters_model3D";

    private readonly CommandDataBase _dataBase = new CommandDataBase();
    private static Coroutine _process;
    public static bool IsRunningProcess => _process != null;
    private List<CommandProcess> activeProcesses = new List<CommandProcess>();
    private Dictionary<string, CommandDataBase> subDatabases = new Dictionary<string, CommandDataBase>();
    private CommandProcess topProcess => activeProcesses.Last();


    private void Awake()
    {
        I = this;
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] extensionTypes = assembly.GetTypes().Where(t =>
            t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();
        foreach (Type extension in extensionTypes)
        {
            MethodInfo extendMethod = extension.GetMethod("Extend");
            extendMethod?.Invoke(null, new object[] { _dataBase });
        }
    }

    public CoroutineWrapper Extend(string commandName, params string[] messages)
    {
        if (commandName.Contains(SUB_COMMAND_IDENTIFIER))//判断命令中是否有点符号
            return ExecuteSubcommand(commandName, messages);

        Delegate command = _dataBase.GetCommand(commandName);

        if (command == null)
            return null;

        return StartProcess(commandName, command, messages);
    }

    private CoroutineWrapper ExecuteSubcommand(string commandName, string[] args)
    {
        string[] parts = commandName.Split(SUB_COMMAND_IDENTIFIER);
        string databaseName = string.Join(SUB_COMMAND_IDENTIFIER, parts.Take(parts.Length - 1));
        string subCommandName = parts.Last();
        if (subDatabases.ContainsKey(databaseName))
        {
            Delegate command = subDatabases[databaseName].GetCommand(subCommandName);
            if (command != null)
            {
                return StartProcess(commandName, command, args);
            }
            else
            {
                Debug.LogError($"没有命令调用 {subCommandName}是在子数据库中找到的吗 '{databaseName}'");
                return null;
            }
        }

        string characterName = databaseName;
        //如果我们把它放在这里，那么我们应该尝试作为字符命令运行
        if (R.CharacterSystem.HasCharacter(databaseName))
        {
            List<string> newArgs = new List<string>(args);
            newArgs.Insert(0, characterName);
            args = newArgs.ToArray();
            return ExecuteCharacterCommand(subCommandName, args);
        }

        Debug.LogError($"没有子数据库调用 {databaseName} 存在的命令 ‘{subCommandName}’无法运行.");
        return null;
    }

    private CoroutineWrapper ExecuteCharacterCommand(string commandName, params string[] args)
    {
        Delegate command = null;
        CommandDataBase db = subDatabases[DATABASE_CHARACTERS_BASE];
        if (db.HasCommand(commandName))
        {
            command = db.GetCommand(commandName);
            return StartProcess(commandName, command, args);
        }

        CharacterConfigData characterConfigData = R.CharacterSystem.GetCharacterConfig(args[0]);
        switch (characterConfigData.characterType)
        {
            case Character.CharacterType.Text:
                break;
            case Character.CharacterType.Sprite:
            case Character.CharacterType.SpriteSheet:
                db = subDatabases[DATABASE_CHARACTERS_SPRITE];
                break;
            case Character.CharacterType.Live2D:
                db = subDatabases[DATABASE_CHARACTERS_LIVE2D];
                break;
            case Character.CharacterType.Model3D:
                db = subDatabases[DATABASE_CHARACTERS_Model3D];
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        command = db.GetCommand(commandName);
        if (command != null)
            return StartProcess(commandName, command, args);
        Debug.LogError($"命令管理器无法执行命令 '{commandName}'在角色 '{args[0]}.角色名称或命令可能无效.");
        return null;
    }

    private CoroutineWrapper StartProcess(string commandName, Delegate command, params string[] messages)
    {
        System.Guid processID = System.Guid.NewGuid();
        CommandProcess cmd = new CommandProcess(processID, commandName, command, null, messages, null);
        activeProcesses.Add(cmd);
        Coroutine co = R.StartCoroutine(RunningProcess(cmd));
        cmd.runningProcess = new CoroutineWrapper(this, co);
        return cmd.runningProcess;
    }

    private IEnumerator RunningProcess(CommandProcess process)
    {
        yield return WaitingForProcessToComplete(process.command, process.args);
        KillProcess(process);
    }

    private void KillProcess(CommandProcess cmd)
    {
        activeProcesses.Remove(cmd);
        if (cmd.runningProcess != null && !cmd.runningProcess.IsDone)
            cmd.runningProcess.Stop();

        cmd.onTerminateAction?.Invoke();
    }

    private IEnumerator WaitingForProcessToComplete(Delegate command, string[] messages)
    {
        switch (command)
        {
            case Action:
                command.DynamicInvoke();
                break;
            case Action<string>:
                command.DynamicInvoke(messages.Length == 0 ? string.Empty : messages[0]);
                break;
            case Action<string[]>:
                command.DynamicInvoke((object)messages);
                break;
            case Func<IEnumerator> func:
                yield return func();
                break;
            case Func<string, IEnumerator> func:
                yield return func(messages.Length == 0 ? string.Empty : messages[0]);
                break;
            case Func<string[], IEnumerator> func:
                yield return func(messages);
                break;
        }
    }


    public void StopCurrentProcess()
    {
        if (topProcess != null)
            KillProcess(topProcess);
    }

    public void StopAllProcess()
    {
        foreach (var c in activeProcesses)
        {
            if (c.runningProcess != null && !c.runningProcess.IsDone)
                c.runningProcess.Stop();
            c.onTerminateAction?.Invoke();
        }

        activeProcesses.Clear();
    }

    /// <summary>
    /// 向当前进程添加终止操作
    /// </summary>
    /// <param name="action"></param>
    public void AddTerminationActionToCurrentProcess(UnityAction action)
    {
        CommandProcess process = topProcess;
        if (process == null)
            return;
        process.onTerminateAction = new UnityEvent();
        process.onTerminateAction.AddListener(action);
    }

    public CommandDataBase CreateSubDatabase(string name)
    {
        name = name.ToLower();
        if (subDatabases.TryGetValue(name, out CommandDataBase db))
        {
            Debug.LogWarning($"在数据中的名字：'{name}'已经存在!");
            return db;
        }

        CommandDataBase newDatabase = new CommandDataBase();
        subDatabases.Add(name, newDatabase);
        return newDatabase;
    }
}