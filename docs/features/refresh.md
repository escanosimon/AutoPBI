# Refresh

The Refresh feature allows you to bulk refresh each selected report's linked dataset, overriding the scheduled refresh and triggering an immediate refresh.

## Prerequisites

- You must be logged into AutoPBI
- You need to have some workspaces loaded and visible
- You should select at least one report from any of the shown workspaces
- Reports should have linked datasets to refresh

## Step-by-Step Process

### Step 1: Load Workspaces
- Launch AutoPBI and log in
- Ensure some workspaces are loaded and visible in the application
- Click on workspaces to expand and view their reports

### Step 2: Select Reports
- Select one or more reports from any of the shown workspaces
- You can select reports from different workspaces
- The selected reports will be highlighted

### Step 3: Click the Refresh Button
- Once you're satisfied with your report selection, click the "Refresh" button
- A refresh popup will appear

### Step 4: Review Selection
- The popup will show the reports you've selected for refreshing
- Review the list to ensure you're refreshing the correct reports

### Step 5: Confirm Refresh
- Click the "Refresh" button in the popup
- A confirmation popup will appear asking if you're sure to override the scheduled refresh
- This is a safety measure to prevent accidental refreshes
- Click "Yes" to confirm the refresh

### Step 6: Monitor Refresh Progress
- The refresh process will begin
- Each selected report's linked dataset will be refreshed immediately
- Monitor the status indicators:
  - **Loading**: Dataset is being refreshed
  - **Success**: Dataset refreshed successfully
  - **Warning**: Dataset refreshed with warnings
  - **Error**: Failed to refresh dataset

### Step 7: View Details
- Hover over status indicators to see detailed information about the refresh process
- Click the table icon (📊) in the top right of the popup to see a tabular view
- The table shows the status of each selected report and its refresh progress

## Important Notes

- **Immediate Refresh**: This feature triggers an immediate refresh, overriding any scheduled refresh
- **Linked Datasets Only**: Only reports with linked datasets will be refreshed
- **Reports Without Datasets**: Reports with no linked dataset will be skipped
- **Scheduled Refresh Override**: The manual refresh will override the scheduled refresh for the selected datasets
- **Time Consumption**: Large datasets may take significant time to refresh

## What Gets Refreshed

The refresh feature will:
- **Trigger Manual Refresh**: Override the scheduled refresh and start an immediate refresh
- **Update Data**: Pull the latest data from all data sources
- **Process Changes**: Apply any changes to the dataset structure
- **Update Reports**: Refresh the data in all connected reports

## Troubleshooting

**Refresh Fails**
- Check your permissions for the datasets
- Ensure the datasets still exist and are accessible
- Verify your internet connection is stable
- Check if the data sources are accessible

**Dataset Not Found**
- Some reports may not have linked datasets
- These reports will be skipped during refresh

**Permission Issues**
- Ensure you have appropriate permissions to refresh the datasets
- Contact your Power BI administrator if needed

**Refresh Takes Too Long**
- Large datasets may take significant time to refresh
- Be patient and don't close the application during refresh
- The refresh time depends on the size and complexity of the dataset

**Data Source Issues**
- Check if the data sources are accessible
- Verify the data source credentials are still valid
- Ensure the data sources are not experiencing issues 