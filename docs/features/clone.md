# Clone

The Clone feature allows you to duplicate reports (along with their linked datasets) in bulk across different target workspaces.

## Prerequisites

- You must be logged into AutoPBI
- You need to have some workspaces loaded and visible
- You should select at least one report from any of the shown workspaces
- You need access to the target workspaces where you want to clone the reports

## Step-by-Step Process

### Step 1: Load Workspaces
- Launch AutoPBI and log in
- Ensure some workspaces are loaded and visible in the application
- Click on workspaces to expand and view their reports

### Step 2: Select Reports
- Select one or more reports from any of the shown workspaces
- You can select reports from different workspaces
- The selected reports will be highlighted

### Step 3: Click the Clone Button
- Once you're satisfied with your report selection, click the "Clone" button
- A clone popup will appear

### Step 4: Select Target Workspaces
- Click "Target Workspaces" to choose where to clone the selected reports
- A list of your accessible workspaces will appear
- You can search for specific workspaces using the search function
- Select one or more target workspaces
- **Note**: By default, you cannot clone reports in the same workspace

### Step 5: Review and Confirm
- Click "Clone" to proceed
- A summary will show the selected target workspaces for confirmation
- Review the summary to ensure you've selected the correct target workspaces

### Step 6: Monitor Cloning Progress
- The cloning process will begin
- Each selected report will be cloned to each selected target workspace
- Monitor the status indicators:
  - **Loading**: Report is being cloned
  - **Success**: Report cloned successfully
  - **Warning**: Report cloned with issues
  - **Error**: Failed to clone report

### Step 7: View Details
- Hover over status indicators to see detailed information
- Click the table icon (📊) in the top right of the popup to see a tabular view
- The table shows the status of each selected report and its cloning progress

## Important Notes

- **Same Workspace Restriction**: By default, you cannot clone reports within the same workspace
- **Linked Datasets**: Each cloned report will include its linked dataset
- **Permissions**: Ensure you have appropriate permissions in the target workspaces
- **Naming**: Cloned reports will maintain their original names
- **Data Sources**: The cloned reports will retain their data source connections

## Troubleshooting

**Clone Fails**
- Check your permissions in the target workspace
- Ensure the target workspace exists and is active
- Verify you have sufficient space in the target workspace

**Workspace Not Available**
- Make sure you have access to the workspace
- Check if the workspace exists and is active
- Verify your permissions in the workspace

**Same Workspace Error**
- You cannot clone reports within the same workspace by default
- Select a different target workspace

**Permission Issues**
- Ensure you have appropriate permissions in the target workspace
- Contact your Power BI administrator if needed 