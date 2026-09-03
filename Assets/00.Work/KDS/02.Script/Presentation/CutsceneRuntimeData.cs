using System;

namespace FollowMe.KDS
{
    [Serializable]
    public class CutsceneBeatJson
    {
        public string characterId;
        public string expressionId;
        public float durationSeconds = 1.5f;
    }

    [Serializable]
    public class CutsceneSequenceJson
    {
        public string sequenceId;
        public CutsceneBeatJson[] beats;
    }
}
