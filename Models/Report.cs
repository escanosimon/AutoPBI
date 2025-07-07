using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.Models;

public class Report(string? id, string? name, string? webUrl, string? datasetId) : ObservableObject
{
    private string? Id { get; set; } = id;
    internal string? Name { get; set; } = name;
    private string? WebUrl { get; set; } = webUrl;
    private string? DatasetId { get; set; } = datasetId;
}