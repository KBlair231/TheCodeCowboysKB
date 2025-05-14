using System.Text;
using Newtonsoft.Json;
using PromptQuest.Models;

namespace PromptQuest.Services {
	public interface IDallEApiService {
		public Task<string> GenerateImageAsync(string prompt);
		void StoreImageDataInSession(string imageData);
		string GetImageDataFromSession();
	}

	public class DallEApiService:IDallEApiService {
		private readonly HttpClient _httpClient;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private const string ImageDataSessionKey = "ImageData";

		public DallEApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) {
			_httpClient = httpClient;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<string> GenerateImageAsync(string description) {
			//Clean up the prompt for optimal outputs. This is just the start of this, it will certainly need adjustments over time.
			description = "Give me a gloomy realistic image of a character with only their upper body in view" 
				+ "and a dark forest in the background. The character is described as follows: " + description 
				+ "The iamge must have the character alone, no one else in it. Also, do not include diagrams or text.";
			//Build the HTTPS request.
			var requestBody = new {
				model = "dall-e-3",
				prompt = description,   // The prompt for image generation
				n = 1,                  // Number of images to generate
				size = "1024x1024",     // The size of the generated image (e.g., "256x256", "512x512", or "1024x1024")
				response_format = "b64_json" // The response format (use "url" if you want image links instead of Base64)
			};
			var json = JsonConvert.SerializeObject(requestBody);
			StringContent content = new StringContent(json,Encoding.UTF8,"application/json");
			//Send post Request.
			var response = await _httpClient.PostAsync("images/generations",content);
			//response.EnsureSuccessStatusCode();//Throw if response is bad.
			if(!response.IsSuccessStatusCode) {
				var errorContent = await response.Content.ReadAsStringAsync();
				throw new Exception($"API call failed with status {response.StatusCode}: {errorContent}");
			}
			//Deserialize result.
			var responseContent = await response.Content.ReadAsStringAsync();
			var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
			//Return image as base 64 string.
			return responseObject.data[0].b64_json;
		}

		public void StoreImageDataInSession(string imageData) {
			if(_httpContextAccessor.HttpContext == null) {
				throw new Exception("Error writing image data to session: _httpContextAccessor.HttpContext was null.");
			}
			var session = _httpContextAccessor.HttpContext.Session;
			session.SetString(ImageDataSessionKey,imageData);
		}

		public string GetImageDataFromSession() {
			if(_httpContextAccessor.HttpContext == null) {
				throw new Exception("Error reading Image data from session: _httpContextAccessor.HttpContext was null.");
			}
			var session = _httpContextAccessor.HttpContext.Session;
			var ImageData = session.GetString(ImageDataSessionKey);
			return ImageData ?? "";
		}
	}
}
