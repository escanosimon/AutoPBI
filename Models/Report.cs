using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.Models;

public class Report : ObservableObject
{
    private string? _id;
    private string? _name;
    private string? _webUrl;
    private string? _datasetId;
    private bool? _isSelected;
    private Workspace _workspace;

    public Report(string? id, string? name, string? webUrl, string? datasetId, Workspace workspace)
    {
        Id = id;
        Name = name;
        WebUrl = webUrl;
        DatasetId = datasetId;
        _workspace = workspace;
        IsSelected = false;
    }
    
    public string? Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? WebUrl
    {
        get => _webUrl;
        set => SetProperty(ref _webUrl, value);
    }

    public string? DatasetId
    {
        get => _datasetId;
        set => SetProperty(ref _datasetId, value);
    }

    public bool IsSelected
    {
        get => (bool)_isSelected!;
        set => SetProperty(ref _isSelected, value);
    }

    public Workspace Workspace
    {
        get => _workspace;
        set => SetProperty(ref _workspace, value);
    }
}