namespace AutoPBI.Models;

public class Dataset
{
    public string Id;
    public string Name;
    public string? ConfiguredBy;
    public string WebUrl;
    public bool IsRefreshable;
    public string CreatedDate;
    public Workspace Workspace;

    public Dataset(string id, string name, string webUrl, bool isRefreshable, string createdDate, Workspace workspace,
        string? configuredBy = null)
    {
        Id = id;
        Name = name;
        ConfiguredBy = configuredBy;
        WebUrl = webUrl;
        IsRefreshable = isRefreshable;
        CreatedDate = createdDate;
        Workspace = workspace;
    }
}