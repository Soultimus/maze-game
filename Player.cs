using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MazeGame;

/// <summary>
/// Represents the player in the maze
/// </summary>
public class Player
{
    private const float MOVEMENT_SPEED = 2.5f;
    private const float ROTATION_SPEED = 1.5f;
    private const int FOV_DEGREES = 60;

    /// <summary>
    /// Player angle in radians
    /// </summary>
    public float Angle {get; private set;}

    /// <summary>
    /// Player field of view
    /// </summary>
    public float FOV {get;}

    /// <summary>
    /// Player X position
    /// </summary>
    public float X {get; private set;}

    /// <summary>
    /// Player Y Position
    /// </summary>
    public float Y {get; private set;}

    /// <summary>
    /// Creates a new player at the given position and angle
    /// </summary>
    /// <param name="x">Initial X position</param>
    /// <param name="y">Initial Y position</param>
    /// <param name="angle">Initial facing angle in degrees</param>
    public Player(float x, float y, float angle)
    {
        this.X = x;
        this.Y = y;
        this.Angle = (float)(angle * Math.PI / 180); // Degrees to Radians

        this.FOV = (float)(FOV_DEGREES * Math.PI / 180);
    }

    /// <summary>
    /// Updates the player's movement and rotation based on keyboard input
    /// </summary>
    /// <param name="gameTime">Elapsed time since last frame</param>
    public void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        float moveStep = MOVEMENT_SPEED * delta;
        float rotStep = ROTATION_SPEED * delta;

        // Rotation
        if (keyboard.IsKeyDown(Keys.Left))
            Angle -= rotStep;
        if (keyboard.IsKeyDown(Keys.Right))
            Angle += rotStep;

        float dirX = MathF.Cos(Angle);
        float dirY = MathF.Sin(Angle);

        // Forward
        if (keyboard.IsKeyDown(Keys.W))
        {
            X += dirX * moveStep;
            Y += dirY * moveStep;
        }

        // Back
        if (keyboard.IsKeyDown(Keys.S))
        {
            X -= dirX * moveStep;
            Y -= dirY * moveStep;
        }

        // Strafe Left
        if (keyboard.IsKeyDown(Keys.A))
        {
            X += dirY * moveStep;
            Y -= dirX * moveStep;
        }

        // Strafe Right
        if (keyboard.IsKeyDown(Keys.D))
        {
            X -= dirY * moveStep;
            Y += dirX * moveStep;
        }
    }

    /// <summary>
    /// Player's position as a Vector2
    /// </summary>
    public Vector2 Position => new Vector2(X, Y);
}