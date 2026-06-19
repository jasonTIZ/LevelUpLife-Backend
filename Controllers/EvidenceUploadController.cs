using Microsoft.AspNetCore.Mvc;

namespace LevelUpLifeBackend.Controllers;

[ApiController]
[Route("api/evidences")]
public class EvidenceUploadController : ControllerBase
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    private readonly IWebHostEnvironment _environment;

    public EvidenceUploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                code = "NO_FILE",
                message = "No file received.",
                details = "Include a file in the 'file' field of the multipart/form-data body.",
            });
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return BadRequest(new
            {
                code = "FILE_TOO_LARGE",
                message = "File exceeds the 10 MB limit.",
                details = $"Received {file.Length / 1024 / 1024} MB.",
            });
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            return BadRequest(new
            {
                code = "UNSUPPORTED_FILE_TYPE",
                message = "Only jpg, jpeg, png, and webp images are accepted.",
                details = $"Received extension: '{extension}'.",
            });
        }

        var webRoot = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var evidencesDir = Path.Combine(webRoot, "evidences");
        Directory.CreateDirectory(evidencesDir);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var absolutePath = Path.Combine(evidencesDir, fileName);

        await using (var stream = System.IO.File.Create(absolutePath))
        {
            await file.CopyToAsync(stream);
        }

        var publicUrl = $"{Request.Scheme}://{Request.Host}/evidences/{fileName}";

        return Ok(new { url = publicUrl });
    }
}
