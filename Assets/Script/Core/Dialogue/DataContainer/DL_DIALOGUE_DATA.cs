using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// DL=Dialogue 对话=>发言者内容
/// </summary>
public class DL_DIALOGUE_DATA
{
    public List<DIALOGUE_SEGMENT> segments;

    public DL_DIALOGUE_DATA(string rawDialogue)
    {
        segments = RipSegments(rawDialogue);
    }

    private List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
    {
        List<DIALOGUE_SEGMENT> segments = new List<DIALOGUE_SEGMENT>();
        MatchCollection matches = Regex.Matches(rawDialogue, ConfigString.SegmentIdentifierPattern);
        int lastIndex = 0;
        //查找filedialgue_segment中的第一个或唯一一个段
        DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
        segment.dialogue = (matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index));
        segment.startSignal = DIALOGUE_SEGMENT.StartSignal.NONE;
        segment.signalDelay = 0;
        segments.Add(segment);
        if (matches.Count == 0)
            return segments;
        lastIndex = matches[0].Index;
        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            segment = new DIALOGUE_SEGMENT();

            ///获取段的开始信号
            string signalMatch = match.Value; //{A}
            signalMatch = signalMatch.Substring(1, match.Length - 2);
            string[] signalSplit = signalMatch.Split();
            segment.startSignal = (DIALOGUE_SEGMENT.StartSignal)Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal), signalSplit[0].ToUpper());

            ///获取信号延迟
            if (signalSplit.Length > 1)
                float.TryParse(signalSplit[1], out segment.signalDelay);

            //获取该片段的对话。
            int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
            segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
            lastIndex = nextIndex;
            segments.Add(segment);
        }

        return segments;
    }

    /// <summary>
    /// 对话片段数据结构
    /// </summary>
    public struct DIALOGUE_SEGMENT
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string dialogue;

        /// <summary>
        /// 起始
        /// </summary>
        public StartSignal startSignal;

        /// <summary>
        /// 延迟
        /// </summary>
        public float signalDelay;

        /// <summary>
        /// 是否是追加内容
        /// </summary>
        public bool AppendText => startSignal is StartSignal.A or StartSignal.WA;

        public enum StartSignal
        {
            NONE,

            /// <summary>
            /// 点击屏幕触发下一句话 {c}
            /// </summary>
            C,

            /// <summary>
            /// 等待用户输入追加一句话 {a}
            /// </summary>
            A,

            /// <summary>
            /// 等待指定秒数自动触发下一句话 {wa}
            /// </summary>
            WA,

            /// <summary>
            /// 等待指定秒数点追加一句话 {wc}
            /// </summary>
            WC
        }
    }
}