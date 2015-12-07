using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Bouncer
{
    /// <summary>
    /// A parent class for both the 'Player' and 'Enemy' AI classes, providing virtual functions and constants.
    /// </summary>
    class Sprite
    {
        public const int SPRITE_SPEED = 3;
        public const int MOVE_LEFT = -1;
        public const int MOVE_RIGHT = 1;
        public const float GRAVITY = 9.8f;
        public float MASS = 300;
        public const float FRICTION = 0.01f;

        public enum SpriteState
        {
            Rolling,
            Jumping,
            Falling
        }

        public enum DebugState
        {
            True,
            False
        }

        public Texture2D _texture;                  // Sprite Texture

        public Vector2 _position;                   // Position of Sprite
        public Vector2 _startingPosition;           // Position of Sprite before Jump
        public Vector2 _groundPosition;             // Position of Ground
        public Vector2 _origin;                     // Center of the Texture
        public Vector2 _direction;                  // Direction of Travel
        public Vector2 _speed;                      // Speed of Sprite
        public Vector2 _velocity;

        public float _rotation;                     // Rotation of the Texture
        public int _touched;                        // Position of Touched Block
        public float timePassed;                    // Total Time Passed
        public bool _active;                        // Active?
        public bool _isTouching;                    // Is Sprite touching the ground / object?
        public float _accel;                        // Acceleration of Sprite
        public float gravStrength;                  // Gravitational Strength
        public float t;

        public Rectangle touchBoxPos;               // Touched Box Pos

        public SpriteState mCurrentState;                   // State of Sprite
        public KeyboardState mPrevKeyboardState;            // Previous KeyboardState
        public DebugState debugState = DebugState.False;    // Debug State


        public int Width                            // Width of Sprite
        {
            get { return _texture.Width; }
        }
        public int Height                           // Height of Sprite
        {
            get { return _texture.Height; }
        }
        public Rectangle Bounds
        {
            get { return new Rectangle((int)_position.X, (int)_position.Y, Width / 2, Height / 2); }
        }

        public virtual void Initialize(Texture2D texture, SpriteFont font)     // Initalise Player Variables
        {
            _texture = texture;

            _startingPosition = Vector2.Zero;

            _groundPosition = _position;

            _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

            _isTouching = true;

            _rotation = 0f;

            _active = true;

            _touched = 0;

            mCurrentState = SpriteState.Rolling;

            _accel = 0.1f;

            t = 0;

            timePassed = 0;

            _speed = Vector2.Zero;
            _direction = Vector2.Zero;
            _velocity = Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime)
        {
            timePassed = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            /*Vector2 velocity = _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;      //Vector-Base Movement
            _position += velocity;*/

            if(_isTouching)
            {
                t = 0;
            }
            else
            {
                t = t + timePassed;
            }

            if (_accel != 0.01f && mCurrentState == SpriteState.Rolling)                                  //Physics-based Movement i.e. acceleration, gravity
            {
                _accel -= FRICTION;

                if (NearlyEqual(_accel, 0.01f))
                {
                    _accel = FRICTION;
                }
            }


            if (_velocity.X > 2.8f)
            {
                _velocity.X -= 0.1f;
            }
            else if(_velocity.X < -2.8f)
            {
                _velocity.X += 0.1f;
            }
            else
            {
                _velocity.X += _direction.X * _speed.X * _accel * timePassed;
            }

            if ((touchBoxPos.X > Bounds.Center.X || touchBoxPos.X + touchBoxPos.Width < Bounds.Center.X) && mCurrentState == SpriteState.Rolling && _position.Y < 345.0f)
            {
                mCurrentState = SpriteState.Falling;
            }

            if(mCurrentState == SpriteState.Falling)
            {
                _velocity.Y = GRAVITY * t;
                t = t + timePassed;
                if(_isTouching)
                {
                    mCurrentState = SpriteState.Rolling;
                    t = 0;
                }
            }

            _position.X += _velocity.X;
            _position.Y += _velocity.Y;
        }

        public virtual void Draw(SpriteBatch spriteBatch)     
        {
            spriteBatch.Draw(_texture, _position, null, Color.White, _rotation, _origin, 1f,
                SpriteEffects.None, 0f);

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
                _speed.Y = MASS;
                _direction.Y = 1;
            }
        }

        /// <summary>
        /// Calculate the jumping of the player based on the previous
        /// KeyboardState.
        /// </summary>
        /// <param name="state">Current KeyboardState</param>
        public void CheckJump(KeyboardState state)
        {
            if (mCurrentState == SpriteState.Rolling)
            {
                if (state.IsKeyDown(Keys.Space) == true && mPrevKeyboardState.IsKeyDown(Keys.Space) == false)
                {
                    Jump();
                }
            }
        }


        /// <summary>
        /// A Helper Class to fine tune float '=' comparisons
        /// </summary>
        /// <param name="f1">Float 1</param>
        /// <param name="f2">Float 2</param>
        /// <returns> True if within range</returns>
        public static bool NearlyEqual(float f1, float f2)
        {
            // Equal if they are within 0.00001 of each other
            return Math.Abs(f1 - f2) < 0.00001;
        }
    }
}
