using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.Json;
using AutoPBI.Controls;
using AutoPBI.Models;
using AutoPBI.Services;
using AutoPBI.ViewModels.Overlays;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoPBI.ViewModels.Popups;

public partial class ScanPopupViewModel : PopupViewModel
{
    public ScanPopupViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public ScanPopupViewModel() : base(new MainViewModel()) {}

    [RelayCommand]
    private async void Scan()
    {
        IsProcessing = true;
        RestartCts();

        var successes = 0;
        var warnings = 0;
        var errors = 0;

        foreach (var workspace in MainViewModel.Workspaces.ToList())
        {
            foreach (var report in workspace.SelectedReports.ToList())
            {
                string message;
                report.Loading();
                Dataset dataset;
                try
                {
                    if (report.DatasetId == null)
                    {
                        message = "Report has no linked dataset.";
                        report.Error(message);
                        errors++;
                        continue;
                    }
                    dataset = MainViewModel.Datasets[report.DatasetId];
                }
                catch (Exception)
                {
                    message = "You do not have permissions to the underlying dataset. Please contact the dataset owner to request access.";
                    report.Error(message);
                    errors++;
                    continue;
                }

                if (dataset.IsRefreshable)
                {
                    PSObject refreshHistory;
                    var apiUrl =
                        $"https://api.powerbi.com/v1.0/myorg/groups/{dataset.Workspace.Id}/datasets/{dataset.Id}/refreshes";
                    try
                    {
                        var result = await Psr.Wrap()
                            .WithArguments(args => args.Add("Invoke-PowerBIRestMethod"))
                            .WithArguments(args => args
                                .Add("-Url")
                                .Add(apiUrl)
                                .Add("-Method")
                                .Add("Get")
                                .Add("-ErrorAction")
                                .Add("Stop")
                                .Add("|")
                                .Add("ConvertFrom-Json")
                            )
                            .WithStandardErrorPipe(Console.Error.WriteLine)
                            .ExecuteAsync(Cts.Token);
                        refreshHistory = result.Objects[0];
                    }
                    catch (OperationCanceledException)
                    {
                        SetReportsSelectable();
                        MainViewModel.Toast(Toast.StatusType.Normal, "Scanning cancelled!", $"Last to scan: {report.Name}");
                        return;
                    }
                    catch (Exception e)
                    {
                        report.Error(e.Message);
                        errors++;
                        continue;
                    }
                    
                    var value = (Object[]) refreshHistory.Properties["value"].Value;
                    var count = value.Length;

                    if (count > 0)
                    {
                        var obj = (PSObject) value[0];

                        switch (obj.Properties["status"].Value.ToString()!)
                        {
                            case "Failed":
                                var serviceExceptionJson = obj.Properties["serviceExceptionJson"].Value.ToString()!;
                                var serviceException = JsonSerializer.Deserialize<Dictionary<string, string>>(serviceExceptionJson)!;
                                try
                                {
                                    message = serviceException["errorDescription"];
                                    report.Error(message);
                                    errors++;
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        message = MainViewModel.ErrorMessages[serviceException["errorCode"]];
                                        report.Error(message);
                                        errors++;
                                    }
                                    catch (Exception)
                                    {
                                        message = serviceException["errorCode"];
                                        report.Error(message);
                                        Console.Error.WriteLine($"{report.Name}({report.Workspace!.Name}): {obj.Properties["serviceExceptionJson"].Value}");
                                        errors++;
                                    }
                                }
                                break;
                            case "Unknown":
                                message = "Last refresh is still loading.";
                                report.Warning(message);
                                warnings++;
                                break;
                            default:
                                message = "Last refresh was successful.";
                                report.Success(message);
                                successes++;
                                break;
                        }
                    }
                    else
                    {
                        message = "Dataset is refreshable, but no refresh history (try refreshing the dataset).";
                        report.Success(message);
                        successes++;
                    }
                }
                else
                {
                    message = "Dataset is not refreshable (DirectQuery or Live Connection).";
                    report.Warning(message);
                    warnings++;
                }
            }
        }
        
        ToastCommand(successes, warnings, errors).Execute(("Scanning finished!", $"{successes} successful, {warnings} warnings, {errors} errors."));
        IsProcessing = false;
    }

    [RelayCommand]
    private async void Refresh()
    {
        IsProcessing = true;
        RestartCts();
        
        var successes = 0;
        var warnings = 0;
        var errors = 0;
        
        foreach (var workspace in MainViewModel.Workspaces.ToList())
        {
            foreach (var report in workspace.SelectedReports.ToList())
            {
                report.Loading();
                Dataset dataset;
                try
                {
                    if (report.DatasetId == null)
                    {
                        report.Error("Report has no linked dataset.");
                        errors++;
                        continue;
                    }
                    dataset = MainViewModel.Datasets[report.DatasetId];
                }
                catch (Exception)
                {
                    report.Error("You do not have permissions to the underlying dataset. Please contact the dataset owner to request access.");
                    errors++;
                    continue;
                }

                if (dataset.IsRefreshable)
                {
                    var apiUrl =
                        $"https://api.powerbi.com/v1.0/myorg/groups/{dataset.Workspace.Id}/datasets/{dataset.Id}/refreshes";
                    try
                    {
                        await Psr.Wrap()
                            .WithArguments(args => args.Add("Invoke-PowerBIRestMethod"))
                            .WithArguments(args => args
                                .Add("-Url")
                                .Add(apiUrl)
                                .Add("-Method")
                                .Add("Post")
                            )
                            .WithStandardErrorPipe(Console.Error.WriteLine)
                            .ExecuteAsync(Cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        SetReportsSelectable();
                        MainViewModel.Toast(Toast.StatusType.Normal, "Refreshing cancelled!", $"Last to refresh: {report.Name}");
                        return;
                    }
                    catch (Exception e)
                    {
                        report.Error(e.Message);
                        errors++;
                        continue;
                    }
                }
                else
                {
                    report.Warning("Dataset is not refreshable (DirectQuery or Live Connection).");
                    warnings++;
                    continue;
                }
                report.Success("Refreshed dataset successfully");
                successes++;
            }
        }

        ToastCommand(successes, warnings, errors).Execute(("Refreshing finished!", $"{successes} refreshed, {warnings} warnings, {errors} errors."));
        IsProcessing = false;
    }
}