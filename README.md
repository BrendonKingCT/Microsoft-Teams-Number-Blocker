# NumberBlocker

## Overview
NumberBlocker is a Windows Forms application designed to manage blocked number patterns for Microsoft Teams. The application utilizes PowerShell commands to connect to Microsoft Teams, log in with device authentication, and create new inbound blocked number patterns based on user-defined criteria.

## Features
- Connect to Microsoft Teams: Authenticate to Microsoft Teams using device authentication.
- Create Blocked Number Patterns: Specify a number pattern to block, along with a description and whether the pattern is enabled or disabled.
- User-Friendly Interface: Simple and intuitive interface for managing blocked number patterns.

## Prerequisites
Before running the application, ensure you have the following installed on your system:

- Windows OS
- .NET Framework 8.0
- Microsoft Teams PowerShell Module: This can be installed via PowerShell with the command:
```powershell
Install-Module -Name MicrosoftTeams -Force -AllowClobber
```

## Usage
1. Launch the application by running the executable or from within Visual Studio.
2. Connect to Teams:
    - Enter your tenant ID in the designated textbox.
    - Click the "Connect" button to authenticate.
3. Create a Blocked Number Pattern:
    - Enter the number pattern you wish to block in the Pattern textbox.
    - Provide a description/reason in the Description textbox.
    - Select whether the pattern should be Enabled or Disabled using the corresponding radio buttons.
    - Click the "Create Blocked Pattern" button to execute the command.
4. You will see feedback in the status label indicating whether the operation was successful or if there were any errors.

## License
This project is licensed under the GNU General Public License v3.0. See the LICENSE file for details.
