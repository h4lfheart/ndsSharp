using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace ndsSharp.Core.Data.Reader;

public class MemoryReader : BaseReader
{
    private Memory<byte> _memory;

	public MemoryReader(byte[] buffer, int start, int length) : this(new Memory<byte>(buffer, start, length)) { }
	public MemoryReader(byte[] buffer) : this(new Memory<byte>(buffer)) { }
	public MemoryReader(ReadOnlyMemory<byte> memory) : this(MemoryMarshal.AsMemory(memory)) { }
	public MemoryReader(Memory<byte> memory)
	{
		_memory = memory;
		Length = memory.Length;
	}

	public override int Position { get; set; }

	public override int Length { get; }

	public override int Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
	{
		SeekVoid(offset, origin);
		return Position;
	}
	public override void SeekVoid(int offset, SeekOrigin origin = SeekOrigin.Current)
	{
		Position = origin switch
		{
			SeekOrigin.Begin => offset,
			SeekOrigin.Current => Position + offset,
			SeekOrigin.End => Length + offset,
			_ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
		};
	}

	public override T Read<T>() where T : struct
	{
		var result = Unsafe.ReadUnaligned<T>(ref _memory.Span[Position]);
		var size = Unsafe.SizeOf<T>();
		Position += size;
		return result;
	}

	public override void Read<T>(Span<T> dest) where T : struct
	{
		var size = Unsafe.SizeOf<T>();
		var span = MemoryMarshal.CreateSpan(ref Unsafe.As<T, byte>(ref dest[0]), dest.Length * size);
		_memory.Span.Slice(Position, span.Length).CopyTo(span);
		Position += span.Length;
	}

	public override string ReadString(int length, Encoding enc)
	{
		var result = enc.GetString(_memory.Span.Slice(Position, length));
		Position += length;
		return result;
	}

	public override T[] ReadArray<T>(int length) where T : struct
	{
		if (length == 0)
			return [];

		var size = length * Unsafe.SizeOf<T>();
		var result = new T[length];
		Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, byte>(ref result[0]), ref _memory.Span[Position], (uint)size);
		Position += size;
		return result;
	}

	public MemoryReader Slice(int start, bool sliceAtPosition = false)
	{
		var sliceStart = sliceAtPosition ? start + Position : start;
		return new MemoryReader(_memory.Slice(sliceStart));
	}

	public MemoryReader Slice(int start, int length, bool sliceAtPosition = false)
	{
		var sliceStart = sliceAtPosition ? start + Position : start;
		return new MemoryReader(_memory.Slice(sliceStart, length));
	}

	public Memory<byte> AsMemory(bool sliceAtPosition = false)
	{
		return sliceAtPosition ? _memory.Slice(Position) : _memory;
	}

	public Span<byte> AsSpan(bool sliceAtPosition = false)
	{
		return sliceAtPosition ? _memory.Span.Slice(Position) : _memory.Span;
	}

	public Memory<byte> ReadMemory(int length) => _memory.Slice(Position, length);

	public Span<byte> ReadSpan(int length) => _memory.Span.Slice(Position, length);

	public Span<T> ReadSpan<T>(int length) where T : struct
	{
		var size = length * Unsafe.SizeOf<T>();
		var memorySpan = _memory.Span.Slice(Position, size);
		ref var reference = ref Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(memorySpan));
		var resultSpan = MemoryMarshal.CreateSpan(ref reference, length);
		return resultSpan;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing) { }
	}

	public override void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}