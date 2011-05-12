using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Wheel : Actor
    {

        const float ROTATION_INCREMENT = 0.1f;

        public Actor[] StoredCupcakes
        {
            get { return storedCupcakes; }
            set { storedCupcakes = value; }
        }
        Actor[] storedCupcakes;

        public int ContainerCount
        {
            get { return containerCount; }
            set { containerCount = value; }
        }
        int containerCount;

        public Wheel(Vector2 pos, Vector2 dest, Vector2 vel, float rot, Texture2D text, int containerCount) : base(pos, dest, vel, rot, text)
        {

            storedCupcakes = new Actor[containerCount];
            this.containerCount = containerCount;

            for (int i = 0; i < containerCount; i++ )
                storedCupcakes[i] = null;
            
        }

        /**
 * Call this on a player-controlled wheel to rotate it based on input
 * direction: negative=CCW zero=none position=CW
 */
        public void spinWheel(int direction)
        {
            switch (direction)
            {
                case -1: rotateActorCCW();
                    break;
                case 0: break;
                case 1: rotateActorCW();
                    break;
                default: break;
            }
        }

        private void rotateActorCCW()
        {
            this.Rotation -= ROTATION_INCREMENT;
            if (this.Rotation < 0.0f)
                this.Rotation = (float) (2 * Math.PI);//6.2f;
            foreach (Actor kvp in storedCupcakes)
            {
                
                if (kvp != null)
                {
                    Vector2 newPos = kvp.Position;
                    newPos.X -= this.Position.X;
                    newPos.Y -= this.Position.Y;

                    newPos = Vector2.Transform(newPos, Matrix.CreateRotationZ(-1 * ROTATION_INCREMENT));

                    newPos.X += this.Position.X;
                    newPos.Y += this.Position.Y;

                    kvp.Position = newPos;
                }
                
               
            }
        }

        private void rotateActorCW()
        {
            this.Rotation += ROTATION_INCREMENT;
            if (this.Rotation > (float) (2 * Math.PI))
                this.Rotation = 0.0f;
            foreach (Actor kvp in storedCupcakes)
            {

                if (kvp != null)
                {

                    Vector2 newPos = kvp.Position;
                    newPos.X -= this.Position.X;
                    newPos.Y -= this.Position.Y;

                    newPos = Vector2.Transform(newPos, Matrix.CreateRotationZ(ROTATION_INCREMENT));

                    newPos.X += this.Position.X;
                    newPos.Y += this.Position.Y;

                    kvp.Position = newPos;
                }
            }
        }

        public void takeCupcake(Actor cupcake)
        {
            if (cupcake != null)
            {
                if (storedCupcakes[getLowerFacingContainerFromRotation()] == null)
                    storedCupcakes[getLowerFacingContainerFromRotation()] = cupcake;
            }

            
        }

        public Actor removeCupcake()
        {
            Actor cupcake = null;
            if (storedCupcakes[getOvenFacingContainerFromRotation()] != null)
            {
                cupcake = storedCupcakes[getOvenFacingContainerFromRotation()];
                storedCupcakes[getOvenFacingContainerFromRotation()] = null;
            }
            return cupcake;
        }

        /**
         * Calculate the wheel container facing downward (accepting the cupcakes)
         * Same for each wheel. Use modulo division to return the remainder.
         * Possible result ranges from 0 to n-1 containers.  Just gotta figure out
         * how to initially orient or figure out which rotational value means the circle 
         * is facing down. i believe rotation from 0.0-1.0 represents 360 degrees of rotation
         */
        public int getLowerFacingContainerFromRotation()
        {
            int container;
            if (Math.Abs((MathHelper.ToDegrees(Rotation)) % 360) < 120)
                container = 0;
            else if (Math.Abs((MathHelper.ToDegrees(Rotation)) % 360) > 240)
                container = 2;
            else
                container = 1;
            
            return container;
        }

        /**
         * Calculate the wheel container facing the oven.
         * Different value is calculated based on which wheel it is.
         */
        private int getOvenFacingContainerFromRotation()
        {
            int temp = getLowerFacingContainerFromRotation();
            if (this.Position.X > 250)
            {
                switch (temp)
                {
                    case 0: return 2;
                    case 1: return 0;
                    case 2: return 1;
                    default: Console.WriteLine("error! default case reached in getOverFacingContainerFromRotation");
                        return 0;
                }

            } 
            else
            {
                switch (temp)
                {
                    case 0: return 1;
                    case 1: return 2;
                    case 2: return 0;
                    default: Console.WriteLine("error! default case reached in getOverFacingContainerFromRotation");
                        return 0;
                }

            }
             
        }

    }
}
