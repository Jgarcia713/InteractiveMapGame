using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InteractiveMapGame.Data;
using InteractiveMapGame.Models;
using System.Text;

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

        // GET: api/MapObjectAdmin/list
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<MapObject>>> GetMapObjects()
        {
            var mapObjects = await _context.MapObjects
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return Ok(mapObjects);
        }

        // POST: api/MapObjectAdmin/create
        [HttpPost("create")]
        public async Task<ActionResult<MapObject>> CreateMapObject([FromBody] MapObject mapObject)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(mapObject.Name))
            {
                return BadRequest(new { message = "Name is required" });
            }

            if (string.IsNullOrWhiteSpace(mapObject.Type))
            {
                return BadRequest(new { message = "Type is required" });
            }

            // Set timestamps
            mapObject.CreatedAt = DateTime.UtcNow;
            mapObject.UpdatedAt = DateTime.UtcNow;

            // Validate string lengths
            if (mapObject.Name.Length > 200)
            {
                return BadRequest(new { message = "Name must be 200 characters or less" });
            }

            if (mapObject.Type.Length > 100)
            {
                return BadRequest(new { message = "Type must be 100 characters or less" });
            }

            if (!string.IsNullOrEmpty(mapObject.Description) && mapObject.Description.Length > 1000)
            {
                return BadRequest(new { message = "Description must be 1000 characters or less" });
            }

            try
            {
                _context.MapObjects.Add(mapObject);
                await _context.SaveChangesAsync();

                return Ok(mapObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating map object", error = ex.Message });
            }
        }

        // POST: api/MapObjectAdmin/bulk-upload
        [HttpPost("bulk-upload")]
        public async Task<ActionResult> BulkUpload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "File must be a CSV file" });
            }

            var results = new BulkUploadResult
            {
                SuccessCount = 0,
                ErrorCount = 0,
                Errors = new List<string>()
            };

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream()))
                {
                    var content = await stream.ReadToEndAsync();
                    var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length < 2)
                    {
                        return BadRequest(new { message = "CSV file must contain at least a header row and one data row" });
                    }

                    // Parse header
                    var headerLine = lines[0].Trim();
                    var headers = ParseCsvLine(headerLine);
                    
                    // Expected headers (in order)
                    var expectedHeaders = new[]
                    {
                        "Name", "Type", "Description", "Category", "Era", "Manufacturer",
                        "FirstFlight", "Status", "X", "Y", "Z", "ImageUrl", "ModelUrl",
                        "Video360Url", "IsInteractive", "IsDiscoverable", "IsUnlocked",
                        "ExperienceValue", "DifficultyLevel"
                    };

                    // Validate headers (case-insensitive)
                    if (headers.Count != expectedHeaders.Length)
                    {
                        return BadRequest(new { message = $"CSV must have exactly {expectedHeaders.Length} columns. Found {headers.Count}." });
                    }

                    // Process data rows
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var line = lines[i].Trim();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        try
                        {
                            var values = ParseCsvLine(line);
                            
                            if (values.Count != expectedHeaders.Length)
                            {
                                results.ErrorCount++;
                                results.Errors.Add($"Row {i + 1}: Expected {expectedHeaders.Length} columns, found {values.Count}");
                                continue;
                            }

                            var mapObject = new MapObject
                            {
                                Name = values[0]?.Trim() ?? string.Empty,
                                Type = values[1]?.Trim() ?? string.Empty,
                                Description = string.IsNullOrWhiteSpace(values[2]) ? null : values[2].Trim(),
                                Category = string.IsNullOrWhiteSpace(values[3]) ? null : values[3].Trim(),
                                Era = string.IsNullOrWhiteSpace(values[4]) ? null : values[4].Trim(),
                                Manufacturer = string.IsNullOrWhiteSpace(values[5]) ? null : values[5].Trim(),
                                FirstFlight = ParseDateTime(values[6]),
                                Status = string.IsNullOrWhiteSpace(values[7]) ? null : values[7].Trim(),
                                X = ParseDouble(values[8], 0),
                                Y = ParseDouble(values[9], 0),
                                Z = ParseDouble(values[10], 0),
                                ImageUrl = string.IsNullOrWhiteSpace(values[11]) ? null : values[11].Trim(),
                                ModelUrl = string.IsNullOrWhiteSpace(values[12]) ? null : values[12].Trim(),
                                Video360Url = string.IsNullOrWhiteSpace(values[13]) ? null : values[13].Trim(),
                                IsInteractive = ParseBoolean(values[14], true),
                                IsDiscoverable = ParseBoolean(values[15], true),
                                IsUnlocked = ParseBoolean(values[16], false),
                                ExperienceValue = ParseInt(values[17], 0),
                                DifficultyLevel = ParseInt(values[18], 1),
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            // Validate required fields
                            if (string.IsNullOrWhiteSpace(mapObject.Name))
                            {
                                results.ErrorCount++;
                                results.Errors.Add($"Row {i + 1}: Name is required");
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(mapObject.Type))
                            {
                                results.ErrorCount++;
                                results.Errors.Add($"Row {i + 1}: Type is required");
                                continue;
                            }

                            // Validate string lengths
                            if (mapObject.Name.Length > 200)
                            {
                                results.ErrorCount++;
                                results.Errors.Add($"Row {i + 1}: Name exceeds 200 characters");
                                continue;
                            }

                            if (mapObject.Type.Length > 100)
                            {
                                results.ErrorCount++;
                                results.Errors.Add($"Row {i + 1}: Type exceeds 100 characters");
                                continue;
                            }

                            _context.MapObjects.Add(mapObject);
                            results.SuccessCount++;
                        }
                        catch (Exception ex)
                        {
                            results.ErrorCount++;
                            results.Errors.Add($"Row {i + 1}: {ex.Message}");
                        }
                    }
                }

                // Save all valid objects
                if (results.SuccessCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    count = results.SuccessCount,
                    errors = results.Errors,
                    message = $"Successfully uploaded {results.SuccessCount} exhibit(s). {(results.ErrorCount > 0 ? $"{results.ErrorCount} row(s) had errors." : "")}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing CSV file", error = ex.Message });
            }
        }

        // DELETE: api/MapObjectAdmin/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMapObject(int id)
        {
            var mapObject = await _context.MapObjects.FindAsync(id);
            if (mapObject == null)
            {
                return NotFound(new { message = "Map object not found" });
            }

            try
            {
                _context.MapObjects.Remove(mapObject);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Map object deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting map object", error = ex.Message });
            }
        }

        // Helper methods for CSV parsing
        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var currentValue = new StringBuilder();
            bool inQuotes = false;

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
                    values.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            // Add the last value
            values.Add(currentValue.ToString());

            return values;
        }

        private DateTime? ParseDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, out DateTime result))
                return result;

            return null;
        }

        private double ParseDouble(string value, double defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (double.TryParse(value, out double result))
                return result;

            return defaultValue;
        }

        private int ParseInt(string value, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (int.TryParse(value, out int result))
                return result;

            return defaultValue;
        }

        private bool ParseBoolean(string value, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (bool.TryParse(value, out bool result))
                return result;

            // Also handle "true"/"false" strings (case-insensitive)
            if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;

            return defaultValue;
        }

        private class BulkUploadResult
        {
            public int SuccessCount { get; set; }
            public int ErrorCount { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
        }
    }
}



