﻿using System;
using System.Collections.Generic;
using RogueElements;
using Microsoft.Xna.Framework.Graphics;
#if EDITORS
using System.Windows.Forms;
using RogueEssence.Dungeon;
#endif

namespace RogueEssence.Content
{
    [Serializable]
    public abstract class BaseEmitter : Dev.EditorData
    {
        protected BaseEmitter()
        {
            Coverages = new bool[8];
        }

        [NonSerialized]
        protected Loc Origin;
        [NonSerialized]
        protected Loc Destination;
        [NonSerialized]
        protected Dir8 Dir;

        //TODO: origin/destination heights
        
        /// <summary>
        /// Used to keep track of where particles have previously spawned relative to the origin.  Used for balancing particle spawns.
        /// </summary>
        [NonSerialized]
        protected bool[] Coverages;

        public void SetupEmit(Loc origin, Loc dest, Dir8 dir) { Origin = origin; Destination = dest; Dir = dir; }
        public abstract void Update(BaseScene scene, FrameTick elapsedTime);

        public abstract BaseEmitter Clone();


        protected List<int> getOpenDirs()
        {
            List<int> openDirs = new List<int>();
            for (int jj = 0; jj < Coverages.Length; jj++)
            {
                if (!Coverages[jj] && !Coverages[(jj + 7) % 8] && !Coverages[(jj + 1) % 8])
                    openDirs.Add(jj);
            }
            if (openDirs.Count > 0)
                return openDirs;
            for (int jj = 0; jj < Coverages.Length; jj++)
            {
                if (!Coverages[jj])
                    openDirs.Add(jj);
            }
            if (openDirs.Count > 0)
                return openDirs;
            for (int jj = 0; jj < Coverages.Length; jj++)
            {
                Coverages[jj] = false;
                openDirs.Add(jj);
            }
            return openDirs;
        }



    }
    [Serializable]
    public abstract class EndingEmitter : BaseEmitter, IFinishableSprite
    {
        public Loc MapLoc { get { return Origin; } }
        public int LocHeight { get; set; }
        public abstract bool Finished { get; }

        public virtual void Draw(SpriteBatch spriteBatch, Loc offset) { }

        public virtual Loc GetDrawLoc(Loc offset) { return Origin - offset; }
        public virtual Loc GetDrawSize() { return new Loc(); }


#if EDITORS
        protected override void LoadClassControls(TableLayoutPanel control)
        {
            int initialHeight = control.Height;
            base.LoadClassControls(control);

            int totalHeight = control.Height - initialHeight;

            Button btnTest = new Button();
            btnTest.Name = "btnTest";
            btnTest.Dock = DockStyle.Fill;
            btnTest.Size = new System.Drawing.Size(0, 29);
            btnTest.TabIndex = 0;
            btnTest.Text = "Test";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += new System.EventHandler(btnTest_Click);
            control.Controls.Add(btnTest);
        }




        protected virtual void btnTest_Click(object sender, EventArgs e)
        {
            if (DungeonScene.Instance.ActiveTeam.Players.Count > 0 && DungeonScene.Instance.FocusedCharacter != null)
            {
                Character player = DungeonScene.Instance.FocusedCharacter;

                EndingEmitter data = (EndingEmitter)this.Clone();
                data.SaveClassControls((TableLayoutPanel)((Button)sender).Parent);
                data.SetupEmit(player.MapLoc, player.MapLoc, player.CharDir);
                DungeonScene.Instance.CreateAnim(data, DrawLayer.NoDraw);
            }
        }

#endif //WINDOWS

    }
    [Serializable]
    public abstract class FiniteEmitter : EndingEmitter, IEmittable
    {
        public IEmittable CloneIEmittable() { return (FiniteEmitter)Clone(); }

        public IEmittable CreateStatic(Loc mapLoc, int locHeight, Dir8 dir)
        {
            FiniteEmitter endingEmitter = (FiniteEmitter)Clone();
            endingEmitter.SetupEmit(mapLoc, mapLoc, dir);
            return endingEmitter;
        }
    }
    [Serializable]
    public class EmptyFiniteEmitter : FiniteEmitter
    {
        public override bool Finished { get { return true; } }
        public override BaseEmitter Clone() { return new EmptyFiniteEmitter(); }
        public override void Update(BaseScene scene, FrameTick elapsedTime) { }

