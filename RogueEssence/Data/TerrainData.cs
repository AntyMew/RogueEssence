﻿using System;
using RogueEssence.Dungeon;
using RogueElements;

namespace RogueEssence.Data
{
    [Serializable]
    public class TerrainData : Dev.EditorData, IEntryData
    {
        public override string ToString()
        {
            return Name.DefaultText;
        }

        public enum Mobility
        {
            Impassable = -1,
            Passable,
            Water,
            Lava,
            Abyss,
            Block
        }

        public LocalText Name { get; set; }
        public bool Released { get { return true; } }
        public string Comment { get; set; }

        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        [Dev.DataType(0, DataManager.DataType.Element, false)]
        public int Element;
        public Mobility BlockType;

        public bool BlockDiagonal;
        public bool BlockLight;
        public int ShadowType;

        public PriorityList<SingleCharEvent> LandedOnTiles;

        public TerrainData()
        {
            Name = new LocalText();
            Comment = "";
            LandedOnTiles = new PriorityList<SingleCharEvent>();
        }
    }
}
