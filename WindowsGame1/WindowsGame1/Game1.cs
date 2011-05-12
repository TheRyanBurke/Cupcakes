using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        KeyboardState lastKBS;
        GamePadState lastGPS;

        Wheel leftWheel, rightWheel;
        Oven oven;
        List<Actor> cupcakes;

        Vector2 LEFT_WHEEL_STARTING_POSITION, RIGHT_WHEEL_STARTING_POSITION, OVEN_POSITION,
            LEFT_CUPCAKE_SPAWN_POSITION, RIGHT_CUPCAKE_SPAWN_POSITION, CUPCAKE_SPEED;
        const float WHEEL_STARTING_ROTATION = 1.0f;
        const int CONTAINER_COUNT = 3;

        const int CUPCAKE_SPAWN_INTERVAL = 4000;
        const int HEAT_REDUCTION_INTERVAL = 500;
        int timeSinceLastSpawn, timeSinceLastHeatReduction;

        String comboMessages;
        int totalScore;

        enum GAME_STATES { MENU, PLAYING, GAME_OVER };
        int gameState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            float magicX = 125.0f;
            float magicY = 125.0f;
            LEFT_WHEEL_STARTING_POSITION = new Vector2(magicX, magicY);
            RIGHT_WHEEL_STARTING_POSITION = new Vector2(graphics.GraphicsDevice.Viewport.Width - magicX, magicY);

            OVEN_POSITION = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - magicX, 0.0f);

            cupcakes = new List<Actor>(30);

            CUPCAKE_SPEED = new Vector2(0.0f, -1.0f);

            LEFT_CUPCAKE_SPAWN_POSITION = new Vector2(magicX - 32, graphics.GraphicsDevice.Viewport.Height);
            RIGHT_CUPCAKE_SPAWN_POSITION = new Vector2(graphics.GraphicsDevice.Viewport.Width - magicX - 32, 
                graphics.GraphicsDevice.Viewport.Height);

            timeSinceLastSpawn = CUPCAKE_SPAWN_INTERVAL / 2;
            timeSinceLastHeatReduction = 0;

            totalScore = 0;
            comboMessages = "";

            lastKBS = Keyboard.GetState();
            lastGPS = GamePad.GetState(PlayerIndex.One);

            gameState = (int)GAME_STATES.PLAYING;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");


            // TODO: use this.Content to load your game content here
            oven = new Oven(OVEN_POSITION, Vector2.Zero, Vector2.Zero, 0.0f, Content.Load<Texture2D>("oven"), 6);
            leftWheel = new Wheel(LEFT_WHEEL_STARTING_POSITION, Vector2.Zero, Vector2.Zero, 
                WHEEL_STARTING_ROTATION, Content.Load<Texture2D>("wheel2"), CONTAINER_COUNT);
            rightWheel = new Wheel(RIGHT_WHEEL_STARTING_POSITION, Vector2.Zero, Vector2.Zero, 
                WHEEL_STARTING_ROTATION, Content.Load<Texture2D>("wheel2"), CONTAINER_COUNT);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().GetPressedKeys().Contains(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            GamePadState gps = GamePad.GetState(PlayerIndex.One);
            KeyboardState kbs = Keyboard.GetState();

            if (gameState == (int)GAME_STATES.PLAYING)
            {
                if (kbs.GetPressedKeys().Contains(Keys.OemComma) || gps.ThumbSticks.Left.X > 0)
                    leftWheel.spinWheel(-1);
                else if (kbs.GetPressedKeys().Contains(Keys.OemPeriod) || gps.ThumbSticks.Left.X < 0)
                    leftWheel.spinWheel(1);

                if (kbs.GetPressedKeys().Contains(Keys.Left) || gps.ThumbSticks.Right.X > 0)
                    rightWheel.spinWheel(-1);
                else if (kbs.GetPressedKeys().Contains(Keys.Right) || gps.ThumbSticks.Right.X < 0)
                    rightWheel.spinWheel(1);

                if (kbs.GetPressedKeys().Contains(Keys.Space) || gps.IsButtonDown(Buttons.A))
                {
                    oven.takeCupcake(leftWheel.removeCupcake());
                    oven.takeCupcake(rightWheel.removeCupcake());
                }

                if ((kbs.GetPressedKeys().Contains(Keys.Enter) && lastKBS.IsKeyUp(Keys.Enter))
                    || (gps.IsButtonDown(Buttons.B) && lastGPS.IsButtonUp(Buttons.B)))
                {
                    comboMessages = oven.cookCupcakes();
                    totalScore += oven.Score;
                }


                if (timeSinceLastSpawn > CUPCAKE_SPAWN_INTERVAL)
                {
                    makeNewCupcakes();
                    timeSinceLastSpawn = 0;
                }
                else
                    timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;

                moveCupcakes();

                checkForCupcakeCollisions();


                if (oven.Heat > 100)
                    gameState = (int)GAME_STATES.GAME_OVER;

                if (timeSinceLastHeatReduction > HEAT_REDUCTION_INTERVAL)
                {
                    oven.reduceHeat();
                    timeSinceLastHeatReduction = 0;
                }
                else
                    timeSinceLastHeatReduction += gameTime.ElapsedGameTime.Milliseconds;

            }


            if (gameState == (int)GAME_STATES.GAME_OVER)
            {
                if (gps.IsButtonDown(Buttons.Back) || kbs.IsKeyDown(Keys.R))
                {
                    this.Initialize();
                }
            }

            lastGPS = gps;
            lastKBS = kbs;


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(leftWheel.Texture, leftWheel.getRectangle(), null, Color.White, leftWheel.Rotation, 
                leftWheel.getCenter(), SpriteEffects.None, 0);
            spriteBatch.Draw(rightWheel.Texture, rightWheel.getRectangle(), null, Color.White, rightWheel.Rotation, 
                rightWheel.getCenter(), SpriteEffects.None, 0);
            spriteBatch.Draw(oven.Texture, oven.getRectangle(), Color.White);
            drawText();

            foreach (Actor cupcake in cupcakes)
            {
                //spriteBatch.Draw(cupcake.Texture, cupcake.getRectangle(), null, Color.White, cupcake.Rotation, cupcake.getCenter(), SpriteEffects.None, 0);
                spriteBatch.Draw(cupcake.Texture, cupcake.getRectangle(), Color.White);
            }

            drawCupcakesOnWheels();

            drawCupcakesOnOven();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawText()
        {
            String leftInfo = "Left Wheel\nRotation: "+leftWheel.Rotation.ToString()+"\nTo Degrees: "+
                MathHelper.ToDegrees(leftWheel.Rotation).ToString() + "\nDegrees%360: " + 
                Math.Abs((MathHelper.ToDegrees(leftWheel.Rotation)) % 360) + 
                "\ncontainer count: " + leftWheel.ContainerCount +
                "\nget container from rotation: " + leftWheel.getLowerFacingContainerFromRotation();
            String rightInfo = "Right Wheel\nRotation: " + rightWheel.Rotation.ToString();

            String gameOver = "";
            if (gameState == (int)GAME_STATES.GAME_OVER)
                gameOver = "GAME OVER!\n\n Controller.Back or Keyboard.R to restart";

            //spriteBatch.DrawString(font, leftInfo+"\n\n"+rightInfo, new Vector2(300, 250), Color.White);

            spriteBatch.DrawString(font, "Score: " + totalScore + "\nHeat: " + oven.Heat + "/100" 
                + "\n\n" + comboMessages + "\n" + gameOver, new Vector2(300, 250), Color.White);
        }

        private void drawCupcakesOnWheels()
        {
            foreach (Actor kvp in leftWheel.StoredCupcakes)
            {
                if (kvp != null)
                    spriteBatch.Draw(kvp.Texture, kvp.getRectangle(), Color.White);               
            }
            foreach (Actor kvp in rightWheel.StoredCupcakes)
            {
                if (kvp != null)
                    spriteBatch.Draw(kvp.Texture, kvp.getRectangle(), Color.White);
            }
        }

        private void drawCupcakesOnOven()
        {
            
            foreach (Actor cupcake in oven.StoredCupcakes)
            {
                if (cupcake != null)
                {
                    spriteBatch.Draw(cupcake.Texture, cupcake.getRectangle(), Color.White);
                    
                }
            }
        }

        private void makeNewCupcakes()
        {
            Texture2D[] tex = new Texture2D[4];
            tex[0] = Content.Load<Texture2D>("cupcake-brown");
            tex[1] = Content.Load<Texture2D>("cupcake-blue");
            tex[2] = Content.Load<Texture2D>("cupcake-green");
            tex[3] = Content.Load<Texture2D>("cupcake-red");

            Random rand = new Random();
            

            cupcakes.Add(new Actor(LEFT_CUPCAKE_SPAWN_POSITION, leftWheel.Position, CUPCAKE_SPEED, 0.0f, tex[rand.Next(tex.Length)]));
            cupcakes.Add(new Actor(RIGHT_CUPCAKE_SPAWN_POSITION, rightWheel.Position, CUPCAKE_SPEED, 0.0f, tex[rand.Next(tex.Length)]));

            //Console.WriteLine("making cupcakes -- number cupcakes = " + cupcakes.Count.ToString());

        }

        private void moveCupcakes()
        {
            foreach (Actor cupcake in cupcakes)
            {
                cupcake.moveActor();
            }
            //Console.WriteLine("moving cupcakes -- number cupcakes = " + cupcakes.Count.ToString());

            
        }

        private void checkForCupcakeCollisions()
        {
            foreach (Actor cupcake in cupcakes)
            {
                //if (cupcake.getRectangle().Intersects(leftWheel.getRectangle()) || cupcake.getRectangle().Intersects(rightWheel.getRectangle()))
                //{
                //    cupcakes.Remove(cupcake);
                //    break;
                //}

                if (cupcake.Position.Y < 250.0f)
                {
                    cupcake.Velocity = Vector2.Zero;

                    if (cupcake.Position.X < 250)
                    {

                        cupcake.Position = new Vector2(leftWheel.Position.X - 32, leftWheel.Position.Y + 32);
                        leftWheel.takeCupcake(cupcake);
                    }
                    else
                    {
                        cupcake.Position = new Vector2(rightWheel.Position.X - 32, rightWheel.Position.Y + 32);
                        rightWheel.takeCupcake(cupcake);
                    }
                    cupcakes.Remove(cupcake);
                    break;
                }
            }
        }
    }
}
