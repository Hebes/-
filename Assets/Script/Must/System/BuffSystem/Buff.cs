// using System;
// using UnityEngine;
//
// public enum BuffState
// {
//     None,
//     Delay, //延迟中
//     Start, //开始触发
//     Update, //Buff更新中
//     End, //buff结束
// }
//
// /// <summary>
// /// Buff类型
// /// </summary>
// public enum BuffType
// {
//     None,
// }
//
// /// <summary>
// /// 表示当前buff触发时所需要播放的动画
// /// </summary>
// public enum ObjectAnimationState
// {
//     None,
//     BeHit, //受击
//     Stiff, //僵直
// }
//
// public class Buff
// {
//     /// <summary>
//     /// Buff唯一id
//     /// </summary>
//     public readonly int Buffid;
//
//     /// <summary>
//     /// buff当前状态
//     /// </summary>
//     public BuffState BuffState;
//
//     /// <summary>
//     /// Buff所需要的一些参数
//     /// </summary>
//     public object[] ParamsObjs;
//
//     /// <summary>
//     /// buff配置
//     /// </summary>
//     public IBuffConfig BuffCfg { get; private set; }
//
//     /// <summary>
//     /// buff释放者
//     /// </summary>
//     public IBuffCharacter releaser;
//
//     /// <summary>
//     /// buff附加/攻击目标
//     /// </summary>
//     public IBuffCharacter attachTarget;
//
//     /// <summary>
//     /// 当前延迟时间
//     /// </summary>
//     private int _mCurDelayTime;
//
//     /// <summary>
//     /// Buff逻辑组合对象
//     /// </summary>
//     private BuffComposite _mBuffLogic;
//
//     /// <summary>
//     /// 当前真实运行时间
//     /// </summary>
//     private int mCurRealRuntime;
//     /// <summary>
//     /// 当前累计运行时间
//     /// </summary>
//     private int mAccRumTime;
//     public Buff(int buffId, IBuffCharacter releaser, IBuffCharacter attachTarget, object[] paramsObjs)
//     {
//         Buffid = buffId;
//         this.releaser = releaser;
//         this.attachTarget = attachTarget;
//         ParamsObjs = paramsObjs;
//     }
//
//     public void OnCreate()
//     {
//         ScriptableObject buffCfg = Load<ScriptableObject>(BuffSystem.LoadPath);
//         if (!buffCfg.GetType().IsSubclassOf(typeof(IBuffConfig)))
//             throw new Exception("请继承IBuffConfig接口");
//         //反射创建
//         BuffState = BuffCfg.BuffDelay == 0 ? BuffState.Start : BuffState.Delay;
//         _mCurDelayTime = BuffCfg.BuffDelay;
//     }
//
//     public void BuffStart()
//     {
//         CreateBuffEffect();
//         mBuffRender?.InitBuffRender(releaser, attachTarget, BuffCfg, skill.sKillGuidePos);
//         //1.调用buffStart接口
//         mBuffLogic.BuffStart();
//         attachTarget.AddBuff(this);
//     }
//
//
//     public void OnDestroy()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public void OnLogicFrameUpdate()
//     {
//         switch (BuffState)
//         {
//             case BuffState.Delay:
//                 if (_mCurDelayTime == BuffCfg.BuffDelay)
//                     _mBuffLogic.BuffDelay(); //处理BUff延迟逻辑
//                 _mCurDelayTime -= BuffSystem.LogicFrameIntervalMs;
//                 if (_mCurDelayTime <= 0)
//                     BuffState = BuffState.Start;
//                 break;
//             case BuffState.Start:
//                 BuffStart();
//                 BuffTrigger();
//                 //判断buff是否需要切换为更新状态，如果当前buff持续时间为有限或无限，才进入更新状态
//                 BuffState = BuffCfg.BuffDurationms is -1 or > 0 ? BuffState.Update : BuffState.End;
//                 break;
//             case BuffState.Update:
//                 UpdateBuffLogic();
//                 break;
//             case BuffState.End:
//                 OnDestroy();
//                 break;
//         }
//     }
//
//     private void UpdateBuffLogic()
//     {
//         int logicFrameintervalMs = BuffSystem.LogicFrameIntervalMs;
//         //1.处理buff间隔逻辑
//         if (BuffCfg.BuffIntervalms > 0)
//         {
//             mCurRealRuntime += logicFrameintervalMs;
//             if (mCurRealRuntime >= BuffCfg.BuffIntervalms)//当前累计运行时间是否大于buff触发间隔，如果大于就触发buff效果
//             {
//                 BuffTrigger();
//                 mCurRealRuntime -= BuffCfg.BuffIntervalms;
//             }
//         }
//         //处理当前Buff的持续时间,更新Buff持续时间
//         UpdateBuffDurationTime();
//     }
//
//     private void UpdateBuffDurationTime()
//     {
//         mAccRumTime += BuffSystem.LogicFrameIntervalMs;
//         if (mAccRumTime >= BuffCfg.BuffDurationms)
//             BuffState = BuffState.End;
//     }
//
//
//     private T Load<T>(string path) where T : UnityEngine.Object
//     {
//         return default;
//     }
//
//     private void CreateBuffEffect()
//     {
//     }
//
//     private void BuffTrigger()
//     {
//         _mBuffLogic.BuffTrigger();
//         attachTarget.PlayAnim(BuffCfg.BuffTriggerAnim);
//     }
// }