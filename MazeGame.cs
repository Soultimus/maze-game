using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeGame;

public class MazeGame : Game
{
    private const int SCREEN_HEIGHT = 480;
    private const int SCREEN_WIDTH = 640; 

    private bool _mapUsed;
    private int _levelCount;
    private Dictionary<int, Texture2D> _wallTextures;
    private FirstPersonRenderer _fpr;
    private GraphicsDeviceManager _graphics;
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;
    private MazeLogic _ml;
    private Player _player;
    private Random _mazeSize;
    private SpriteBatch _spriteBatch;
    private SpriteFont _spriteFont;
    private Texture2D _pixel;

    private Stopwatch timer;

    public MazeGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Set application dimensions
        _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
        _graphics.ApplyChanges();

        _levelCount = 0;
        _wallTextures = new Dictionary<int, Texture2D>();
        _ml = new MazeLogic();
        timer = new Stopwatch();

        // Logical maze size (n x n)
        _mazeSize = new Random();
        GenerateWorld(_mazeSize.Next(5, 11));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _spriteFont = Content.Load<SpriteFont>("fonts/big-shot");

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);

        _wallTextures[1] = Content.Load<Texture2D>("textures/cobble");
        _wallTextures[2] = Content.Load<Texture2D>("textures/vine_cobble");
        _wallTextures[3] = Content.Load<Texture2D>("textures/entrance");
        _wallTextures[4] = Content.Load<Texture2D>("textures/exit");

        _fpr = new FirstPersonRenderer(_ml.Maze, _player, _spriteBatch, _wallTextures, SCREEN_WIDTH, SCREEN_HEIGHT);
        timer.Start();
    }

    protected override void Update(GameTime gameTime)
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (!_mapUsed && IsKeyPressed(Keys.M))
        {
            _mapUsed = true;
            _ml.PrintMaze();
        }

        _player.Update(gameTime);

        int mapX = (int)_player.Position.X;
        int mapY = (int)_player.Position.Y;

        // Generate new map when goal was reached
        // Play for only 5 levels
        if(_ml.Maze[mapY, mapX] == 4)
            if (_levelCount != 4)
            {
                _levelCount++;
                GenerateWorld(_mazeSize.Next(5, 11));
            }
            else
            {
                timer.Stop();
                Console.Clear();
                Console.WriteLine("Final time: " + timer.Elapsed.ToString(@"m\:ss"));
                Exit();
            }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        // Display a rectangle first for the floor
        _spriteBatch.Draw(_pixel, new Rectangle(0, SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT / 2), Color.DimGray);

        // Render the maze
        _fpr.Render();

        // Display the time in the right corner
        string text = timer.Elapsed.ToString(@"m\:ss");
        Vector2 textSize = _spriteFont.MeasureString(text) / 2;
        // Position at top-right of the window
        Vector2 position = new Vector2(
            SCREEN_WIDTH - textSize.X,
            12
        );
        _spriteBatch.DrawString(_spriteFont, text, position, Color.Yellow, 0, textSize, 1.0f, SpriteEffects.None, 0f);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private bool IsKeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    private void GenerateWorld(int size)
    {
        _mapUsed = false;

        _ml.GenerateMaze(size);

        _player = new Player(1.5f, _ml.PlayerSpawnY, 0);

        _fpr = new FirstPersonRenderer(
            _ml.Maze,
            _player,
            _spriteBatch,
            _wallTextures,
            SCREEN_WIDTH,
            SCREEN_HEIGHT
        );
    }
}
