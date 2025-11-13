# Step-by-Step Testing Guide for Admin Dashboard Form

## 🚀 Quick Start

### Step 1: Ensure App is Running

The app should already be running on **http://localhost:5004**

If not, start it:
```bash
cd /Users/akachukwumba/InteractiveMapGame-1
dotnet run
```

### Step 2: Create Admin Account

**Option A: Using the script (recommended)**
```bash
./create-admin.sh
```

**Option B: Using curl directly**
```bash
curl -X POST http://localhost:5004/api/AdminSeed/create-initial-admin \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123","email":"admin@example.com"}'
```

**Expected Response:**
- ✅ Success: `{"message":"Initial admin created successfully!","username":"admin"}`
- ❌ If admin exists: `{"message":"An admin already exists..."}` - Use login page instead

### Step 3: Open Admin Login Page

1. Open your web browser (Chrome, Firefox, Safari, etc.)
2. Navigate to: **http://localhost:5004/admin/login.html**
3. You should see a login form with:
   - Username field
   - Password field
   - "Remember me" checkbox
   - "Login" button

### Step 4: Log In

1. Enter your credentials:
   - **Username**: `admin`
   - **Password**: `admin123`
2. Optionally check "Remember me"
3. Click **"Login"** button
4. You should be redirected to the dashboard

### Step 5: Navigate to Content Management

1. In the admin dashboard, you'll see a sidebar on the left
2. Click on **"Manage Content"** (the ➕ icon)
3. You should see two tabs:
   - **"Add Single Exhibit"** (active by default)
   - **"Bulk Upload (CSV)"**

---

## 📝 Testing the Single Exhibit Form

### Visual Guide:

```
┌─────────────────────────────────────────────────┐
│ Add New Exhibit                                  │
├─────────────────────────────────────────────────┤
│ Name *              │ Type *                     │
│ [Test Exhibit]     │ [Aircraft]                 │
│                                                     │
│ Description                                       │
│ [This is a test...]                               │
│                                                     │
│ Category          │ Era                          │
│ [Fighter]         │ [2020s]                       │
│                                                     │
│ X Coordinate *    │ Y Coordinate *               │
│ [100]             │ [200]                         │
│                                                     │
│ [Create Exhibit] ← Click here!                    │
└─────────────────────────────────────────────────┘
```

### Test Steps:

1. **Fill Required Fields:**
   - **Name**: Type `Test Exhibit`
   - **Type**: Type `Aircraft`
   - **X Coordinate**: Type `100`
   - **Y Coordinate**: Type `200`

2. **Fill Optional Fields (optional but recommended):**
   - **Description**: `This is a test exhibit for verification`
   - **Category**: `Fighter`
   - **Era**: `2020s`
   - **Manufacturer**: `Test Manufacturer`
   - **Z Coordinate**: Leave as `0` or change to `50`
   - **Experience Value**: `100`
   - **Difficulty Level**: `3`
   - Check **IsInteractive** ✓ (should be checked by default)
   - Check **IsDiscoverable** ✓ (should be checked by default)
   - Leave **IsUnlocked** unchecked ☐

3. **Submit the Form:**
   - Click the **"Create Exhibit"** button
   - Wait for the response

4. **Expected Results:**
   - ✅ Green success message appears: "Exhibit created successfully!"
   - ✅ Form fields reset/clear
   - ✅ The new exhibit appears in the "Existing Exhibits" list below
   - ✅ Dashboard stats update (Total Map Objects increases by 1)

5. **If You See Errors:**
   - Check the browser console (Press F12 → Console tab)
   - Look for red error messages
   - Common issues:
     - "404 Not Found" → Backend controller missing
     - "401 Unauthorized" → Not logged in
     - "400 Bad Request" → Missing required fields

---

## 📊 Testing CSV Bulk Upload

### Step 1: Switch to Bulk Upload Tab

1. Click the **"Bulk Upload (CSV)"** tab
2. You should see:
   - CSV format requirements
   - Download template link
   - File input field
   - Upload button

