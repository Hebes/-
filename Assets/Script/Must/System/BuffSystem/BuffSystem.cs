// using System;
// using System.Collections.Generic;
//
// public class BuffSystem : SingletonMono<BuffSystem>
// {
//     public const string LoadPath = "";
//     public const int LogicFrameIntervalMs = 66;//自定义的毫秒时间，如果未赋值，请修改成Time.deltaTime
//     private readonly List<Buff> _mBuffList = new List<Buff>();
//     
//     
//
//     public Buff AddBuff(int buffId)
//     {
//         if (buffId==0)
//             throw new Exception("Buff id 不能为0，当前附加Buff为无效buff");
//         Buff buff = new Buff(buffid,releaser,attachTarget,skill,paramsObjs);
//         buff.OnCreate();
//         _mBuffList.Add(buff);
//         return buff;
//     }
//
//     public void UnAddBuff(Buff buff)
//     {
//         if (_mBuffList.Contains(buff))
//             _mBuffList.Remove(buff);
//     }
//
//     public void OnLogicFrameUpdate()
//     {
//         for (int i = _mBuffList.Count - 1; i >= 0; i--)
//             _mBuffList[i].OnLogicFrameUpdate();
//     }
//
//     private void OnDestroy()
//     {
//         for (int i = _mBuffList.Count - 1; i >= 0; i--)
//             _mBuffList[i].OnDestroy();
//     }
//     
//     
// }