﻿using FredKB;
using IdentVoice;
using MovieMarvel;
using RestSTT;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TextToSPeechApp;

namespace FredQnA
{
    class FredQnA
    {
        public static HttpClient client = new HttpClient();
        static string appKey = "YU8PVU-HW9YK7TA7E";// Environment.GetEnvironmentVariable("Wolfram_App_Key", EnvironmentVariableTarget.User);
        static string wolframText = "";
        static string welcome = "";
        public static SpeechToText speech = new SpeechToText();
        static TextToSpeech tts = new TextToSpeech();
        public static string word = "";
        public static string cmd = "";

        public async Task FredQ()
        {
            wolframText = "";
            await tts.TextToWords("I am listening...");

            speech.WordsToText().Wait();

            string voice = SpeechToText.text.ToLower();

            if(voice == "nothing recorded")
            {
                cmd = "";
            }
            else
            {
                cmd = KnowledgeBase.FredKB(voice);
                welcome = await VoiceIdentification.IdentVoice();
            }
                
            cmd = cmd.Replace("\"", "");
            switch (cmd)
            {
                case "Fred Sees":
                {
                    tts.TextToWords("I see you Mark!").Wait();
                    break;
                }
                // run Fred Sees code
                case "Fred Reads":
                    // run Fred Reads code
                    break;
                case "Light On":
                    // run Light On code
                    break;
                case "Light Off":
                    // run Light Off code
                    break;
                default:
                    AskFred().Wait();
                    break;
            }
        }

        public static async Task AskFred()
        {
            //Console.WriteLine("Please ask your question");
            //await speech.SpeechToText();
            string question = SpeechToText.text;
            if (question.Equals("Nothing recorded"))
            {
                tts.TextToWords(wolframText + "I didn't hear a question...").Wait();
                // do  nothing!
            }
            else
            {
                wolframText = cmd; // ProgramKB.FredKB(question); // apply 'cmd' variable here instead

                if (wolframText == "")
                {
                    await GetAnswer(question);
                    if(wolframText == "")
                    {
                        tts.TextToWords(welcome + "please rephrase your question").Wait();
                    }
                    else
                    {
                        tts.TextToWords(welcome + " The answer is \n " + wolframText).Wait();
                    }
                }
                else
                {
                    tts.TextToWords(welcome + " The answer is \n " + wolframText).Wait();
                }
            }
        }

        public static async Task GetAnswer(string search)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue
                ("applicationException/json"));

            // grab 20 vids
            HttpResponseMessage response = await client.GetAsync($"https://api.wolframalpha.com/v1/result?i= {search}&appid={appKey}");

            if (response.IsSuccessStatusCode)
            {
                string Data = await response.Content.ReadAsStringAsync();
                wolframText = Data.Split(". ")[0];
                //Console.WriteLine(wolframText);
                if (wolframText.Length < 10)
                {
                    AskQuestion(search).Wait();
                }
                else
                {
                    Console.WriteLine(wolframText);
                }
                //Console.ReadLine();
            }
            else
            {
                wolframText = "";
              //  tts.TextSpeech("please rephrase your question").Wait();
            }
        }

        private static async Task AskQuestion(string question)
        {
            string newText = "";

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue
                ("applicationException/json"));

            HttpResponseMessage response = await client.GetAsync($"http://api.duckduckgo.com/?q= {question} &format=json");

            if (response.IsSuccessStatusCode)
            {
                string Data = await response.Content.ReadAsStringAsync();
                JsonNinja ninja = new JsonNinja(Data);
                List<string> answer = ninja.GetDetails("\"Abstract\"");
                List<string> rTopics = ninja.GetDetails("\"RelatedTopics\"");
                JsonNinja ninji = new JsonNinja(rTopics[0]);
                List<string> texts = ninji.GetDetails("\"Text\"");
                //Console.WriteLine("Answer: \n");
                if (answer[0] != "")
                {
                    string addStr = answer[0].Split('.')[0];
                    wolframText += "\n" + addStr;
                    Console.WriteLine(wolframText);
                }
                else
                {
                    if (texts.Count == 0)
                    {
                        //string data = ""; //this is so that if the search field is empty it does not show what was last searched
                        Console.WriteLine(wolframText);
                    }
                    else
                    {
                        int count = 1;
                        wolframText += "\nFound " + texts.Count + " other result(s)";
                        //Console.WriteLine("Found " + texts.Count + " results");
                        foreach (string text in texts)
                        {
                            newText += count + ": " + text.Split('.')[0].Replace("\\", "") + "\n";
                            //Console.WriteLine(count + ": " + newText + "\n");
                            count++;
                        }
                        wolframText += "\n" + newText;
                        Console.WriteLine(wolframText);
                    }
                }
            }
            else
            {
                string Data = ""; //this is so that if the search field is empty it does not show what was last searched
                Console.WriteLine(Data);
            }
        }
    }
}