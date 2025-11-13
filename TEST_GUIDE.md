# Test Guide - Exhibit Management Feature

## Quick Test Steps

### Step 1: Configure Database Connection (if not already done)

If you don't have a database connection string configured, you need to set it up:

```bash
# Set your database connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=InteractiveMapGame;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=true;MultipleActiveResultSets=true"

# Or if using SQLite for local testing:
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=InteractiveMapGame.db"
```

**Note**: If you're using SQL Server, make sure your database exists and migrations are applied.

### Step 2: Apply Database Migrations

```bash
dotnet ef database update
```

### Step 3: Run the Application

```bash
dotnet run
```

The application will start on `http://localhost:5000` or `https://localhost:5001` (check the console output for the exact URL).

### Step 4: Create Your First Admin Account

If you don't have an admin account yet, you can create one:

**Option A: Using the API directly (if no admin exists)**

```bash
# Using curl (replace with your actual values)
curl -X POST http://localhost:5000/api/Admin/create \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123","email":"admin@example.com"}'
```

**Option B: Using the Admin Seed Controller**

```bash
curl -X POST http://localhost:5000/api/AdminSeed/create-initial-admin \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123","email":"admin@example.com"}'
```

### Step 5: Test the Admin Login

1. Open your browser and go to: `http://localhost:5000/admin/login.html`
2. Enter your admin credentials (username: `admin`, password: `admin123`)
3. Click "Login"

### Step 6: Test Adding a Single Exhibit

1. After logging in, you'll see the admin dashboard
2. Click **"Manage Content"** in the sidebar (the ➕ icon)
3. You should see two tabs: **"Add Single Exhibit"** and **"Bulk Upload (CSV)"**
4. Fill out the form:
   - **Name**: `Test Exhibit`
   - **Type**: `Aircraft`
   - **X Coordinate**: `100`
   - **Y Coordinate**: `200`
   - (Other fields are optional)
5. Click **"Create Exhibit"**
6. You should see a success message and the exhibit should appear in the "Existing Exhibits" list below

### Step 7: Test Bulk CSV Upload

1. Click on the **"Bulk Upload (CSV)"** tab
2. Click **"Download CSV Template"** to get a sample CSV file
3. Open the downloaded CSV file and edit it (or use the provided template)
4. Click **"Select CSV File"** and choose your CSV file
5. Click **"Upload CSV"**
6. You should see a success message showing how many exhibits were uploaded

### Step 8: Verify Exhibits in Database

You can verify that exhibits were created by:
- Checking the "Existing Exhibits" list in the admin dashboard
- Viewing the stats on the dashboard (Total Map Objects should increase)
- Checking the public map at `http://localhost:5000/` (if exhibits are discoverable)

## Troubleshooting

### Issue: "Authentication required" error
- **Solution**: Make sure you're logged in as an admin. Go to `/admin/login.html` and log in first.

### Issue: Database connection error
- **Solution**: Check your connection string in user secrets. Make sure your database server is running and accessible.

### Issue: "No admin exists" error when trying to create admin
- **Solution**: If you already have an admin, use the login page. If not, use one of the API endpoints mentioned in Step 4.

### Issue: Form submission fails
- **Solution**: Check the browser console (F12) for error messages. Make sure all required fields (Name, Type, X, Y) are filled.

### Issue: CSV upload fails
- **Solution**: 
  - Make sure the CSV file has the correct header row
  - Check that required fields (Name, Type, X, Y) are filled for each row
  - Look at the browser console for specific error messages
  - The API will return errors for problematic rows

## Test Data Examples

### Single Exhibit Test Data:
- Name: `SR-71 Blackbird`
- Type: `Aircraft`
- Description: `The fastest aircraft ever built`
- Category: `Reconnaissance`
- Era: `1960s`
- Manufacturer: `Lockheed`
- X: `100`
- Y: `150`
- Z: `0`
- IsInteractive: ✓ (checked)
- IsDiscoverable: ✓ (checked)
- IsUnlocked: ☐ (unchecked)
- ExperienceValue: `100`
- DifficultyLevel: `3`

### CSV Test Data:
The template includes example rows. You can add more rows following the same format.

## Next Steps After Testing

Once you've verified everything works:
1. The exhibits will be saved to the `MapObjects` table in your database
2. New exhibits will appear on the public map (if `IsDiscoverable` is true)
3. You can later add real-time map updates using SignalR or WebSockets



