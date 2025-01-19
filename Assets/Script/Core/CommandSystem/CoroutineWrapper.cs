using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 协同程序包装
/// </summary>
public class CoroutineWrapper
{
    private MonoBehaviour owner;
    private Coroutine coroutine;
    public bool IsDone = false;
    public CoroutineWrapper(MonoBehaviour owner, Coroutine coroutine)
    {
        this.owner = owner;
        this.coroutine = coroutine;
    }

    public void Stop()
    {
        R.StopCoroutine(coroutine);
        IsDone = true;
    }
}