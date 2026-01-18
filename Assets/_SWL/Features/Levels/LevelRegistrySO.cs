using System;
using System.Collections.Generic;
using UnityEngine;
using SWL.Core.Domain.Levels;

namespace SWL.Features.Levels
{
    [CreateAssetMenu(menuName = "SWL/Levels/Level Registry", fileName = "LevelRegistry")]
    public sealed class LevelRegistrySO : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public LevelType Type;
            public GameObject Prefab; // must be a component that implements ILevelRunner
        }

        public List<Entry> Entries = new();

        public bool TryGet(LevelType type, out GameObject prefab)
        {
            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Type == type)
                {
                    prefab = Entries[i].Prefab;
                    return prefab != null;
                }
            }
            prefab = null;
            return false;
        }
    }
}
