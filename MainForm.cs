using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace BeeSafe
{
    public partial class MainForm : Form
    {
        private static SerialPort phonePort;
        private static readonly string sanitizerPortName = "COM3";
        private static readonly string temperaturePortName = "COM4";

        
        private Bitmap image;
        private WMPLib.IWMPPlaylist[] playlists = new WMPLib.IWMPPlaylist[5];

        protected delegate void setValue(string value);
        protected delegate void setPicture();
        protected delegate void hidePicture();
        public MainForm()
        {
            VideoProvider.InitializeVideos();
            InitializePlaylists();
            InitializeComponent();
            SetTemperature("");
            pictureBoxForImage.Hide();
            InitializeVideoPlayer(1);
            CaptureCamera();
            InitializeComPort();
            ReadPort(phonePort);
        }
        public void SetTemperature(string value)
        {
            if (this.InvokeRequired)
            {
                Invoke(new setValue(SetTemperature), value);
            }
            else
            {
                this.temperatureLabel.Text = value;
            }

        }
        private void InitializePlaylists()
        {
            for(int i = 1; i <= 5; i++)
            {
                var videosPath = VideoProvider.GetVideosById(i);
                WMPLib.IWMPPlaylist playlist = videoPlayer.playlistCollection.getAll().Item(i);
                foreach (string video in videosPath)
                {
                    WMPLib.IWMPMedia media = videoPlayer.newMedia(video);
                    playlist.appendItem(media);
                }
                playlists[i] = playlist;
            }
        }
        private void clearPlaylistCollections()
        {
            videoPlayer.currentPlaylist.clear();
            for(int i = 1; i <= 5; i++)
            {
                videoPlayer.playlistCollection.remove(playlists[i]);
            }
        }
     
        private void InitializeVideoPlayer(int id)
        {
            try
            {
                WMPLib.IWMPPlaylist playlist = playlists[id];
                videoPlayer.currentPlaylist = playlist;
                videoPlayer.uiMode = "None";
                videoPlayer.settings.setMode("loop", true);
            }
            catch (Exception e)
            {
                Emailer.getInstance().logException(e);
            }
        }

        private void CaptureCamera()
        {
            Thread camera = new Thread(new ThreadStart(CaptureCameraCallback));
            camera.Start();
        }
        private void CaptureCameraCallback()
        {
            Mat frame = new Mat();
            VideoCapture capture = new VideoCapture(0);
            capture.Open(0);

            if (capture.IsOpened())
            {
                while (true)
                {
                    try
                    {
                        capture.Read(frame);
                        image = BitmapConverter.ToBitmap(frame);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }

        public void ShowPictureBox()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new setPicture(ShowPictureBox));
            }
            else
            {
                pictureBoxForImage.Image = image;
                pictureBoxForImage.Show();
            }
        }

        public void HidePictureBox()
        {
            if (this.InvokeRequired) this.Invoke(new hidePicture((pictureBoxForImage.Hide)));
            else pictureBoxForImage.Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            phonePort.Close();
            Environment.Exit(1);
        }


        private void InitializeComPort()
        {
            phonePort = new SerialPort(sanitizerPortName, 9600, Parity.None, 8, StopBits.One);
            phonePort.DataReceived += DataReceivedHandler;
        }

        private void ReadPort(SerialPort sp)
        {
            sp.Open();
        }


        //Data Received Event Handler
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            char c1 = getSignal(sender);
            HidePictureBox();
            clearPlaylistCollections();
            // Рекламные ролики
            if (c1 == '1')
            {
                SetTemperature("");
                InitializeVideoPlayer(1);
            }

            // Демонстрация
            else if (c1 == '2')
            {
                ShowPictureBox();
                SetTemperature(getTemperature());
                InitializeVideoPlayer(2);
            }

            // Положите телефон
            else if (c1 == '3')
            {
                SetTemperature(getTemperature());
                ShowPictureBox();
                InitializeVideoPlayer(3);
            }

            // Обработка телефона
            else if (c1 == '4')
            {
                InitializeVideoPlayer(4);
                SetTemperature("");
            }

            // Не успешная обработка
            else if (c1 == '5')
            {
                InitializeVideoPlayer(5);
                SetTemperature("");  
            }

            //Показ фотографий и температуры 
            else if (c1 == '7')
            {
                SetTemperature(getTemperature());
                ShowPictureBox();
            }

            else if (c1 == '8')
            {
                Emailer.getInstance().logOnLiquidEnded();
            }

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        private char getSignal(object sender)
        {
            SerialPort sp = sender as SerialPort;

            string data = sp.ReadLine();
            char[] elements = data.ToCharArray();
            char c1 = elements[0];

            return c1;
        }

        //Temperature Read
        private string getTemperature()
        {
            string temperature = "Not Identified";
            SerialPort port = new SerialPort(temperaturePortName, 9600, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
                temperature = port.ReadLine();
                port.Close();
            }
            catch (Exception ex)
            {
                Emailer.getInstance().logException(ex);
            }

            return temperature;
        }
    }
}