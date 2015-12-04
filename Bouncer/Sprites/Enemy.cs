using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Bouncer.Sprites
{
    /// <summary>
    /// This class represents the Enemy sprite within the Game. All Movement and Decision making are to be controlled based on an AI
    /// search Algorithm.
    /// </summary>
    class Enemy: Sprite
    {
        LinkedList<Vector2> path;
        Vector2 nextPoint;

        public LinkedList<Vector2> Path
        {
            get { return path; }
        }

        public Enemy()
        {
            //this._position = new Vector2(50, 350);
            this._position = new Vector2(1550, 350);
            path = new LinkedList<Vector2>();

        }

        public void UpdatePath(LinkedList<Vector2> astar)
        {
            path = astar;
            nextPoint = Path.First.Value;
        }

        public override void Update(GameTime gameTime)
        {
            if(_position.X .Equals(nextPoint.X))
            {
                nextPoint = Path.First.Next.Value;
                Path.RemoveFirst();
            }

            Movement(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Controls Movement 
        /// 
        /// Todo:
        /// 
        /// 1. Add Algorithmic Controls
        /// 2. Ensure smooth movement between nodes.
        /// </summary>
        private void Movement(GameTime gameTime)
        {
            if (mCurrentState == SpriteState.Rolling)
            {
                _speed.X = SPRITE_SPEED;

                t = 0;
                
                if (nextPoint.X < _position.X)
                {
                    _direction = Vector2.Zero;
                    _direction.X = MOVE_LEFT;
                    _rotation += 10.0f;
                    if (_accel < 1.0f)
                    {
                        _accel += 0.02f;
                    }
                    else
                    {
                        _accel = 1.0f;
                    }
                }
                else
                {
                    _direction = Vector2.Zero;
                    _direction.X = MOVE_RIGHT;
                    _rotation += -10.0f;
                    if (_accel < 1.0f)
                    {
                        _accel += 0.02f;
                    }
                    else
                    {
                        _accel = 1.0f;
                    }
                }

                if (mCurrentState == SpriteState.Jumping)
                {
                    if (_velocity.Y <= 7.5f)
                    {
                        gravStrength = GRAVITY * t;

                        _velocity.Y = -7.5f + gravStrength * t;
                    }
                    else
                    {
                        _velocity.Y = 7.5f;
                    }


                    _position += _velocity;
                    t = t + this.timePassed;

                    if (!NearlyEqual(_accel, 0.01f)) { _accel -= 0.01f; }
                    if (_accel < 0.01f)
                    {
                        _accel = 0.01f;
                    }

                    if (_isTouching)
                    {
                        mCurrentState = SpriteState.Rolling;
                    }
                }
            }
        }

        /// <summary>
        /// Perform a 'Jump' of the Player
        /// </summary>
        private void Jump()
        {
            if (mCurrentState != SpriteState.Jumping)
            {
                mCurrentState = SpriteState.Jumping;
                _isTouching = false;
                _speed.Y = 300.0f;
                _direction.Y = 1;
            }
        }

        /// <summary>
        /// Calculate the jumping of the player based on the previous
        /// KeyboardState.
        /// </summary>
        /// <param name="state">Current KeyboardState</param>
        private void CheckJump()
        {
            if (mCurrentState == SpriteState.Rolling)
            {
                if (nextPoint.Y < _position.Y)
                {
                    Jump();
                }
            }
        }
    }
}
