using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeGame;

class FirstPersonRenderer
{
    private const int WALL_SCALE = 480;

    private int[,] _maze;
    private Player _player;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;
    private int _screenWidth;
    private int _screenHeight;
    
    public FirstPersonRenderer(int[,] maze, Player player, SpriteBatch spriteBatch, Texture2D pixel, int screenWidth, int screenHeight)
    {
        this._maze = maze;
        this._player = player;
        this._spriteBatch = spriteBatch;
        this._pixel = pixel;
        this._screenWidth = screenWidth;
        this._screenHeight = screenHeight;
    }

    public void Render()
    {
        for (int x = 0; x < _screenWidth; x++)
        {
            float rayAngle = CalculateRayAngle(x);
            (float distance, int wallType) = CastRayDDA(rayAngle);
            int wallHeight = CalculateWallHeight(distance);
            DrawWallSlice(x, wallHeight, wallType);
        }
    }

    private float CalculateRayAngle(int screenX)
    {
        // How far we are across the screen
        float screenDistance = (float)screenX / _screenWidth;

        // Angle offset from the screen distance
        float angleOffset = (screenDistance * _player.FOV) - (_player.FOV / 2f);

        return _player.Angle + angleOffset;
    }

    private (float, int) CastRayDDA(float rayAngle)
    {
        int wallType = 0;

        // Starting grid tile
        int mapX = (int)_player.Position.X;
        int mapY = (int)_player.Position.Y;

        // Ray direction
        float rayDirX = MathF.Cos(rayAngle);
        float rayDirY = MathF.Sin(rayAngle);

        // Delta distances
        float deltaX = (rayDirX == 0) ? 1e30f : MathF.Abs(1 / rayDirX);
        float deltaY = (rayDirY == 0) ? 1e30f : MathF.Abs(1 / rayDirY);

        // Length of ray from current position to next x or y-side
        float sideDistX = 0f;
        float sideDistY = 0f;

        // What direction to step in x or y-direction (either +1 or -1)
        int stepX = 0;
        int stepY = 0;

        // Calculate step and initial sideDist
        if (rayDirX < 0)
        {
            stepX = -1;
            sideDistX = (_player.Position.X - mapX) * deltaX;
        }
        else
        {
            stepX = 1;
            sideDistX = (mapX + 1.0f - _player.Position.X) * deltaX;
        }

        if (rayDirY < 0)
        {
            stepY = -1;
            sideDistY = (_player.Position.Y - mapY) * deltaY;
        }
        else
        {
            stepY = 1;
            sideDistY = (mapY + 1.0f - _player.Position.Y) * deltaY;
        }

        bool hit = false;
        int side = 0; // 0 = vertical wall hit, 1 = horizontal wall hit



        while (!hit)
        {
            if (sideDistX < sideDistY)
            {
                sideDistX += deltaX;
                mapX += stepX;
                side = 0;
            }
            else
            {
                sideDistY += deltaY;
                mapY += stepY;
                side = 1;
            }
            
            // Check wall hit
            if (_maze[mapY, mapX] > 0)
            {
                hit = true;
                wallType = _maze[mapY, mapX];
            }

            if (mapX < 0 || mapX >= _maze.GetLength(1) || mapY < 0 || mapY >= _maze.GetLength(0))
                hit = true; // Hit the edge of the map
        }

        // Get the actual distance to the wall
        float distance;
        if (side == 0)
            distance = sideDistX - deltaX;
        else
            distance = sideDistY - deltaY;

        // Correct fisheye
        distance *= MathF.Cos(rayAngle - _player.Angle);

        return (distance, wallType);
    }

    private int CalculateWallHeight(float distance)
    {
        // Calculate height of line to draw on screen
        return (int)(WALL_SCALE / distance);
    }

    private void DrawWallSlice(int x, int wallHeight, int wallType)
    {
        int drawY = (_screenHeight - wallHeight) / 2;

        Color color = wallType switch
        {
            8 => Color.CornflowerBlue,      // Start
            9 => Color.Green,               // End
            _ => Color.Gray                 // Normal walls
        };
    
        Rectangle destRect = new Rectangle(x, drawY, 1, wallHeight);
        _spriteBatch.Draw(_pixel, destRect, color);
    }
}