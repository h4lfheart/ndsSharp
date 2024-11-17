using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using NAudio.Wave;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Conversion.Models.Processing;
using ndsSharp.Core.Conversion.Sounds.Formats;
using ndsSharp.Core.Conversion.Sounds.Stream;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.FileExplorer.Extensions;
using ndsSharp.FileExplorer.Framework;
using ndsSharp.FileExplorer.Rendering;
using ndsSharp.FileExplorer.Rendering.Rendering;
using ndsSharp.FileExplorer.Services;
using SixLabors.ImageSharp;
using WaveFormat = NAudio.Wave.WaveFormat;

namespace ndsSharp.FileExplorer.WindowModels;

public partial class STRMWindowModel : WindowModelBase
{
    [ObservableProperty] private TimeSpan _currentTime;
    [ObservableProperty] private TimeSpan _totalTime;
    
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PauseIcon))] private bool _isPaused;
    public MaterialIconKind PauseIcon => IsPaused ? MaterialIconKind.Play : MaterialIconKind.Pause;

    public WaveFileReader AudioReader;

    public WaveOutEvent OutputDevice = new()
    {
        DesiredLatency = 250
    };

    public Wave StreamWave;
    
    private readonly DispatcherTimer UpdateTimer = new();

    public override async Task Initialize()
    {
        UpdateTimer.Tick += OnUpdateTimerTick;
        UpdateTimer.Interval = TimeSpan.FromMilliseconds(1);
        UpdateTimer.Start();
    }
    
    public void LoadSTRM(STRM strm)
    {
        TitleString = $"STRM Viewer - {strm.Owner!.Path}";

        StreamWave = strm.ToWave();
        
        TaskService.Run(Play);
    }

    private void OnUpdateTimerTick(object? sender, EventArgs e)
    {
        if (AudioReader is null) return;
        
        TotalTime = AudioReader.TotalTime;
        CurrentTime = AudioReader.CurrentTime;
    }

    public async Task Play()
    {
        var stream = new MemoryStream(StreamWave.GetBuffer());
        AudioReader = new WaveFileReader(stream);
        
        OutputDevice.Stop();
        OutputDevice.Init(AudioReader);
        OutputDevice.Play();
        while (OutputDevice.PlaybackState != PlaybackState.Stopped) { }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        
        if (IsPaused)
        {
            OutputDevice.Pause();
        }
        else
        {
            OutputDevice.Play();
        }
    }

    public void Scrub(TimeSpan time)
    {
        AudioReader.CurrentTime = time;
    }
    
    public void UpdateOutputDevice()
    {
        OutputDevice.Stop();
        OutputDevice = new WaveOutEvent { DeviceNumber = 0 };
        OutputDevice.Init(AudioReader);
        
        if (!IsPaused && AudioReader is not null)
        {
            OutputDevice.Play();
        }
    }
}