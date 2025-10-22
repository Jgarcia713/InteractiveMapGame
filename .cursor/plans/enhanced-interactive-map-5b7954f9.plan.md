<!-- 5b7954f9-f240-4041-8526-d7e99c4019e3 b1370d65-860c-4bd4-91a8-4f83c5af2858 -->
# Enhanced Interactive Map Development Plan

## Phase 1: Admin Dashboard Foundation

### 1.1 Admin Authentication & Authorization

- Create admin login system with JWT authentication
- Add `AdminController.cs` with authentication endpoints
- Create `Admin` model with roles (Super Admin, Content Manager, Viewer)
- Add admin middleware to protect admin routes

### 1.2 Admin Panel UI Structure

Create `wwwroot/admin/` directory with:

- **index.html**: Main admin dashboard layout with sidebar navigation
- **login.html**: Admin authentication page
- **admin-styles.css**: Dedicated styling for admin interface
- **admin-app.js**: Core admin functionality and API integration

### 1.3 CRUD Interface for Map Objects

Build comprehensive map object management:

- **List View**: Searchable, filterable table of all map objects
- **Create Form**: Add new objects with all properties (coordinates, metadata, media URLs)
- **Edit Form**: Update existing objects with validation
- **Delete Functionality**: Soft delete with confirmation
- **Image Upload**: File upload for object images (or URL input)
- **Bulk Operations**: Import/export CSV or JSON

## Phase 2: Enhanced Map Interactions

### 2.1 Advanced Map Controls

Enhance `wwwroot/index.html` with:

- **Zoom Levels**: Implement 3-5 zoom levels with smooth transitions
- **Pan Boundaries**: Set map boundaries to prevent infinite panning
- **Minimap**: Small overview map showing current viewport
- **Search Bar**: Real-time search for objects by name, type, category
- **Filters**: Toggle visibility by object type, era, status
- **Legend**: Visual key showing object states and types

### 2.2 Map Positioning & Clustering

- **Object Clustering**: Group nearby objects at lower zoom levels
- **Dynamic Visibility**: Show/hide objects based on zoom level
- **Smooth Animations**: Eased transitions for pan, zoom, and object selection
- **Tooltip System**: Rich tooltips with preview information on hover

### 2.3 Visual Map Editor (Admin Only)

- **Drag-and-Drop Positioning**: Admins can drag objects to reposition
- **Grid Overlay**: Optional grid for precise placement
- **Coordinate Display**: Show X, Y coordinates while positioning
- **Save Positions**: Update database with new coordinates via API

## Phase 3: 3D Model Integration

### 3.1 3D Model Viewer Setup

Add to `wwwroot/`:

- Include Three.js library (via CDN or local)
- Create `model-viewer.js` for 3D rendering logic
- Add model viewer modal/container in HTML

### 3.2 Model Viewer Features

- **GLB/GLTF Support**: Load standard 3D model formats
- **Orbit Controls**: Mouse drag to rotate, scroll to zoom
- **Lighting**: Basic three-point lighting setup
- **Loading States**: Progress indicator for model loading
- **Fallback**: Show static image if 3D model unavailable

### 3.3 Model Integration

Update `MapObject` model:

- Ensure `ModelUrl` property exists (already present)
- Add "View 3D Model" button in object details panel
- Lazy load Three.js only when needed

## Phase 4: 360° Media Integration

### 4.1 360° Image Viewer

- **Library Integration**: Add Pannellum or Photo Sphere Viewer library
- **Static 360° Images**: Equirectangular image support
- **Mouse/Touch Controls**: Drag to look around, scroll to zoom
- **Hotspots**: Optional clickable points within 360° images

### 4.2 360° Video Enhancement

Improve existing video player (`wwwroot/index.html`):

- Add 360° controls for video playback
- Support both equirectangular videos and regular videos
- Auto-detect media type based on object properties

### 4.3 Media Manager

- **Media Type Detection**: Automatically choose viewer based on file type
- **Seamless Switching**: Unified "View Immersive Experience" button
- **Media Fallback**: Image → Video → 3D Model priority

## Phase 5: Analytics Dashboard

### 5.1 Analytics API Endpoints

Add to `MapController.cs`:

- `GET /api/Map/analytics/popular-objects`: Most interacted objects
- `GET /api/Map/analytics/user-stats`: User engagement metrics
- `GET /api/Map/analytics/llm-usage`: AI generation statistics
- `GET /api/Map/analytics/interaction-timeline`: Time-based interaction data

### 5.2 Admin Analytics UI

Create `wwwroot/admin/analytics.html`:

