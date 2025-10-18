# Interactive Map Game

A modern ASP.NET Core web application featuring an interactive, game-like map interface with AI-powered content generation and 360° video support. Explore aerospace history through an engaging, Civilization-style navigation experience.

## 🎮 Game Features

### 🗺️ Interactive Map Interface
- **Game-like Navigation**: Civilization-style map exploration with smooth panning and zooming
- **Dynamic Object Discovery**: Interactive objects that can be discovered, unlocked, and explored
- **Visual Feedback**: Glowing objects with different states (undiscovered, discovered, unlocked)
- **Smooth Animations**: Hover effects, scaling, and transition animations

### 🤖 AI-Powered Content Generation
- **LLM Integration**: OpenAI GPT-3.5-turbo for dynamic content generation
- **Context-Aware Responses**: AI generates descriptions, stories, and facts based on object data
- **Multiple Content Types**: 
  - **Descriptions**: Detailed, engaging object descriptions
  - **Stories**: Historical narratives and interesting tales
  - **Facts**: Technical details and fascinating information
- **Smart Prompting**: Context-aware prompts that consider object type, era, and category

### 🎯 Game Mechanics
- **Player Progression**: Experience points, levels, and discovery tracking
- **Object States**: Undiscovered → Discovered → Unlocked progression
- **Interaction Logging**: Comprehensive tracking of player interactions
- **Session Management**: Anonymous player sessions with persistent progress

### 🎥 360° Video Support (Stretch Goal)
- **Immersive Experiences**: 360° video playback for selected objects
- **Full-Screen Mode**: Dedicated video player with controls
- **Video Integration**: Seamless integration with map objects

## 🏗️ Technical Architecture

### Backend (.NET 8.0)
- **ASP.NET Core Web API**: RESTful API with comprehensive endpoints
- **Entity Framework Core**: Database ORM with SQL Server integration
- **Swagger/OpenAPI**: Interactive API documentation
- **CORS Support**: Cross-origin resource sharing for web clients

### Frontend (Vanilla JavaScript)
- **Interactive Map Canvas**: Custom-built map interface with game-like controls
- **Responsive Design**: Works on desktop, tablet, and mobile devices
- **Real-time Updates**: Dynamic content loading and state management
- **Modern UI**: Glass-morphism design with smooth animations

### Database Schema
- **MapObjects**: Core game objects with positioning, metadata, and LLM content
- **PlayerProgress**: Player statistics, discovery progress, and session data
- **InteractionLogs**: Comprehensive interaction tracking and analytics

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server LocalDB (comes with Visual Studio)
- OpenAI API Key (for AI features)

### Installation

1. **Clone and Setup**:
   ```bash
   git clone [repository-url]
   cd InteractiveMapGame
   dotnet restore
   ```

2. **Database Setup**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Configure OpenAI API**:
   - Get API key from https://platform.openai.com/api-keys
   - Add to `appsettings.json`:
     ```json
     {
       "OpenAI": {
         "ApiKey": "your-api-key-here"
       }
     }
     ```

4. **Run the Application**:
   ```bash
   dotnet run
   ```

5. **Open in Browser**:
   - Navigate to `http://localhost:5000`
   - Explore the interactive map!

## 🎮 How to Play

### Basic Navigation
- **Click and Drag**: Pan around the map
- **Click Objects**: Select and interact with map objects
- **Hover**: See object names and basic info

### Object Interaction
- **Select Objects**: Click on glowing objects to view details
- **Generate Content**: Use AI to create descriptions, stories, and facts
- **Discover**: Unlock new objects through exploration
- **Progress**: Track your experience and discovery stats

### Content Generation
- **Get Description**: AI-generated detailed descriptions
- **Hear Story**: Historical narratives and interesting tales
- **Learn Facts**: Technical details and fascinating information

## 🔧 API Endpoints

### Map Management
- `GET /api/Map/objects` - Get all discoverable objects
- `GET /api/Map/objects/{id}` - Get specific object details
- `GET /api/Map/objects/nearby` - Find objects near coordinates
- `GET /api/Map/objects/type/{type}` - Filter objects by type
- `POST /api/Map/objects` - Create new map object
- `PUT /api/Map/objects/{id}` - Update object
- `DELETE /api/Map/objects/{id}` - Delete object

