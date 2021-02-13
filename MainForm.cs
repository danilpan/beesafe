using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace BeeSafe
{
    public partial class MainForm : Form
    {
        private static SerialPort phonePort;
        private static readonly string sanitizerPortName = "COM3";
        private static readonly string temperaturePortName = "COM4";

        static WMPLib.IWMPMedia media;
        CameraProvider camera;

        protected delegate void setValue(string value);
        protected delegate void setPicture();
        protected delegate void hidePicture();
        public MainForm()
        {
            InitializeComponent();
            SetTemperature("");
            InitializeComPort();
            ReadPort(phonePort);
            pictureBoxForImage.Hide();
            VideoProvider.InitializeVideos();
            InitializeVideoPlayer(1);
            camera = new CameraProvider();
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
            try
            {
                var videosPath = VideoProvider.GetVideosById(id);
                videoPlayer.playlistCollection.remove(videoPlayer.currentPlaylist);
                WMPLib.IWMPPlaylist playlist = videoPlayer.playlistCollection.newPlaylist($"myplaylist1{id}");
                foreach (string video in videosPath)
                {
                    SetVideo(video);
                    playlist.appendItem(media);
                }

                videoPlayer.currentPlaylist = playlist;
                videoPlayer.uiMode = "None";
                videoPlayer.settings.setMode("loop", true);
            }
            catch (Exception e)
            {
                Emailer.getInstance().logException(e);
            }
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
     
        public void ShowPictureBox()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new setPicture(ShowPictureBox));
            }
            else
            {
                if(pictureBoxForImage.Image != null)
                {
                    pictureBoxForImage.Image.Dispose();
                }
                pictureBoxForImage.Image = camera.getImage();
                pictureBoxForImage.Show();
            }
        }

        public void HidePictureBox()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new hidePicture((pictureBoxForImage.Hide)));
            }
            else
            {
                pictureBoxForImage.Hide();
                pictureBoxForImage.Image.Dispose();
            }
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
            try { 
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