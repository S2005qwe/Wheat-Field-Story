using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SFarm.Save;

public class TimeManager : Singleton<TimeManager>, ISaveable
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;

    private Season gameSeason = Season.春天;

    private int monthInSeason = 3;

    public bool gameClockPause;

    private float tikTime;//计时器

    //灯光时间差
    private float timeDifference;

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

    public string GUID => GetComponent<DataGUID>().guid;


    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }


    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        gameClockPause = true;
    }
    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;

            if (tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();//更新时间
            }
        }

        // 快速过1分钟 —— 修复版
        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }

        // 快速过1天 —— 【重点修复这里】不再直接++，调用安全方法
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddOneDay();
        }
    }

    private void OnEndGameEvent()
    {
        gameClockPause = true;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        gameClockPause = gameState == GameState.Pause;
    }

    private void OnStartNewGameEvent(int obj)
    {
        NewGameTime();
        gameClockPause = false;
    }
    private void OnAfterSceneLoadEvent()
    {
        gameClockPause = false;
        EventHandler.CallGameDataEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);

        StartCoroutine(DelayedLightShiftCall());
    }

    private System.Collections.IEnumerator DelayedLightShiftCall()
    {
        yield return null;
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }

    private void NewGameTime()//初始化
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 1;
        gameMonth = 5;
        gameYear = 2026;
        gameSeason = Season.春天;
    }

    #region 时间计算核心（已修复日期越界）
    /// <summary>
    /// 安全增加一天（自动处理月份、年份、季节切换）
    /// </summary>
    private void AddOneDay()
    {
        gameDay++;
        ValidateDate(); // 强制校验日期合法性

        EventHandler.CallGameDayEvent(gameDay, gameSeason);
        EventHandler.CallGameDataEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
    }

    public void UpdateGameTime()//更新时间
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    // 统一日期校验
                    ValidateDate();

                    //用来刷新地图和农作物生长
                    EventHandler.CallGameDayEvent(gameDay, gameSeason);
                }
                EventHandler.CallGameDataEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }
    }

    /// <summary>
    /// 日期合法性校验（自动处理月、年、季节）
    /// </summary>
    private void ValidateDate()
    {
        int daysInMonth = GetDaysInMonth(gameMonth, gameYear);

        // 日期超过当月最大天数 → 进位
        if (gameDay > daysInMonth)
        {
            gameDay = 1;
            gameMonth++;

            // 年份进位
            if (gameMonth > 12)
            {
                gameMonth = 1;
                gameYear++;

                if (gameYear > 9999)
                    gameYear = 2022;
            }
        }

        // 统一更新季节
        UpdateSeason();
    }

    /// <summary>
    /// 更新季节（统一逻辑，避免错乱）
    /// </summary>
    private void UpdateSeason()
    {
        int seasonIndex = (gameMonth - 1) / 3;
        gameSeason = (Season)seasonIndex;
        monthInSeason = ((gameMonth - 1) % 3) + 1;
    }
    #endregion

    // 获取月份天数
    private int GetDaysInMonth(int month, int year)
    {
        switch (month)
        {
            case 2:
                return IsLeapYear(year) ? 29 : 28;
            case 4:
            case 6:
            case 9:
            case 11:
                return 30;
            default:
                return 31;
        }
    }

    // 闰年判断
    private bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
    }

    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }
        else
        {
            timeDifference = MathF.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            return LightShift.Night;
        }
    }
    #region 存档/读档
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameYear", gameYear);
        saveData.timeDict.Add("gameMonth", gameMonth);
        saveData.timeDict.Add("gameDay", gameDay);
        saveData.timeDict.Add("gameHour", gameHour);
        saveData.timeDict.Add("gameMinute", gameMinute);
        saveData.timeDict.Add("gameSecond", gameSecond);
        saveData.timeDict.Add("gameSeason", (int)gameSeason);

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        gameYear = saveData.timeDict["gameYear"];
        gameMonth = saveData.timeDict["gameMonth"];
        gameDay = saveData.timeDict["gameDay"];
        gameHour = saveData.timeDict["gameHour"];
        gameMinute = saveData.timeDict["gameMinute"];
        gameSecond = saveData.timeDict["gameSecond"];
        gameSeason = (Season)saveData.timeDict["gameSeason"];

        // 读档后强制校验日期
        ValidateDate();
    }
    #endregion
}