### Step 2: Download CSV Template

1. Click **"Download CSV Template"** link
2. A file named `map_objects_template.csv` should download
3. Open the file in Excel, Numbers, or a text editor

### Step 3: Prepare CSV File

The template should look like this:

```csv
Name,Type,Description,Category,Era,Manufacturer,FirstFlight,Status,X,Y,Z,ImageUrl,ModelUrl,Video360Url,IsInteractive,IsDiscoverable,IsUnlocked,ExperienceValue,DifficultyLevel
SR-71 Blackbird,Aircraft,The fastest aircraft ever built,Reconnaissance,1960s,Lockheed,1964-12-22,Retired,100,150,0,,,true,true,false,100,3
F-22 Raptor,Aircraft,Fifth-generation stealth fighter aircraft,Fighter,2000s,Lockheed Martin,1997-09-07,Active,300,250,0,,,true,true,false,150,4
```

**You can:**
- Edit existing rows
- Add new rows (following the same format)
- Keep the header row as is

**Important:**
- Required fields: Name, Type, X, Y
- Date format: YYYY-MM-DD (e.g., 1969-07-16)
- Boolean values: `true` or `false` (lowercase)

### Step 4: Upload CSV File

1. Click **"Select CSV File"** button
2. Choose your CSV file from the file picker
3. Click **"Upload CSV"** button
4. You should see a progress indicator

### Step 5: Expected Results

- ✅ Green success message: "Successfully uploaded X exhibits!"
- ✅ File input clears
- ✅ New exhibits appear in the "Existing Exhibits" list
- ✅ Dashboard stats update

### Step 6: If Upload Fails

- Check browser console (F12 → Console)
- Check Network tab (F12 → Network) to see API response
- Common issues:
  - Invalid CSV format
  - Missing required fields
  - Incorrect data types

---

## ✅ Verification Checklist

After testing, verify:

- [ ] Can log in successfully
- [ ] Can see the "Manage Content" page
- [ ] Single exhibit form displays correctly
- [ ] Can create a single exhibit
- [ ] Exhibit appears in the list
- [ ] Can download CSV template
- [ ] Can upload CSV file
- [ ] Exhibits from CSV appear in list
- [ ] Can delete exhibits (using Delete button)
- [ ] Dashboard stats update correctly

---

## 🐛 Troubleshooting

### Issue: "Authentication required" or 401 error
**Solution:** Make sure you're logged in. Go back to `/admin/login.html` and log in again.

### Issue: "404 Not Found" when submitting form
**Solution:** The backend controller `MapObjectAdminController.cs` might be missing. Check the browser console for specific endpoint errors.

### Issue: Database errors
**Solution:** The database might need migrations. Try:
```bash
dotnet ef database update --force
```

### Issue: Can't create admin account
**Solution:** 
- If you see "An admin already exists", just use the login page
- Make sure the app is running
- Check if database file exists: `ls -la *.db`

### Issue: Form doesn't submit
**Solution:**
1. Open browser console (F12)
2. Check for JavaScript errors
3. Verify all required fields are filled
4. Check Network tab to see if request is sent

---

## 📸 What to Look For

### Success Indicators:
- ✅ Green success messages
- ✅ Forms reset after submission
- ✅ New items appear in lists
- ✅ Stats update dynamically
- ✅ No red errors in console

### Error Indicators:
- ❌ Red error messages
- ❌ 404/401/500 errors in console
- ❌ Forms don't reset
- ❌ Items don't appear in lists
- ❌ Stats don't update

---

## 🎯 Next Steps After Testing

Once you've verified everything works:

1. **Document any issues** you found
2. **Test edge cases:**
   - Very long names
   - Special characters
   - Invalid coordinates
   - Empty CSV rows
3. **Test with real data** if applicable
4. **Report any bugs** or missing features

---

## 📞 Need Help?

If you encounter issues:
1. Check browser console (F12)
2. Check Network tab for API responses
3. Review error messages carefully
4. Check if app is running: `curl http://localhost:5004/swagger`


