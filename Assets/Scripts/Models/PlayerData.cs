using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Models
{
    public static class PlayerData
    {
        public static void InitDate()
        {
            #if DEBUG
            MaxUnlockLevel = 50;
            CurrentLevel = 0;
            MaxScore = 0;
            HammerCount = 100;
            ScissorsCount = 100;
            SideBarEnterTime = DateTime.Today - TimeSpan.FromDays(1);
            #else
            MaxUnlockLevel = 0;
            CurrentLevel = 0;
            MaxScore = 0;
            HammerCount = 1;
            ScissorsCount = 1;
            SideBarEnterTime = DateTime.Today - TimeSpan.FromDays(1);
            #endif
        }

        public static int MaxUnlockLevel
        {
            get => PlayerPrefs.GetInt(nameof(MaxUnlockLevel));
            set
            {
                PlayerPrefs.SetInt(nameof(MaxUnlockLevel), value);
                PlayerPrefs.Save();
            }
        }

        public static int CurrentLevel
        {
            get => PlayerPrefs.GetInt(nameof(CurrentLevel));
            set
            {
                PlayerPrefs.SetInt(nameof(CurrentLevel), value);
                PlayerPrefs.Save();
            }
        }
        
        public static int MaxScore
        {
            get => PlayerPrefs.GetInt(nameof(MaxScore));
            set
            {
                PlayerPrefs.SetInt(nameof(MaxScore), value);
                PlayerPrefs.Save();
            }
        }
        
        public static int HammerCount
        { 
            get => PlayerPrefs.GetInt(nameof(HammerCount)); 
            set
            {
                PlayerPrefs.SetInt(nameof(HammerCount), Mathf.Max(value, 0)); 
                PlayerPrefs.Save();
            }
        }
        public static int ScissorsCount
        { 
            get => PlayerPrefs.GetInt(nameof(ScissorsCount)); 
            set
            {
                PlayerPrefs.SetInt(nameof(ScissorsCount), Mathf.Max(value, 0));
                PlayerPrefs.Save();
            }
        }

        public static DateTime SideBarEnterTime
        {
            get
            { 
                try
                {
                    var date = DateTime.ParseExact(
                        PlayerPrefs.GetString(nameof(SideBarEnterTime)),
                        "yyyy-MM-dd HH:mm:ss",
                        null);
                    return date;
                }
                catch
                {
                    return DateTime.Today - TimeSpan.FromDays(1);
                }
            }
            set
            {
                PlayerPrefs.SetString(nameof(SideBarEnterTime), value.ToString("yyyy-MM-dd HH:mm:ss")); 
                PlayerPrefs.Save();
            }
        }
        public static bool HasEnterSideBarAward => DateTime.Now >= SideBarEnterTime.AddDays(1);
        public static bool IsFirstGame
        {
            get => PlayerPrefs.GetInt(nameof(IsFirstGame)) == 0;
            set
            {
                PlayerPrefs.SetInt(nameof(IsFirstGame), value ? 0 : 1);
                PlayerPrefs.Save();
            }
        }
    }
}