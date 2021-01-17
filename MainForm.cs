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

        static WMPLib.IWMPMedia media;
        WMPLib.IWMPPlaylist playlist;

        protected delegate void setValue(string value);
        protected delegate void setPicture();
        protected delegate void hidePicture();
        protected delegate void getImage();

        public MainForm()
        {
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

        private void InitializeVideoPlayer(int id)  
        {
            var videosPath = VideoProvider.GetVideosById(id);
            playlist = videoPlayer.playlistCollection.newPlaylist($"myplaylist{id}");
            
            foreach (string video in videosPath)
            {
                SetVideo(video);
                playlist.appendItem(media);
            }
            
            videoPlayer.currentPlaylist = playlist;
            videoPlayer.uiMode = "None";
            videoPlayer.settings.setMode("loop", true);
        }

        public void SetVideo(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new setValue(SetVideo), value);
            }

            else
            {
                media = videoPlayer.newMedia(value);
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

                    capture.Read(frame);
                    Bitmap image = BitmapConverter.ToBitmap(frame);
                    if (pictureBoxForImage.Image != null)
                    {
                        pictureBoxForImage.Image.Dispose();
                    }
                    pictureBoxForImage.Image = image;

                }
            }
        }


        public void ShowPictureBox()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new setPicture(pictureBoxForImage.Show));
            }
            else
            {
                pictureBoxForImage.Show();
            }
        }

        public void HidePictureBox()
        {
            if (this.InvokeRequired) this.Invoke(new setPicture(pictureBoxForImage.Hide));
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

            // Рекламные ролики
            if (c1 == '1')
            {
                HidePictureBox();
                SetTemperature("");
                InitializeVideoPlayer(1);
            }

            // Демонстрация
            else if (c1 == '2')
            {
                HidePictureBox();
                SetTemperature("");
                InitializeVideoPlayer(2);
            }

            // Положите телефон
            else if (c1 == '3')
            {
                InitializeVideoPlayer(3);
                SetTemperature(getTemperature());
                ShowPictureBox();
            }

            // Обработка телефона
            else if (c1 == '4')
            {
                InitializeVideoPlayer(4);
                HidePictureBox();
                SetTemperature("");
            }

            // Не успешная обработка
            else if (c1 == '5')
            {
                InitializeVideoPlayer(5);
                HidePictureBox();
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
                temperature = port.ReadLine().Split(' ')[4];
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