- **Chart.js Integration**: Visual charts and graphs
- **Key Metrics Cards**: Total objects, interactions, users, LLM calls
- **Popular Objects**: Top 10 most viewed/interacted objects
- **User Engagement**: Average session time, interactions per session
- **LLM Cost Tracking**: Token usage and estimated costs
- **Heatmap**: Visual representation of object popularity on map

## Phase 6: Data Import & Content Management

### 6.1 Bulk Import System

- **CSV Import**: Parse CSV files with object data
- **JSON Import**: Bulk import from structured JSON
- **Validation**: Check for required fields, valid coordinates
- **Preview**: Show import preview before committing
- **Error Handling**: Report invalid entries without failing entire import

### 6.2 Media URL Management

Create dedicated media management interface:

- **URL Validation**: Check if URLs are accessible
- **Batch URL Update**: Update multiple objects at once
- **Media Type Auto-Detection**: Identify image/video/model from URL
- **CDN Integration**: Support for cloud storage URLs

### 6.3 Content Templates

- **Object Templates**: Pre-defined templates for common object types
- **Quick Add**: Rapidly add objects using templates
- **LLM Auto-Populate**: Option to auto-generate descriptions for new objects

## Phase 7: Enhanced User Experience

### 7.1 Progressive Loading

- **Lazy Loading**: Load objects only in viewport or nearby
- **Image Optimization**: Thumbnail previews, full images on demand
- **Caching Strategy**: Cache frequently accessed data
- **Loading States**: Skeleton screens and smooth transitions

### 7.2 Responsive Design Improvements

- **Mobile Optimization**: Touch-friendly controls and layouts
- **Tablet Support**: Optimized for medium-sized screens
- **Accessibility**: ARIA labels, keyboard navigation, screen reader support

### 7.3 User Preferences

- **Theme Toggle**: Light/dark mode
- **Sound Effects**: Optional audio feedback (on/off toggle)
- **Tutorial Overlay**: First-time user guide
- **Save Preferences**: LocalStorage for user settings

## Technical Implementation Notes

### Database Updates

- No major schema changes required (existing models support all features)
- Add indexes for performance: `Type`, `Category`, `Era`, `IsDiscoverable`
- Consider adding `IsDeleted` flag for soft deletes

### API Structure

All admin endpoints: `/api/Admin/*`

Enhanced map endpoints: `/api/Map/*` (existing structure)

Analytics endpoints: `/api/Map/analytics/*`

### Security Considerations

- Admin routes protected by JWT authentication
- CORS configuration for admin domain if separate
- Rate limiting for public API endpoints
- Input validation and sanitization throughout

### Performance Targets

- Map loads in < 2 seconds with 100+ objects
- 3D models load in < 5 seconds (with progress indicator)
- Admin dashboard queries execute in < 500ms
- Smooth 60fps animations for map interactions

## File Structure

```
wwwroot/
├── index.html (enhanced map interface)
├── admin/
│   ├── index.html (admin dashboard)
│   ├── login.html (authentication)
│   ├── objects.html (CRUD interface)
│   ├── analytics.html (analytics dashboard)
│   ├── admin-styles.css
│   ├── admin-app.js
│   └── chart.min.js
├── js/
│   ├── map-controls.js (enhanced map interactions)
│   ├── model-viewer.js (3D model rendering)
│   ├── media-viewer.js (360° image/video)
│   └── filters.js (search and filter logic)
├── css/
│   ├── main-styles.css
│   └── admin-styles.css
└── lib/
    ├── three.min.js (3D rendering)
    ├── pannellum.js (360° images)
    └── chart.min.js (analytics charts)

Controllers/
├── AdminController.cs (new)
├── MapController.cs (enhanced with analytics)
└── LLMController.cs (existing)

Models/
├── Admin.cs (new)
├── MapObject.cs (existing, minor enhancements)
├── PlayerProgress.cs (existing)
└── InteractionLog.cs (existing)
```

### To-dos

- [ ] Create admin authentication system with JWT and Admin model
- [ ] Build admin dashboard UI with login, navigation, and main dashboard
- [ ] Implement full CRUD interface for map objects with forms and validation
- [ ] Add bulk import/export functionality for CSV and JSON
- [ ] Enhance map with zoom levels, pan boundaries, and minimap
- [ ] Add search bar and filter system for map objects
- [ ] Create drag-and-drop visual map editor for admin positioning
- [ ] Integrate Three.js and build 3D model viewer with rotation/zoom
- [ ] Add 360° static image viewer with interactive controls
- [ ] Enhance existing video player for 360° video support
- [ ] Create unified media manager that handles images, videos, and 3D models
- [ ] Build analytics API endpoints for popular objects, user stats, and LLM usage
- [ ] Create analytics dashboard with charts and key metrics visualization
- [ ] Implement progressive loading, lazy loading, and caching strategies
- [ ] Enhance responsive design and accessibility features