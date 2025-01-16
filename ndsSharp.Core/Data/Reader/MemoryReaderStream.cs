namespace ndsSharp.Core.Data.Reader;

public class MemoryReaderStream(MemoryReader reader) : Stream
{
    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;

    public override long Length => reader.Length;

    public override long Position
    {
        get => reader.Position;
        set
        {
            if (value < 0 || value > Length)
                throw new ArgumentOutOfRangeException(nameof(value));
            reader.Position = (int)value;
        }
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var span = buffer.AsSpan(offset, count);
        var available = (int) Math.Min(count, Length - Position);

        if (available <= 0)
            return 0;

        var source = reader.ReadSpan(available);
        source.CopyTo(span);

        Position += source.Length;
        return source.Length;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return origin switch
        {
            SeekOrigin.Begin => Position = offset,
            SeekOrigin.Current => Position += offset,
            SeekOrigin.End => Position = Length + offset,
            _ => throw new ArgumentException("Invalid seek origin.", nameof(origin))
        };
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}