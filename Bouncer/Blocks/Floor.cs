using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bouncer.Blocks
{
    /// <summary>
    /// A specialist Block type. This class represents the Floor-Block Type.
    /// </summary>
    class Floor: Block
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.Touch)
            {
                spriteBatch.Draw(BlockTexture, Position, null, Color.Blue, Angle, Vector2.Zero, 2f,
                SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(BlockTexture, Position, null, Color.White, Angle, Vector2.Zero, 2f,
                SpriteEffects.None, 0f);
            }
            base.Draw(spriteBatch);
        }

        public override Rectangle GetBounds()
        {
            Rectangle bounds = new Rectangle((int)Position.X, (int)Position.Y, BlockTexture.Width*2, BlockTexture.Height*2);

            return bounds;
        }
    }
}
