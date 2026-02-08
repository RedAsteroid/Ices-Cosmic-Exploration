using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace ICE.Scheduler.Handlers
{
    internal class WeatherForecastHandler
    {
        private const double Seconds = 1;
        private const double Minutes = 60 * Seconds;
        private const double WeatherPeriod = 23 * Minutes + 20 * Seconds;
        internal static bool AccurateTime = false;

        internal static List<WeatherForecast> weathers = [];

        private static DateTime _lastProcessed = DateTime.MinValue;
        private static TimeSpan _delay = TimeSpan.FromSeconds(300);
        private static uint previousZoneForecast = 0;

        internal static unsafe void Tick()
        {
            if (!PlayerHelper.IsInCosmicZone())
                return;

            if (weathers.Count == 0)
            {
                RefreshForecast();
                return;
            }

            Weather currWeather = GetCurrentWeather();
            var CorrectFirstWeather = true;
            var NeedToRefreshBecauseRedAlert = false;

            if (weathers.Count >= 1)
            {
                CorrectFirstWeather = weathers[0].Name == currWeather.Name;
            }

            if (weathers.Count >= 2)
            {
                if (weathers[1].Time - DateTime.UtcNow < TimeSpan.Zero)
                {
                    NeedToRefreshBecauseRedAlert = true;
                }
            }

            if (Svc.ClientState.TerritoryType == previousZoneForecast
                && CorrectFirstWeather
                && !NeedToRefreshBecauseRedAlert
                && DateTime.UtcNow - _lastProcessed < _delay) return;

            RefreshForecast();
        }

        internal static void RefreshForecast()
        {
            if (!PlayerHelper.IsInCosmicZone()) return;

            _lastProcessed = DateTime.UtcNow;
            GetForecast();
        }

        private static unsafe Weather GetCurrentWeather()
        {
            WeatherManager* wm = WeatherManager.Instance();
            byte currWeatherId = wm->GetCurrentWeather();
            return ExcelHelper.WeatherSheet.GetRow(currWeatherId);
        }

        internal static unsafe uint GetCurrentWeatherId()
        {
            if (!PlayerHelper.IsInCosmicZone()) return default;

            Weather currWeather = GetCurrentWeather();
            return currWeather.RowId;
        }

        internal static unsafe (string, uint, string, uint, string) GetNextWeather()
        {
            if (!PlayerHelper.IsInCosmicZone())
                return default;

            if (weathers.Count == 0)
                return default;

            // 只有一个天气时，返回当前天气，next 留空
            if (weathers.Count == 1)
            {
                var current = weathers[0];
                return (current.Name, current.IconId, "", 0, "");
            }

            // 获取当前和下一个天气信息
            var currentWeather = weathers[0];
            var nextWeather = weathers[1];

            return (
                currentWeather.Name,
                currentWeather.IconId,
                nextWeather.Name,
                nextWeather.IconId,
                FormatForecastTime(nextWeather.Time)
            );
        }
        internal static unsafe List<(string Name, uint IconId, string TimeUntil)> GetNextWeathers(int count = 5)
        {
            if (!PlayerHelper.IsInCosmicZone()) return new List<(string, uint, string)>();
            if (weathers.Count == 0) return new List<(string, uint, string)>();

            var result = new List<(string Name, uint IconId, string TimeUntil)>();

            for (int i = 0; i < Math.Min(count, weathers.Count); i++)
            {
                var weather = weathers[i];
                string timeUntil = i == 0 ? "Now" : FormatForecastTime(weather.Time);
                result.Add((weather.Name, weather.IconId, timeUntil));
            }

            return result;
        }

        internal static unsafe void GetForecast()
        {
            previousZoneForecast = Svc.ClientState.TerritoryType;

            WeatherManager* wm = WeatherManager.Instance();
            Weather currentWeather = GetCurrentWeather();
            Weather lastWeather = currentWeather;

            weathers = [BuildResultObject(currentWeather, GetRootTime(0))];

            for (var i = 1; i <= 24; i++)
            {
                byte weatherId = wm->GetWeatherForDaytime(Svc.ClientState.TerritoryType, i);
                var weather = ExcelHelper.WeatherSheet.GetRow(weatherId)!;
                var time = GetRootTime(i * WeatherPeriod);

                if (lastWeather.RowId != weather.RowId)
                {
                    lastWeather = weather;
                    weathers.Add(BuildResultObject(weather, time));
                }
            }
        }

        internal static unsafe List<WeatherForecast> GetTerritoryForecast(ushort territoryId)
        {
            List<WeatherForecast> territoryForecast = [];

            WeatherManager* wm = WeatherManager.Instance();
            byte weatherId = wm->GetWeatherForDaytime(territoryId, 0);
            Weather currentWeather = ExcelHelper.WeatherSheet.GetRow(weatherId);
            Weather lastWeather = currentWeather;

            territoryForecast = [BuildResultObject(currentWeather, GetRootTime(0))];

            for (var i = 1; i <= 24; i++)
            {
                weatherId = wm->GetWeatherForDaytime(territoryId, i);
                var weather = ExcelHelper.WeatherSheet.GetRow(weatherId)!;
                var time = GetRootTime(i * WeatherPeriod);

                if (lastWeather.RowId != weather.RowId)
                {
                    lastWeather = weather;
                    territoryForecast.Add(BuildResultObject(weather, time));
                }
            }
            return territoryForecast;
        }
        private static WeatherForecast BuildResultObject(Weather weather, DateTime time)
        {
            var name = weather.Name.ExtractText();
            var iconId = (uint)weather.Icon;

            return new(time, name, iconId);
        }
        private static DateTime GetRootTime(double initialOffset)
        {
            var now = DateTime.UtcNow;
            var rootTime = now.AddMilliseconds(-now.Millisecond).AddSeconds(initialOffset);
            var seconds = (long)(rootTime - DateTime.UnixEpoch).TotalSeconds % WeatherPeriod;

            rootTime = rootTime.AddSeconds(-seconds);

            return rootTime;
        }

        internal static string FormatForecastTime(DateTime forecastTime)
        {
            TimeSpan timeDifference = forecastTime - DateTime.UtcNow;
            if (!AccurateTime)
            {
                string format = C.ShowSeconds ? @"hh\:mm\:ss" : @"hh\:mm";
                return timeDifference < TimeSpan.Zero
                    ? "-" + timeDifference.Duration().ToString(format)
                    : timeDifference.ToString(format);
            }
            else
            {
                var ts = timeDifference;
                bool negative = ts < TimeSpan.Zero;
                ts = ts.Duration();

                int hours = (int)ts.TotalHours;
                int minutes = ts.Minutes;
                int seconds = ts.Seconds;

                string format = C.ShowSeconds
                    ? $"{hours:D2}:{minutes:D2}:{seconds:D2}"
                    : $"{hours:D2}:{minutes:D2}";

                return negative ? "-" + format : format;
            }
        }
    }

    internal class WeatherForecast(DateTime time, string name, uint iconId)
    {
        public DateTime Time = time;
        public string Name = name;
        public uint IconId = iconId;
    }
}
