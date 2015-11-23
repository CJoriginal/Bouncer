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
        public const int SPRITE_SPEED = 5;
        public const int MOVE_LEFT = -1;
        public const int MOVE_RIGHT = 1;
        public const float GRAVITY = 9.8f;

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
        public float _accel;                        //Acceleration of Sprite
        public float gravStrength;                  //Gravitational Strength

        public Rectangle bounds;
        
        public SpriteState mCurrentState;           // State of Sprite
        public KeyboardState mPrevKeyboardState;    // Previous KeyboardState
        public DebugState debugState = DebugState.False;    // Debug State


        public int Width                            // Width of Sprite
        {
            get { return _texture.Width; }
        }
        public int Height                           // Height of Sprite
        {
            get { return _texture.Height; }
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

            timePassed = 0;

            bounds = new Rectangle();
            bounds.Height = Height;
            bounds.Width = Width;
            bounds.Location = _position.ToPoint();

            _speed = Vector2.Zero;
            _direction = Vector2.Zero;
            _velocity = Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime)
        {     
            timePassed = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            /*Vector2 velocity = _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;      //Vector-Base Movement
            _position += velocity;*/

            if (_accel != 0.01f && mCurrentState == SpriteState.Rolling)                                  //Physics-based Movement i.e. acceleration, gravity
            {
                _accel -= 0.01f;

                if (NearlyEqual(_accel, 0.01f))
                {
                    _accel = 0.01f;
                    _velocity.X = 0;
                }
            }
            _velocity.X += _direction.X * _speed.X * _accel * timePassed;

            if(NearlyEqual(_velocity.X, 5.0f))                                                          // Velocity Constraints
            {
                _velocity.X = 5.0f;
            }

            if (NearlyEqual(_velocity.X, -5.0f))
            {
                _velocity.X = -5.0f;
            }

            _position.X += _velocity.X;
            bounds.X = (int)_position.X;                                                                //Update Bounds

        }

        public virtual void Draw(SpriteBatch spriteBatch)     
        {
            if (debugState == DebugState.True)                                                          // Debug Mode
            {
                Vector2 jumpLeft = new Vector2(_position.X - 250, _position.Y - 200);
                Vector2 jumpRight = new Vector2(_position.X + 250, _position.Y - 200);

                spriteBatch.Draw(_texture, jumpLeft, null, Color.Green, _rotation, _origin, 1f,
                    SpriteEffects.None, 0f);
                spriteBatch.Draw(_texture, jumpRight, null, Color.Green, _rotation, _origin, 1f,
                    SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(_texture, _position, null, Color.White, _rotation, _origin, 1f,
                SpriteEffects.None, 0f);
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
