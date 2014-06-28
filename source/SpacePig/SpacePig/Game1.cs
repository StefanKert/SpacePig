using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpacePig
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public const float GRAVITY = 800f; // Beschleunigung in Pixel pro Sekunde
        public const float JETPOWER = 600f; // Beschleunigungskraft der Jets in Pixel pro Sekunde

        public const int SCREENWIDTH = 1280;
        public const int SCREENHEIGHT = 780;
        public const int BLOCKSIZE = 20; // Pixelbreite / Höhe pro Block
        public const float MAXLANDINGSPEED = 20f; // Maximale Geschwindigkeit in Pixel/Sekunde für eine sichere Landung

        KeyboardState _oldState = Keyboard.GetState();
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Texture2D _taxiTexture;

        Sprite _cat;
        Sprite _backgroundOne;
        Sprite _backgroundTwo;
        Sprite _backgroundThree;
        Sprite _backgroundFour;
        Sprite _backgroundFive;

        Taxi _taxi;
        Level _level;

        public Game1()
        {
            this._graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = SCREENHEIGHT,
                PreferredBackBufferWidth = SCREENWIDTH
            };
            this.Components.Add(new GamerServicesComponent(this));
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this._taxi = new Taxi();

            this._cat = new Sprite {Scale = 0.25f};
            this._backgroundOne = new Sprite {Scale = 2.0f};
            this._backgroundTwo = new Sprite {Scale = 2.0f};
            this._backgroundThree = new Sprite {Scale = 2.0f};
            this._backgroundFour = new Sprite {Scale = 2.0f};
            this._backgroundFive = new Sprite {Scale = 2.0f};
            
            Reset();

            base.Initialize();
        }


        private void Reset()
        {
            this._level = new Level(@"Levels\level1.txt");
            this._cat.Position = this._level.StartPoint * BLOCKSIZE;
        }


        protected override void LoadContent()
        {
            this._spriteBatch = new SpriteBatch(GraphicsDevice);

            this._cat .LoadContent(this.Content, "CatCreature");
            this._cat.Position = new Vector2(500, 500);

            this._backgroundOne.LoadContent(this.Content, "Background01");
            this._backgroundOne.Position = new Vector2(0, 0);

            this._backgroundTwo.LoadContent(this.Content, "Background02");
            this._backgroundTwo.Position = new Vector2(this._backgroundOne.Position.X + this._backgroundOne.Size.Width, 0);

            this._backgroundThree.LoadContent(this.Content, "Background03");
            this._backgroundThree.Position = new Vector2(this._backgroundTwo.Position.X + this._backgroundTwo.Size.Width, 0);

            this._backgroundFour.LoadContent(this.Content, "Background04");
            this._backgroundFour.Position = new Vector2(this._backgroundThree.Position.X + this._backgroundThree.Size.Width, 0);

            this._backgroundFive.LoadContent(this.Content, "Background05");
            this._backgroundFive.Position = new Vector2(this._backgroundFour.Position.X + this._backgroundFour.Size.Width, 0);
        }

        protected override void Update(GameTime gameTime)
        {
            // Gerätestatus abfragen
            var gamePad = GamePad.GetState(PlayerIndex.One);
            var keyboard = Keyboard.GetState();


            if (keyboard.IsKeyDown(Keys.Space))
                this._taxi.Speed = new Vector2(0, -JETPOWER);
            if (keyboard.IsKeyUp(Keys.Space))
                this._taxi.Speed = new Vector2(0, GRAVITY);

            this._cat.Position += this._taxi.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this._cat.Position.Y < 0)
                Reset();
            if (this._cat.Position.Y > SCREENHEIGHT)
                

            //this.UpdateCollisionDetection(gameTime);
            this.UpdateBackground(gameTime);       

            base.Update(gameTime);
        }

        private void UpdateCollisionDetection(GameTime gameTime)
        {
            Rectangle taxiCollision = new Rectangle(
               (int)this._taxi.Position.X, (int)this._taxi.Position.Y,
               Taxi.WIDTH, Taxi.HEIGHT);
            for (int y = 0; y < this._level.Fields.GetLength(1); y++)
            {
                for (int x = 0; x < this._level.Fields.GetLength(0); x++)
                {
                    // Skip Non-Blocks
                    if (this._level.Fields[x, y] != FieldType.Bridge)
                        continue;

                    Rectangle blockCollision = new Rectangle(
                        x * BLOCKSIZE, y * BLOCKSIZE, BLOCKSIZE, BLOCKSIZE);
                    if (blockCollision.Intersects(taxiCollision))
                    {
                        // Auf Landungsbedingungen prüfen
                        if (this._taxi.Speed.LengthSquared() < MAXLANDINGSPEED * MAXLANDINGSPEED)
                        {
                            // Chance auf Landevorgang
                            float oldPosX = this._taxi.Position.X -
                                this._taxi.Speed.X * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            Rectangle taxiWithoutXCollision = new Rectangle(
                                 (int)oldPosX, (int)this._taxi.Position.Y,
                                Taxi.WIDTH, Taxi.HEIGHT);

                            if (taxiWithoutXCollision.Intersects(blockCollision) && this._taxi.Speed.Y >= 0)
                            {
                                // Landeanflug
                                taxiCollision.Y = blockCollision.Y - Taxi.HEIGHT;
                                this._taxi.Position = new Vector2(this._taxi.Position.X, blockCollision.Y - Taxi.HEIGHT);
                                this._taxi.Speed = new Vector2();
                                this._taxi.Landed = true;
                            }
                            else
                                Reset();
                        }
                        else
                            Reset();
                    }
                }
            }
        }
        private void UpdateBackground(GameTime gameTime)
        {
            if (this._backgroundOne.Position.X < -this._backgroundOne.Size.Width)
                this._backgroundOne.Position.X = this._backgroundFive.Position.X + this._backgroundFive.Size.Width;
            if (this._backgroundTwo.Position.X < -this._backgroundTwo.Size.Width)
                this._backgroundTwo.Position.X = this._backgroundOne.Position.X + this._backgroundOne.Size.Width;
            if (this._backgroundThree.Position.X < -this._backgroundThree.Size.Width)
                this._backgroundThree.Position.X = this._backgroundTwo.Position.X + this._backgroundTwo.Size.Width;
            if (this._backgroundFour.Position.X < -this._backgroundFour.Size.Width)
                this._backgroundFour.Position.X = this._backgroundThree.Position.X + this._backgroundThree.Size.Width;
            if (this._backgroundFive.Position.X < -this._backgroundFive.Size.Width)
                this._backgroundFive.Position.X = this._backgroundFour.Position.X + this._backgroundFour.Size.Width;

            var aDirection = new Vector2(-1, 0);
            var aSpeed = new Vector2(160, 0);

            this._backgroundOne.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this._backgroundTwo.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this._backgroundThree.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this._backgroundFour.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this._backgroundFive.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;   
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            this._backgroundOne.Draw(this._spriteBatch);
            this._backgroundTwo.Draw(this._spriteBatch);
            this._backgroundThree.Draw(this._spriteBatch);
            this._backgroundFour.Draw(this._spriteBatch);
            this._backgroundFive.Draw(this._spriteBatch);
            this._cat.Draw(this._spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
