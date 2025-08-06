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

## Security Features

- **Secure Storage**: If "Remember me" is enabled, credentials are encrypted and stored locally
- **Session Management**: Login sessions are managed securely
- **No Credential Sharing**: Your credentials are never shared or transmitted to third parties

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