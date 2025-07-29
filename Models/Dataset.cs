using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.Models;

public class Dataset : ObservableObject
{
    public string Id;
    public string Name;
    public string? ConfiguredBy;
    public string WebUrl;
    public bool IsRefreshable;
    public DateTime CreatedDate;
    public Workspace Workspace;

    public Dataset(string id, string name, string webUrl, bool isRefreshable, string createdDate, Workspace workspace,
        string? configuredBy = null)
    {
        Id = id;
        Name = name;
        ConfiguredBy = configuredBy;
        WebUrl = webUrl;
        IsRefreshable = isRefreshable;
        CreatedDate = DateTime.Parse(createdDate);
        Workspace = workspace;
        Console.WriteLine(createdDate);
    }
}