using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://www.bilibili.com/read/cv14521774/ Animator实现动画
/// </summary>
public static partial class Tools
{
    /// <summary>
    /// 动画添加事件
    /// 是因为动画切换的时候融合问题，把事件设置到融合时间内会不触发，
    /// 1.用别的办法，
    /// 2.把动画播放完退出的勾去掉
    /// 3把事件触发事件设置在融合时间之前。
    /// </summary>
    /// <param name="animator">动画机</param>
    /// <param name="eventName">事件名称</param>
    /// <param name="animationName"></param>
    /// <param name="parameter">执行方法后要传入的参数</param>
    /// <param name="frame">从左边开始为0 如果是秒0.02 就是第2帧</param>
    public static void AddAnimatorEvent(this Animator animator, string animationName, int frame, string eventName = default, System.Object parameter = default)
    {
        if (eventName == default)
            return;

        AnimationClip clip = null;
        foreach (var animationClip in animator.runtimeAnimatorController.animationClips)
        {
            if (!animationClip.name.Equals(animationName)) continue;
            clip = animationClip; // 设置目标动画剪辑
            break;
        }

        if (clip == null)
            throw new Exception($"没有找到动画片段{animationName}");
        AnimationEvent evt = new(); // 创建一个事件
        evt.functionName = eventName; // 绑定触发事件后要执行的方法名

        switch (parameter)
        {
            case String s:
                evt.stringParameter = s;
                break;
            case Int32 i:
                evt.intParameter = i;
                break;
            case Single o:
                evt.floatParameter = o;
                break;
        }

        evt.time = 1 / clip.frameRate * frame; // 设置事件关键帧的位置，当事件过了1.3秒后执行
        List<AnimationEvent> animationEvents = new List<AnimationEvent>(clip.events) { evt };
        clip.events = animationEvents.ToArray();
        //clip.AddEvent(evt); // 绑定事件此方法会使事件添加的顺序出现变化
    }

    /// <summary>
    /// 检查动画片段
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animationName"></param>
    /// <returns></returns>
    public static bool CheckClip(this Animator animator, string animationName)
    {
        foreach (var animationClip in animator.runtimeAnimatorController.animationClips)
        {
            if (!animationClip.name.Equals(animationName)) continue;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animationName"></param>
    /// <param name="eventName"></param>
    public static void UnAnimatorAddEvent(this Animator animator, string animationName, params string[] eventName)
    {
        //查找AnimationClip
        AnimationClip clip = null;
        foreach (var animationClip in animator.runtimeAnimatorController.animationClips)
        {
            if (!animationClip.name.Equals(animationName)) continue;
            clip = animationClip;
            break;
        }

        //移除
        List<AnimationEvent> events = new List<AnimationEvent>(clip.events);
        foreach (var str in eventName)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (str.Equals(events[i].functionName))
                {
                    events.Remove(events[i]);
                }
            }
        }

        //重新监听
        clip.events = events.ToArray();
    }

    /// <summary>
    /// 清空所有动画的事件
    /// </summary>
    /// <param name="animator"></param>
    public static void UnAnimatorAddEventAll(this Animator animator)
    {
        try
        {
            AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
            foreach (var animationClip in animationClips)
                animationClip.events = default;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 可以使用下边的方法在程序运行时找到当前播放的动画是否有我们查找的关键帧
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="mathName"></param>
    /// <returns></returns>
    public static bool CurClipHaveChangeHeroEvents(Animator animator, string mathName)
    {
        if (animator.GetCurrentAnimatorClipInfo(0).Length <= 0) return false;
        AnimationClip clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        if (clip == null || clip.events.Length <= 0) return false;
        AnimationEvent[] events = clip.events;
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i].functionName == mathName)
            {
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR

    /// <summary>
    /// 清空所有动画的事件(编辑器模式下)
    /// </summary>
    public static void UnAddAnimatorEventEditor(this Animator animator)
    {
        RuntimeAnimatorController controller = animator.runtimeAnimatorController; // 获取Animator Controller
        //移除
        if (controller == null) throw new Exception("未找到Animator里面的动画");
        foreach (AnimationClip clip in controller.animationClips)
            UnityEditor.AnimationUtility.SetAnimationEvents(clip, Array.Empty<AnimationEvent>()); // 移除动画片段上的所有事件
    }

    /// <summary>
    /// 添加所有动画的事件(编辑器模式下)
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="animationName"></param>
    /// <param name="frame"></param>
    /// <param name="eventName"></param>
    /// <param name="parameter"></param>
    public static void AddAnimatorEventEditor(this Animator animator, string animationName, int frame, string eventName = default, System.Object parameter = default)
    {
        //添加
        RuntimeAnimatorController controller = animator.runtimeAnimatorController; // 获取Animator Controller
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (!clip.name.Equals(animationName)) continue;
            AnimationEvent evt = new AnimationEvent();
            evt.functionName = eventName; // 绑定触发事件后要执行的方法名
            evt.time = 1 / clip.frameRate * frame; // 设置事件关键帧的位置，当事件过了1.3秒后执行
            // 获取动画片段上的事件列表
            AnimationEvent[] events = UnityEditor.AnimationUtility.GetAnimationEvents(clip);
            // 添加新的事件到事件列表
            UnityEditor.ArrayUtility.Add(ref events, evt);
            switch (parameter)
            {
                case String s:
                    evt.stringParameter = s;
                    break;
                case Int32 i:
                    evt.intParameter = i;
                    break;
                case Single o:
                    evt.floatParameter = o;
                    break;
            }

            // 设置动画片段上的事件列表
            UnityEditor.AnimationUtility.SetAnimationEvents(clip, events);
        }
    }
#endif
}