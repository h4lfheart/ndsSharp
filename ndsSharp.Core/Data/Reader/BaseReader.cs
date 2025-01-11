using System.Text;

namespace ndsSharp.Core.Data.Reader;

public abstract class BaseReader : IDisposable
{
    public abstract int Position { get; set; }
    public abstract int Length { get; }
    public abstract int Seek(int offset, SeekOrigin origin = SeekOrigin.Current);

    public abstract T Read<T>() where T : struct;

    public T Read<T>(int offset, SeekOrigin origin = SeekOrigin.Current)
        where T : struct
    {
        SeekVoid(offset, origin);
        return Read<T>();
    }

    public abstract void Read<T>(Span<T> dest) where T : struct;

    public void Read<T>(Span<T> dest, int offset, SeekOrigin origin = SeekOrigin.Current)
        where T : struct
    {
        SeekVoid(offset, origin);
        Read(dest);
    }

    public string ReadString(Encoding enc)
    {
        var length = Read<int>();
        return ReadString(length, enc);
    }

    public string ReadString(Encoding enc, int offset, SeekOrigin origin = SeekOrigin.Current)

    {
        SeekVoid(offset, origin);
        return ReadString(enc);
    }

    public abstract string ReadString(int length, Encoding enc);

    public string ReadString(int length, Encoding enc, int offset, SeekOrigin origin = SeekOrigin.Current)

    {
        SeekVoid(offset, origin);
        return ReadString(length, enc);
    }

    public T[] ReadArray<T>() where T : struct
    {
        var length = Read<int>();
        return ReadArray<T>(length);
    }

    public T[] ReadArray<T>(int offset, SeekOrigin origin = SeekOrigin.Current)
        where T : struct

    {
        SeekVoid(offset, origin);
        return ReadArray<T>();
    }

    public abstract T[] ReadArray<T>(int length) where T : struct;

    public T[] ReadArray<T>(int length, int offset, SeekOrigin origin = SeekOrigin.Current)
        where T : struct

    {
        SeekVoid(offset, origin);
        return ReadArray<T>(length);
    }

    public T[] ReadArray<T>(Func<T> getter)
    {
        var length = Read<int>();
        return ReadArray(length, getter);
    }

    public T[] ReadArray<T>(Func<T> getter, int offset, SeekOrigin origin = SeekOrigin.Current)

    {
        SeekVoid(offset, origin);
        var length = Read<int>();
        return ReadArray(length, getter);
    }

    public T[] ReadArray<T>(Func<BaseReader, T> getter)
    {
        var length = Read<int>();
        return ReadArray(length, getter);
    }

    public T[] ReadArray<T>(Func<BaseReader, T> getter, int offset,
        SeekOrigin origin = SeekOrigin.Current)

    {
        SeekVoid(offset, origin);
        var length = Read<int>();
        return ReadArray(length, getter);
    }

    public T[] ReadArray<T>(int length, Func<T> getter)
    {
        if (length == 0)
            return [];

        var result = new T[length];

        for (var i = 0; i < length; i++)
            result[i] = getter();

        return result;
    }

    public T[] ReadArray<T>(int length, Func<T> getter, int offset, SeekOrigin origin = SeekOrigin.Current)

    {
        SeekVoid(offset, origin);
        return ReadArray(length, getter);
    }

    public T[] ReadArray<T>(int length, Func<BaseReader, T> getter)
    {
        if (length == 0)
            return [];

        var result = new T[length];

        for (var i = 0; i < length; i++)
            result[i] = getter(this);

        return result;
    }

    public T[] ReadArray<T>(int length, Func<BaseReader, T> getter, int offset,
        SeekOrigin origin = SeekOrigin.Current)

    {
        SeekVoid(offset, origin);
        return ReadArray(length, getter);
    }

    public abstract void Dispose();
    public abstract void SeekVoid(int offset, SeekOrigin origin = SeekOrigin.Current);
}