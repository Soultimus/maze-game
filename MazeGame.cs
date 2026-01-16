using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeGame;

public class MazeGame : Game
{
    private const int SCREEN_HEIGHT = 480;
    private const int SCREEN_WIDTH = 640; 

    private FirstPersonRenderer fpr;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private int[,] _maze;

    private Player _player;

    private Texture2D _pixel;

    Dictionary<int, Texture2D> _wallTextures;

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

        var ml = new MazeLogic();

        int n = 5; // Logical maze size (n x n)
        _maze = ml.GenerateMaze(n);
        ml.PrintMaze();

        _player = new Player(1.5f, ml.PlayerSpawn, 0);

        _wallTextures = new Dictionary<int, Texture2D>();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);

        _wallTextures[1] = Content.Load<Texture2D>("cobble");
        _wallTextures[2] = Content.Load<Texture2D>("vine_cobble");
        _wallTextures[3] = Content.Load<Texture2D>("entrance");
        _wallTextures[4] = Content.Load<Texture2D>("exit");

        fpr = new FirstPersonRenderer(_maze, _player, _spriteBatch, _pixel, _wallTextures, SCREEN_WIDTH, SCREEN_HEIGHT);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _player.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        _spriteBatch.Draw(_pixel, new Rectangle(0, SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT / 2), Color.DimGray);

        fpr.Render();

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
