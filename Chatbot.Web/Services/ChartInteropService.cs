using Microsoft.JSInterop;
using System.Text.Json;

namespace Chatbot.Web.Services;

/// <summary>
/// Service for JavaScript interop with Chart.js library
/// </summary>
public class ChartInteropService
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _chartModule;

    public ChartInteropService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initialize a Chart.js chart on a canvas element
    /// </summary>
    public async Task<bool> InitializeChartAsync(string canvasId, ChartConfig config)
    {
        try
        {
            _chartModule ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/chart-interop.js");

            return await _chartModule.InvokeAsync<bool>("initializeChart", canvasId, config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing chart: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Update an existing chart's data
    /// </summary>
    public async Task<bool> UpdateChartAsync(string canvasId, ChartData newData)
    {
        try
        {
            _chartModule ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/chart-interop.js");

            return await _chartModule.InvokeAsync<bool>("updateChart", canvasId, newData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating chart: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Destroy a chart and clean up resources
    /// </summary>
    public async Task<bool> DestroyChartAsync(string canvasId)
    {
        try
        {
            _chartModule ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/chart-interop.js");

            return await _chartModule.InvokeAsync<bool>("destroyChart", canvasId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error destroying chart: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Resize a chart to fit its container
    /// </summary>
    public async Task<bool> ResizeChartAsync(string canvasId)
    {
        try
        {
            _chartModule ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/chart-interop.js");

            return await _chartModule.InvokeAsync<bool>("resizeChart", canvasId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resizing chart: {ex.Message}");
            return false;
        }
    }
}

/// <summary>
/// Chart.js configuration object
/// </summary>
public class ChartConfig
{
    public string Type { get; set; } = "line"; // line, bar, pie, doughnut, etc.
    public ChartData Data { get; set; } = new();
    public ChartOptions Options { get; set; } = new();
}

/// <summary>
/// Chart data definition
/// </summary>
public class ChartData
{
    public List<string>? Labels { get; set; }
    public List<ChartDataset>? Datasets { get; set; }
}

/// <summary>
/// Individual dataset for a chart
/// </summary>
public class ChartDataset
{
    public string? Label { get; set; }
    public List<double>? Data { get; set; }
    public object? BorderColor { get; set; }  // Can be string or List<string>
    public object? BackgroundColor { get; set; }  // Can be string or List<string>
    public double? BorderWidth { get; set; }
    public double? Tension { get; set; }
    public bool? Fill { get; set; }
}

/// <summary>
/// Chart options configuration
/// </summary>
public class ChartOptions
{
    public bool Responsive { get; set; } = true;
    public bool MaintainAspectRatio { get; set; } = true;
    public ChartScales? Scales { get; set; }
    public ChartPlugins? Plugins { get; set; }
}

/// <summary>
/// Scale configuration for charts
/// </summary>
public class ChartScales
{
    public ChartAxis? Y { get; set; }
    public ChartAxis? X { get; set; }
}

/// <summary>
/// Individual axis configuration
/// </summary>
public class ChartAxis
{
    public bool BeginAtZero { get; set; } = true;
    public string? Title { get; set; }
}

/// <summary>
/// Plugin configuration (legend, tooltip, etc.)
/// </summary>
public class ChartPlugins
{
    public ChartLegend? Legend { get; set; }
    public ChartTooltip? Tooltip { get; set; }
}

/// <summary>
/// Legend plugin configuration
/// </summary>
public class ChartLegend
{
    public bool Display { get; set; } = true;
    public string? Position { get; set; } = "top";
}

/// <summary>
/// Tooltip plugin configuration
/// </summary>
public class ChartTooltip
{
    public bool Enabled { get; set; } = true;
    public string? Mode { get; set; } = "index";
}
