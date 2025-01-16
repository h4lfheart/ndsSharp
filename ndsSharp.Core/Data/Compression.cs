using AuroraLib.Compression.Algorithms;
using AuroraLib.Compression.Interfaces;
using ndsSharp.Core.Data.Reader;

namespace ndsSharp.Core.Data;

public static class Compression
{
    private delegate bool MatchTest(Stream stream, ReadOnlySpan<char> span);
    
    public static ICompressionAlgorithm? GetCompression(DataReader reader, DataPointer pointer)
    {
        var stream = new MemoryReaderStream(reader);
        reader.Position = pointer.Offset;

        bool Matches(MatchTest matchTest)
        {
            var preTestPosition = reader.Position;
            var doesMatch = matchTest(stream, []);
            if (!doesMatch) reader.Position = preTestPosition;
            
            return doesMatch;
        }
        
        if (Matches(LZ11.IsMatchStatic)) return new LZ11();
        if (Matches(LZ10.IsMatchStatic)) return new LZ10();

        return null;
    }
}