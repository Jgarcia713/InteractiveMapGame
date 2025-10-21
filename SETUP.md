# Interactive Map Game - Setup Guide

## 🚀 Quick Start for Team Members

### Prerequisites
- .NET 8.0 SDK installed
- Access to the team's MSSQL server (credentials provided separately)
- OpenAI API Key (for AI features)

### Step 1: Clone and Restore
```bash
git clone [repository-url]
cd InteractiveMapGame
dotnet restore
```

### Step 2: Database Setup (MSSQL Server)

**Configure User Secrets for Database Connection**

The project uses **User Secrets** to securely store database credentials. This ensures sensitive information is not committed to version control.

1. **Initialize User Secrets** (if not already done):
   ```bash
   dotnet user-secrets init
   ```

2. **Set Database Connection String**:
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER_NAME;Database=InteractiveMapGame;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=true;MultipleActiveResultSets=true"
   ```

   **Replace the placeholders with your actual credentials:**
   - `YOUR_SERVER_NAME` - The MSSQL server name/IP address
   - `YOUR_USERNAME` - Your database username  
   - `YOUR_PASSWORD` - Your database password

3. **Verify User Secrets**:
   ```bash
   dotnet user-secrets list
   ```

4. **Create Database Tables**:
   ```bash
   dotnet ef database update
   ```

5. **Populate with Sample Data**:
   ```bash
   # Start the application
   dotnet run
   
   # In another terminal, call the seeding endpoint
   Invoke-WebRequest -Uri "http://localhost:5004/api/Map/seed" -Method POST
   ```

**Note**: The `appsettings.json` file contains an empty connection string - this is intentional. The actual connection string is stored securely in user secrets.

### Step 3: Configure OpenAI API

**Option A: Using User Secrets (Recommended)**
```bash
dotnet user-secrets set "OpenAI:ApiKey" "your-api-key-here"
```

**Option B: Using appsettings.json**
```json
{
  "OpenAI": {
    "ApiKey": "your-api-key-here"
  }
}
```

**Option C: Using Environment Variables**
```bash
set OpenAI__ApiKey=your-api-key-here
```

**Get your API key from**: https://platform.openai.com/api-keys

### Step 4: Run the Application
```bash
dotnet run
```

### Step 5: Test the Setup
- Open browser to `http://localhost:5004`
- You should see the interactive map interface
- Try clicking on objects and generating AI content
- Visit `http://localhost:5004/swagger` for API documentation

## 🎮 Sample Data

The project includes a **seeding endpoint** that automatically populates the database with 16 aerospace objects including:

- **Aircraft**: SR-71 Blackbird, F-22 Raptor, Boeing 747, Concorde
- **Spacecraft**: Apollo 11, Space Shuttle Discovery, Dragon Capsule  
- **Satellites**: Hubble Space Telescope, Sputnik 1, GPS Satellite
- **Rockets**: Saturn V, Falcon Heavy
- **Helicopters**: Bell UH-1 Iroquois, Apache AH-64
- **Museums**: National Air and Space Museum, Kennedy Space Center

**The seeding is automatic** - just follow Step 2 above and the database will be populated with sample data.

## 🔧 Development Tools

### Database Management
- **SQL Server Management Studio (SSMS)**: https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
- **Azure Data Studio**: https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio
- **Visual Studio Code**: Install "SQL Server (mssql)" extension

### API Testing
- **Swagger UI**: `http://localhost:5004/swagger`
- **Postman**: Import the API collection
- **PowerShell**: Use `Invoke-WebRequest` for testing endpoints

## 🐛 Troubleshooting

### Common Issues

**Database Connection Errors**
- Verify user secrets are set correctly: `dotnet user-secrets list`
- Check MSSQL server is accessible from your machine
- Ensure database credentials are correct
- Test connection string format

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

## 🔐 Security & User Secrets

### What are User Secrets?
User Secrets allow you to store sensitive configuration data (like database connection strings and API keys) locally on your development machine without committing them to version control.

### Managing User Secrets
```bash
# List all secrets
dotnet user-secrets list

# Set a secret
dotnet user-secrets set "Key" "Value"

# Remove a secret
dotnet user-secrets remove "Key"

# Clear all secrets
dotnet user-secrets clear
```

### Security Best Practices
- ✅ **Never commit secrets** to version control
- ✅ **Use user secrets** for development
- ✅ **Use environment variables** for production
- ✅ **Rotate credentials** regularly
- ❌ **Don't hardcode** sensitive data in code
- ❌ **Don't share** user secrets files

### Team Collaboration
- Each team member needs their own user secrets
- Share connection details through secure channels (not in code)
- Use the same database server for consistency
- Document any additional configuration needed

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
