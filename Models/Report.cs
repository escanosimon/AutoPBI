using AutoPBI.Controls;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.Models;

public class Report : ObservableObject
{
    private StatusIcon.StatusType _status;
    private string? _id;
    private string? _name;
    private string? _webUrl;
    private string? _datasetId;
    private bool? _isSelected;
    private Workspace? _workspace;
    private object? _message;

    public Report(string? id, string? name, string? webUrl, string? datasetId, Workspace? workspace)
    {
        Status = StatusIcon.StatusType.Selectable;
        Id = id;
        Name = name;
        WebUrl = webUrl;
        DatasetId = datasetId;
        Workspace = workspace;
        IsSelected = false;
        Message = false;
    }

    public void Selectable()
    {
        SetStatus(StatusIcon.StatusType.Selectable, "");
    }

    public void Loading()
    {
        SetStatus(StatusIcon.StatusType.Loading, "Processing...");
    }
    
    public void Success(object? message)
    {
        SetStatus(StatusIcon.StatusType.Success, message);
    }

    public void Warning(object? message)
    {
        SetStatus(StatusIcon.StatusType.Warning, message);
    }

    public void Error(object? message)
    {
        SetStatus(StatusIcon.StatusType.Error, message);
    }

    public void SetStatus(StatusIcon.StatusType status, object? message = null)
    {
        Status = status;
        Message = message;
    }

    public StatusIcon.StatusType Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
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

    public Workspace? Workspace
    {
        get => _workspace;
        set => SetProperty(ref _workspace, value);
    }
    
    public object? Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
}