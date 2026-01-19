using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeGame;

/// <summary>
/// Renders the maze from a first-person perspective using raycasting
/// </summary>
public class FirstPersonRenderer
{
    private const int WALL_SCALE = 480;

    private int[,] _maze;
    private Player _player;
    private SpriteBatch _spriteBatch;
    private Dictionary<int, Texture2D> _wallTextures;
    private int _screenWidth;
    private int _screenHeight;
    
    /// <summary>
    /// Creates a new first-person renderer
    /// </summary>
    /// <param name="maze">The game's maze</param>
    /// <param name="player">Player</param>
    /// <param name="spriteBatch">Monogame's spritebatch</param>
    /// <param name="wallTextures">Dictionary containing loaded textures for walls</param>
    /// <param name="screenWidth">The program's screen width</param>
    /// <param name="screenHeight">The program's screen height</param>
    public FirstPersonRenderer(
        int[,] maze,
        Player player,
        SpriteBatch spriteBatch,
        Dictionary<int, Texture2D> wallTextures,
        int screenWidth,
        int screenHeight)
    {
        _maze = maze;
        _player = player;
        _spriteBatch = spriteBatch;
        _wallTextures = wallTextures;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
    }

    /// <summary>
    /// Renders the entire scene by casting one ray per screen column
    /// </summary>
    public void Render()
    {
        for (int x = 0; x < _screenWidth; x++)
        {
            float rayAngle = CalculateRayAngle(x);
            (float distance, float wallX, Texture2D texture) = CastRayDDA(rayAngle);
            if (texture == null)
                continue;
                
            int wallHeight = CalculateWallHeight(distance);
            DrawWallSlice(x, wallHeight, wallX, texture);
        }
    }

    /// <summary>
    /// Calculates the angle of a ray based on its screen X position
    /// </summary>
    /// <param name="screenX">The X coordinate of the screen column</param>
    /// <returns>
    /// The angle of the ray in radians
    /// </returns>
    private float CalculateRayAngle(int screenX)
    {
        // How far we are across the screen
        float screenDistance = (float)screenX / _screenWidth;

        // Angle offset from the screen distance
        float angleOffset = (screenDistance * _player.FOV) - (_player.FOV / 2f);

        return _player.Angle + angleOffset;
    }

    /// <summary>
    /// Casts a ray into the maze using the DDA algorithm to find the nearest wall
    /// </summary>
    /// <param name="rayAngle">The angle of the ray in radians</param>
    /// <returns>
    /// A tuple containing the distance to the wall, the horizontal texture coordinate, and the appropriate wall texture
    /// </returns>
    private (float, float, Texture2D) CastRayDDA(float rayAngle)
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
        float sideDistX = 0.0f;
        float sideDistY = 0.0f;

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

            if (mapX < 0 || mapX >= _maze.GetLength(1) || mapY < 0 || mapY >= _maze.GetLength(0))
                return (0.0f, 0.0f, null); // Hit the edge of the map
            
            // Check wall hit
            if (_maze[mapY, mapX] > 0)
            {
                hit = true;
                wallType = _maze[mapY, mapX];
            }
        }

        // Get the actual distance to the wall and where on the wall we hit
        float distance = 0.0f;
        float wallX = 0.0f;
        if (side == 0) // Vertical
        {
            distance = sideDistX - deltaX;
            wallX = _player.Position.Y + distance * rayDirY;
        }
        else // Horizontal
        {
            distance = sideDistY - deltaY;
            wallX = _player.Position.X + distance * rayDirX;
        }
        wallX -= MathF.Floor(wallX);

        // Flip north and west side's textures
        if ((side == 0 && stepX < 0) || (side == 1 && stepY > 0))
            wallX = 1.0f - wallX;

        // Correct fisheye
        distance *= MathF.Cos(rayAngle - _player.Angle);

        return (distance, wallX, _wallTextures[wallType]);
    }

    /// <summary>
    /// Calculates the on-screen height of a wall slice based on its distance.
    /// </summary>
    /// <param name="distance">Distance from the player to the wall</param>
    /// <returns>
    /// Height of line to draw on screen
    /// </returns>
    private int CalculateWallHeight(float distance)
    {
        return (int)(WALL_SCALE / distance);
    }

    /// <summary>
    /// Draws a single vertical slice of a wall to the screen
    /// </summary>
    /// <param name="x">Screen X coordinate of the slice</param>
    /// <param name="wallHeight">Height of the wall slice</param>
    /// <param name="wallX">Horizontal texture coordinate (0–1)</param>
    /// <param name="texture">Wall texture to draw</param>
    private void DrawWallSlice(int x, int wallHeight, float wallX, Texture2D texture)
    {
        int textureX = (int)(wallX * texture.Width);

        Rectangle sourceRect = new Rectangle(textureX, 0, 1, texture.Height);

        int drawY = (_screenHeight - wallHeight) / 2;
    
        Rectangle destRect = new Rectangle(x, drawY, 1, wallHeight);
        _spriteBatch.Draw(texture, destRect, sourceRect, Color.White);
    }
}