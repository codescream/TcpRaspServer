﻿using DemoHarnessUpd;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextToSPeechApp;

namespace TextToSPeechApp
{
    class TextToSpeech
    {
        public static WaveFileReader wave = new WaveFileReader(@"sample.wav");
        public int count = 0;
        public static DirectSoundOut output = null;
        public async Task TextToWords(string textInput)
        {
            while (true)
            {
                // Prompts the user to input text for TTS conversion
                //Console.Write("What would you like to convert to Speech? ");
                string text = textInput;
                string path = "";
                // Gets an access token
                string accessToken;
                Console.WriteLine("Attempting token exchange. Please wait...\n");

                // Add your subscription key here
                // If your resource isn't in WEST US, change the endpoint
                Authentication auth = new Authentication("https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken", Environment.GetEnvironmentVariable("azure_TTS_Key", EnvironmentVariableTarget.User));
                try
                {
                    accessToken = await auth.FetchTokenAsync().ConfigureAwait(false);
                    Console.WriteLine("Successfully obtained an access token. \n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to obtain an access token.");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.Message);
                    return;
                }

                string host = "https://westus.tts.speech.microsoft.com/cognitiveservices/v1";

                string body = @"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'><voice  name='Microsoft Server Speech Text to Speech Voice (en-US, BenjaminRUS)'> <break time='90ms' />" +
                                text + "</voice></speak>";

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage())
                    {
                        // Set the HTTP method
                        request.Method = HttpMethod.Post;
                        // Construct the URI
                        request.RequestUri = new Uri(host);
                        // Set the content type header
                        request.Content = new StringContent(body, Encoding.UTF8, "application/ssml+xml");
                        // Set additional header, such as Authorization and User-Agent
                        request.Headers.Add("Authorization", "Bearer " + accessToken);
                        request.Headers.Add("Connection", "Keep-Alive");
                        // Update your resource name
                        request.Headers.Add("User-Agent", "TextToSpeechApp");
                        request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
                        // Create a request
                        Console.WriteLine("Calling the TTS service. Please wait... \n");
                        using (var response = await client.SendAsync(request).ConfigureAwait(false))
                        {
                            response.EnsureSuccessStatusCode();
                            // Asynchronously read the response
                            using (var dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                Console.WriteLine("Your speech file is being written to file...");
                                wave.Close();
                                using (var fileStream = new FileStream(@"sample" + count + ".wav", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                                {
                                    await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                    path = fileStream.Name;
                                    count++;
                                }
                                //Console.WriteLine("\nYour file is ready. Press any key to exit.");
                                NetCoreSample.Audio(path);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}