using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{

    class Oven : Wheel
    {

        const int SIX_COMBO = 5000;
        const int FIVE_COMBO = 3000;
        const int FOUR_COMBO = 2000;
        const int THREE_COMBO = 1000;
        const int STRAIGHT_COMBO = 500;
        const int VERTICAL_COMBO = 250;
        const int EACH_CUPCAKE = 50;

        public int Heat
        {
            get { return heat; }
        }
        int heat;

        public int Score
        {
            get { return score; }
        }
        int score;

        String comboMessages;

      

        public Oven(Vector2 pos, Vector2 dest, Vector2 vel, float rot, Texture2D text, int containerCount)
            : base(pos, dest, vel, rot, text, containerCount)
        {

            heat = 0;
            score = 0;
            comboMessages = "";
        }


        public new bool takeCupcake(Actor cupcake)
        {
            if (cupcake != null)
            {
                for (int i = 0; i < StoredCupcakes.Count(); i++)
                {
                    if (StoredCupcakes[i] == null)
                    {
                        float tempX = this.Position.X;
                        float tempY = this.Position.Y + 64;
                        tempX = tempX + (i % 3) * 64;
                        tempY = tempY + (float)Math.Floor((double)(i / 3)) * 64;

                        cupcake.Position = new Vector2(tempX, tempY);
                        //Console.WriteLine("cupcake " + i + " at " + cupcake.Position.ToString());
                        StoredCupcakes[i] = cupcake;
                        return true;
                    }
                }                
            }
            //if you return false, show some kind of PAN FULL warning to player
            return false;
        }

        public String cookCupcakes()
        {
            comboMessages = "";
            score = 0;
            heat += 15;

            int spotsFilled = 0; ;
            for (int i = 0; i < StoredCupcakes.Count(); i++ )
            {
                if (StoredCupcakes[i] != null)
                {
                    score += EACH_CUPCAKE;
                    spotsFilled++;
                }
            }
            if (spotsFilled < StoredCupcakes.Length)
            {
                comboMessages += "\nDIDN'T USE ALL SLOTS! +5 heat";
                heat += 5;
            }

            //optimization. no combos to find if there aren't 3 cupcakes present
            if (spotsFilled > 2)
                findCombos();

            clearCupcakes();

            return comboMessages;
        }

        private void clearCupcakes()
        {
            for (int i = 0; i < StoredCupcakes.Count(); i++)
            {
                StoredCupcakes[i] = null;
            }
        }       

        private void findCombos()
        {
            Dictionary<Texture2D, int> textureCount = new Dictionary<Texture2D, int>();


            foreach (Actor cupcake in StoredCupcakes)
            {
                if (cupcake != null)
                {
                    if (textureCount.ContainsKey(cupcake.Texture))
                        textureCount[cupcake.Texture]++;
                    else
                        textureCount.Add(cupcake.Texture, 1);
                }
            }

            if (textureCount.ContainsValue(6))
            {
                score += SIX_COMBO;
                comboMessages += "\nSIX COMBO!! +" + SIX_COMBO;
                return;
            }
            else if (textureCount.ContainsValue(5))
            {
                score += FIVE_COMBO;
                comboMessages += "\nFIVE COMBO!! +" + FIVE_COMBO;
                return;
            }
            else if (textureCount.ContainsValue(4))
            {
                score += FOUR_COMBO;
                comboMessages += "\nFOUR COMBO!! +" + FOUR_COMBO;
                return;
            }
            else if (textureCount.ContainsValue(3))
            {
                foreach (KeyValuePair<Texture2D, int> kvp in textureCount)
                {
                    if (kvp.Value == 3)
                    {
                        score += THREE_COMBO;
                        comboMessages += "\nTHREE COMBO!! +" + THREE_COMBO;
                        findStraightCombos();
                    }
                }
                
            }
            else if (textureCount.ContainsValue(2))
            {

                foreach (KeyValuePair<Texture2D, int> kvp in textureCount)
                {
                    if (kvp.Value == 2)
                    {
                        findVerticalCombos();
                    }
                }

            }
        }

        private void findStraightCombos()
        {
            try
            {
                if (StoredCupcakes[0].Texture.Equals(StoredCupcakes[1].Texture) && StoredCupcakes[0].Equals(StoredCupcakes[2].Texture))
                {
                    score += STRAIGHT_COMBO;
                    comboMessages += "\nSTRAIGHT! +" + STRAIGHT_COMBO;
                }
                if (StoredCupcakes[3].Texture.Equals(StoredCupcakes[4].Texture) && StoredCupcakes[0].Equals(StoredCupcakes[5].Texture))
                {
                    score += STRAIGHT_COMBO;
                    comboMessages += "\nSTRAIGHT! +" + STRAIGHT_COMBO;
                }
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine("ERROR: null reference in findStraightCombos " + nre.ToString());
            }

        }

        private void findVerticalCombos()
        {
            try
            {
                if (StoredCupcakes[0].Texture.Equals(StoredCupcakes[3].Texture))
                {
                    score += VERTICAL_COMBO;
                    comboMessages += "\nVERT! +" + VERTICAL_COMBO;
                }
                if (StoredCupcakes[1].Texture.Equals(StoredCupcakes[4].Texture))
                {
                    score += VERTICAL_COMBO;
                    comboMessages += "\nVERT! +" + VERTICAL_COMBO;
                }
                if (StoredCupcakes[2].Texture.Equals(StoredCupcakes[5].Texture))
                {
                    score += VERTICAL_COMBO;
                    comboMessages += "\nVERT! +" + VERTICAL_COMBO;
                }
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine("ERROR: null reference in findVerticalCombos " + nre.ToString());
            }

        }

        /**
         * Function to reduce heat over time. How often should it run??
         */
        public void reduceHeat()
        {
            heat--;
            if (heat < 0)
                heat = 0;

        }


    }
}
