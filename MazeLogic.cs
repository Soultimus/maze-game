using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MazeGame;

class MazeLogic
{
    public int[,] Maze {get; set;}

    public float PlayerSpawn {get; private set;}

    public int[,] GenerateMaze(int n)
    {
        int size = 2 * n + 1; // Pixel maze size
        Maze = new int[size, size];

        // Fill with walls (1)
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                Maze[r, c] = 1;

        // Open cell centers (0)
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                Maze[2 * r + 1, 2 * c + 1] = 0;

        // Kruskal setup
        UnionFind uf = new UnionFind(n * n);
        List<(int r1, int c1, int r2, int c2)> walls = new List<(int r1, int c1, int r2, int c2)>();

        // Collect all possible walls
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                if (r < n - 1) walls.Add((r, c, r + 1, c));
                if (c < n - 1) walls.Add((r, c, r, c + 1));
            }
        }

        // Shuffle walls
        Random rand = new Random();
        for (int i = walls.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (walls[i], walls[j]) = (walls[j], walls[i]);
        }

        // Kruskal
        foreach (var (r1, c1, r2, c2) in walls)
        {
            int id1 = r1 * n + c1;
            int id2 = r2 * n + c2;

            if (uf.Union(id1, id2))
            {
                int pr1 = 2 * r1 + 1;
                int pc1 = 2 * c1 + 1;
                int pr2 = 2 * r2 + 1;
                int pc2 = 2 * c2 + 1;

                int wr = (pr1 + pr2) / 2;
                int wc = (pc1 + pc2) / 2;
                Maze[wr, wc] = 0;
            }
        }

        // Start (8) and End (9)
        while (true)
        {
            int startPos = rand.Next(1, size - 1);

            if (Maze[startPos, 1] == 0) // Ensure you don't start enclosed
            {
                Maze[startPos, 0] = 8;
                PlayerSpawn = startPos + 0.5f;
                break;
            }
        }

        while (true)
        {
            int endPos = rand.Next(1, size - 1);

            if (Maze[endPos, size - 2] == 0) // Likewise for end
            {
                Maze[endPos, size - 1] = 9;
                break;
            }
        }

        return Maze;
    }

    public void PrintMaze()
    {
        int rows = Maze.GetLength(0);
        int cols = Maze.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++) {
                int tile = Maze[r, c];
                if (tile == 1)
                    Console.Write('█');
                else if (tile == 0)
                    Console.Write(' ');
                else if (tile == 8)
                    Console.Write('S');
                else if (tile == 9)
                    Console.Write('E');
                else
                    throw new ArgumentException("Invalid tile number was generated somehow: " + tile);
            }
            Console.WriteLine("");
        }
    }

    public void PrintActualMaze()
    {
        int rows = Maze.GetLength(0);
        int cols = Maze.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Console.Write(Maze[r, c]);
            }
            Console.WriteLine("");
        }
    }
}
