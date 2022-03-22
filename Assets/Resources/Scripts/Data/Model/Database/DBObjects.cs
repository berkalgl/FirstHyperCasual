using System;

namespace Assets.Scripts.Data.Model.Database
{
    [Serializable]
    public class DeviceCurrent
    {
        public int levelNo;
        public float totalPoint;

        public static DeviceCurrent Default()
        {
            return new DeviceCurrent()
            {
                levelNo = 1,
                totalPoint = 0
            };
        }
    }

    [Serializable]
    public class DeviceMain
    {
        public int width;
        public int height;
        public string language;
        public string type;

        public static DeviceMain GetMain()
        {
            return new DeviceMain()
            {
                width = UnityEngine.Screen.width,
                height = UnityEngine.Screen.height,
                language = UnityEngine.Application.systemLanguage.ToString(),
                type = DeviceHelper.GetDeviceType()
            };
        }
    }

    [Serializable]
    public class DeviceLevel
    {
        public int point;
        public int tryCount;
        public object startDate;
        public object endDate;

        public static DeviceLevel Default()
        {
            return new DeviceLevel()
            {
                tryCount = 1,
                startDate = Firebase.Database.ServerValue.Timestamp
            };
        }

        public void EndLevel(int point)
        {
            this.point = point;
            this.endDate = Firebase.Database.ServerValue.Timestamp;
        }
    }
}
