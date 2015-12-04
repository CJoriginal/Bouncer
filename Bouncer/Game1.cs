﻿using System;
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
            GameOver
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont  spriteFont;
        Player player;
        Pathfinder finder;
        Enemy enemy;
        Camera camera;
        BlockManager blocks;
        LinkedList<Vector2> path;
        GameState gameState;

        float updateTime = 0;

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

            graphics.ToggleFullScreen();

            gameState = GameState.Playing;

            Vector2 floorPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y + 400);

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

            finder = new Pathfinder(player._texture.Height, player._texture.Width);
            path = finder.FindPath(player._position, enemy._position, blocks);
            enemy.UpdatePath(path);
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
                if(updateTime > 5.0f)
                {
                    path = finder.FindPath(player._position, enemy._position, blocks);
                    enemy.UpdatePath(path);
                    updateTime = 0.0f;
                }

                player.Update(gameTime);

                if (gameTime.TotalGameTime.TotalSeconds > 5.0f)
                {
                    enemy.Update(gameTime);
                    updateTime += gameTime.ElapsedGameTime.Seconds;
                }

                AdjustCamera();

                camera.Update(gameTime, player._position);

                blocks.CheckList(camera.view.Bottom);

                CollisionDetection();

                if (player._position.Y > camera.view.Bottom)
                {
                    gameState = GameState.GameOver;
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
                GraphicsDevice.Clear(Color.WhiteSmoke);

                // TODO: Add your drawing code here
                spriteBatch.Begin(SpriteSortMode.BackToFront,
                            null, null, null, null, null,
                            camera.GetTransformation());

                player.Draw(spriteBatch);

                enemy.Draw(spriteBatch);

                camera.Draw(spriteBatch, spriteFont, player.score, gameTime.TotalGameTime.Seconds, player, enemy);

                foreach (Block b in blocks)
                {
                    b.Draw(spriteBatch);
                }

                spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(Color.IndianRed);

                Vector2 center = new Vector2(GraphicsDevice.Viewport.Bounds.Center.X - 75, GraphicsDevice.Viewport.Bounds.Center.Y - 50);

                spriteBatch.Begin(SpriteSortMode.BackToFront,
                            null, null, null, null, null,
                            null);

                spriteBatch.DrawString(spriteFont, "Game Over", center, Color.DarkSeaGreen);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }


        /// <summary>
        /// This function is responsible for all collision detection between the Player and Blocks.
        /// 
        /// To-do:
        /// 
        /// 1. Change from Bounding box to more accurate system.
        /// </summary>
        private void CollisionDetection()
        {
            Rectangle playerBounds = new Rectangle((int)player._position.X, (int)player._position.Y, player.Width / 2, player.Height / 2);
            Rectangle enemyBounds = new Rectangle((int)enemy._position.X, (int)enemy._position.Y, enemy.Width / 2, enemy.Height / 2);

            int count = 0;

            if (playerBounds.Intersects(enemyBounds))                                                                    // If Enemy Catches Player, Game Over
            {
                gameState = GameState.GameOver;
            }


            foreach (Block b in blocks)
            {
                if (b.GetBounds().Intersects(playerBounds))
                {
                    player._touched = count;
                    b.Touch = true;
                    player._isTouching = false;
                    player._position.Y = b.Position;
                    player.mCurrentState = Sprite.SpriteState.Rolling;

                    Rectangle boxBounds = b.GetBounds();

                    if (b.ID == "Floor")
                    {
                        player.touchBoxPos = blocks.Find(b).Previous.Value.GetBounds();
                    }
                    else
                    {
                        player.touchBoxPos = boxBounds;
                    }

                    if (player._position.Y > 350.0f)
                    {
                        player._position.Y = 349.0f;
                        player._isTouching = false;
                    }

                    if (playerBounds.Bottom > boxBounds.Top && player._position.Y < boxBounds.Top)                                                         // Hit the Bottom side
                    {
                        player.mCurrentState = Sprite.SpriteState.Rolling;

                        player._position.Y = boxBounds.Top - playerBounds.Height - 0.01f;
                        player._velocity.Y = 0;

                        if (boxBounds.Left <= playerBounds.Right)                                                       // Hit the Left Side
                        {
                            player._direction.X = -player._direction.X;
                        }
                        else if (boxBounds.Right >= playerBounds.Left)                                                  // Hit the Right Side
                        {
                            player._direction.X = -player._direction.X;
                        }
                    }
                    else if (boxBounds.Top < playerBounds.Top)                                                          // Hit the Top side
                    {
                        player._position.Y = boxBounds.Bottom + 10f;

                        if (boxBounds.Left <= playerBounds.Right)                                                       // Hit the Left Side
                        {
                            player._direction.X = -player._direction.X;
                        }
                        else if (boxBounds.Right >= playerBounds.Left)                                                  // Hit the Right Side
                        {
                            player._direction.X = -player._direction.X;
                        }
                    }
                }
                else
                {
                    b.Touch = false;
                }

                if (b.GetBounds().Intersects(enemyBounds))
                {
                    enemy._isTouching = true;
                    enemy._touched = count;
                    b.Touch = true;

                    Rectangle boxBounds = b.GetBounds();

                    if (enemy._position.Y == boxBounds.Top - enemyBounds.Height)
                    {
                        enemy._position.Y = boxBounds.Top - enemyBounds.Height;
                    }

                    if (b.ID == "Floor")
                    {
                        enemy.touchBoxPos = blocks.Find(b).Previous.Value.GetBounds();
                    }
                    else
                    {
                        enemy.touchBoxPos = boxBounds;
                    }

                    if (enemy._position.Y > 350.0f)
                    {
                        enemy._position.Y = 349.0f;
                        enemy._isTouching = false;
                    }

                    if (enemyBounds.Bottom > boxBounds.Top && enemy._position.Y < boxBounds.Top)                                                         // Hit the Bottom side
                    {
                        enemy.mCurrentState = Sprite.SpriteState.Rolling;

                        enemy._position.Y = boxBounds.Top - enemyBounds.Height - 0.01f;
                        enemy._velocity.Y = 0;

                        if (boxBounds.Left <= enemyBounds.Right)                                                       // Hit the Left Side
                        {
                            enemy._direction.X = -enemy._direction.X;
                        }
                        else if (boxBounds.Right >= enemyBounds.Left)                                                  // Hit the Right Side
                        {
                            enemy._direction.X = -enemy._direction.X;
                        }
                    }
                    else if (boxBounds.Top < enemyBounds.Top)                                                          // Hit the Top side
                    {
                        enemy._position.Y = boxBounds.Bottom + 10f;

                        if (boxBounds.Left <= enemyBounds.Right)                                                       // Hit the Left Side
                        {
                            enemy._direction.X = -enemy._direction.X;
                        }
                        else if (boxBounds.Right >= enemyBounds.Left)                                                  // Hit the Right Side
                        {
                            enemy._direction.X = -enemy._direction.X;
                        }
                    }
                }
                else
                {
                    b.Touch = false;
                }

                count++;
            }

            if (player._isTouching)
            {
                Block x = blocks.ElementAt(player._touched);
                Rectangle rect = x.GetBounds();

                if (!playerBounds.Intersects(rect))
                {
                    player._isTouching = false;
                    x.Touch = false;
                }
            }
            if (enemy._isTouching)
            {
                Block x = blocks.ElementAt(enemy._touched);
                Rectangle rect = x.GetBounds();

                if (!enemyBounds.Intersects(rect))
                {
                    enemy._isTouching = false;
                    x.Touch = false;
                }
            }
        }

        private void AdjustCamera()
        {
            float distance = Math.Abs(player._position.Y - enemy._position.Y);

            if(distance > 100)
            {
                if (camera.Zoom != 0.1f)
                {
                    camera.Zoom -= 0.05f;
                }
                else
                {
                    camera.Zoom = 0.1f;
                }
            }
            else
            {
                if (camera.Zoom != 0.5f)
                {
                    camera.Zoom += 0.05f;
                }
                else
                {
                    camera.Zoom = 0.5f;
                }
            }
        }

        /// <summary>
        /// WIP Method: Reload Game Functionality
        /// </summary>
        private void LoadGame()
        {
            camera = new Camera(GraphicsDevice.Viewport,
               GraphicsDevice.Viewport.TitleSafeArea.Width,
               GraphicsDevice.Viewport.TitleSafeArea.Height,
               0.5f);

            player = new Player();

            camera.Move(new Vector2(400, 0));

            blocks = new BlockManager(GraphicsDevice.Viewport.X,
                                        GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width);

            gameState = GameState.Playing;
        }
    }
}
