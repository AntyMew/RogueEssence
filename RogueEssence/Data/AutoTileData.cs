﻿using System;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{
    [Serializable]
    public class AutoTileData : Dev.EditorData, IEntryData
    {
        public override string ToString()
        {
            return Name.DefaultText;
        }

        public LocalText Name { get; set; }
        public bool Released { get { return true; } }
        public string Comment { get; set; }

        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        public AutoTileBase Tiles;

        public AutoTileData()
        {
            Name = new LocalText();
            Comment = "";
        }

    }
}
