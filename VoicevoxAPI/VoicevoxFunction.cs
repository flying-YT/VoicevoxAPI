using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VoicevoxAPI
{
    public class VoicevoxFunction
    {
        private readonly string ipPort;

        public VoicevoxFunction(string _ipAdress, string _port)
        {
            ipPort = _ipAdress + ":" + _port;
        }

        private async Task<string> MakeQuery(string _text, int _speakerId)
        {
            string jsonQuery;
            using (var httpClient = new HttpClient())
            {
                string url = ipPort + "/audio_query?text=" + _text + "&speaker=" + _speakerId;
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                {
                    request.Headers.TryAddWithoutValidation("accept", "application/json");

                    request.Content = new StringContent("");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                    jsonQuery = await response.Content.ReadAsStringAsync();
                }
            }
            return jsonQuery;
        }

        public async Task MakeSound(string _path, string _title, string _text, bool _upspeak, int _speakerId)
        {
            using (var httpClient = new HttpClient())
            {
                string url = ipPort + "/synthesis?speaker=" + _speakerId + "&enable_interrogative_upspeak=" + _upspeak;
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                {
                    request.Headers.TryAddWithoutValidation("accept", "audio/wav");

                    request.Content = new StringContent(await MakeQuery(_text, _speakerId));

                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string fileName = _title + ".wav";
                        using (var fileStream = File.Create(_path + @"\" + fileName))
                        {
                            using (var httpStream = await response.Content.ReadAsStreamAsync())
                            {
                                httpStream.CopyTo(fileStream);
                                fileStream.Flush();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
            }
        }
    }
}

