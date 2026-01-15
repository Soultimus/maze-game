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

    private int[,] maze;

    private Player player;

    private Texture2D pixel;

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

        var logic = new MazeLogic();

        int n = 5; // Logical maze size (n x n)
        maze = logic.GenerateMaze(n);
        logic.PrintMaze();

        player = new Player(1.5f, logic.PlayerSpawn, 0);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([Color.White]);

        fpr = new FirstPersonRenderer(maze, player, _spriteBatch, pixel, SCREEN_WIDTH, SCREEN_HEIGHT);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        player.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        fpr.Render();

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
