# Publish

The Publish feature allows you to import and publish multiple .pbix files to Power BI Service workspaces in bulk.

## Prerequisites

- You must be logged into AutoPBI
- You need access to the target workspaces where you want to publish reports
- You should have .pbix files ready for import

## Step-by-Step Process

### Step 1: Open the Application
- Launch AutoPBI and log in with your Power BI account
- Ensure your workspaces are loaded

### Step 2: Click the Publish Button
- Look for the "Publish" button in the application interface
- Click it to open the publish popup

### Step 3: Import Power BI Files
- In the publish popup, click the "Import Power BI Files" button
- A file picker dialog will open
- Select one or more .pbix files you want to publish
- The selected files will appear in the imported reports list

### Step 4: Select Target Workspaces
- Click "Target Workspaces" to choose where to publish your reports
- A list of your accessible workspaces will appear
- You can search for specific workspaces using the search function
- Select one or more target workspaces

### Step 5: Review and Confirm
- Click "Publish" to proceed
- A summary will show the selected target workspaces for confirmation
- Review the summary to ensure you've selected the correct workspaces

### Step 6: Monitor Progress
- The publishing process will begin
- Each .pbix file will be published as a separate report and semantic model for each selected workspace
- Monitor the status indicators:
  - **Loading**: Report is being processed
  - **Success**: Report published successfully
  - **Warning**: Report published with issues
  - **Error**: Failed to publish report

### Step 7: View Details
- Hover over status indicators to see detailed information
- The popup will show progress for each report being published

## Important Notes

- **Separate Reports**: Each .pbix file becomes a separate report in the target workspace
- **Semantic Models**: Each report will also create its own semantic model
- **Permissions**: Ensure you have publish permissions in the target workspaces
- **File Size**: Large .pbix files may take longer to publish

## Troubleshooting

**Publish Fails**
- Check your permissions in the target workspace
- Ensure the .pbix files are valid and not corrupted
- Verify your internet connection is stable

**Workspace Not Available**
- Make sure you have access to the workspace
- Check if the workspace exists and is active

**File Import Issues**
- Ensure the files are valid .pbix format
- Check if the files are not locked or in use by another application 