﻿using System;
using RogueElements;

namespace RogueEssence.LevelGen
{
    [Serializable]
    public abstract class ZonePostProc : Dev.EditorData
    {
        /// <summary>
        /// Shallow copy + Initialize any runtime variables
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public abstract ZonePostProc Instantiate(ulong seed);
        public abstract void Apply(ZoneGenContext zoneContext, IGenContext context, StablePriorityQueue<int, IGenStep> queue);
    }
}
