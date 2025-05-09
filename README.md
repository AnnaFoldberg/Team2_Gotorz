# Gotorz
Blazor Web Application for creating and booking holidays.

## Prerequisites
Before performing the steps below, ensure the following are in place:
1. **SQL Server Management Studio (SSMS)** is installed.
2. **SQL Server instance is running** via SSMS.

## Running Application with Server
___
### Step 1: Clone git repository
1. Open terminal or Git Bash
2. Run the following command to clone the project:
   ```bash
   git clone https://github.com/Longmose/Team2_Gotorz.git```

### Step 2: Set up connection to server
1. Open `Gotorz.sln`
2. In `appsettings.json`, add personal connection string
3. On **line 27** in `/Gotorz.Server/Program.cs`, insert the name of your connection string as the parameter

### Step 3: Run the application
1. Open `Gotorz.sln`
2. Build and run the application
