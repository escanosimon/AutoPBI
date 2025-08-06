# Takeover

The Takeover feature allows you to bulk transfer the ownership of each report's linked dataset and data sources to you, the currently authenticated user.

## Prerequisites

- You must be logged into AutoPBI
- You need to have some workspaces loaded and visible
- You should select at least one report from any of the shown workspaces
- Reports should have linked datasets to take over

## Step-by-Step Process

### Step 1: Load Workspaces
- Launch AutoPBI and log in
- Ensure some workspaces are loaded and visible in the application
- Click on workspaces to expand and view their reports

### Step 2: Select Reports
- Select one or more reports from any of the shown workspaces
- You can select reports from different workspaces
- The selected reports will be highlighted

### Step 3: Click the Takeover Button
- Once you're satisfied with your report selection, click the "Takeover" button
- A takeover popup will appear

### Step 4: Review Selection
- The popup will show the reports you've selected for takeover
- Review the list to ensure you're taking over the correct reports

### Step 5: Confirm Takeover
- Click the "Takeover" button in the popup
- A confirmation popup will appear asking if you're sure to take over the selected reports
- This is a safety measure to prevent accidental ownership transfers
- Click "Yes" to confirm the takeover

### Step 6: Monitor Takeover Progress
- The takeover process will begin
- Each selected report's linked dataset and data sources will be transferred to your ownership
- Monitor the status indicators:
  - **Loading**: Ownership is being transferred
  - **Success**: Ownership transferred successfully
  - **Warning**: Ownership transferred with warnings
  - **Error**: Failed to transfer ownership

### Step 7: View Details
- Hover over status indicators to see detailed information about the takeover process
- Click the table icon (📊) in the top right of the popup to see a tabular view
- The table shows the status of each selected report and its takeover progress

## Important Notes

- **Ownership Transfer**: This feature transfers ownership of both the dataset and its associated data sources
- **Linked Datasets Only**: Only reports with linked datasets will be processed
- **Reports Without Datasets**: Reports with no linked dataset will be skipped
- **Permanent Action**: Ownership transfer is permanent and cannot be easily undone
- **Permissions**: Ensure you have appropriate permissions to take over the datasets

## What Gets Transferred

The takeover feature will transfer ownership of:
- **Linked Datasets**: The semantic models associated with the reports
- **Data Sources**: All data sources connected to the datasets
- **Permissions**: You will become the owner of these resources

## Benefits of Takeover

Taking over ownership provides you with:
- **Full Control**: Complete control over the datasets and data sources
- **Modification Rights**: Ability to modify dataset configurations
- **Refresh Control**: Control over refresh schedules and settings
- **Sharing Rights**: Ability to share and manage access to the resources

## Troubleshooting

**Takeover Fails**
- Check your permissions for the datasets and data sources
- Ensure the datasets still exist and are accessible
- Verify your internet connection is stable
- Check if you have the necessary permissions to take ownership

**Dataset Not Found**
- Some reports may not have linked datasets
- These reports will be skipped during takeover

**Permission Issues**
- Ensure you have appropriate permissions to take over the datasets
- Contact your Power BI administrator if needed
- Some datasets may have restrictions that prevent ownership transfer

**Ownership Already Yours**
- If you already own the dataset, the takeover will be skipped
- This is normal behavior and not an error

**Data Source Issues**
- Check if the data sources are accessible
- Verify you have permissions to access the data sources
- Some data sources may have restrictions that prevent ownership transfer 