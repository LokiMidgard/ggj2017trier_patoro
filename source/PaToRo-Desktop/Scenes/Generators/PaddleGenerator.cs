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
    public class PaddleGenerator : Entity, Generator
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

        public PaddleGenerator(BaseGame game)
        {
            this.game = game;

            minbraodness = 120;
            maxbraodness = 400;
            distanceFromEdge = 10f;

            broadness = minbraodness + (maxbraodness - minbraodness) / 2f;
        }

        public float GetUpper(float t) => Top;
        public float GetLower(float t) => Bottom;

        internal override void Update(GameTime gameTime)
        {
            var inputState = game.Inputs.Player(1);

            float broadnessChange = 0;
            float possitionChange = 0;


            if (inputState?.Provider is Engine.Input.XBoxController)
            {
                broadnessChange = inputState.Value(Engine.Input.Sliders.RightStickX) * broadnessSpeed;
                possitionChange = inputState.Value(Engine.Input.Sliders.LeftStickY) * baseSpeed * (broadness / 120);
            }
            else if (inputState?.Provider is Engine.Input.KeyboardController)
            {
                broadnessChange += inputState.IsDown(Engine.Input.Buttons.X) ? broadnessSpeed : 0;
                broadnessChange += inputState.IsDown(Engine.Input.Buttons.A) ? -broadnessSpeed : 0;
                possitionChange += inputState.IsDown(Engine.Input.Buttons.DPad_Up) ? -baseSpeed * (broadness / 120) : 0;
                possitionChange += inputState.IsDown(Engine.Input.Buttons.DPad_Down) ? baseSpeed * (broadness / 120) : 0;
            }


            broadness = MathHelper.Clamp(broadness + broadnessChange, minbraodness, maxbraodness);
            var distanceFromCenter = game.Screen.Height / 2f - distanceFromEdge;
            position = MathHelper.Clamp(position + possitionChange, -(distanceFromCenter - broadness / 2f), distanceFromCenter - broadness / 2f);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            var left = Left;
            float top = Top;
            var height = this.broadness;
            var width = paddleThikness;

            spriteBatch.FillRectangle(new RectangleF(left, top, width, height), Color.Red);
        }

        private float Top => position - this.broadness / 2f + game.Screen.Height / 2f;
        private float Bottom => position + this.broadness / 2f + game.Screen.Height / 2f;
        private float Left => game.Screen.Width - paddleThikness;
        private float Rigth => game.Screen.Width;



    }
}
