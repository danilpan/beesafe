using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BeeSafe
{
    class VideoProvider
    {
        private const string RUSSIAN = "ru";
        private const string KAZAKH = "kz";

        private static string lastVideoLanguage = RUSSIAN;
        private static string lastVideoProcessingLanguage = RUSSIAN;


        public static string[] GetAdvertVideos()
        {
            Random rnd = new Random();

            string[] advertVideos = Directory.GetFiles($"{Application.StartupPath}\\Media\\AdvertVideo");
            string[] randomizedVideoUrls = advertVideos.OrderBy(x => rnd.Next()).ToArray();

            return randomizedVideoUrls;
        }

        public static string[] GetDemonstrationVideo()
        {
            if (lastVideoLanguage == RUSSIAN)
            {
                lastVideoLanguage = KAZAKH;

                return new string[] { $"{Application.StartupPath}\\Media\\Demonstration\\{KAZAKH}.mp4" };
            }
            lastVideoLanguage = RUSSIAN;

            return new string[] { $"{Application.StartupPath}\\Media\\Demonstration\\{RUSSIAN}.mp4" };
        }

        public static string[] GetAlertVideo()
        {
            return new string[] { $"{Application.StartupPath}\\Media\\Alarm\\Alarm.mp4" };
        }

        public static string[] GetPutPhoneVideo()
        {
            return new string[] { $"{Application.StartupPath}\\Media\\PutPhone\\PutPhone.mp4" };
        }

        public static string[] GetPrococessVideo()
        {
            if (lastVideoProcessingLanguage == RUSSIAN)
            {
                lastVideoProcessingLanguage = KAZAKH;

                return new string[] { $"{Application.StartupPath}\\Media\\Process\\{KAZAKH}.mp4" };
            }
            lastVideoProcessingLanguage = RUSSIAN;

            return new string[] { $"{Application.StartupPath}\\Media\\Process\\{RUSSIAN}.mp4" }; ;
        }

        public static string[] GetVideosById(int id)
        {
            var videosPath = new string[] { };
            switch (id)
            {
                case 1:
                    videosPath = VideoProvider.GetAdvertVideos();
                    break;
                case 2:
                    videosPath = VideoProvider.GetDemonstrationVideo();
                    break;
                case 3:
                    videosPath = VideoProvider.GetPutPhoneVideo();
                    break;
                case 4:
                    videosPath = VideoProvider.GetPrococessVideo();
                    break;
                case 5:
                    videosPath = VideoProvider.GetAlertVideo();
                    break;
            }

            return videosPath;
        }
    }
}
