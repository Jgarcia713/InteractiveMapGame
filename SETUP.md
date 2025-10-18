# Interactive Map Game - Setup Guide

## 🚀 Quick Start for Team Members

### Prerequisites
- .NET 8.0 SDK installed
- SQL Server LocalDB or SQL Server Express
- OpenAI API Key (for AI features)

### Step 1: Clone and Restore
```bash
git clone [repository-url]
cd InteractiveMapGame
dotnet restore
```

### Step 2: Database Setup

**Option A: SQL Server LocalDB (Recommended for Development)**
```bash
# Install SQL Server LocalDB if not already installed
# Download from: https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

# Create and update the database
dotnet ef database update
```

**Option B: SQL Server Express**
1. Install SQL Server Express from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
2. Update connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=InteractiveMapGame;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```
3. Run: `dotnet ef database update`

**Option C: SQLite (Alternative)**
1. Install SQLite package: `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`
2. Update `Program.cs` to use SQLite:
   ```csharp
   builder.Services.AddDbContext<MapGameDbContext>(options =>
       options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```
3. Update connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=InteractiveMapGame.db"
     }
   }
   ```

### Step 3: Configure OpenAI API
1. Get API key from https://platform.openai.com/api-keys
2. Add to `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "your-api-key-here"
     }
   }
   ```
   Or set environment variable: `OpenAI__ApiKey=your-api-key-here`

### Step 4: Run the Application
```bash
dotnet run
```

### Step 5: Test the Setup
- Open browser to `http://localhost:5000`
- You should see the interactive map interface
- Try clicking on objects and generating AI content

## 🎮 Adding Sample Data

### Create Sample Map Objects
```bash
# Run this SQL script in your database to add sample objects
```

```sql
INSERT INTO MapObjects (Name, Description, Type, Category, Era, Manufacturer, X, Y, Z, IsDiscoverable, IsUnlocked, CreatedAt, UpdatedAt)
VALUES 
('SR-71 Blackbird', 'The fastest aircraft ever built', 'Aircraft', 'Reconnaissance', '1960s', 'Lockheed', 100, 150, 0, 1, 1, GETUTCDATE(), GETUTCDATE()),
('Apollo 11 Command Module', 'The spacecraft that took humans to the moon', 'Spacecraft', 'Manned', '1960s', 'North American Aviation', 200, 100, 0, 1, 1, GETUTCDATE(), GETUTCDATE()),
('Hubble Space Telescope', 'Revolutionary space observatory', 'Satellite', 'Scientific', '1990s', 'Lockheed Martin', 150, 200, 0, 1, 0, GETUTCDATE(), GETUTCDATE()),
('F-22 Raptor', 'Fifth-generation fighter aircraft', 'Aircraft', 'Fighter', '2000s', 'Lockheed Martin', 300, 250, 0, 1, 0, GETUTCDATE(), GETUTCDATE()),
('Space Shuttle Discovery', 'Most flown space shuttle', 'Spacecraft', 'Manned', '1980s', 'Rockwell International', 250, 300, 0, 1, 0, GETUTCDATE(), GETUTCDATE());
```

## 🔧 Development Tools

### Database Management
- **SQL Server Management Studio (SSMS)**: https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
- **Azure Data Studio**: https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio
- **Visual Studio Code**: Install "SQL Server (mssql)" extension

### API Testing
- **Swagger UI**: `http://localhost:5000/swagger`
- **Postman**: Import the API collection
- **curl**: Use the provided examples

## 🐛 Troubleshooting

### Common Issues

**Database Connection Errors**
- Ensure SQL Server is running
- Check connection string format
- Verify database exists

**OpenAI API Errors**
- Verify API key is correct
- Check API key has sufficient credits
- Ensure internet connection

**Build Errors**
- Run `dotnet clean` and `dotnet restore`
- Check .NET 8.0 SDK is installed
- Verify all NuGet packages are restored

**Frontend Issues**
- Check browser console for JavaScript errors
- Ensure all static files are served correctly
- Verify CORS settings

### Performance Issues
- Reduce number of map objects for testing
- Use browser developer tools to profile
- Check database query performance

## 📊 Monitoring

### Application Logs
- Check console output for errors
- Enable detailed logging in `appsettings.json`
- Monitor database connection status

### Database Monitoring
```sql
-- Check object counts
SELECT Type, COUNT(*) as Count FROM MapObjects GROUP BY Type;

-- Check interaction logs
SELECT InteractionType, COUNT(*) as Count FROM InteractionLogs GROUP BY InteractionType;

-- Check LLM usage
SELECT COUNT(*) as TotalCalls, SUM(LLMTokens) as TotalTokens 
FROM InteractionLogs WHERE UsedLLM = 1;
```

## 🚀 Deployment

### Production Setup
1. **Database**: Configure production SQL Server
2. **Environment**: Set production environment variables
3. **HTTPS**: Enable SSL certificates
4. **CDN**: Consider CDN for static assets

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "InteractiveMapGame.dll"]
```

## 📝 Next Steps

### For Developers
1. **Explore the Code**: Review the project structure
2. **Add Features**: Implement new game mechanics
3. **Test Thoroughly**: Ensure all features work correctly
4. **Document Changes**: Update README and API docs

### For Content Creators
1. **Add Objects**: Create new map objects with rich metadata
2. **Generate Content**: Use AI to populate object descriptions
3. **Test Interactions**: Verify all interactions work smoothly
4. **Optimize Performance**: Ensure smooth gameplay experience

---

**Happy coding! 🎮✨**
