using CamTheGeek.GpioDotNet;
using ImageAnalyze;
using NetCoreAudio;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace DemoHarnessUpd
{
    class Program
    {
       // public static GpioPin led = new GpioPin(17, Direction.Out);
        public static String[] ctrl_cmd = {"forward", "backward", "left", "right", "stop", "read cpu_temp", "home", "distance", "x+",
            "x-", "y+", "y-", "xy_home", "Speak", "Image", "Voice", "PlayB", "StopR"};

        public static string text = "";

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
                Byte[] bytes = new Byte[256];
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
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    //}

                    //Console.WriteLine("Done!");

                    // Shutdown and end connection
                    client.Close();

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
                    else if (test == ctrl_cmd[13].ToUpper())
                    {
                        Console.WriteLine(data.Remove(dataLen));
                        Entry entry = new Entry();
                        entry.Speak(data.Remove(dataLen));
                    }
                    else if (data == ctrl_cmd[14].ToUpper())
                    {
                        Console.WriteLine(data.Remove(dataLen));
                        ProgramCV imageCV = new ProgramCV();

                        string text = imageCV.AnalyzeImage();
                        Entry entry = new Entry();
                        entry.Speak(text);
                    }
                    else if (data == ctrl_cmd[15].ToUpper())
                    {
                        Console.WriteLine(data.Remove(dataLen));
                        Player player = new Player();
                        player.Record().Wait();
                    }
                    else if (data == ctrl_cmd[16].ToUpper())
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
                        Player player = new Player();
                        player.Play(path).Wait();
                        //string path = player.Path;
                        //player.Play(path).Wait();
                    }
                    else if (data == ctrl_cmd[17].ToUpper())
                    {
                        //Console.WriteLine(data.Remove(dataLen));
                        //Console.WriteLine("\\n");
                        Player player = new Player();
                        player.StopRecording().Wait();
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