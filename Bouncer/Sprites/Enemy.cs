using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bouncer.Sprites
{
    /// <summary>
    /// This class represents the Enemy sprite within the Game. All Movement and Decision making are to be controlled based on an AI
    /// search Algorithm.
    /// </summary>
    class Enemy: Sprite
    {
        public Enemy()
        {
            this._position = new Vector2(1550, 350);
        }

        public void Update(GameTime gameTime, Vector2 playerPos)
        {
            Movement(gameTime, playerPos);

            _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Controls Movement 
        /// 
        /// Todo:
        /// 
        /// 1. Add Algorithmic Controls
        /// 2. Ensure smooth movement between nodes.
        /// </summary>
        private void Movement(GameTime gameTime, Vector2 playerPos)
        {
            if (mCurrentState == SpriteState.Rolling)
            {
                _speed = Vector2.Zero;
                _direction = Vector2.Zero;

                /*if (playerPos.X < mPosition.X && playerPos.Y == mPosition.Y)
                {
                    mSpeed.X = SPRITE_SPEED;
                    mDirection.X = MOVE_LEFT;
                    mRotation += 10.0f;
                }
                else
                {
                    mSpeed.X = SPRITE_SPEED;
                    mDirection.X = MOVE_RIGHT;
                    mRotation += -10.0f;
                }*/
            }
        }
    }
}
