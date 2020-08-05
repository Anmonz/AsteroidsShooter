using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.AndryKram
{
    /// <summary>
    /// Содержит список всех уровней которые будут в игре
    /// </summary>
    [CreateAssetMenu(fileName = "LevelsData", menuName = "ScriptableObjects/LevelsScriptableObject", order = 0)]
    public class LevelsScriptableObject : ScriptableObject
    {
        [SerializeField] private List<LevelScriptableObject> _levels;//список уровней

        /// <summary>
        /// Передает уровень по индексу
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LevelScriptableObject GetLevel(int index)
        {
            return this._levels[index];
        }

        /// <summary>
        /// Передает количество уровней
        /// </summary>
        /// <returns></returns>
        public int GetCountLevels()
        {
            return this._levels.Count;
        }
    }
}
