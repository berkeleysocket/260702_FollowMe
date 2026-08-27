using System;

namespace FollowMe.KDS
{
    [Serializable]
    public class DialogueSequenceJson
    {
        public string sequenceId;
        public DialogueLineJson[] lines;
    }

    [Serializable]
    public class DialogueLineJson
    {
        public string characterId;
        public string expressionId;
        public string text;
        public bool autoAdvance;
        public float autoAdvanceSeconds;
    }
}
