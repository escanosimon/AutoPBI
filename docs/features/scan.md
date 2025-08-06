# Scan

The Scan feature allows you to bulk scan each report's linked dataset/semantic model for errors in the last refresh.

## Prerequisites

- You must be logged into AutoPBI
- You need to have some workspaces loaded and visible
- You should select at least one report from any of the shown workspaces
- Reports should have linked datasets to scan

## Step-by-Step Process

### Step 1: Load Workspaces
- Launch AutoPBI and log in
- Ensure some workspaces are loaded and visible in the application
- Click on workspaces to expand and view their reports

### Step 2: Select Reports
- Select one or more reports from any of the shown workspaces
- You can select reports from different workspaces
- The selected reports will be highlighted

### Step 3: Click the Scan Button
- Once you're satisfied with your report selection, click the "Scan" button
- A scan popup will appear

### Step 4: Review Selection
- The popup will show the reports you've selected for scanning
- Review the list to ensure you're scanning the correct reports

### Step 5: Start Scanning
- Click the "Scan" button in the popup to begin the scanning process
- The scanning will start immediately

### Step 6: Monitor Scanning Progress
- The scanning process will begin
- Each selected report's linked dataset will be scanned for errors
- Monitor the status indicators:
  - **Loading**: Report is being scanned
  - **Success**: Report scanned successfully (no errors found)
  - **Warning**: Report scanned with warnings
  - **Error**: Errors found in the dataset

### Step 7: View Details
- Hover over status indicators to see detailed information about any errors or warnings
- Click the table icon (📊) in the top right of the popup to see a tabular view
- The table shows the status of each selected report and its scanning results

## Important Notes

- **Linked Datasets Only**: Only reports with linked datasets will be scanned
- **Reports Without Datasets**: Reports with no linked dataset will be skipped
- **Recent Refreshes**: Only datasets that have been recently refreshed will be checked for errors
- **No Refresh History**: Datasets that were recently uploaded with no refresh will be treated as non-erroneous
- **Error Detection**: The scan checks for errors in the last refresh of each dataset

## What Gets Scanned

The scan feature checks for:
- **Data Source Errors**: Issues with data source connections
- **Refresh Errors**: Problems that occurred during the last refresh
- **Permission Issues**: Access problems with data sources
- **Configuration Errors**: Issues with dataset configuration

## Troubleshooting

**Scan Fails**
- Check your permissions for the reports and datasets
- Ensure the reports and datasets still exist
- Verify your internet connection is stable

**No Datasets Found**
- Some reports may not have linked datasets
- These reports will be skipped during scanning

**Permission Issues**
- Ensure you have appropriate permissions to access the datasets
- Contact your Power BI administrator if needed

**Scan Takes Too Long**
- Large datasets may take longer to scan
- Be patient and don't close the application during scanning 