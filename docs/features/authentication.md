# Authentication

AutoPBI uses Microsoft Power BI accounts for authentication. You cannot sign up through the application - you must use an existing Power BI account.

## Step-by-Step Login Process

### Step 1: Open the Application
- Launch AutoPBI by double-clicking `AutoPBI.exe`
- The main interface will appear

### Step 2: Click the Login Button
- Look for the "Login" button in the application interface
- Click it to open the login popup

### Step 3: Enter Your Credentials
- In the login popup, you'll see two input fields:
  - **Username**: Enter your Microsoft Power BI account email address
  - **Password**: Enter your account password
- The password field is masked by default for security

### Step 4: Remember Me (Optional)
- If you want to save your login credentials for future use:
  - Check the "Remember me" checkbox
  - This will allow you to skip the login process on future application launches
  - Your credentials are stored securely using encryption

### Step 5: Complete Login
- Click the "Login" button to proceed
- The application will authenticate with Power BI Service
- Upon successful login, the application will automatically load your accessible workspaces and their reports

## Post-Login Experience

After successful authentication:

- **Workspaces Load**: All workspaces you have access to will be displayed
- **Reports Load**: Reports within each workspace will be loaded automatically
- **Interactive Interface**: You can click on workspaces to expand/collapse and view their reports
- **Ready for Operations**: You can now use all the bulk operation features
- **User Profile**: Your user image will appear in the top right corner of the application

## Sign Out Process

### Step 1: Access User Menu
- Click on your user image in the top right corner of the application
- A popup will appear displaying your username and profile image

### Step 2: Sign Out
- Click the "Sign Out" button in the popup
- The application will immediately sign you out

### Step 3: Post-Sign Out
After signing out:
- **Saved Credentials**: If you had "Remember me" enabled, your saved login information will be deleted
- **Workspaces Cleared**: All previously loaded workspaces and reports will be removed from the interface
- **Session Ended**: Your Power BI Service session will be terminated
- **Return to Login**: The application will return to the login state

## Security Features

- **Secure Storage**: If "Remember me" is enabled, credentials are encrypted and stored locally
- **Session Management**: Login sessions are managed securely
- **No Credential Sharing**: Your credentials are never shared or transmitted to third parties
- **Automatic Cleanup**: Sign out automatically clears saved credentials and session data

## Troubleshooting

**Login Fails**
- Verify your Power BI account credentials
- Ensure you have an active Power BI account
- Check your internet connection
- Make sure the MicrosoftPowerBIMgmt PowerShell module is installed

**Remember Me Not Working**
- Check if the application has permission to write to the AppData folder
- Try logging in again and re-checking the "Remember me" option

**Workspaces Not Loading**
- Ensure you have access to at least one Power BI workspace
- Check your Power BI Service permissions
- Try logging out and logging back in

**Sign Out Issues**
- If the sign out button doesn't respond, try clicking it again
- If the user menu doesn't appear, ensure you're logged in
- If saved credentials aren't cleared, manually delete them from the AppData folder 