using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[Route("api/recommendation")]
[ApiController]
public class RecommendationController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public RecommendationController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("get")]
    public async Task<IActionResult> GetRecommendations([FromBody] RecommendationRequest request)
    {
        var flaskUrl = "http://127.0.0.1:5000/process";  // Ensure Flask is running on this port

        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(flaskUrl, content);
        var result = await response.Content.ReadAsStringAsync();

        return Ok(result);
    }

    [HttpPost("submit-rating")]
    public async Task<IActionResult> SubmitRating([FromBody] RatingRequest request)
    {
        var flaskUrl = "http://127.0.0.1:5000/update-rating";  // Ensure Flask has this route

        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(flaskUrl, content);
        var result = await response.Content.ReadAsStringAsync();

        return Ok(result);
    }
}

public class RecommendationRequest
{
    public string[] course_names { get; set; }  // Change based on your frontend input
    public string project_focus { get; set; }
}

public class RatingRequest
{
    public int studentId { get; set; }
    public int svId { get; set; }
    public int rating { get; set; }
}
