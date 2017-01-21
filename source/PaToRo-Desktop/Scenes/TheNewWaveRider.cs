﻿using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine.Components;
using Microsoft.Xna.Framework.Content;
using PaToRo_Desktop.Scenes.Funcs;

namespace PaToRo_Desktop.Scenes
{
    public class TheNewWaveRider : Entity, IHasPhysics
    {
        private readonly BaseGame game;

        private Texture2D halo;
        private Vector2 haloOrigin;

        private Texture2D part;
        private Vector2 partOrigin;

        private Color color;
        public Color BaseColor { get; private set; }

        public bool Active;

        public Physics Phy { get; private set; }

        private float initialRadius;
        public float Radius
        {
            get { return this.Phy.HitBox.Radius; }
            set { this.Phy.HitBox.Radius = value; }
        }
        public float Colliding;
        public static float POINTS_PER_FRAME = 0.2f;
        public float Points;

        public Level Level { get; set; }

        public int PlayerNum { get; private set; }
        public float RespawnTimerInSec { get; private set; }

        public static Color[] colors = new Color[] {
            new Color(0xCC, 0x00, 0x00), new Color(0x99, 0xFF, 0x00), new Color(0xFF, 0xCC, 0x00), new Color(0x33, 0x33, 0xFF)
        };

        public TheNewWaveRider(BaseGame game, int playerNum, float radius)
        {
            this.game = game;
            this.PlayerNum = playerNum;
            this.BaseColor = colors[playerNum % colors.Length];
            Phy = new Physics(radius, game);
            initialRadius = radius;
        }

        internal void LoadContent(ContentManager content)
        {
            halo = content.Load<Texture2D>("Images/halo");
            haloOrigin = new Vector2(halo.Width * 0.5f, halo.Height * 0.5f);

            part = content.Load<Texture2D>("Images/particle");
            partOrigin = new Vector2(part.Width * 0.5f, part.Height * 0.5f);
        }

        public void Spawn()
        {
            if (!Active)
            {
                Active = true;
                Phy.Pos.X = game.Screen.Width * 0.1f;
                Phy.Pos.Y = game.Screen.Height * 0.5f;
                Radius = initialRadius;
            }
        }

        public void Die()
        {
            if (Active)
            {
                Active = false;
                RespawnTimerInSec = 3;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Active)
            {
                var t = (float)gameTime.TotalGameTime.TotalSeconds;
                var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Colliding Viz
                if (Colliding <= 0)
                {
                    color = new Color(BaseColor.R, BaseColor.G, BaseColor.B, BaseColor.A);
                    //new Color(
                    //    BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t)),      // red
                    //    BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t + 1)),    // green
                    //    BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t + 2)),    // blue
                    //    1.0f);
                    Points += POINTS_PER_FRAME;
                }
                else
                {
                    Colliding -= delta;
                }

                // Outer Halo
                var scl = 2 * Radius / halo.Width;
                var scale = new Vector2(scl, scl);
                spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, 0, scale, color);


                // Dot
                spriteBatch.Draw(part, Phy.Pos, null, null, partOrigin, 0, null, color);

                // Halos
                float factor = BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.SawUp(t));
                scale.X = scale.Y = scl * factor;
                color.A = (byte)(255 * factor);
                spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, 0, scale, color);

                // Inner Halos
                factor = BaseFuncs.ToZeroOne(BaseFuncs.SawUp(2 + t * 2.6f));   // -> 0..1
                scale.X = scale.Y = scl * factor;
                color.A = (byte)(255 * factor);
                spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, 0, scale, color);

                factor = BaseFuncs.ToZeroOne(BaseFuncs.SawUp(0.5f + t * 1.4f));   // -> 0..1
                scale.X = scale.Y = scl * factor;
                color.A = (byte)(255 * factor);
                spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, 0, scale, color);
            }
        }

        internal override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Active)
            {
                Phy.Update(gameTime);

                // Ensure Physics Bounds
                if (Phy.Pos.X < Radius) Phy.Pos.X = Radius;
                if (Phy.Pos.X > game.Screen.Width - Radius) Phy.Pos.X = game.Screen.Width - Radius;
                if (Phy.Pos.Y < Radius) Phy.Pos.Y = Radius;
                if (Phy.Pos.Y > game.Screen.Height - Radius) Phy.Pos.Y = game.Screen.Height - Radius;

                if (Phy.Spd.X > 1200) Phy.Spd.X = 1200;
                if (Phy.Spd.X < -1200) Phy.Spd.X = -1200;
                if (Phy.Spd.Y > 1200) Phy.Spd.Y = 1200;
                if (Phy.Spd.Y < -1200) Phy.Spd.Y = -1200;

                // Check Collision with Level
                if (Level != null)
                {
                    var upper = Level.getUpperAt(Phy.Pos.X);
                    var lower = Level.getLowerAt(Phy.Pos.X);

                    if (Phy.Pos.Y < upper + Radius)
                    {
                        Phy.Spd.Y = 200f + 0.5f * Math.Abs(Phy.Spd.Y);
                        Phy.Pos.Y = upper + Radius + Phy.Spd.Y * delta;
                        Phy.Accel.Y = 0;
                        Collide(true);
                    }

                    if (Phy.Pos.Y > lower - Radius)
                    {
                        Phy.Spd.Y = -200f + 0.5f * -Math.Abs(Phy.Spd.Y);
                        Phy.Pos.Y = lower - Radius + Phy.Spd.Y * delta;
                        Phy.Accel.Y = 0;
                        Collide(false);
                    }
                }
            } else
            {
                RespawnTimerInSec -= delta;
                if (RespawnTimerInSec <= 0)
                {
                    Spawn();
                }
            }
        }

        public void Collide(bool upper)
        {
            // color = upper ? Color.Green : Color.Red;

            if (Colliding <= 0)
            {
                Colliding = 1.5f;

                game.Inputs.Player(0)?.Rumble(upper ? 0.5f : 0, upper ? 0 : 0.5f, 200);

                if (Radius <= 15.0f)
                {
                    Die();
                }
                else
                {
                    Radius -= 1.2f;
                }
            }
        }
    }
}
