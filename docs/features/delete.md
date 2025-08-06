# Delete

The Delete feature allows you to bulk delete reports and their linked datasets/semantic models from Power BI Service workspaces.

## Prerequisites

- You must be logged into AutoPBI
- You need to have some workspaces loaded and visible
- You should select at least one report from any of the shown workspaces
- Ensure you have delete permissions for the reports and datasets

## Step-by-Step Process

### Step 1: Load Workspaces
- Launch AutoPBI and log in
- Ensure some workspaces are loaded and visible in the application
- Click on workspaces to expand and view their reports

### Step 2: Select Reports
- Select one or more reports from any of the shown workspaces
- You can select reports from different workspaces
- The selected reports will be highlighted

### Step 3: Click the Delete Button
- Once you're satisfied with your report selection, click the "Delete" button
- A delete popup will appear

### Step 4: Review Selection
- The popup will show the reports you've selected for deletion
- Review the list carefully to ensure you're deleting the correct reports

### Step 5: Confirm Deletion
- Click the "Delete" button in the popup
- A confirmation popup will appear asking if you really want to delete the reports
- This is a safety measure to prevent accidental deletions
- Click "Yes" to confirm the deletion

### Step 6: Monitor Deletion Progress
- The deletion process will begin
- Both the report and its linked dataset/semantic model will be deleted
- Monitor the status indicators:
  - **Loading**: Report is being deleted
  - **Success**: Report and dataset deleted successfully
  - **Warning**: Report deleted but dataset deletion had issues
  - **Error**: Failed to delete report or dataset

### Step 7: View Details
- Hover over status indicators to see detailed information
- Click the table icon (📊) in the top right of the popup to see a tabular view
- The table shows the status of each selected report and its deletion progress

## Important Notes

- **Linked Datasets**: The feature will attempt to delete the linked dataset/semantic model if:
  - The report has a linked dataset
  - You have access to the linked dataset
  - The dataset is in a workspace you have access to

- **Dataset Deletion Rules**:
  - If the report has no linked dataset, only the report will be deleted
  - If the report has a linked dataset but it's in another workspace you don't have access to, only the report will be deleted
  - If you have access to the linked dataset, both the report and dataset will be deleted

- **Permanent Action**: Deletion is permanent and cannot be undone
- **Permissions**: Ensure you have delete permissions for both reports and datasets

## Troubleshooting

**Delete Fails**
- Check your permissions for the reports and datasets
- Ensure the reports and datasets still exist
- Verify your internet connection is stable

**Dataset Not Deleted**
- Check if you have access to the dataset's workspace
- Verify you have delete permissions for the dataset
- The dataset may be shared across multiple reports

**Permission Issues**
- Ensure you have appropriate permissions in the workspace
- Contact your Power BI administrator if needed

**Confirmation Popup Not Appearing**
- Make sure you clicked the "Delete" button in the popup
- Check if the popup is hidden behind other windows 