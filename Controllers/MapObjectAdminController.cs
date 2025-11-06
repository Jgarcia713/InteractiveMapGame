using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using InteractiveMapGame.Data;
using InteractiveMapGame.Models;

namespace InteractiveMapGame.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MapObjectAdminController : ControllerBase
    {
        private readonly MapGameDbContext _context;

        public MapObjectAdminController(MapGameDbContext context)
        {
            _context = context;
        }

        // POST: api/MapObjectAdmin/create
        [HttpPost("create")]
        public async Task<ActionResult<MapObject>> CreateMapObject([FromBody] MapObject mapObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(mapObject.Name))
            {
                return BadRequest(new { message = "Name is required" });
            }

            if (string.IsNullOrWhiteSpace(mapObject.Type))
            {
                return BadRequest(new { message = "Type is required" });
            }

            mapObject.CreatedAt = DateTime.UtcNow;
            mapObject.UpdatedAt = DateTime.UtcNow;

            _context.MapObjects.Add(mapObject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMapObject), new { id = mapObject.Id }, mapObject);
        }

        // GET: api/MapObjectAdmin/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MapObject>> GetMapObject(int id)
        {
            var mapObject = await _context.MapObjects.FindAsync(id);

            if (mapObject == null)
            {
                return NotFound();
            }

            return mapObject;
        }

        // PUT: api/MapObjectAdmin/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMapObject(int id, [FromBody] MapObject mapObject)
        {
            if (id != mapObject.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingObject = await _context.MapObjects.FindAsync(id);
            if (existingObject == null)
            {
                return NotFound();
            }

            // Update properties
            existingObject.Name = mapObject.Name;
            existingObject.Description = mapObject.Description;
            existingObject.Type = mapObject.Type;
            existingObject.Category = mapObject.Category;
            existingObject.Era = mapObject.Era;
            existingObject.Manufacturer = mapObject.Manufacturer;
            existingObject.FirstFlight = mapObject.FirstFlight;
            existingObject.Status = mapObject.Status;
            existingObject.X = mapObject.X;
            existingObject.Y = mapObject.Y;
            existingObject.Z = mapObject.Z;
            existingObject.ImageUrl = mapObject.ImageUrl;
            existingObject.ModelUrl = mapObject.ModelUrl;
            existingObject.Video360Url = mapObject.Video360Url;
            existingObject.IsInteractive = mapObject.IsInteractive;
            existingObject.IsDiscoverable = mapObject.IsDiscoverable;
            existingObject.IsUnlocked = mapObject.IsUnlocked;
            existingObject.ExperienceValue = mapObject.ExperienceValue;
            existingObject.DifficultyLevel = mapObject.DifficultyLevel;
            existingObject.GeneratedDescription = mapObject.GeneratedDescription;
            existingObject.GeneratedStory = mapObject.GeneratedStory;
            existingObject.GeneratedFacts = mapObject.GeneratedFacts;
            existingObject.UpdatedAt = DateTime.UtcNow;

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

            return Ok(new { message = "MapObject updated successfully", mapObject = existingObject });
        }

        // DELETE: api/MapObjectAdmin/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMapObject(int id)
        {
            var mapObject = await _context.MapObjects.FindAsync(id);
            if (mapObject == null)
            {
                return NotFound();
            }

            _context.MapObjects.Remove(mapObject);
            await _context.SaveChangesAsync();

            return Ok(new { message = "MapObject deleted successfully" });
        }

        // GET: api/MapObjectAdmin/list
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<MapObject>>> GetMapObjects()
        {
            var mapObjects = await _context.MapObjects
                .OrderBy(o => o.Name)
                .ToListAsync();

            return Ok(mapObjects);
        }

        // POST: api/MapObjectAdmin/bulk-upload
        [HttpPost("bulk-upload")]
        public async Task<ActionResult> BulkUploadMapObjects([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Only CSV files are allowed" });
            }

            var mapObjects = new List<MapObject>();
            var errors = new List<string>();
            int lineNumber = 0;

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                {
                    string? headerLine = await reader.ReadLineAsync();
                    if (headerLine == null)
                    {
                        return BadRequest(new { message = "CSV file is empty" });
                    }

                    // Expected CSV format (header):
                    // Name,Type,Description,Category,Era,Manufacturer,FirstFlight,Status,X,Y,Z,ImageUrl,ModelUrl,Video360Url,IsInteractive,IsDiscoverable,IsUnlocked,ExperienceValue,DifficultyLevel
                    var headers = headerLine.Split(',').Select(h => h.Trim()).ToArray();

                    while (!reader.EndOfStream)
                    {
                        lineNumber++;
                        string? line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var values = ParseCsvLine(line);

                        if (values.Length < headers.Length)
                        {
                            errors.Add($"Line {lineNumber}: Not enough columns");
                            continue;
                        }

                        try
                        {
                            var mapObject = new MapObject
                            {
                                Name = GetValue(values, headers, "Name") ?? "",
                                Type = GetValue(values, headers, "Type") ?? "",
                                Description = GetValue(values, headers, "Description"),
                                Category = GetValue(values, headers, "Category"),
                                Era = GetValue(values, headers, "Era"),
                                Manufacturer = GetValue(values, headers, "Manufacturer"),
                                FirstFlight = ParseDateTime(GetValue(values, headers, "FirstFlight")),
                                Status = GetValue(values, headers, "Status"),
                                X = ParseDouble(GetValue(values, headers, "X"), 0),
                                Y = ParseDouble(GetValue(values, headers, "Y"), 0),
                                Z = ParseDouble(GetValue(values, headers, "Z"), 0),
                                ImageUrl = GetValue(values, headers, "ImageUrl"),
                                ModelUrl = GetValue(values, headers, "ModelUrl"),
                                Video360Url = GetValue(values, headers, "Video360Url"),
                                IsInteractive = ParseBool(GetValue(values, headers, "IsInteractive"), true),
                                IsDiscoverable = ParseBool(GetValue(values, headers, "IsDiscoverable"), true),
                                IsUnlocked = ParseBool(GetValue(values, headers, "IsUnlocked"), false),
                                ExperienceValue = ParseInt(GetValue(values, headers, "ExperienceValue"), 0),
                                DifficultyLevel = ParseInt(GetValue(values, headers, "DifficultyLevel"), 1),
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            // Validate required fields
                            if (string.IsNullOrWhiteSpace(mapObject.Name))
                            {
                                errors.Add($"Line {lineNumber}: Name is required");
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(mapObject.Type))
                            {
                                errors.Add($"Line {lineNumber}: Type is required");
                                continue;
                            }

                            mapObjects.Add(mapObject);
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Line {lineNumber}: {ex.Message}");
                        }
                    }
                }

                if (mapObjects.Count == 0)
                {
                    return BadRequest(new { 
                        message = "No valid MapObjects found in CSV", 
                        errors = errors 
                    });
                }

                _context.MapObjects.AddRange(mapObjects);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Successfully uploaded {mapObjects.Count} MapObjects",
                    count = mapObjects.Count,
                    errors = errors.Count > 0 ? errors : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing CSV file", error = ex.Message });
            }
        }

        // Helper methods for CSV parsing
        private string[] ParseCsvLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            var currentValue = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Escaped quote
                        currentValue.Append('"');
                        i++;
                    }
                    else
                    {
                        // Toggle quote state
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            values.Add(currentValue.ToString().Trim());
            return values.ToArray();
        }

        private string? GetValue(string[] values, string[] headers, string headerName)
        {
            int index = Array.IndexOf(headers, headerName);
            if (index >= 0 && index < values.Length)
            {
                var value = values[index].Trim();
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
            return null;
        }

        private DateTime? ParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, out DateTime result))
                return result;

            return null;
        }

        private double ParseDouble(string? value, double defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (double.TryParse(value, out double result))
                return result;

            return defaultValue;
        }

        private int ParseInt(string? value, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (int.TryParse(value, out int result))
                return result;

            return defaultValue;
        }

        private bool ParseBool(string? value, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (bool.TryParse(value, out bool result))
                return result;

            // Also handle "true"/"false" strings
            if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;

            return defaultValue;
        }

        private bool MapObjectExists(int id)
        {
            return _context.MapObjects.Any(e => e.Id == id);
        }
    }
}

