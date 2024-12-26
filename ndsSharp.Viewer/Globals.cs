using System;
using Avalonia.Platform.Storage;

namespace ndsSharp.Viewer;

public static class Globals
{
    public static readonly Version Version = new(1, 0, 0);
    
    public static readonly FilePickerFileType RomFileType = new("NDS Rom") { Patterns = [ "*.nds" ] };
}