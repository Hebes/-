using System;
using System.Collections.Generic;

public class CommandDataBase
{
    private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();
    public bool HasCommand(string commandName) => database.ContainsKey(commandName.ToLower());

    public void AddCommand(string commandName, Delegate command)
    {
        commandName = commandName.ToLower();
        if (!database.TryAdd(commandName, command))
            $"命令在数据库中已经存在 {commandName}".LogError();
    }

    public Delegate GetCommand(string commandName)
    {
        commandName = commandName.ToLower();
        if (database.TryGetValue(commandName, out Delegate command)) 
            return command;
        $"命令 {commandName}在数据库中不存在!".LogError();
        return null;
    }
    
}