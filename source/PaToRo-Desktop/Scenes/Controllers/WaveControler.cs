using MonoGame.Extended.Shapes;
using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaToRo_Desktop.Scenes.Generators
{
    public class WaveControler : Entity
    {
        private readonly BaseGame game;

        const float paddleThikness = 10f;

        private float position;
        private float broadness;

        private float minbraodness;
        private float maxbraodness;
        private float distanceFromEdge;

        private float broadnessSpeed = 10;
        private float baseSpeed = 5;

        public WaveControler(BaseGame game)
        {
            this.game = game;

            minbraodness = 200;
            maxbraodness = 300;
            distanceFromEdge = 10f;

            Broadness = minbraodness + (maxbraodness - minbraodness) / 2f;
        }


        internal override void Update(GameTime gameTime)
        {
            var inputState = game.Inputs.Player(1);

            float broadnessChange = 0;
            float possitionChange = 0;


            if (inputState?.Provider is Engine.Input.XBoxController)
            {
                broadnessChange = inputState.Value(Engine.Input.Sliders.RightStickX) * broadnessSpeed;
                possitionChange = inputState.Value(Engine.Input.Sliders.LeftStickY) * baseSpeed * (Broadness / 120);
            }
            else if (inputState?.Provider is Engine.Input.KeyboardController)
            {
                broadnessChange += inputState.IsDown(Engine.Input.Buttons.X) ? broadnessSpeed : 0;
                broadnessChange += inputState.IsDown(Engine.Input.Buttons.A) ? -broadnessSpeed : 0;
                possitionChange += inputState.IsDown(Engine.Input.Buttons.DPad_Up) ? -baseSpeed * (Broadness / 120) : 0;
                possitionChange += inputState.IsDown(Engine.Input.Buttons.DPad_Down) ? baseSpeed * (Broadness / 120) : 0;
            }


            Broadness = MathHelper.Clamp(Broadness + broadnessChange, minbraodness, maxbraodness);
            var distanceFromCenter = game.Screen.Height / 2f - distanceFromEdge;
            Position = MathHelper.Clamp(Position + possitionChange, -(distanceFromCenter - Broadness / 2f), distanceFromCenter - Broadness / 2f);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            var left = Left;
            float top = Top;
            var height = this.Broadness;
            var width = paddleThikness;

            spriteBatch.FillRectangle(new RectangleF(left, top, width, height), Color.Red);
        }

        private float Top => Position - this.Broadness / 2f + game.Screen.Height / 2f;
        private float Bottom => Position + this.Broadness / 2f + game.Screen.Height / 2f;
        private float Left => game.Screen.Width - paddleThikness;
        private float Rigth => game.Screen.Width;

        public float Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public float Broadness
        {
            get
            {
                return broadness;
            }

            set
            {
                broadness = value;
            }
        }
    }
}
