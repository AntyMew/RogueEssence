﻿using System;
using System.Collections.Generic;
using RogueElements;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;

namespace RogueEssence.Dungeon
{
    [Serializable]
    public class AutoTile : Dev.EditorData
    {
        public List<TileLayer> Layers;

        public int AutoTileset { get; private set; }
        public int BorderTileset { get; private set; }

        public AutoTile(params TileLayer[] layers)
        {
            Layers = new List<TileLayer>();
            Layers.AddRange(layers);
            AutoTileset = -1;
            BorderTileset = -1;
        }

        public AutoTile(int autotile, int bordertile)
        {
            Layers = new List<TileLayer>();
            AutoTileset = autotile;
            BorderTileset = bordertile;
        }
        protected AutoTile(AutoTile other) : this()
        {
            foreach (TileLayer layer in other.Layers)
                Layers.Add(new TileLayer(layer));
            AutoTileset = other.AutoTileset;
            BorderTileset = other.BorderTileset;
        }
        public AutoTile Copy() { return new AutoTile(this); }

        public void Draw(SpriteBatch spriteBatch, Loc pos)
        {
            foreach (TileLayer anim in Layers)
                anim.Draw(spriteBatch, pos, GraphicsManager.TotalFrameTick);
        }

    }
}
