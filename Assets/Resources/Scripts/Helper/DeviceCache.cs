
public static class DeviceCache
{
    //totalpoint
    public static void SetGameTotalPoint(float point) { UnityEngine.PlayerPrefs.SetFloat("game.totalpoint", point); }
    public static float GetGameTotalPoint() { return UnityEngine.PlayerPrefs.GetFloat("game.totalpoint", 0); }

    //level
    public static void SetGameLevel(int level) { UnityEngine.PlayerPrefs.SetInt("game.levelno", level); }
    public static int GetGameLevel() { return UnityEngine.PlayerPrefs.GetInt("game.levelno", 1); }
    public static void SetGameNextLevel() { SetGameLevel(GetGameLevel() + 1); }

    //sound
    public static void SetOptionSound(bool value) { UnityEngine.PlayerPrefs.SetInt("game.option.sound", System.Convert.ToInt32(value)); }
    public static int GetOptionSound() { return UnityEngine.PlayerPrefs.GetInt("game.option.sound", 1); }
    public static int ToggleOptionSound()
    {
        var value = GetOptionSound();
        value = value == 0 ? 1 : 0;
        SetOptionSound(System.Convert.ToBoolean(value));
        return value;
    }

    //vibration
    public static void SetOptionVibration(bool value) { UnityEngine.PlayerPrefs.SetInt("game.option.vibration", System.Convert.ToInt32(value)); }
    public static int GetOptionVibration() { return UnityEngine.PlayerPrefs.GetInt("game.option.vibration", 1); }
    public static int ToggleOptionVibration()
    {
        var value = GetOptionVibration();
        value = value == 0 ? 1 : 0;
        SetOptionVibration(System.Convert.ToBoolean(value));
        return value;
    }



    public static System.Collections.Generic.Dictionary<int, Level.LevelData.BaseItem> levelItems;
    public static Level.LevelData.Level currentLevelInfo;
}
