using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static partial class Tools
{
    /// <summary>
    /// 为EventTrigger的事件类型绑定Action方法
    /// </summary>
    /// <param name="trigger">EventTrigger组件对象</param>
    /// <param name="eventType">
    /// 事件类型
    /// PointerEnter：鼠标指针进入目标对象的事件。
    /// PointerExit：鼠标指针离开目标对象的事件。
    /// PointerDown：鼠标按下事件。
    /// PointerUp：鼠标释放事件。
    /// PointerClick：鼠标点击事件。
    /// Drag：拖拽事件。
    /// Drop：释放拖拽事件。
    /// Scroll：滚动事件。</param>
    /// <param name="listenedAction">要执行的方法</param>
    private static void AddEventTrigger(this EventTrigger trigger, EventTriggerType eventType, Action<PointerEventData> listenedAction)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(data => listenedAction.Invoke((PointerEventData)data));
        trigger.triggers.Add(entry);
    }

    public static void AddEventTrigger<T>(this T component, EventTriggerType et, Action<PointerEventData> ev) where T : UnityEngine.Component //, UnityEngine.Object 
    {
        EventTrigger t = component.gameObject.GetComponent<EventTrigger>();
        t ??= component.gameObject.AddComponent<EventTrigger>();
        t.AddEventTrigger(et, ev);
    }

    public static void AddEventTrigger(this GameObject goValue, EventTriggerType eventType, Action<PointerEventData> action)
    {
        var eventTrigger = goValue.GetComponent<EventTrigger>();
        eventTrigger ??= goValue.AddComponent<EventTrigger>();
        eventTrigger.AddEventTrigger(eventType, action);
    }

    public static void AddEventTrigger(this EventTrigger trigger, EventTriggerType eventType, IEnumerator routine)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(data => { trigger.StartCoroutine(routine); });
        trigger.triggers.Add(entry);
    }

    public static void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, string iEnumeratorRoutineName, Type type)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(data => { trigger.StartCoroutine($"{type.Name}{iEnumeratorRoutineName}", data); });
        trigger.triggers.Add(entry);
    }

    public static void RemoveEventTrigger(this EventTrigger trigger, EventTriggerType eventType, UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action);
        trigger.triggers.Remove(entry);
    }

    //寻找父物体
    public static Transform FindParentByName(this Transform value, string nameValue)
    {
        Transform parent = value.parent;
        return parent.name.Equals(nameValue) ? parent : FindParentByName(parent, nameValue);
    }

    public static T FindParentByName<T>(this Transform value, string nameValue) where T : UnityEngine.Object
    {
        return FindParentByName(value, nameValue).GetComponent<T>();
    }

    public static T FindParentByName<T>(this Transform value) where T : UnityEngine.Object
    {
        while (value != null)
        {
            T tValue = value.GetComponent<T>();
            if (tValue)
            {
                return tValue;
            }

            value = value.parent;
        }

        throw new Exception("没有找到组件");
    }

    //寻找子物体
    public static void FindChildList<T>(this Transform value, ref List<T> list) where T : UnityEngine.Object
    {
        T t = value.GetComponent<T>();
        if (t != null)
            list.Add(t);
        for (int i = 0; i < value.childCount; i++)
            FindChildList(value.GetChild(i), ref list);
    }

    /// <summary>
    /// 常用于在一个物体下大批量找按钮
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public static List<T> FindChildList<T>(this Transform value)where T : UnityEngine.Object
    {
        List<T> tList = new List<T>();
        for (int i = 0; i < value.childCount; i++)
        {
            var temp = value.GetChild(i).GetComponent<T>();
            if (temp!=null)
                tList.Add(temp);
        }

        return tList;
    }

    public static Transform FindComponent(this GameObject value, string nameValue)
    {
        return FindComponent(value.transform, nameValue);
    }

    public static Transform FindComponent(this Transform value, string nameValue)
    {
        Transform temp = value.Find(nameValue);
        if (temp != null)
            return temp;

        for (int i = 0; i < value.childCount; i++)
        {
            temp = FindComponent(value.GetChild(i), nameValue);
            if (temp != null) return temp;
        }

        return null;
    }

   
    public static T FindComponent<T>(this GameObject value) where T : UnityEngine.Object
    {
        return FindComponent<T>(value.transform);
    }

    public static T FindComponent<T>(this Transform value) where T : UnityEngine.Object
    {
        T t = value.GetComponent<T>();
        if (t != null) return t;
        for (int i = 0; i < value.childCount; i++)
        {
            t = FindComponent<T>(value.GetChild(i));
            if (t != null) return t;
        }

        return t;
    }

    public static T FindComponentByName<T>(this GameObject value, string valueName) where T : UnityEngine.Object
    {
        return FindComponentByName<T>(value.transform, valueName);
    }

    public static T FindComponentByName<T>(this Transform value, string valueName) where T : UnityEngine.Object
    {
        T t = value.GetComponent<T>();
        if (t != null && t.name.Equals(valueName)) return t;
        for (int i = 0; i < value.childCount; i++)
        {
            t = FindComponentByName<T>(value.GetChild(i), valueName);
            if (t != null && t.name.Equals(valueName)) return t;
        }

        return t;
    }
}