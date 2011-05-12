using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Actor
    {

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        public Vector2 Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        Vector2 destination;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;


        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        float rotation;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        Texture2D texture;

        

        public Actor(GraphicsDevice g)
            : this(new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), 0.0f, new Texture2D(g, 20, 20))
        { }

        public Actor(Vector2 pos, Vector2 dest, Vector2 vel, float rot, Texture2D text)
        {

            position = pos;
            destination = dest;
            velocity = vel;

            rotation = rot;

            texture = text;

        }



        /**
         * Calculate new position based on destination and velocity.
         */
        public void moveActor()
        {
            position.X += velocity.X;
            position.Y += velocity.Y;
        }

        public Vector2 getCenter()
        {
            return new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

    }
}
