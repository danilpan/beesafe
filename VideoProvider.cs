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

        private static  string [] advertVideos;
        private static string[] demonstrationVideosKz = new string[1];
        private static string[] demonstrationVideosRu = new string[1];
        private static string[] alertVideo = new string[1];
        private static string[] putPhoneVideo = new string[1];
        private static string[] proccessVideosKz = new string[1];
        private static string[] proccessVideosRu = new string[1];

        public static void SetAdvertVideos()
        {
            advertVideos = Directory.GetFiles($"{Application.StartupPath}\\Media\\AdvertVideo");
        }

        public static void SetDemonstrationVideo()
        {
            if (lastVideoLanguage == RUSSIAN)
            {
                lastVideoLanguage = KAZAKH;

                demonstrationVideosKz[0] =  $"{Application.StartupPath}\\Media\\Demonstration\\{KAZAKH}.mp4" ;
            }
            lastVideoLanguage = RUSSIAN;
            demonstrationVideosRu[0] = $"{Application.StartupPath}\\Media\\Demonstration\\{RUSSIAN}.mp4";
        }

        public static void SetAlertVideo()
        {
            alertVideo[0]=  $"{Application.StartupPath}\\Media\\Alarm\\Alarm.mp4" ;
        }

        public static void SetPutPhoneVideo()
        {
            putPhoneVideo[0] = $"{Application.StartupPath}\\Media\\PutPhone\\PutPhone.mp4";
        }

        public static void SetPrococessVideos()
        {
            if (lastVideoProcessingLanguage == RUSSIAN)
            {
                lastVideoProcessingLanguage = KAZAKH;

                proccessVideosKz[0]= $"{Application.StartupPath}\\Media\\Process\\{KAZAKH}.mp4";
            }
            lastVideoProcessingLanguage = RUSSIAN;

            proccessVideosRu[0] = $"{Application.StartupPath}\\Media\\Process\\{RUSSIAN}.mp4";
        }


        public static void InitializeVideos()
        {
            SetAdvertVideos();
            SetDemonstrationVideo();
            SetAlertVideo();
            SetPutPhoneVideo();
            SetPrococessVideos();
        }
        public static string[] GetVideosById(int id)
        {
            switch (id)
            {
                case 1:
                    return  advertVideos;
                case 2:
                    return lastVideoLanguage == KAZAKH ? demonstrationVideosKz : demonstrationVideosRu;
                case 3:
                    return putPhoneVideo;
                case 4:
                    return lastVideoProcessingLanguage == KAZAKH ? proccessVideosKz : proccessVideosRu;
                case 5:
                    return alertVideo;
            }
            return null;
        }
    }
}
