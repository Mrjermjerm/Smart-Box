

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace SmartBox
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private string[] _piUrls = new string[]
        {
            "http://10.42.0.1:5000"
        };

        public MainPage()
        {
            InitializeComponent();
            _ = CheckSmartFixStatusLoop();
        }

        private async Task CheckSmartFixStatusLoop()
        {
            while (true)
            {
                await CheckSmartFixStatus();
                await Task.Delay(10000); // check every 10 seconds
            }
        }

        private async Task CheckSmartFixStatus()
        {
            string json = "";
            foreach (var url in _piUrls)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{url}/api/devices");
                    if (response.IsSuccessStatusCode)
                    {
                        json = await response.Content.ReadAsStringAsync();
                        SmartBoxDisplay.Text = $"Connected Devices:\n{json}";
                        return;
                    }
                }
                catch { /* try next URL */ }
            }

            SmartBoxDisplay.Text = "SmartBox Offline";
        }

        private async void OnSendWifiClicked(object sender, EventArgs e)
        {
            string ssid = SSIDEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(ssid) || string.IsNullOrEmpty(password))
            {
                WifiStatusLabel.Text = "Please enter SSID and password";
                return;
            }

            var payload = new { ssid = ssid, password = password };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            foreach (var url in _piUrls)
            {
                try
                {
                    var response = await _httpClient.PostAsync($"{url}/setup_wifi", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseText = await response.Content.ReadAsStringAsync();
                        WifiStatusLabel.Text = $"Sent! Pi response: {responseText}";
                        return;
                    }
                }
                catch { /* try next URL */ }
            }

            WifiStatusLabel.Text = "Could not connect to SmartBox";
        }
    }
}