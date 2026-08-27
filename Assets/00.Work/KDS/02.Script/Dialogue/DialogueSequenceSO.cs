using System;
using System.Collections.Generic;
using UnityEngine;

namespace FollowMe.KDS
{
    [Serializable]
    public class DialogueLineSO
    {
        public string CharacterId = "Haru";
        public string ExpressionId = "Neutral";
        [TextArea(2, 5)] public string Text = "대사를 입력하세요.";
        public bool AutoAdvance;
        public float AutoAdvanceSeconds = 1.5f;
    }

    [CreateAssetMenu(
        fileName = "DialogueSequence_",
        menuName = "FollowMe/KDS/Dialogue Sequence",
        order = 1000)]
    public class DialogueSequenceSO : ScriptableObject
    {
        public string SequenceId = "Stage01_Intro";
        public string JsonFileName = "stage01_intro";
        public List<DialogueLineSO> Lines = new List<DialogueLineSO>();

        public DialogueSequenceJson ToRuntime()
        {
            int count = Lines != null ? Lines.Count : 0;
            var result = new DialogueSequenceJson
            {
                sequenceId = SequenceId,
                lines = new DialogueLineJson[count]
            };

            for (int i = 0; i < count; i++)
            {
                var line = Lines[i];
                result.lines[i] = new DialogueLineJson
                {
                    characterId = line.CharacterId,
                    expressionId = line.ExpressionId,
                    text = line.Text,
                    autoAdvance = line.AutoAdvance,
                    autoAdvanceSeconds = line.AutoAdvanceSeconds
                };
            }

            return result;
        }
    }
}
