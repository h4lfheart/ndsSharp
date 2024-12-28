using System.Collections;
using System.Text;

namespace ndsSharp.Core.Mathematics;

public class Matrix<T>(int width = 0, int height = 0) : IEnumerable<(int x, int y, T value)>
{
    public int Width = width;
    public int Height = height;
    public int Length => Width * Height;

    private readonly T[,] _matrixData = new T[width, height]; 
    
    public T this[int x, int y] 
    { 
        get => _matrixData[x, y];
        set => _matrixData[x, y] = value;
    } 
    
    public T this[int index] 
    { 
        get => _matrixData[index % Width, index / Width];
        set => _matrixData[index % Width, index / Width] = value;
    }

    public IEnumerator<(int x, int y, T value)> GetEnumerator()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                yield return (x, y, this[x, y]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}