        public override string ToString()
        {
            return "---";
        }
    }
    [Serializable]
    public abstract class CircleSquareEmitter : EndingEmitter
    {
        [NonSerialized]
        protected Dungeon.Hitbox.AreaLimit AreaLimit;


        /// <summary>
        /// In Pixels
        /// </summary>
        [NonSerialized]
        protected int Range;

        /// <summary>
        /// In Pixels Per Second
        /// </summary>
        [NonSerialized]
        protected int Speed;

        public void SetupEmit(Loc origin, Dir8 dir, Dungeon.Hitbox.AreaLimit areaLimit, int range, int speed)
        {
            SetupEmit(origin, origin, dir);
            AreaLimit = areaLimit;
            Range = range;
            Speed = speed;
        }


#if EDITORS
        protected override void btnTest_Click(object sender, EventArgs e)
        {
            if (DungeonScene.Instance.ActiveTeam.Players.Count > 0 && DungeonScene.Instance.FocusedCharacter != null)
            {
                Character player = DungeonScene.Instance.FocusedCharacter;

                CircleSquareEmitter data = (CircleSquareEmitter)this.Clone();
                data.SaveClassControls((TableLayoutPanel)((Button)sender).Parent);
                data.SetupEmit(player.MapLoc, player.CharDir, Hitbox.AreaLimit.Full, 2 * GraphicsManager.TileSize + GraphicsManager.TileSize / 2, 10 * GraphicsManager.TileSize);
                DungeonScene.Instance.CreateAnim(data, DrawLayer.NoDraw);
            }
        }

#endif //WINDOWS
    }
    [Serializable]
    public class EmptyCircleSquareEmitter : CircleSquareEmitter
    {
        public override bool Finished { get { return true; } }
        public override BaseEmitter Clone() { return new EmptyCircleSquareEmitter(); }
        public override void Update(BaseScene scene, FrameTick elapsedTime) { }

        public override string ToString()
        {
            return "---";
        }
    }
    [Serializable]
    public abstract class ShootingEmitter : EndingEmitter
    {
        [NonSerialized]
        protected int Range;
        [NonSerialized]
        protected int Speed;

        public void SetupEmit(Loc origin, Dir8 dir, int range, int speed)
        {
            SetupEmit(origin, origin, dir);
            Range = range;
            Speed = speed;
        }


#if EDITORS
        protected override void btnTest_Click(object sender, EventArgs e)
        {
            if (DungeonScene.Instance.ActiveTeam.Players.Count > 0 && DungeonScene.Instance.FocusedCharacter != null)
            {
                Character player = DungeonScene.Instance.FocusedCharacter;

                ShootingEmitter data = (ShootingEmitter)this.Clone();
                data.SaveClassControls((TableLayoutPanel)((Button)sender).Parent);
                data.SetupEmit(player.MapLoc, player.CharDir, 4 * GraphicsManager.TileSize + GraphicsManager.TileSize / 2, 10 * GraphicsManager.TileSize);
                DungeonScene.Instance.CreateAnim(data, DrawLayer.NoDraw);
            }
        }

#endif //WINDOWS
    }
    [Serializable]
    public class EmptyShootingEmitter : ShootingEmitter
    {
        public override bool Finished { get { return true; } }
        public override BaseEmitter Clone() { return new EmptyShootingEmitter(); }
        public override void Update(BaseScene scene, FrameTick elapsedTime) { }

        public override string ToString()
        {
            return "---";
        }
    }
    [Serializable]
    public abstract class AttachPointEmitter : BaseEmitter
    {
        [NonSerialized]
        public int LocHeight;
        public virtual void SetupEmit(ICharSprite user, Loc origin, Loc dest, Dir8 dir, int locHeight) { SetupEmit(origin, dest, dir); LocHeight = locHeight; }
    }
    [Serializable]
    public class EmptyAttachEmitter : AttachPointEmitter
    {
        public override BaseEmitter Clone() { return new EmptyAttachEmitter(); }
        public override void Update(BaseScene scene, FrameTick elapsedTime) { }

        public override string ToString()
        {
            return "---";
        }
    }

    [Serializable]
    public abstract class SwitchOffEmitter : EndingEmitter
    {
        public abstract void SwitchOff();
    }
    [Serializable]
    public class EmptySwitchOffEmitter : SwitchOffEmitter
    {
        public override bool Finished { get { return true; } }
        public override BaseEmitter Clone() { return new EmptySwitchOffEmitter(); }
        public override void Update(BaseScene scene, FrameTick elapsedTime) { }
        public override void SwitchOff() { }
    }
}
