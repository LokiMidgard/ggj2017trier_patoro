﻿using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes.Generators
{
    public class UpDownGenerator : Generator
    {
        private readonly BaseGame game;

        int changecounter = 0;
        int BoxesDone = 0;
        int BoxesToDo = 30;
        int MaxBoxes = 50;

        float height = 0;
        bool Down = true;
        Random random;

        public UpDownGenerator(BaseGame game)
        {
            this.game = game;
            random = new Random();
        }

        public float GetUpper(float t)
        {
            BoxesDone++;
            if (BoxesDone - BoxesToDo == 0)
            {
                changecounter++;
                if (changecounter == 2)
                {
                    BoxesToDo = random.Next(MaxBoxes - 1) + 1;
                    changecounter = 0;
                }
                BoxesDone = 0;
                Down = !Down;
            }
            if (Down)
            {
                height += 10.0f;
            } else
            {
                height -= 10.0f;
            }
            return height + (float)Math.Sin(t*3) * 60;

        }

        public float GetLower(float t)
        {
            return height + 400 + (float)Math.Sin(t*3) * 60;
        }
    }
}
