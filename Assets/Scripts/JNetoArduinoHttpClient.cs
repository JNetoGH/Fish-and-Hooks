using System.Collections;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using Sirenix.OdinInspector;


public class JNetoArduinoHttpClient : MonoBehaviour
{
	private static readonly HttpClient Client = new HttpClient();
	[SerializeField] private string _httpUrl = "http://192.168.4.1"; 
	[ReadOnly, SerializeField] private int _encoderValue = 0;

	void Start()
	{
		// Inicia a leitura contínua do counter
		StartCoroutine(ReadCounterContinuously());
	}

	IEnumerator ReadCounterContinuously()
	{
		while (true)
		{
			yield return ReadEncoder();
			yield return new WaitForSeconds(0.016f); // Espera 16 ms entre as leituras
		}
	}

	async Task ReadEncoder()
	{
		try
		{
			// Faz a requisição HTTP GET para obter o valor do counter
			HttpResponseMessage response = await Client.GetAsync(_httpUrl);
			response.EnsureSuccessStatusCode();
			string responseBody = await response.Content.ReadAsStringAsync();

			// Tenta converter o resultado para um inteiro
			if (int.TryParse(responseBody, out int result))
			{
				_encoderValue = result;
				Debug.Log($"Counter Value: {_encoderValue}");
			}
			else
			{
				Debug.LogWarning("Failed to parse counter value.");
			}
		}
		catch (HttpRequestException e)
		{
			Debug.LogError($"Request error: {e.Message}");
		}
	}
	
	private void OnDisable()
	{
		Client.Dispose();
	}
}