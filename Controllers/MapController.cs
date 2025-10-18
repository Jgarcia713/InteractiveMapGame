using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InteractiveMapGame.Data;
using InteractiveMapGame.Models;

namespace InteractiveMapGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapController : ControllerBase
    {
        private readonly MapGameDbContext _context;

        public MapController(MapGameDbContext context)
        {
            _context = context;
        }

        // GET: api/Map/objects
        [HttpGet("objects")]
        public async Task<ActionResult<IEnumerable<MapObject>>> GetMapObjects()
        {
            return await _context.MapObjects
                .Where(o => o.IsDiscoverable)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        // GET: api/Map/objects/{id}
        [HttpGet("objects/{id}")]
        public async Task<ActionResult<MapObject>> GetMapObject(int id)
        {
            var mapObject = await _context.MapObjects.FindAsync(id);

            if (mapObject == null)
            {
                return NotFound();
            }

            return mapObject;
        }

        // GET: api/Map/objects/nearby
        [HttpGet("objects/nearby")]
        public async Task<ActionResult<IEnumerable<MapObject>>> GetNearbyObjects(
            double x, double y, double z = 0, double radius = 100)
        {
            var nearbyObjects = await _context.MapObjects
                .Where(o => o.IsDiscoverable && 
                           Math.Sqrt(Math.Pow(o.X - x, 2) + Math.Pow(o.Y - y, 2) + Math.Pow(o.Z - z, 2)) <= radius)
                .OrderBy(o => Math.Sqrt(Math.Pow(o.X - x, 2) + Math.Pow(o.Y - y, 2) + Math.Pow(o.Z - z, 2)))
                .ToListAsync();

            return nearbyObjects;
        }

        // GET: api/Map/objects/type/{type}
        [HttpGet("objects/type/{type}")]
        public async Task<ActionResult<IEnumerable<MapObject>>> GetObjectsByType(string type)
        {
            var objects = await _context.MapObjects
                .Where(o => o.Type.ToLower() == type.ToLower() && o.IsDiscoverable)
                .ToListAsync();

            return objects;
        }

        // GET: api/Map/objects/unlocked
        [HttpGet("objects/unlocked")]
        public async Task<ActionResult<IEnumerable<MapObject>>> GetUnlockedObjects()
        {
            var unlockedObjects = await _context.MapObjects
                .Where(o => o.IsUnlocked && o.IsDiscoverable)
                .ToListAsync();

            return unlockedObjects;
        }

        // POST: api/Map/objects
        [HttpPost("objects")]
        public async Task<ActionResult<MapObject>> CreateMapObject(MapObject mapObject)
        {
            mapObject.CreatedAt = DateTime.UtcNow;
            mapObject.UpdatedAt = DateTime.UtcNow;
            
            _context.MapObjects.Add(mapObject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMapObject", new { id = mapObject.Id }, mapObject);
        }

        // PUT: api/Map/objects/{id}
        [HttpPut("objects/{id}")]
        public async Task<IActionResult> UpdateMapObject(int id, MapObject mapObject)
        {
            if (id != mapObject.Id)
            {
                return BadRequest();
            }

            mapObject.UpdatedAt = DateTime.UtcNow;
            _context.Entry(mapObject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MapObjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Map/objects/{id}
        [HttpDelete("objects/{id}")]
        public async Task<IActionResult> DeleteMapObject(int id)
        {
            var mapObject = await _context.MapObjects.FindAsync(id);
            if (mapObject == null)
            {
                return NotFound();
            }

            _context.MapObjects.Remove(mapObject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Map/objects/{id}/interact
        [HttpPost("objects/{id}/interact")]
        public async Task<ActionResult> InteractWithObject(int id, [FromBody] InteractionRequest request)
        {
            var mapObject = await _context.MapObjects.FindAsync(id);
            if (mapObject == null)
            {
                return NotFound();
            }

            // Log the interaction
            var interaction = new InteractionLog
            {
                PlayerId = request.PlayerId,
                MapObjectId = id,
                InteractionType = request.InteractionType,
                InteractionData = request.InteractionData,
                Duration = request.Duration,
                WasSuccessful = true,
                Timestamp = DateTime.UtcNow
            };

            _context.InteractionLogs.Add(interaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interaction logged successfully" });
        }

        private bool MapObjectExists(int id)
        {
            return _context.MapObjects.Any(e => e.Id == id);
        }
    }

    public record InteractionRequest(
        string PlayerId,
        string InteractionType,
        string? InteractionData = null,
        int Duration = 0
    );
}
