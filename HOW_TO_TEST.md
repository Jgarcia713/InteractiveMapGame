# How to Test the Admin Dashboard Form

## Quick Start Guide

### Step 1: Make Sure the App is Running

The app should be running on **http://localhost:5004**

If it's not running, start it with:
```bash
dotnet run
```

### Step 2: Create an Admin Account (if needed)

If you don't have an admin account yet, create one using one of these methods:

**Option A: Using the Admin Seed Endpoint (if no admin exists)**
```bash
curl -X POST http://localhost:5004/api/AdminSeed/create-initial-admin \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123","email":"admin@example.com"}'
```

**Option B: Using the Admin Create Endpoint**
```bash
curl -X POST http://localhost:5004/api/Admin/create \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123","email":"admin@example.com"}'
```

### Step 3: Access the Admin Dashboard

1. Open your web browser
2. Navigate to: **http://localhost:5004/admin/login.html**
3. Enter your credentials:
   - Username: `admin` (or whatever you used)
   - Password: `admin123` (or whatever you used)
4. Click "Login"
5. You should be redirected to the dashboard

### Step 4: Test the Single Exhibit Form

1. In the admin dashboard, click **"Manage Content"** in the sidebar (the ➕ icon)
2. You should see the **"Add Single Exhibit"** tab (already active)
3. Fill out the form with test data:

   **Required Fields:**
   - **Name**: `Test Exhibit`
   - **Type**: `Aircraft`
   - **X Coordinate**: `100`
   - **Y Coordinate**: `200`

   **Optional Fields (you can fill these too):**
   - **Description**: `This is a test exhibit`
   - **Category**: `Fighter`
   - **Era**: `2020s`
   - **Manufacturer**: `Test Manufacturer`
   - **Z Coordinate**: `0`
   - **Experience Value**: `100`
   - **Difficulty Level**: `3`
   - Check **IsInteractive** and **IsDiscoverable** (already checked by default)

4. Click **"Create Exhibit"** button
5. **Expected Result:**
   - ✅ Success message: "Exhibit created successfully!"
   - ✅ Form resets
   - ✅ New exhibit appears in the "Existing Exhibits" list below
   - ✅ Dashboard stats update (Total Map Objects increases)

### Step 5: Test CSV Bulk Upload

1. Still on the "Manage Content" page, click the **"Bulk Upload (CSV)"** tab
2. Click **"Download CSV Template"** - this downloads a sample CSV file
3. Open the downloaded CSV file (`map_objects_template.csv`)
4. You can:
   - Edit the existing rows
   - Add new rows following the same format
   - Keep the header row as is
5. Back in the browser, click **"Select CSV File"** and choose your CSV file
6. Click **"Upload CSV"** button
7. **Expected Result:**
   - ✅ Progress indicator shows
   - ✅ Success message: "Successfully uploaded X exhibits!"
   - ✅ File input clears
   - ✅ New exhibits appear in the "Existing Exhibits" list
   - ✅ Dashboard stats update

### Step 6: Verify the Exhibits

1. Scroll down to the **"Existing Exhibits"** section
2. You should see all the exhibits you created
3. Each exhibit shows:
   - Name
   - Type
   - Category (if provided)
   - Position coordinates
   - Lock status
   - Discoverable status
4. You can delete exhibits by clicking the **"Delete"** button

### Step 7: Check Dashboard Stats

1. Click **"Dashboard"** in the sidebar (📊 icon)
2. You should see:
   - **Total Map Objects**: Count of all exhibits
   - **Total Interactions**: Count of user interactions
   - **Total Admins**: Count of admin accounts

## Troubleshooting

### Issue: "Authentication required" error
**Solution:** Make sure you're logged in. Go back to `/admin/login.html` and log in again.

### Issue: Form submission fails with 404 error
**Solution:** The backend controller might be missing. Check the browser console (F12) for specific errors.

### Issue: CSV upload fails
**Solution:**
- Make sure the CSV file has the correct header row
- Check that required fields (Name, Type, X, Y) are filled for each row
- Look at the browser console (F12) for specific error messages
- Check the Network tab to see the API response

### Issue: Can't create admin account
**Solution:** 
- If you get "An admin already exists", use the login page instead
- If you get connection errors, make sure the database is running

## Testing Checklist

- [ ] Admin account created successfully
- [ ] Can log in to admin dashboard
- [ ] Can access "Manage Content" page
- [ ] Single exhibit form displays correctly
- [ ] Can create a single exhibit successfully
- [ ] Exhibit appears in the list after creation
- [ ] CSV template downloads correctly
- [ ] Can upload CSV file successfully
- [ ] Exhibits from CSV appear in the list
- [ ] Can delete exhibits
- [ ] Dashboard stats update correctly

## Browser Developer Tools

To debug issues:
1. Press **F12** to open Developer Tools
2. Check the **Console** tab for JavaScript errors
3. Check the **Network** tab to see API requests/responses
4. Look for red error messages in the console

## Expected API Endpoints

The form uses these endpoints:
- `POST /api/MapObjectAdmin/create` - Create single exhibit
- `POST /api/MapObjectAdmin/bulk-upload` - Upload CSV file
- `GET /api/MapObjectAdmin/list` - List all exhibits
- `DELETE /api/MapObjectAdmin/{id}` - Delete exhibit

If these endpoints don't exist, you'll need to create the `MapObjectAdminController.cs` file.


