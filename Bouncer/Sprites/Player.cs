using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bouncer
{
    class Player : Sprite
    {
        public double score;                         // Height Player has Achieved
        public bool hitBottom;
        public bool hitTop;


        public Player()
        {
            //this._position = new Vector2(250, -50);
            //this._position = new Vector2(1550, 350);
            this._position = new Vector2(50, 350);
            score = 0;
            hitBottom = false; hitTop = false;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            Movement(gameTime, currKeyboard);
            CheckJump(currKeyboard);

            mPrevKeyboardState = currKeyboard;

            score = (double)Math.Abs(_position.Y - _groundPosition.Y) - 5;

            base.Update(gameTime);
        }

        /// <summary>
        /// Controls Player Movement
        /// TODO:
        /// 1. Fine Tune Velocity / Friction
        /// 2. Improve Gravitational Influence i.e no autosnapping
        /// </summary>
        private void Movement(GameTime gameTime, KeyboardState state)
        {
            if (mCurrentState == SpriteState.Rolling)
            {
                _speed.X = SPRITE_SPEED;

                t = 0;

                if (state.IsKeyDown(Keys.A))
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
                else if (state.IsKeyDown(Keys.D))
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
                if(_accel < 0.01f)
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
        private void CheckJump(KeyboardState state)
        {
            if (mCurrentState == SpriteState.Rolling)
            {
                if (state.IsKeyDown(Keys.Space) == true && mPrevKeyboardState.IsKeyDown(Keys.Space) == false)
                {
                    Jump();
                }
            }
        }
    }
}
