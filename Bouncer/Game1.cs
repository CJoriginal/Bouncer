using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Bouncer.Sprites;
using Bouncer.AI;
using Bouncer.Blocks;

namespace Bouncer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        enum GameState
        {
            Playing, 
            GameOver,
            Victory
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont  spriteFont;

        Player player;
        Pathfinder finder;
        Enemy enemy;
        Camera camera;
        BlockManager blocks;

        GameState gameState;

        float time;
        float playEnemDistance;
        int displayTime;

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

            camera = new Camera(GraphicsDevice.Viewport,
                GraphicsDevice.Viewport.TitleSafeArea.Width,
                GraphicsDevice.Viewport.TitleSafeArea.Height,
                0.5f);

            player = new Player();

            enemy = new Enemy();

            camera.Move(new Vector2(400, 0));

            blocks = new BlockManager(GraphicsDevice.Viewport.X,
                                        GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width);

            gameState = GameState.Playing;

            Vector2 floorPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y + 400);

            time = 0;
            displayTime = 0;
            playEnemDistance = 0;

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

            // TODO: use this.Content to load your game content here

            Vector2 floorPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y + 400);

            blocks.LoadTextures(Content, floorPosition);

            spriteFont = Content.Load<SpriteFont>("Pesca");

            player.Initialize(Content.Load<Texture2D>("Graphics//Player"), spriteFont);

            enemy.Initialize(Content.Load<Texture2D>("Graphics//Enemy"), spriteFont);

            camera.LoadDebugBox(Content.Load<Texture2D>("Graphics//Blue Block"));

            finder = new Pathfinder(50, 100);
            enemy.path = finder.FindPath(enemy._position, blocks.nextBlock, blocks);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                player.debugState = Sprite.DebugState.True;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                player.debugState = Sprite.DebugState.False;
            }

            // TODO: Add your update logic here

            if (gameState == GameState.Playing)
            {
                if (time > 5.0f)
                {
                    player.Update(gameTime);
                }

                enemy.Update(gameTime);

                playEnemDistance = Math.Abs(player._position.Y - enemy._position.Y);

                if (player._position.Y < enemy._position.Y)
                {
                    camera.Update(gameTime, player._position);
                }
                else
                {
                    camera.Update(gameTime, enemy._position);
                }

                blocks.CheckList(camera.view.Bottom);

                if(enemy.Path.Count < 5)
                {
                    enemy.path = finder.FindPath(enemy.nextPoint, blocks.nextBlock, blocks);
                }

                CollisionDetection();

                if (player._position.Y > camera.view.Bottom) 
                {
                   gameState = GameState.GameOver;
                }

                if (enemy._position.Y > camera.view.Bottom)
                {
                    gameState = GameState.Victory;
                }

                time += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(player._position.Y < enemy._position.Y && playEnemDistance > 100)
                {
                    displayTime += (int)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    displayTime = 0;
                }
            }
            else
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    LoadGame();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (gameState == GameState.Playing)
            {
                if (time < 45.0f || time > 60.0f)
                {
                    GraphicsDevice.Clear(Color.WhiteSmoke);
                }
                else
                {
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                }

                // TODO: Add your drawing code here
                spriteBatch.Begin(SpriteSortMode.BackToFront,
                            null, null, null, null, null,
                            camera.GetTransformation());


                player.Draw(spriteBatch);

                enemy.Draw(spriteBatch);

                camera.Draw(spriteBatch, spriteFont, player.score, time, player, enemy);

                Messages();

                foreach (Block b in blocks)
                {
                    b.Draw(spriteBatch);
                }

                spriteBatch.End();
            }
            else if(gameState == GameState.GameOver)
            {
                GraphicsDevice.Clear(Color.IndianRed);

                Vector2 center = new Vector2(GraphicsDevice.Viewport.Bounds.Center.X - 75, GraphicsDevice.Viewport.Bounds.Center.Y - 50);

                spriteBatch.Begin(SpriteSortMode.BackToFront,
                            null, null, null, null, null,
                            null);

                spriteBatch.DrawString(spriteFont, "Game Over", center, Color.DarkSeaGreen);
                spriteBatch.DrawString(spriteFont, "Score: " + player.score, new Vector2(center.X - 5.0f, center.Y + 50.0f), Color.DarkSeaGreen);
                spriteBatch.DrawString(spriteFont, "Press Enter to Restart", new Vector2(center.X - 40.0f, center.Y + 100.0f), Color.DarkSeaGreen);
                spriteBatch.DrawString(spriteFont, "Or ESC to Exit", new Vector2(center.X - 20.0f, center.Y + 130.0f), Color.DarkSeaGreen);

                spriteBatch.End();
            }
            else
            {

                GraphicsDevice.Clear(Color.Green);

                Vector2 center = new Vector2(GraphicsDevice.Viewport.Bounds.Center.X - 75, GraphicsDevice.Viewport.Bounds.Center.Y - 50);

                spriteBatch.Begin(SpriteSortMode.BackToFront,
                            null, null, null, null, null,
                            null);

                spriteBatch.DrawString(spriteFont, "Victory", center, Color.Black);
                spriteBatch.DrawString(spriteFont, "Score: " + player.score, new Vector2(center.X - 5.0f, center.Y + 50.0f), Color.Black);
                spriteBatch.DrawString(spriteFont, "Press Enter to Restart", new Vector2(center.X - 40.0f, center.Y + 100.0f), Color.Black);
                spriteBatch.DrawString(spriteFont, "Or ESC to Exit", new Vector2(center.X - 20.0f, center.Y + 130.0f), Color.Black);

                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// A Collection of SpriteFonts to be drawn based on Game Parameters
        /// </summary>
        private void Messages()
        {
            if (time < 5.0f && time > 1.0f)         // Intro Conversation
            {
                if (time < 3.0f)
                {
                    spriteBatch.DrawString(spriteFont, "You can't catch me!", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
                    spriteBatch.DrawString(spriteFont, "Hmm ... what?", new Vector2(player._position.X - 40.0f, player._position.Y - 100.0f), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(spriteFont, "Mwahaha!", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
                    spriteBatch.DrawString(spriteFont, "Oh ... ok ...", new Vector2(player._position.X - 40.0f, player._position.Y - 100.0f), Color.Black);
                }
            }

            if(time < 8.0f && time > 5.0f)
            {
                spriteBatch.DrawString(spriteFont, "Use A and D to Move. Space to Jump!", new Vector2(player._position.X - 40.0f, player._position.Y - 100.0f), Color.Black);
            }

            if (time > 15.0f && time < 17.0f)        // A Provocation
            {
                spriteBatch.DrawString(spriteFont, "You have as much chance as there is textures in this game!", new Vector2(enemy._position.X - 250.0f, enemy._position.Y - 100.0f), Color.Black);
            }

            if (time > 30.0f && time < 45.0f)
            {
                if (time < 42.0f)
                {
                    spriteBatch.DrawString(spriteFont, "Who is who?", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
                }

                Texture2D tex = player._texture;
                player._texture = enemy._texture;
                enemy._texture = tex;
            }

            if (time > 45.0f && time < 47.0f)       // Swap Background from White to Blue
            {
                spriteBatch.DrawString(spriteFont, "Oh I know ... Hard to see, Right?!?", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
            }

            if (time > 60.0f && time < 62.0f)       // A Provocation
            {
                spriteBatch.DrawString(spriteFont, "Still, here?", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
            }

            if(time > 75.0f && time < 90.0f)        // Half Gravitational Strength
            {
                if (time < 77.0f)
                {
                    spriteBatch.DrawString(spriteFont, "Half the gravity ... Airhead!", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
                }

                player.GRAVITY = 4.9f;
                enemy.GRAVITY = 4.9f;
            }

            if (time > 90.0f && time < 92.0f)
            {
                spriteBatch.DrawString(spriteFont, "You realise you are just doing the same thing over and over?", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
                player.GRAVITY = 9.8f;
                enemy.GRAVITY = 9.8f;
            }

            if (time > 105.0f && time < 107.0f)
            {
                spriteBatch.DrawString(spriteFont, "Oh ... you're good!", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
            }

            if(player._position.Y < enemy._position.Y && playEnemDistance > 100)
            {
                if (displayTime < 3.0f)
                {
                    spriteBatch.DrawString(spriteFont, "Impossible!!!", new Vector2(enemy._position.X - 40.0f, enemy._position.Y - 100.0f), Color.Black);
                }
            }
        }

        /// <summary>
        /// This function is responsible for all collision detection between the Player and Blocks.
        /// </summary>
        private void CollisionDetection()
        {
            List<Sprite> sprites = new List<Sprite>();

            sprites.Add(player);
            sprites.Add(enemy);

            int count = 0;

            if(player.Bounds.Intersects(enemy.Bounds) || enemy.Bounds.Intersects(player.Bounds))
            {
                //player._velocity = -player._velocity;
                //enemy._velocity = -enemy._velocity;
                CollisionForce();
            }

            foreach (Sprite sprite in sprites)
            {
                foreach (Block b in blocks)
                {
                    if (b.GetBounds().Intersects(sprite.Bounds))
                    {
                        sprite._touched = count;
                        b.Touch = true;
                        sprite._isTouching = false;
                        sprite.mCurrentState = Sprite.SpriteState.Rolling;

                        Rectangle boxBounds = b.GetBounds();

                        if (b.ID != "Floor")
                        {
                            sprite.touchBoxPos = boxBounds;
                        }

                        if (sprite._position.Y > 350.0f && b.GetType() != typeof(Floor)) 
                        {
                            sprite._position.Y = 349.0f;
                            sprite._isTouching = false;
                        }

                        if (sprite.Bounds.Bottom > boxBounds.Top && sprite._position.Y < boxBounds.Top)                      // Hit the Bottom side
                        {
                            sprite._position.Y = boxBounds.Top - sprite.Bounds.Height - 0.01f;
                            sprite._velocity.Y = 0;

                            if (boxBounds.Left <= sprite.Bounds.Right)                                                       // Hit the Left Side
                            {
                                sprite._direction.X = -sprite._direction.X;
                            }
                            else if (boxBounds.Right >= sprite.Bounds.Left)                                                  // Hit the Right Side
                            {
                                sprite._direction.X = -sprite._direction.X;
                            }
                        }
                        else if (boxBounds.Top < sprite.Bounds.Top)                                                          // Hit the Top side
                        {
                            sprite.mCurrentState = Sprite.SpriteState.Falling;

                            if (boxBounds.Left <= sprite.Bounds.Right)                                                       // Hit the Left Side
                            {
                                sprite._direction.X = -sprite._direction.X;
                            }
                            else if (boxBounds.Right >= sprite.Bounds.Left)                                                  // Hit the Right Side
                            {
                                sprite._direction.X = -sprite._direction.X;
                            }
                        }

                    }
                    else
                    {
                        b.Touch = false;
                    }


                    if (sprite._isTouching)
                    {
                        Block x = blocks.ElementAt(sprite._touched);
                        Rectangle rect = x.GetBounds();

                        if (!sprite.Bounds.Intersects(rect))
                        {
                            sprite._isTouching = false;
                            x.Touch = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Collision based on Calculated Collision Force on Sprites
        /// </summary>
        private void CollisionForce()
        {
            Vector2 centralVel = (player.MASS * player._velocity + enemy.MASS * enemy._velocity) / (player.MASS + enemy.MASS);

            Vector2 playerNorm = Vector2.Normalize(player._velocity);
            player._velocity -= centralVel;
            player._velocity = Vector2.Reflect(player._velocity, playerNorm);
            player._velocity += centralVel;

            Vector2 enemyNorm = Vector2.Normalize(enemy._velocity);
            enemy._velocity -= centralVel;
            enemy._velocity = Vector2.Reflect(enemy._velocity, enemyNorm);
            enemy._velocity += centralVel;
        }

        /// <summary>
        /// Reload Game
        /// </summary>
        private void LoadGame()
        {
            Initialize();
        }
    }
}
