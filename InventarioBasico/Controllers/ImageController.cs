using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace InventarioBasico.Controllers
{
    [ApiController]
    [Route("upload-image")]
    public class ImageController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ImageController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No se envió imagen");

            var imgbbApiKey = "6e5bb83f4ba0780d984e1464d4c3bcf8";

            using var content = new MultipartFormDataContent();
            using var imageContent = new StreamContent(image.OpenReadStream());
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(image.ContentType);
            content.Add(imageContent, "image", image.FileName);

            var url = $"https://api.imgbb.com/1/upload?key={imgbbApiKey}";

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error subiendo imagen");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            // Parseamos el JSON y extraemos SOLO la URL
            var json = System.Text.Json.JsonDocument.Parse(responseBody);
            var imageUrl = json.RootElement.GetProperty("data").GetProperty("url").GetString();

            // Le devolvemos a React SOLO el url (así es más limpio)
            return Ok(new { url = imageUrl });
        }

    }
}
