# Installation Guide

Follow these steps to install and set up AutoPBI on your system.

## Prerequisites

Before installing AutoPBI, ensure you have the following:

1. **.NET Runtime 9.0 or higher**
   - Download from [Microsoft's official .NET download page](https://dotnet.microsoft.com/download)
   - Verify installation by running `dotnet --version` in Command Prompt

2. **MicrosoftPowerBIMgmt PowerShell Module**
   - Open PowerShell as Administrator
   - Run the following command:
   ```powershell
   Install-Module -Name MicrosoftPowerBIMgmt
   ```
   - This module is required for AutoPBI to communicate with Power BI Service

## Download and Install AutoPBI

1. **Download the Application**
   - Click the button below to redirect to the latest release on GitHub
   - Download the portable ZIP file from the releases page

2. **Extract the Files**
   - Extract the downloaded ZIP file to a folder of your choice
   - The extracted folder will contain multiple files including `AutoPBI.exe`

3. **Launch AutoPBI**
   - Navigate to the extracted folder
   - If `AutoPBI.exe` is hard to find among the other files, use the search function in File Explorer
   - Double-click `AutoPBI.exe` to launch the application

## First Launch

- The application will start and display the main interface
- You'll need to log in with your Microsoft Power BI account (see [Authentication](../features/authentication.md))
- The application will automatically load your accessible workspaces and reports

## Troubleshooting

**Issue: Application won't start**
- Ensure .NET 9.0+ is installed
- Check if all files were extracted properly
- Try running as Administrator

**Issue: PowerShell module not found**
- Run PowerShell as Administrator
- Execute: `Get-Module -ListAvailable -Name MicrosoftPowerBIMgmt`
- If not found, reinstall using: `Install-Module -Name MicrosoftPowerBIMgmt -Force`

**Issue: Login fails**
- Verify your Power BI account credentials
- Ensure you have appropriate permissions in Power BI Service
- Check your internet connection 