using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text.Json;
using AutoPBI.Models;
using AutoPBI.Services;
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

        var successes = 0;
        var warnings = 0;
        var errors = 0;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsProcessing) return;
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
                PSObject refreshHistory;
                var apiUrl =
                    $"https://api.powerbi.com/v1.0/myorg/groups/{dataset.Workspace.Id}/datasets/{dataset.Id}/refreshes";
                try
                {
                    var result = await MainViewModel.PowerShellService.BuildCommand()
                        .WithCommand("Invoke-PowerBIRestMethod")
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
                        .ExecuteAsync();
                    refreshHistory = result.Objects[0];
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
                                report.Error(serviceException["errorDescription"]);
                                errors++;
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    report.Error(MainViewModel.ErrorMessages[serviceException["errorCode"]]);
                                    errors++;
                                }
                                catch (Exception)
                                {
                                    report.Error(serviceException["errorCode"]);
                                    Console.Error.WriteLine($"{report.Name}({report.Workspace!.Name}): {obj.Properties["serviceExceptionJson"].Value}");
                                    errors++;
                                }
                            }
                            break;
                        case "Unknown":
                            report.Warning("Last refresh is still loading.");
                            warnings++;
                            break;
                        default:
                            report.Success("Last refresh was successful.");
                            successes++;
                            break;
                    }
                }
                else
                {
                    report.Warning("Dataset is refreshable, but no refresh history (try refreshing the dataset).");
                    warnings++;
                }
            }
            else
            {
                report.Warning("Dataset is not refreshable (DirectQuery or Live Connection).");
                warnings++;
            }
        }
        
        ToastCommand(successes, warnings, errors).Execute(("Scanning finished!", $"{successes} successful, {warnings} warnings, {errors} errors."));
        IsProcessing = false;
    }

    [RelayCommand]
    private async void Refresh()
    {
        IsProcessing = true;
        
        var successes = 0;
        var warnings = 0;
        var errors = 0;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            if (!IsProcessing) return;
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
                    await MainViewModel.PowerShellService.BuildCommand()
                        .WithCommand("Invoke-PowerBIRestMethod")
                        .WithArguments(args => args
                            .Add("-Url")
                            .Add(apiUrl)
                            .Add("-Method")
                            .Add("Post")
                        )
                        .WithStandardErrorPipe(Console.Error.WriteLine)
                        .ExecuteAsync();
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

        ToastCommand(successes, warnings, errors).Execute(("Refreshing finished!", $"{successes} refreshed, {warnings} warnings, {errors} errors."));
        IsProcessing = false;
    }

    [RelayCommand]
    private async void ReAuth()
    {
        var successes = 0;
        var warnings = 0;
        var errors = 0;
        
        foreach (var report in MainViewModel.SelectedReports)
        {
            Dataset dataset;
            try
            {
                dataset = MainViewModel.Datasets[report.DatasetId!];
            }
            catch (Exception e)
            {
                report.Error(e.Message);
                errors++;
                continue;
            }

            var datasourceResult = await MainViewModel.PowerShellService.BuildCommand()
                .WithCommand($"Get-PowerBIDataSource -DatasetId {dataset.Id}")
                .ExecuteAsync();
            foreach (var datasourceObj in  datasourceResult.Objects)
            {
                var gatewayId = datasourceObj.Properties["GatewayId"].Value.ToString();
                Console.Error.WriteLine(gatewayId);
                CommandResult gatewayResult;
                try
                {
                    gatewayResult = await MainViewModel.PowerShellService.BuildCommand()
                        .WithCommand(
                            $"Invoke-PowerBIRestMethod -Url 'gateways/{gatewayId}' -Method Get | ConvertFrom-Json")
                        .ExecuteAsync();
                }
                catch (Exception e)
                {
                    report.Error(e.Message);
                    errors++;
                    continue;
                }

                foreach (var obj in gatewayResult.Objects)
                {
                    
                }
                // var gateway = gatewayResult.Objects[0];
                // foreach (var gatewayProperty in gateway.Properties)
                // {
                //     Console.WriteLine($"{gatewayProperty.Name}: {gatewayProperty.Value}");
                // }
            }
        }
        
        ToastCommand(successes, warnings, errors).Execute(("Scripting finished!", $"{successes} re-authed, {warnings} warnings, {errors} errors."));
    }
}