using EpsonMarkingAPI.Common;
using EpsonMarkingAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using static EpsonMarkingAPI.Common.FunctionStatus;


namespace EpsonMarkingAPI.Services
{
    /// <summary>
    /// Generate Week Code for MA/SG product
    /// </summary>
    public class MaSgWeekCodeDataApi : IMaSgMarkingDataApi
    {
        private string _dbPath = "";


        /// <summary>
        /// API Constructor
        /// </summary>
        public MaSgWeekCodeDataApi()
        {
            _dbPath = ConfigurationManager.AppSettings["masgWeekCodeDatabase"];
        }

        /// <summary>
        /// API Constructor
        /// </summary>
        /// <param name="dbPath"></param>
        public MaSgWeekCodeDataApi(string dbPath)
        {
            _dbPath = dbPath;
        }

        /// <summary>
        /// Get Week Code
        /// </summary>
        /// <param name="specNo"></param>
        /// <param name="qryItem"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        public SuccessFailure GetMarkingData(string specNo, string qryItem, ref string returnValue)
        {
            SuccessFailure ret = SuccessFailure.error;

            if (!string.IsNullOrEmpty(this._dbPath))
            {
                if (File.Exists(this._dbPath))
                {
                    string[] fileContent = File.ReadAllLines(this._dbPath, Encoding.ASCII);

                    if (fileContent.Length > 0)
                    {
                        List<string> selectedItems = fileContent.Where(n => n.StartsWith(specNo)).ToList();

                        if (selectedItems.Count > 0)
                        {
                            List<EpmDb003> epmdb003 = new List<EpmDb003>();

                            foreach (var item in selectedItems)
                            {
                                string[] itm = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                if (itm.Length == 3)
                                {
                                    epmdb003.Add(new EpmDb003()
                                    {
                                        SpecNo = itm[0].Trim(),
                                        GColumn = itm[1].Trim(),
                                        DataProperty = itm[2].Trim(),
                                    });
                                }
                            }

                            List<MarkingData> markingData = new List<MarkingData>();
                            ret = FormWeekCodeData(epmdb003, specNo, qryItem, ref markingData);

                            if (ret == SuccessFailure.Success && markingData.Count > 0)
                            {
                                int[] row = markingData.Select(n => n.RowNo).Distinct().ToArray();

                                if (row.Length > 0)
                                {
                                    string markingDataString = "";

                                    foreach (var item in row)
                                    {
                                        List<MarkingData> selectMarkingData = markingData.Where(n => n.RowNo == item).OrderBy(m => m.RowData).ToList();

                                        if (selectMarkingData != null && selectMarkingData.Count > 0)
                                        {
                                            foreach (var dataItem in selectMarkingData)
                                            {
                                                markingDataString += dataItem.Value;
                                            }
                                        }

                                        markingDataString += "|";
                                    }

                                    returnValue = markingDataString;
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        private SuccessFailure FormWeekCodeData(List<EpmDb003> epmdb003, string qryItem, string inaCode, ref List<MarkingData> markingData)
        {
            SuccessFailure ret = SuccessFailure.error;
            DateTime weekCodeDate = DateTime.Now;
            string yr = weekCodeDate.Year.ToString();

            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            int weekCode_weekNo = myCal.GetWeekOfYear(weekCodeDate, myCWR, myFirstDOW);
            string weekCode_weekNoStr = string.Format("{0:D2}", weekCode_weekNo);

            string weekCode = "";
            string weekType = epmdb003[0].GColumn;

            if (string.IsNullOrEmpty(weekType))
            {
                return ret;
            }

            if (weekType.ToUpper().StartsWith("IF"))
            {

                switch (weekType.ToUpper())
                {
                    case "IF01":
                        weekCode = yr + weekCode_weekNoStr.ToString() + "2Z";
                        break;

                    case "IF02":
                        weekCode = yr + weekCode_weekNoStr.ToString() + "2Z";
                        break;

                    case "IF03":
                        weekCode = yr + weekCode_weekNoStr.ToString() + "2Z";
                        break;

                    case "IF23":
                        weekCode = yr + weekCode_weekNoStr.ToString() + "2Z";
                        break;

                    default:
                        weekCode = "";
                        break;
                }

                markingData = new List<MarkingData>();
                markingData.Add(new MarkingData()
                {
                    RowNo = 0,
                    RowData = 0,
                    DataFormat = "",
                    Value = weekCode
                });
                markingData.Add(new MarkingData()
                {
                    RowNo = 1,
                    RowData = 0,
                    DataFormat = "",
                    Value = weekType
                });
            }
            else
            {
                string weekCode_Year = "";
                string customMarking = "";

                int weekCode_yr = Convert.ToInt32(yr.Substring(yr.Length - 1));
                int weekCode_day = Convert.ToInt32(weekCodeDate.DayOfWeek) + 1; //Sunday=1, Monday=2, Tuesday=3, Wednesday=4, Thurday=5, Friday=6, Saturday=7

                int weekTypeNo = Convert.ToInt32(weekType.Substring(1));

                if (weekTypeNo > 200 && weekTypeNo < 300)
                {
                    weekCode_Year = Constant.Yrs_Table200.Substring(weekCode_yr, 1);
                    weekTypeNo -= 200;
                }
                else if (weekTypeNo > 300 && weekTypeNo < 400)
                {
                    weekCode_Year = Constant.Yrs_Table300.Substring(weekCode_yr, 1);
                    weekTypeNo -= 300;
                }
                else if (weekTypeNo > 400 && weekTypeNo < 500)
                {
                    weekCode_Year = Constant.Yrs_Table400.Substring(weekCode_yr, 1);
                    weekTypeNo -= 400;
                }

                //Week Type start with???
                string weekTypeSymbol = weekType.Substring(0, 1);

                switch (weekTypeSymbol.ToUpper())
                {
                    case "S":
                        if (weekTypeNo <= 2)
                        {
                            char ver = (char)(0x0040 + weekTypeNo);

                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() + 
                                ConfigurationManager.AppSettings["factory"] + ver.ToString();
                        }
                        else if (weekTypeNo == 3)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "Z";
                        }
                        else if (weekTypeNo == 4)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() + "A" +
                                ConfigurationManager.AppSettings["factory"] ;
                        }
                        else if (weekTypeNo == 5)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() + "Z" +
                                ConfigurationManager.AppSettings["factory"];
                        }
                        else if (weekTypeNo == 6)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "A";
                        }
                        else if (weekTypeNo == 7)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "B";
                        }
                        else if (weekTypeNo == 8)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "A";
                        }
                        else if (weekTypeNo == 9)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"];
                        }
                        else if (weekTypeNo == 10)
                        {
                            weekCode = yr.Substring(yr.Length - 2) + weekCode_weekNoStr.ToString();
                        }
                        else if (weekTypeNo == 11)
                        {
                            weekCode = yr.Substring(yr.Length - 2) + weekCode_weekNoStr.ToString() + "A";
                        }
                        else if (weekTypeNo == 12)
                        {
                            weekCode = weekCode_weekNoStr.ToString() + yr.Substring(yr.Length - 2);
                        }
                        else if (weekTypeNo == 13)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "B";
                        }
                        else if (weekTypeNo == 14)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "D";
                        }
                        else if (weekTypeNo == 15)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "A";
                        }
                        else if (weekTypeNo == 16)
                        {
                            weekCode = weekCode_weekNoStr.ToString() + yr.Substring(yr.Length - 2) + "A";
                        }
                        else if (weekTypeNo == 17)
                        {
                            weekCode = weekCode_weekNoStr.ToString() + yr.Substring(yr.Length - 2) + "D";
                        }
                        else if (weekTypeNo == 18)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() + "A";
                        }
                        else if (weekTypeNo == 19)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() + "B";
                        }
                        else if (weekTypeNo == 20)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "C";
                        }
                        else if (weekTypeNo == 21)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "C";
                        }
                        else if (weekTypeNo == 21)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() + "C";
                        }
                        else if (weekTypeNo == 23)
                        {
                            weekCode = yr.Substring(yr.Length - 2) + weekCode_weekNoStr.ToString() + "C";
                        }
                        else if (weekTypeNo == 24)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "C";
                        }
                        else if (weekTypeNo == 25)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "C";
                        }
                        else if (weekTypeNo == 26)
                        {
                            weekCode = weekCode_weekNoStr.ToString() + yr.Substring(yr.Length - 2) + "C";
                        }
                        else if (weekTypeNo == 27)
                        {
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1) + "A";
                        }
                        else if (weekTypeNo == 28)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() + "D";
                        }
                        else if (weekTypeNo == 29)
                        {
                            weekCode = yr.Substring(yr.Length - 2) + weekCode_weekNoStr.ToString() + "D";
                        }
                        else if (weekTypeNo == 30)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "D";
                        }
                        else if (weekTypeNo == 31)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "E";
                        }
                        else if (weekTypeNo == 32)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() + "E";
                        }
                        else if (weekTypeNo == 33)
                        {
                            weekCode = yr.Substring(yr.Length - 2) + weekCode_weekNoStr.ToString() + "E";
                        }
                        else if (weekTypeNo == 34)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString() +
                                ConfigurationManager.AppSettings["factory"] + "E";
                        }

                        break;

                    case "T":
                        if (weekTypeNo == 1)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0040 + weekTypeNo + tmp);
                            weekCode = weekCode_Year + 
                                weekCode_weekNoStr.ToString() + 
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() + 
                                ver.ToString(); 
                        }
                        else if (weekTypeNo == 2)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0040 + weekTypeNo + tmp);
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                ver.ToString();
                        }
                        else if (weekTypeNo == 3)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0040 + weekTypeNo + tmp);
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                ver.ToString();
                        }
                        else if (weekTypeNo == 4)
                        {
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                "X";
                        }
                        else if (weekTypeNo == 5)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0041 + tmp);
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1) +
                                ver.ToString();
                        }
                        else if (weekTypeNo == 6)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0042 + tmp);
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1) +
                                ver.ToString();
                        }
                        else if (weekTypeNo == 7)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0043 + tmp);
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1) +
                                ver.ToString();
                        }
                        else if (weekTypeNo == 8)
                        {
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1) +
                                "X";
                        }
                        else if (weekTypeNo == 9)
                        {
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                "S";
                        }
                        else if (weekTypeNo == 10)
                        {
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                "F";
                        }
                        else if (weekTypeNo == 11)
                        {
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                "H";
                        }
                        else if (weekTypeNo == 12)
                        {
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                "I";
                        }
                        else if (weekTypeNo == 13)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0041 + tmp);
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1) +
                                ver.ToString() + ".";
                        }
                        else if (weekTypeNo == 14)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0043 + tmp);
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1) +
                                ver.ToString() + ".";
                        }
                        else if (weekTypeNo == 17)
                        {
                            int tmp = GetFactoryCode();
                            char ver = (char)(0x0043 + tmp);
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1) +
                                "J";
                        }
                        else if (weekTypeNo == 18)
                        {
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                "J";
                        }
                        else if (weekTypeNo == 19)
                        {
                            weekCode = weekCode_Year +
                                weekCode_weekNoStr.ToString() +
                                ((int)weekCodeDate.DayOfWeek + 1).ToString() +
                                "K";
                        }


                        break;

                    case "C":
                        if (weekTypeNo == 1)
                        {
                            weekCode = yr.Substring(yr.Length - 2) + Constant.Mon_C1.Substring(weekCodeDate.Month - 1, 1) + "C";
                        }
                        else if (weekTypeNo == 2)
                        {
                            weekCode = weekCode_Year + weekCode_weekNoStr.ToString();
                        }
                        else if (weekTypeNo == 3)
                        {
                            if (yr.Substring(yr.Length - 2) == "01")
                            {
                                if (Constant.SunnyTable[0, 2] != "")
                                {
                                    weekCode = Constant.SunnyTable[0, 2].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[0, 0].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[0, 1].Substring(weekCode_weekNo, 1);
                                }
                                else
                                {
                                    customMarking = weekType;
                                }
                            }
                            else if (yr.Substring(yr.Length - 2) == "02")
                            {
                                if (Constant.SunnyTable[1, 2] != "")
                                {
                                    weekCode = Constant.SunnyTable[1, 2].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[1, 0].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[1, 1].Substring(weekCode_weekNo, 1);
                                }
                                else
                                {
                                    customMarking = weekType;
                                }
                            }
                            else if (yr.Substring(yr.Length - 2) == "03")
                            {
                                if (Constant.SunnyTable[2, 2] != "")
                                {
                                    weekCode = Constant.SunnyTable[2, 2].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[2, 0].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[2, 1].Substring(weekCode_weekNo, 1);
                                }
                                else
                                {
                                    customMarking = weekType;
                                }
                            }
                            else if (yr.Substring(yr.Length - 2) == "04")
                            {
                                if (Constant.SunnyTable[3, 2] != "")
                                {
                                    weekCode = Constant.SunnyTable[3, 2].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[3, 0].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[3, 1].Substring(weekCode_weekNo, 1);
                                }
                                else
                                {
                                    customMarking = weekType;
                                }
                            }
                            else if (yr.Substring(yr.Length - 2) == "05")
                            {
                                if (Constant.SunnyTable[4, 2] != "")
                                {
                                    weekCode = Constant.SunnyTable[4, 2].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[4, 0].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[4, 1].Substring(weekCode_weekNo, 1);
                                }
                                else
                                {
                                    customMarking = weekType;
                                }
                            }
                            else if (yr.Substring(yr.Length - 2) == "06")
                            {
                                if (Constant.SunnyTable[5, 2] != "")
                                {
                                    weekCode = Constant.SunnyTable[5, 2].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[5, 0].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[5, 1].Substring(weekCode_weekNo, 1);
                                }
                                else
                                {
                                    customMarking = weekType;
                                }
                            }
                            else if (yr.Substring(yr.Length - 2) == "07")
                            {
                                if (Constant.SunnyTable[6, 2] != "")
                                {
                                    weekCode = Constant.SunnyTable[6, 2].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[6, 0].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[6, 1].Substring(weekCode_weekNo, 1);
                                }
                                else
                                {
                                    customMarking = weekType;
                                }
                            }
                            else
                            {
                                int sunnyRow = Convert.ToInt32(yr.Substring(yr.Length - 2)) - 1;

                                if (Constant.SunnyTable[sunnyRow, 2] != "")
                                {
                                    weekCode = Constant.SunnyTable[sunnyRow, 2].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[sunnyRow, 0].Substring(weekCode_weekNo, 1) + Constant.SunnyTable[sunnyRow, 1].Substring(weekCode_weekNo, 1);
                                }
                                else
                                {
                                    customMarking = weekType;
                                }
                            }
                        }
                        else if (weekTypeNo == 4)
                        {
                            weekCode = Constant.Mon_C1.Substring(weekCodeDate.Month - 1, 1) + weekCode_Year;
                        }
                        else if (weekTypeNo == 5)
                        {
                            weekCode = weekCode_Year +
                                Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1) +
                                Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                        }

                        break;

                    case "M":
                    case "D":
                        if (weekTypeNo == 1)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = "";

                            if (weekTypeSymbol.ToUpper() == "M")
                            {
                                month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);
                            }
                            else if (weekTypeSymbol.ToUpper() == "D")
                            {
                                month_Code = Constant.Mon_C2.Substring(weekCodeDate.Month - 1, 1);
                            }

                            if (inaCode.ToUpper().StartsWith("MAFA"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code;
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFH"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code;
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFB"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code;
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFD"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code;
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFE"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code;
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFF"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code;
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFG"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code;
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFM"))
                            {
                                weekCode = "M" + weekCode_Year + month_Code + day_Code;
                            }
                            else if (inaCode.ToUpper().StartsWith("MCA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MNA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MKA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MUA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MSA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MRA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MCB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MNB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MKB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MUB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MRB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MSB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAJ"))
                            {
                                weekCode = weekCode_Year + month_Code + day_Code + "A";
                            }
                            else
                            {
                                weekCode = weekCode_Year + month_Code + day_Code;
                            }
                        }
                        else if (weekTypeNo == 2)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            weekCode = "A" + weekCode_Year + month_Code + day_Code;
                        }
                        else if (weekTypeNo == 3)
                        {
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);
                            weekCode = weekCode_Year + month_Code + "E";
                        }
                        else if (weekTypeNo == 4)
                        {
                            if (inaCode.ToUpper().StartsWith("MCA"))
                            {
                                weekCode = "5" + weekCode_Year + weekCode_weekNoStr.ToString() +
                                    ConfigurationManager.AppSettings["factory"] + "A";
                            }
                            else if (inaCode.ToUpper().StartsWith("MCB"))
                            {
                                weekCode = "6" + weekCode_Year + weekCode_weekNoStr.ToString() +
                                    ConfigurationManager.AppSettings["factory"] + "A";
                            }
                        }
                        else if (weekTypeNo == 5)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            weekCode = "T" + weekCode_Year + month_Code + day_Code;
                        }
                        else if (weekTypeNo == 6)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            weekCode = "B" + weekCode_Year + month_Code + day_Code;
                        }
                        else if (weekTypeNo == 7)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            weekCode = "C" + weekCode_Year + month_Code + day_Code;
                        }
                        else if (weekTypeNo == 8)
                        {
                            string month_Code = Constant.Mon_C2.Substring(weekCodeDate.Month - 1, 1);
                            weekCode = weekCode_Year + month_Code;
                        }
                        else if (weekTypeNo == 9)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = "";

                            if (weekTypeSymbol.ToUpper() == "M")
                            {
                                month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);
                            }
                            else if (weekTypeSymbol.ToUpper() == "D")
                            {
                                month_Code = Constant.Mon_C2.Substring(weekCodeDate.Month - 1, 1);
                            }

                            if (weekTypeSymbol.ToUpper().Substring(3,1) != "K" &&
                                weekTypeSymbol.ToUpper().Substring(3, 1) != "J" &&
                                weekTypeSymbol.ToUpper().Substring(3, 1) != "L")
                            {
                                month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);
                            }

                            weekCode = "N" + weekCode_Year + month_Code + day_Code;
                        }
                        else if (weekTypeNo == 10)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            weekCode = "R" + weekCode_Year + month_Code + day_Code;
                        }
                        else if (weekTypeNo == 11)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            weekCode = "S" + weekCode_Year + month_Code + day_Code;
                        }
                        else if (weekTypeNo == 12)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            weekCode = "S" + weekCode_Year + month_Code + day_Code;
                        }
                        else if (weekTypeNo == 13)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            if (inaCode.ToUpper().StartsWith("MAFH"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code + ".";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFA"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code + ".";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFD"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code + ".";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFE"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code + ".";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFF"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code + ".";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFG"))
                            {
                                weekCode = "H" + weekCode_Year + month_Code + day_Code + ".";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAFM"))
                            {
                                weekCode = "M" + weekCode_Year + month_Code + day_Code + ".";
                            }
                            else if (inaCode.ToUpper().StartsWith("MCA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A.";
                            }
                            else if (inaCode.ToUpper().StartsWith("MKA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A.";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAA"))
                            {
                                weekCode = "5" + weekCode_Year + month_Code + day_Code + "A.";
                            }
                            else if (inaCode.ToUpper().StartsWith("MCB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A.";
                            }
                            else if (inaCode.ToUpper().StartsWith("MKB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A.";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAB"))
                            {
                                weekCode = "6" + weekCode_Year + month_Code + day_Code + "A.";
                            }
                            else if (inaCode.ToUpper().StartsWith("MAJ"))
                            {
                                weekCode = weekCode_Year + month_Code + day_Code + "A.";
                            }
                            else
                            {
                                weekCode = weekCode_Year + month_Code + day_Code + ".";
                            }
                        }
                        else if (weekTypeNo == 14)
                        {
                            string day_Code = Constant.Day_Table.Substring(weekCodeDate.Day - 1, 1);
                            string month_Code = Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);

                            weekCode = weekCode_Year + month_Code + day_Code;
                        }

                        break;

                    case "A":
                        if (weekType.ToUpper().Substring(0,4) == "A001" || weekType.ToUpper().Substring(0, 4) == "A002")
                        {
                            weekCode = weekCode_Year + Constant.Mon_Table.Substring(weekCodeDate.Month - 1, 1);
                        }

                        break;

                    case "B":
                        int yr_code = Convert.ToInt32(yr.Substring(yr.Length - 3));

                        if (weekType.ToUpper().Substring(0, 4) == "B001")
                        {
                            weekCode = Constant.tblx001.Substring(yr_code - 1, 1) +
                                Constant.Mon_Table.Substring(yr_code - 1, 1); ;
                        }
                        else if (weekType.ToUpper().Substring(0, 4) == "B201")
                        {
                            weekCode = Constant.tblx201.Substring(yr_code - 1, 1) +
                                Constant.Mon_Table.Substring(yr_code - 1, 1); ;
                        }
                        else if (weekType.ToUpper().Substring(0, 4) == "B301")
                        {
                            weekCode = Constant.tblx301.Substring(yr_code - 1, 1) +
                                Constant.Mon_Table.Substring(yr_code - 1, 1); ;
                        }
                        else if (weekType.ToUpper().Substring(0, 4) == "B401")
                        {
                            weekCode = Constant.tblx401.Substring(yr_code - 1, 1) +
                                Constant.Mon_Table.Substring(yr_code - 1, 1); ;
                        }

                        break;

                    default:
                        break;
                }

                markingData = new List<MarkingData>();
                markingData.Add(new MarkingData()
                {
                    RowNo = 0,
                    RowData = 0,
                    DataFormat = "",
                    Value = weekCode
                });

                if (!string.IsNullOrEmpty(customMarking))
                {
                    markingData.Add(new MarkingData()
                    {
                        RowNo = 0,
                        RowData = 0,
                        DataFormat = "",
                        Value = customMarking
                    });
                }
            }


            ret = SuccessFailure.Success;

            return ret;
        }

        private int GetFactoryCode()
        {
            string factory = ConfigurationManager.AppSettings["factory"];
            int factoryCode = 0;

            if(Int32.TryParse(factory, out factoryCode))
            {
                if (factoryCode == 2)
                {
                    factoryCode = 5;
                }
                else if (factoryCode == 9)
                {
                    factoryCode = 10;
                }
            }

            return factoryCode;
        }
    }
}