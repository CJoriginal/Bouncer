using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bouncer
{
    /// <summary>
    /// This class respresents the parent class for 'Blocks'. It provides a standard block including virtual functions and companents. 
    /// </summary>
    class Block
    {

        public float MASS = 1000;                   // Mass of block
        public string ID;                           // ID of Block
        public Texture2D BlockTexture;              // Block Texture
        public Vector2 Position;                    // Position of Block
        public float Angle;                         // Angle of Block
        public bool Active;                         // Active?
        public bool Touch;                          // Is being touched?

        public int Width                            // Width of Block
        {
            get { return BlockTexture.Width; }
            set {  }
        }
        public int Height                           // Height of Block
        {
            get { return BlockTexture.Height; }
            set { }
        }
        public Rectangle TriggerZone                //Triggerzone of Block
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y - 100, Width, 100); }
        }

        /// <summary>
        /// Initialise Block
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="id"></param>
        public virtual void Initialize(Texture2D texture, Vector2 position, float angle, string id)
        {
            BlockTexture = texture;

            Position = position;

            Angle = angle;

            Active = true;

            Touch = false;

            ID = id;
        }

        /// <summary>
        /// Draw Block using the SpriteBatch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BlockTexture, Position, null, Color.White, Angle, Vector2.Zero, 1f,
                SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Calculate an estimate of the bounds for the block.
        /// </summary>
        /// <returns>Rectangle based on Bounds of the Texturedd</returns>
        public virtual Rectangle GetBounds()
        {
            Rectangle bounds = new Rectangle((int)Position.X, (int)Position.Y, BlockTexture.Width, BlockTexture.Height);
            return bounds;
        }
    }
}
