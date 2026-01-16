namespace MazeGame;

/// <summary>
/// Union-Find (Disjoint Set Union) data structure
/// </summary>
public class UnionFind
{
    private int[] _parent;

    /// <summary>
    /// Initializes n disjoint sets, one for each element.
    /// Every element starts as its own parent
    /// </summary>
    /// <param name="n">Number of elements</param>
    public UnionFind(int n)
    {
        _parent = new int[n];
        for (int i = 0; i < n; i++)
            _parent[i] = i;
    }

    /// <summary>
    /// Finds the root of the set containing x
    /// </summary>
    /// <param name="x">Element to find</param>
    /// <returns>Root of the set containing x</returns>
    public int Find(int x)
    {
        if (_parent[x] != x)
            _parent[x] = Find(_parent[x]);
        return _parent[x];
    }

    /// <summary>
    /// Unites the sets containing a and b
    /// </summary>
    /// <param name="a">First element</param>
    /// <param name="b">Second element</param>
    /// <returns>
    /// True if the sets were merged successfully, false if a and b were already in the same set
    /// </returns>
    public bool Union(int a, int b)
    {
        int ra = Find(a);
        int rb = Find(b);
        if (ra == rb) return false;
        _parent[rb] = ra;
        return true;
    }
}