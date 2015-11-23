using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Bouncer
{
    /// <summary>
    /// A Class to represent the functionality of a camera and to control the view space.
    /// </summary>
    class Camera
    {
        private const float zoomUpperLimit = 1.5f;
        private const float zoomLowerLimit = .5f;

        private float _zoom;
        private Matrix _transform;
        private Vector2 _pos;
        private float _rotation;
        private int _viewportWidth;
        private int _viewportHeight;
        private int _worldWidth;
        private int _worldHeight;

        public Rectangle view;                     // View of Box


        public Camera(Viewport viewport, int worldWidth,
            int worldHeight, float initialZoom)
        {
            _zoom = initialZoom;
            _rotation = 0.0f;
            _pos = new Vector2(400, 0);
            _viewportWidth = viewport.Width;
            _viewportHeight = viewport.Height;
            _worldWidth = worldWidth;
            _worldHeight = worldHeight;

            view.Width = viewport.Width;
            view.Height = viewport.Height;
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            if(Pos.Y > playerPosition.Y)
            {
                Vector2 movement = new Vector2(0, -1) * 100.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;              // Update view positon based on player position

                Move(movement);

                view.Y = (int)_pos.Y;
                view.X = (int)_pos.X;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, double score, float time, Player player)
        {
            string scoreText = "Score: " + score;
            string timeText = "Time: " + time;

            Vector2 topRight = new Vector2( _pos.X + 550, _pos.Y - 400);

            spriteBatch.DrawString(font, scoreText, topRight, Color.White);
            spriteBatch.DrawString(font, timeText, new Vector2(topRight.X, topRight.Y + 30), Color.White);

            //Debug Stats
            if (player.debugState == Sprite.DebugState.True)
            {
                spriteBatch.DrawString(font, "Debug", new Vector2(topRight.X, topRight.Y + 60), Color.Red);
                spriteBatch.DrawString(font, "Player Pos: " + player._position, new Vector2(topRight.X - 50, topRight.Y + 90), Color.Red);
                spriteBatch.DrawString(font, "Camera Pos: " + Pos, new Vector2(topRight.X - 50, topRight.Y + 120), Color.Red);
                spriteBatch.DrawString(font, "Accel: " + player._accel, new Vector2(topRight.X, topRight.Y + 150), Color.Red);
                spriteBatch.DrawString(font, "Velocity: " + player._velocity, new Vector2(topRight.X - 50, topRight.Y + 180), Color.Red);
                spriteBatch.DrawString(font, "Direction: " + player._direction, new Vector2(topRight.X - 50, topRight.Y + 210), Color.Red);
                spriteBatch.DrawString(font, "Touching: " + player._isTouching, new Vector2(topRight.X, topRight.Y + 240), Color.Red);
                spriteBatch.DrawString(font, "State: " + player.mCurrentState.ToString(), new Vector2(topRight.X, topRight.Y + 270), Color.Red);
                spriteBatch.DrawString(font, "Gravity: " + player.gravStrength.ToString(), new Vector2(topRight.X, topRight.Y + 300), Color.Red);
                spriteBatch.DrawString(font, "Time Passed: " + player.t.ToString(), new Vector2(topRight.X, topRight.Y + 330), Color.Red);
                spriteBatch.DrawString(font, "Player Bounds Pos: " + player.bounds, new Vector2(topRight.X - 50, topRight.Y + 360), Color.Red);
            }
        }

        #region Properties

        /// <summary>
        /// Control Zoom
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < zoomLowerLimit)
                    _zoom = zoomLowerLimit;
                if (_zoom > zoomUpperLimit)
                    _zoom = zoomUpperLimit;
            }
        }

        /// <summary>
        /// Control Rotation
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// Control Movement of the Camera
        /// </summary>
        /// <param name="amount"></param>
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }

        /// <summary>
        /// Position of Camera
        /// </summary>
        public Vector2 Pos
        {
            get { return _pos; }
            set
            {
                float leftBarrier = (float)_viewportWidth *
                        .5f / _zoom;
                float rightBarrier = _worldWidth -
                        (float)_viewportWidth * .5f / _zoom;
                float topBarrier = _worldHeight -
                        (float)_viewportHeight * .5f / _zoom;
                float bottomBarrier = (float)_viewportHeight *
                        .5f / _zoom;
                _pos = value;
                if (_pos.X < leftBarrier)
                    _pos.X = leftBarrier;
                if (_pos.X > rightBarrier)
                    _pos.X = rightBarrier;
                if (_pos.Y > topBarrier)
                    _pos.Y = topBarrier;
                if (_pos.Y < bottomBarrier)
                    _pos.Y = bottomBarrier;
            }
        }

        #endregion

        /// <summary>
        /// Get the Transformation Matrix of the Camera
        /// </summary>
        /// <returns>Transformation Matrix</returns>
        public Matrix GetTransformation()
        {
            _transform =
                Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f,
                    _viewportHeight * 0.5f, 0));

            return _transform;
        }
    }
}
