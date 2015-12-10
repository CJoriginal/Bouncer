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
        public LinkedList<Vector2> path;
        public Vector2 curPoint;
        public Vector2 nextPoint;
        public int movement;
        private bool foundNode;
        public float time;
        private float jumpLimit;

        public LinkedList<Vector2> Path
        {
            get { return path; }
        }

        public Enemy()
        {
            this._position = new Vector2(1550, 350);
            path = new LinkedList<Vector2>();
            foundNode = true;
            jumpLimit = 0;
            time = 0;
        }

        public override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Movement(gameTime);

            CheckPath();
            CheckJump();

            if(jumpLimit > 0.0f)
            {
                jumpLimit -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                jumpLimit = 0;
            }

            base.Update(gameTime);
        }

        private void CheckPath()
        {
            curPoint = Path.First.Value;
            if (Path.First.Next != null) nextPoint = Path.First.Next.Value;

            if (curPoint.X < _position.X && foundNode)
            {
                movement = MOVE_LEFT;
                foundNode = false;
            }

            if (curPoint.X > _position.X && foundNode)
            {
                movement = MOVE_RIGHT;
                foundNode = false;
            }

            if (movement == MOVE_LEFT && _position.X < curPoint.X)
            {
                Path.RemoveFirst();
                foundNode = true;
            }
            else if (movement == MOVE_RIGHT && _position.X > curPoint.X)
            {
                Path.RemoveFirst();
                foundNode = true;
            }
        }

        /// <summary>
        /// Controls Movement 
        /// </summary>
        private void Movement(GameTime gameTime)
        {
            if (mCurrentState == SpriteState.Rolling)
            {
                _speed.X = SPRITE_SPEED;

                t = 0;

                if (curPoint.X < _position.X)
                {
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

        /// <summary>
        /// Perform a 'Jump'
        /// </summary>
        private void Jump()
        {
            if (mCurrentState != SpriteState.Jumping)
            {
                mCurrentState = SpriteState.Jumping;
                _isTouching = false;
                _speed.Y = MASS;
                _direction.Y = 1;
                jumpLimit = 2.0f;
            }
        }

        /// <summary>
        /// Calculate the jumping of the player based on the previous
        /// KeyboardState.
        /// </summary>
        /// <param name="state">Current KeyboardState</param>
        private void CheckJump()
        {
            float distance = Math.Abs(_position.X - curPoint.X);

            if (mCurrentState == SpriteState.Rolling)
            {
                if (_position.Y > curPoint.Y && distance < 260.0f && jumpLimit <= 0.0f)
                {
                    if(_position.X < curPoint.X)
                    {
                        _velocity.X = 2.6f;
                    }
                    else
                    {
                        _velocity.X = -2.6f;
                    }
                    Jump();
                }
            }
        }
    }
}
