using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MazeGame;

class Player
{
    public float FOV {get;}
    public float X {get; private set;}
    public float Y {get; private set;}
    public float Angle {get; set;}

    public float Speed { get; set; } = 2.5f;

    public Player(float x, float y, float angle)
    {
        this.X = x;
        this.Y = y;
        this.Angle = (float)(angle * Math.PI / 180); // Degrees to Radians

        this.FOV = (float)(60f * Math.PI / 180);
    }

    public void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        float moveStep = Speed * delta;
        float rotStep = 1.5f * delta;

        if (keyboard.IsKeyDown(Keys.Left))
            Angle -= rotStep;
        if (keyboard.IsKeyDown(Keys.Right))
            Angle += rotStep;

        float dirX = MathF.Cos(Angle);
        float dirY = MathF.Sin(Angle);

        if (keyboard.IsKeyDown(Keys.W))
        {
            X += dirX * moveStep;
            Y += dirY * moveStep;
        }
        if (keyboard.IsKeyDown(Keys.S))
        {
            X -= dirX * moveStep;
            Y -= dirY * moveStep;
        }
        if (keyboard.IsKeyDown(Keys.A))
        {
            X += dirY * moveStep;
            Y -= dirX * moveStep;
        }
        if (keyboard.IsKeyDown(Keys.D))
        {
            X -= dirY * moveStep;
            Y += dirX * moveStep;
        }
    }

    public Vector2 Position => new Vector2(X, Y);

}