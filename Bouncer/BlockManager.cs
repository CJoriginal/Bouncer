using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Bouncer.Blocks;

namespace Bouncer
{

    /// <summary>
    /// Manager Class for the Blocks. Creates and Destroys Blocks.
    /// </summary>
    class BlockManager : LinkedList<Block>
    {
        private float _Left;
        private float _Right;

        private int blockCount;

        private List<Texture2D> blockTextures;

        private float[] xLoc = {150.0f, 300.0f, 450.0f, 600.0f, 750.0f, 900.0f, 1050.0f};

        public Vector2 nextBlock
        {
            get { return this.First.Value.TriggerZone.Location.ToVector2() ; }
        }

        public BlockManager(float leftSide, float rightSide)
        {
            _Left = leftSide;
            _Right = rightSide;
            blockCount = 0;
            blockTextures = new List<Texture2D>();
        }

        /// <summary>
        /// Load All Block Textures
        /// </summary>
        /// <param name="content"></param>
        /// <param name="floorPosition"></param>
        public void LoadTextures(ContentManager content, Vector2 floorPosition)
        {
            Texture2D blueTexture = content.Load<Texture2D>("Graphics//Blue Block");
            Texture2D floorTexture = content.Load<Texture2D>("Graphics//Floor");
            blockTextures.Add(blueTexture);
            blockTextures.Add(floorTexture);

            LoadInitialBlocks(floorPosition);
        }

        /// <summary>
        /// Initialise the first 10 blocks
        /// </summary>
        /// <param name="floorPosition">Position of the Floor Block</param>
        private void LoadInitialBlocks(Vector2 floorPosition)
        {
            Floor f = new Floor();

            f.Initialize(blockTextures.Last(), floorPosition, 0.0f, "Floor");

            this.AddFirst(f);

            blockCount++;

            Block b = new Block();

            b.Initialize(blockTextures.First(), new Vector2(xLoc[3], 200.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
            b.Initialize(blockTextures.First(), new Vector2(xLoc[0], 50.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
            b.Initialize(blockTextures.First(), new Vector2(xLoc[3], -100.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
            b.Initialize(blockTextures.First(), new Vector2(xLoc[6], -250.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
            b.Initialize(blockTextures.First(), new Vector2(xLoc[0], -250.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
            b.Initialize(blockTextures.First(), new Vector2(xLoc[3], -400.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
            b.Initialize(blockTextures.First(), new Vector2(xLoc[1], -550.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
            b.Initialize(blockTextures.First(), new Vector2(xLoc[3], -700.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
            b.Initialize(blockTextures.First(), new Vector2(xLoc[5], -850.0f), 0f, "Block " + blockCount);
            blockCount++;
            this.AddFirst(b);
            b = new Block();
        }

        /// <summary>
        /// Check validity of the list and proceedually generate blocks. Remove Block if no longer visible and add replacement ontop.
        /// </summary>
        /// <param name="viewBottom"></param>
        public void CheckList(float viewBottom)
        {
            Block b = this.Last();                                              // Grab Bottom Block

            if(b.Position.Y > viewBottom + 100.0f)                                       // If the Block is below the view space, remove and replace
            {
                Block prev = this.First();                                      // Grab the previous block

                this.RemoveLast();                                              // Remove bottom block
                Block newBlock = new Block();                                   // Add a replacement block

                Random rand = new Random();

                int check = rand.Next(0, 1);

                float y = this.First.Value.Position.Y - 150.0f;

                float x;
                float distance;

                do                                                              // Generate X Location based on the previous and neighbour block, including distance constraint.
                {
                    x = xLoc[rand.Next(0, 6)];
                    distance = Math.Abs(x - prev.Position.X);
                } while (x == b.Position.X || x == prev.Position.X || distance < 300.0f || distance > 450.0f);

                string id = "Block " + blockCount;

                newBlock.Initialize(blockTextures.First(),                     // Initalise new Block and add to List
                                   new Vector2(x, y),
                                   0.0f,
                                   id);

                this.AddFirst(newBlock);

                if(check == 1)                                                  // Todo: Multi-Blocks per level.
                {
                    y = this.First.Value.Position.Y;

                    newBlock.Initialize(blockTextures.First(),
                                   new Vector2(x + 300.0f, y),
                                   0.0f,
                                   id);

                    this.AddFirst(newBlock);
                }

                blockCount++;
            }
        }
    }
}