### AI Content Generation
- `POST /api/LLM/generate-content` - Generate AI content for objects
- `POST /api/LLM/populate-object` - Auto-populate object with AI content

### Interaction Tracking
- `POST /api/Map/objects/{id}/interact` - Log player interactions

## 🎨 Customization

### Adding New Object Types
1. Update the `Type` enum in your frontend
2. Add corresponding icons in `getObjectIcon()` function
3. Update AI prompts for new object types

### Customizing AI Prompts
- Modify `CreateSystemPrompt()` and `CreateUserPrompt()` in `LLMController.cs`
- Adjust temperature and max_tokens for different content types
- Add new content types by extending the API

### Styling the Map
- Update CSS variables for colors and animations
- Modify object appearance in `.map-object` styles
- Customize the sidebar and UI components

## 🎥 360° Video Integration

### Adding 360° Videos
1. Upload 360° videos to your server
2. Set `Video360Url` property on map objects
3. Videos will automatically appear in the video player

### Supported Formats
- MP4 (recommended)
- WebM
- OGG

### Video Player Features
- Full-screen 360° playback
- Touch/mouse controls for navigation
- Automatic aspect ratio adjustment

## 📊 Analytics & Tracking

### Player Analytics
- **Interaction Logging**: Every click, hover, and interaction is tracked
- **Performance Metrics**: Duration, success rates, and user behavior
- **LLM Usage**: Token consumption and API costs
- **Discovery Patterns**: Which objects are most popular

### Database Queries
```sql
-- Most interacted objects
SELECT mo.Name, COUNT(il.Id) as InteractionCount
FROM MapObjects mo
JOIN InteractionLogs il ON mo.Id = il.MapObjectId
GROUP BY mo.Id, mo.Name
ORDER BY InteractionCount DESC

-- LLM usage statistics
SELECT COUNT(*) as TotalLLMCalls, SUM(LLMTokens) as TotalTokens
FROM InteractionLogs
WHERE UsedLLM = 1
```

## 🔒 Security & Performance

### Security Features
- **Input Validation**: All user inputs are validated and sanitized
- **SQL Injection Protection**: Entity Framework parameterized queries
- **CORS Configuration**: Proper cross-origin resource sharing
- **API Key Security**: Environment-based configuration

### Performance Optimizations
- **Database Indexing**: Optimized queries for map objects and interactions
- **Caching**: Consider implementing Redis for frequently accessed data
- **Lazy Loading**: Objects loaded on-demand based on map position
- **Compression**: Static file compression for assets

## 🚀 Deployment

### Production Setup
1. **Database**: Configure production SQL Server connection
2. **Environment Variables**: Set OpenAI API key securely
3. **HTTPS**: Enable SSL certificates
4. **CDN**: Consider CDN for static assets and videos

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "InteractiveMapGame.dll"]
```

## 🤝 Contributing

### Development Setup
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

### Code Style
- Follow C# naming conventions
- Use async/await for database operations
- Include XML documentation for public APIs
- Write unit tests for controllers

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🎯 Future Enhancements

### Planned Features
- **Multiplayer Support**: Real-time collaboration and shared discoveries
- **Quest System**: Guided exploration with objectives and rewards
- **Social Features**: Share discoveries and compete with friends
- **Mobile App**: Native mobile application with offline support
- **VR Integration**: Virtual reality exploration mode
- **Advanced AI**: More sophisticated content generation and personalization

### Technical Improvements
- **Real-time Updates**: SignalR for live collaboration
- **Advanced Analytics**: Machine learning for user behavior analysis
- **Performance**: WebGL rendering for complex 3D maps
- **Accessibility**: Full screen reader and keyboard navigation support

## 🆘 Support

### Common Issues
- **Database Connection**: Ensure SQL Server LocalDB is running
- **OpenAI API**: Verify API key is correctly configured
- **CORS Errors**: Check browser console for cross-origin issues
- **Performance**: Consider reducing map object count for better performance

### Getting Help
- Check the GitHub Issues page
- Review the API documentation at `/swagger`
- Contact the development team

---

**Built with ❤️ using ASP.NET Core, Entity Framework, and OpenAI GPT-3.5-turbo**
