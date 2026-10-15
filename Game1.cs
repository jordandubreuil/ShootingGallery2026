using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ShootingGallery
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D backgroundSprite;
        Texture2D crosshairSprite;
        Texture2D targetSprite;
        Texture2D buttonSprite;
        SpriteFont gameFont;
        SpriteFont titleFont;

        Target target;

        Vector2 crossHairPosition;
       
        Vector2 targetDestination = new Vector2(350, 200);

        MouseState mState;
        MouseState oldMState;
        bool mReleased = true;

        const int targetRadius = 45;


        int score = 0;
        double timer = 10;
        bool gameOver = true;

        Rectangle buttonRect;

        public enum GameState { MainMenu, Game}
        GameState currentGameState = GameState.MainMenu;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            oldMState = Mouse.GetState();

            crossHairPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            backgroundSprite = Content.Load<Texture2D>("sky");
            crosshairSprite = Content.Load<Texture2D>("crosshairs");
            
            buttonSprite = Content.Load<Texture2D>("button");

            gameFont = Content.Load<SpriteFont>("GameFont");
            titleFont = Content.Load<SpriteFont>("TitleFont");
            
            targetSprite = Content.Load<Texture2D>("target");
            target = new Target(targetSprite, new Vector2(350,200), targetRadius);
            target.Center = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (!gameOver)
            {
                if (timer > 0)
                {
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    gameOver = true;
                    timer = 0;
                    currentGameState = GameState.MainMenu;
                }
            }

            target.Update(gameTime);
            
            

            mState = Mouse.GetState();

            if (mState.Position != oldMState.Position)
            {
                //Mouse moved ad we will use the mouse position
                crossHairPosition = mState.Position.ToVector2();
            }

            //Clamp the crosshair position to stay within the window bounds
            crossHairPosition.X = MathHelper.Clamp(crossHairPosition.X, 0, _graphics.PreferredBackBufferWidth);
            crossHairPosition.Y = MathHelper.Clamp(crossHairPosition.Y, 0, _graphics.PreferredBackBufferHeight);

            //Check for input
            bool actionPressed = mState.LeftButton == ButtonState.Pressed;
            bool actionReleased = mReleased;

            if (actionPressed && actionReleased && !gameOver)
            {
                //var hitDistance = Vector2.Distance(targetPosition, crossHairPosition);

                //if (hitDistance < targetRadius)
                //{
                //    Random rand = new Random();
                //    targetDestination = new Vector2((rand.Next(0, _graphics.PreferredBackBufferWidth - targetRadius * 2)), rand.Next(0, _graphics.PreferredBackBufferHeight - targetRadius * 2));
                //    score++;
                //    targetScale = 0f;
                //}

                if (target.IsHit(crossHairPosition))
                {
                    Random rand = new Random();
                    target.Destination = new Vector2((rand.Next(target.Radius, _graphics.PreferredBackBufferWidth - target.Radius)), rand.Next(target.Radius, _graphics.PreferredBackBufferHeight - target.Radius));
                    score++;
                    target.Scale = 0f;
                }
                mReleased = false;
            }

            if(gameOver && actionPressed && actionReleased)
            {
                //if crosshair is over the button, start the game
                if (buttonRect.Contains(crossHairPosition))
                {
                    
                    gameOver = false;
                    score = 0;
                    timer = 10;
                    
                    currentGameState = GameState.Game;
                }
            }

            if (!actionPressed)
            {
                mReleased = true;
            }

            oldMState = mState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(backgroundSprite, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            //State machine for Game States
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    MenuScreen("Start Game", "Shooting Gallery");
                    break;
                case GameState.Game:
                    if(timer > 0)
                    {
                        //Vector2 origin = new Vector2(targetSprite.Width / 2, targetSprite.Height / 2); 
                        //_spriteBatch.Draw(targetSprite, targetPosition, null, Color.White, 0f, origin, targetScale, SpriteEffects.None, 0f);

                        target.Draw(_spriteBatch);
                    }
                    break;
            }

            // TODO: Add your drawing code here
            
            _spriteBatch.Draw(crosshairSprite, new Vector2(crossHairPosition.X - 20, crossHairPosition.Y - 25), Color.White);

            //Draw HUD Elements
            _spriteBatch.DrawString(gameFont, $"Score: {score}", new Vector2(10, 10), Color.Yellow);
            _spriteBatch.DrawString(gameFont, $"Time: {Math.Ceiling(timer)}", new Vector2(150, 10), Color.Yellow);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void MenuScreen(string buttonLabel, string title)
        {
            //Sets up button for menu screen
            buttonRect = new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 - 50, 200, 100);

            bool isHovering = buttonRect.Contains(crossHairPosition);
            Color buttonColor = isHovering ? Color.Gold : Color.White;

            //setup the text on the button
            string buttonText = buttonLabel;
            Vector2 textSize = gameFont.MeasureString(buttonText);
            Vector2 textPosition = new Vector2(buttonRect.X + (buttonRect.Width - textSize.X) / 2, buttonRect.Y + (buttonRect.Height - textSize.Y) / 2);

            //setup text for the menu title
            string titleText = title;
            Vector2 titleSize = titleFont.MeasureString(title);
            Vector2 titlePosition = new Vector2((_graphics.PreferredBackBufferWidth - titleSize.X) / 2, buttonRect.Y - 100);

            _spriteBatch.Draw(buttonSprite, buttonRect, buttonColor);
            _spriteBatch.DrawString(gameFont, buttonText, textPosition, Color.Yellow);
            _spriteBatch.DrawString(titleFont, titleText, titlePosition, Color.Yellow);

        }
    }
}
