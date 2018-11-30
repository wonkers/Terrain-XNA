using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terrain
{
    public class SeasonController
    {
        public enum Season
        {
            Spring,
            Summer,
            Autumn,
            Winter
        }
        public enum Month
        {
            January, February, March, April, May, June,
            July, August, September, October, November, December
        }

        public Season CurrentSeason { get; set; }
        public Month CurrentMonth { get; set; }
        public static float[] SeasonColour { get; set; }
        public int Day { get; set; }

        
        public SeasonController()
        {
            CurrentSeason = Season.Spring;
            CurrentMonth = Month.May;
            SeasonColour = new float[4] { 0.13f, 0.58f, 0.2f, 0.0f };
            Day = 121;
        }

        public void Update()
        {
            Day++;
            if (Day > 360) Day = 1;
            CurrentMonth = (Month)((int)Day / 30);

            switch (CurrentMonth)
            {
                case Month.February:
                case Month.March:
                case Month.April:
                    CurrentSeason = Season.Winter;
                    break;
                case Month.May:
                case Month.June:
                case Month.July:
                    CurrentSeason = Season.Spring;
                    break;
                case Month.August:
                case Month.September:
                case Month.October:
                    CurrentSeason = Season.Summer;
                    break;
                case Month.November:
                case Month.December:
                case Month.January:
                    CurrentSeason = Season.Autumn;
                    break;
            }
            //0.004 = 1 from(256)
            switch(CurrentSeason)
            {
                case Season.Spring:
                    SeasonColour[0] += 0.0086f;
                    SeasonColour[1] += 0.0037f;
                    SeasonColour[2] += 0.0028f;
                    break;
                case Season.Summer:
                    SeasonColour[0] -= 0.0086f;
                    SeasonColour[1] -= 0.0037f;
                    SeasonColour[2] -= 0.0028f;
                    break;
                case Season.Autumn:
                    SeasonColour[0] += 0.0074f;
                    SeasonColour[1] += 0.0037f;
                    SeasonColour[2] += 0.0067f;
                    break;
                case Season.Winter:
                    SeasonColour[0] -= 0.0074f;
                    SeasonColour[1] -= 0.0037f;
                    SeasonColour[2] -= 0.0067f;
                    break;

            }
            
        }
    }
}
