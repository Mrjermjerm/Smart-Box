


namespace SmartBox;
using System;
using System.Net.Http;
using System.Text.Json;



public partial class MainPage : ContentPage
{
	int count = 0;
	private readonly HttpClient _httpClient = new HttpClient();



	public MainPage()
	{
		InitializeComponent();
		
		// Connection to SmartBox
		CheckSmartFixStatus();
	}

	private async void CheckSmartFixStatus()
	{
		try
        {
            var response = await _httpClient.GetAsync("http://10.0.0.28:5000/api/devices");
            var json = await response.Content.ReadAsStringAsync();

            SmartBoxDisplay.Text = json; 
        }
        catch (Exception)
        {
            SmartBoxDisplay.Text = "SmartFix Offline";
        }
	}

}
