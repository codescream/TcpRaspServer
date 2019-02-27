using CamTheGeek.GpioDotNet;
using FredQnA;
using ImageAnalyze;
using NetCoreAudio;
using RestSTT;
using SpeakerRecognition;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TcpRaspServer.Utility;
using TextToSPeechApp;
using UpdatedKB;

namespace DemoHarnessUpd
{
    class Program
    {
       // public static GpioPin led = new GpioPin(17, Direction.Out);
        public static String[] ctrl_cmd = {"forward", "backward", "left", "right", "stop", "read cpu_temp", "home", "distance", "x+",
            "x-", "y+", "y-", "xy_home", "Speak", "Image", "Voice", "PlayB", "StopR", "AFred", "FredS", "Reco1", "Reco2", "ConfE", "CancE", "UpdKB", "GetEn", "DelPr"};

        public static string text = "";
        static TextToSpeech tts = new TextToSpeech();
        static Player player = new Player();


        public static void Main()
        {
            TcpListener server = null;

            /*ProgramCV prgcv = new ProgramCV();
            prgcv.AnalyzeImage();*/

            //automatically retrieve local ip address
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            //Boolean turnOn;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse(localIP);

                Console.WriteLine(localAddr);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[1024];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i = stream.Read(bytes, 0, bytes.Length);

                    // Loop to receive all the data sent by the client.
                    //while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    //{
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();
                        string test = "";

                        int dataLen = data.Length - 5;
                        if(dataLen > 0)
                        {
                            test = data.Substring(dataLen);
                        }
                        
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        //stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    //}

                    //Console.WriteLine("Done!");

                    // Shutdown and end connection
                        //stream.Close();

                    Console.WriteLine(data);

                   if(data == ctrl_cmd[0])
                    {
                        //move forward
                        GpioPin fwd = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        fwd.Value = PinValue.High;
                    }
                   else if(data == ctrl_cmd[1])
                    {
                        //move backward
                        GpioPin bkwd = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        bkwd.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[2])
                    {
                        //move left
                        GpioPin lft = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        lft.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[3])
                    {
                        //move right
                        GpioPin rgt = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        rgt.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[4])
                    {
                        //stop
                        GpioPin stp = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        stp.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[5])
                    {
                        //read cpu_temp
                    }
                    else if (data == ctrl_cmd[6])
                    {
                        //home
                        GpioPin hme = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        hme.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[7])
                    {
                        //distance
                    }
                    else if (data == ctrl_cmd[8])
                    {
                        //x+
                        GpioPin xup = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        xup.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[9])
                    {
                        //x-
                        GpioPin xdn = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        xdn.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[10])
                    {
                        //y+
                        GpioPin yup = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        yup.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[11])
                    {
                        //y-
                        GpioPin ydn = new GpioPin(17, Direction.Out);//change 17 to the actual pin #
                        ydn.Value = PinValue.High;
                    }
                    else if (data == ctrl_cmd[12])
                    {
                        //xy_home
                    }
                    else if (test == ctrl_cmd[13].ToUpper()) // Speak
                    {
                        Console.WriteLine(data.Remove(dataLen));
                        tts.TextToWords(data.Remove(dataLen)).Wait();
                    }
                    else if (data == ctrl_cmd[14].ToUpper()) // Image and Read-CV
                    {
                        Console.WriteLine(data.Remove(dataLen));
                        CompVision imageCV = new CompVision();

                        string text = imageCV.AnalyzeImage();
                        tts.TextToWords(text).Wait();
                    }
                    else if (data == ctrl_cmd[15].ToUpper()) // Voice - Record
                    {
                        VarHolder.LinuxRecTime = "";
                        Console.WriteLine(data.Remove(dataLen));
                        player.Record().Wait();
                    }
                    else if (data == ctrl_cmd[16].ToUpper()) // PlayBack recording
                    {
                        string path = Directory.GetCurrentDirectory();
                        if(path.Contains("\\"))
                        {
                            path += "\\record.wav";
                        }
                        else
                        {
                            path += "/record.wav";
                        }
                        Console.WriteLine(data.Remove(dataLen));
                        player.Play(path).Wait();
                    }
                    else if (data == ctrl_cmd[17].ToUpper()) // Stop recording
                    {
                        VarHolder.LinuxRecTime = "";
                        player.StopRecording().Wait();
                    }
                    else if (data == ctrl_cmd[18].ToUpper()) // Ask Fred - QnA
                    {
                        VarHolder.LinuxRecTime = "-d 5";
                        FredQnA.FredQnA qna = new FredQnA.FredQnA();
                        qna.FredQ().Wait();
                    }
                    else if (data == ctrl_cmd[19].ToUpper()) // Fred Spy
                    {
                        string path = Directory.GetCurrentDirectory();
                        if (path.Contains("\\"))
                        {
                            path += "\\record.wav";
                        }
                        else
                        {
                            path += "/record.wav";
                        }
                        Byte[] audio = File.ReadAllBytes(path);
                        stream.WriteAsync(audio, 0, audio.Length);
                    }
                    else if (test == ctrl_cmd[20].ToUpper()) // Record 1st voice profile
                    {
                        VarHolder.LinuxRecTime = "-d 10";
                        VoiceSignature.RecVoiceOne(data.Remove(dataLen)).Wait();
                    }
                    else if (data == ctrl_cmd[21].ToUpper()) // Record 2nd voice profile
                    {
                        VarHolder.LinuxRecTime = "-d 10";
                        VoiceSignature.RecVoiceTwo().Wait();
                    }
                    else if (data == ctrl_cmd[22].ToUpper()) // Confirm enrollment
                    {
                        VoiceSignature.ConfirmEnroll();
                    }
                    else if (data == ctrl_cmd[23].ToUpper()) // Cancel enrollment
                    {
                        VoiceSignature.CancelEnroll();
                    }
                    else if (test == ctrl_cmd[24].ToUpper()) // Update Fred KB
                    {                        
                        string trial = data.Remove(dataLen);
                        string[] splitTrial = trial.Split(";");
                        string[] question = new string[splitTrial.Length];
                        string[] answer = new string[splitTrial.Length];
                        for (int j = 0; j < splitTrial.Length; j++)
                        {
                            question[j] = splitTrial[j].Split(":")[0].Replace("'", "");
                            answer[j] = splitTrial[j].Split(":")[1].Replace("'", "");
                        }
                        UpdateFredKB.UpdateKB(question, answer);
                    }
                    else if (data == ctrl_cmd[25].ToUpper()) // Retrieve enrollment
                    {
                        string path = Directory.GetCurrentDirectory();
                        if (path.Contains("\\"))
                        {
                            path += "\\speaker_recog.txt";
                        }
                        else
                        {
                            path += "/speaker_recog.txt";
                        }
                        Byte[] text = File.ReadAllBytes(path);
                        stream.WriteAsync(text, 0, text.Length);
                    }
                    else if (test == ctrl_cmd[26].ToUpper()) // Delete voice profile
                    {
                        string profileId = data.Remove(dataLen).ToLower();
                        VoiceSignature.DeleteProfile(profileId);
                    }
                    //while (turnOn)
                    //{
                    //    led.Value = PinValue.High;
                    //    Thread.Sleep(TimeSpan.FromSeconds(1));

                    //    led.Value = PinValue.Low;
                    //    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            //Console.WriteLine("\nHit enter to continue...");
            //Console.Read();
        }
    }
}