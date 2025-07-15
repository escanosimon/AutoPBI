using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoPBI.ViewModels;

public class ViewModelBase : ObservableObject
{
    public Dictionary<string, string> ErrorMessages = new()
    {
        ["DMTS_MonikerWithUnboundDataSources"] = "We are unable to access some data source because the artifact is missing connection details. Please contact the artifact owner to bind the data source to a data connection or use default connection settings for the unbound data source.",
        ["ModelRefreshFailed_CredentialsNotSpecified"] = "It looks like scheduled refresh failed because at least one data source is missing credentials. To start the refresh again, go to this dataset's settings page and enter credentials for all data sources."
    };
}