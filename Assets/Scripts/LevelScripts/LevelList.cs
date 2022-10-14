using System.Collections.Generic;
using UnityEngine;

namespace LevelScripts
{
    [CreateAssetMenu(fileName = "LevelList", menuName = "ScriptableObjects/LevelList", order = 1)]
    public class LevelList : ScriptableObject
    {
        public List<Level> Levels = new List<Level>();
        [HideInInspector]
        public GameReplayType GameReplayType;

        [HideInInspector]
        public int StartIndex;
        [HideInInspector]
        public int EndIndex;

        public Level GetLevel(int index)
        {
            if (index < Levels.Count)
            {
                return Levels[index];
            }
            else
            {
                switch (GameReplayType)
                {
                    case GameReplayType.FromBeginning:
                        return Levels[index % Levels.Count];
                    case GameReplayType.LastLevel:
                        return Levels[Levels.Count - 1];
                    case GameReplayType.FromLevelIndex:
                        int mod = Levels.Count - StartIndex;
                        int i = StartIndex + ((index - Levels.Count) % mod);
                        return Levels[i];
                    case GameReplayType.RandomBetweenLevels:
                        i = Random.Range(StartIndex, EndIndex + 1);
                        return Levels[i];
                    default:
                        return Levels[index % Levels.Count];
                }
            }
        }
    }
    public enum GameReplayType
    {
        FromBeginning,
        LastLevel,
        FromLevelIndex,
        RandomBetweenLevels
    }
}