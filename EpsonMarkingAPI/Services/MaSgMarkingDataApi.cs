using EpsonMarkingAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using static EpsonMarkingAPI.Common.FunctionStatus;

namespace EpsonMarkingAPI.Services
{
    /// <summary>
    /// Generate Marking Data for MA/SG product
    /// </summary>
    public class MaSgMarkingDataApi : IMaSgMarkingDataApi
    {
        private string _dbPath = "";


        /// <summary>
        /// API Constructor
        /// </summary>
        public MaSgMarkingDataApi()
        {
            _dbPath = ConfigurationManager.AppSettings["masgMarkingDatabase"];
        }

        /// <summary>
        /// API Constructor
        /// </summary>
        /// <param name="dbPath"></param>
        public MaSgMarkingDataApi(string dbPath)
        {
            _dbPath = dbPath;
        }

        /// <summary>
        /// Get Marking Data Method
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
                            List<EpmDb002> epmdb002 = new List<EpmDb002>();

                            foreach (var item in selectedItems)
                            {
                                string[] itm = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                if (itm.Length == 4)
                                {
                                    epmdb002.Add(new EpmDb002()
                                    {
                                        SpecNo = itm[0].Trim(),
                                        GColumn = itm[1].Trim(),
                                        DataProperty = itm[2].Trim(),
                                        DataFormat = itm[3].Trim()
                                    });
                                }
                            }

                            List<MarkingData> markingData = new List<MarkingData>();
                            ret = FormMarkingData(epmdb002, specNo, qryItem, ref markingData);

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

        private SuccessFailure FormMarkingData(List<EpmDb002> epmdb002, string specNo, string qryItem, ref List<MarkingData> markingData)
        {
            SuccessFailure ret = SuccessFailure.error;
            string[] z12_Data = new string[9];
            string[] qryData = new string[] { };
            string freqVal = string.Empty;
            string weekCode = string.Empty;

            if (qryItem.Contains("|"))
            {
                qryData = qryItem.Split(new char[] { '|' });
                freqVal = qryData[0];
                weekCode = qryData[0];
            }
            else
            {
                freqVal = qryItem;
            }

            foreach (var item in epmdb002)
            {
                if (item.GColumn == "G1200")
                {
                    if (item.DataProperty == "Z1201")
                    {
                        z12_Data[0] = item.DataFormat;
                    }
                    else if (item.DataProperty == "Z1202")
                    {
                        if (item.DataFormat.StartsWith("#"))
                        {
                            z12_Data[1] = item.DataFormat.Substring(2);     //#1NDK-1 -> NDK-1
                        }
                        else
                        {
                            z12_Data[1] = item.DataFormat;
                        }
                    }
                    else if (item.DataProperty == "Z1203")
                    {
                        z12_Data[2] = item.DataFormat;
                    }
                    else if (item.DataProperty == "Z1251")
                    {
                        z12_Data[5] = item.DataFormat;
                    }
                    else if (item.DataProperty == "Z1252")
                    {
                        z12_Data[6] = item.DataFormat;
                    }
                    else if (item.DataProperty == "Z1253")
                    {
                        z12_Data[7] = item.DataFormat;
                    }
                    else if (item.DataProperty == "Z1254")
                    {
                        z12_Data[8] = item.DataFormat;
                    }
                    else if (item.DataProperty == "Z1255")
                    {
                        z12_Data[9] = item.DataFormat;
                    }
                }
            }

            markingData = new List<MarkingData>();

            try
            {
                foreach (var item in epmdb002.Where(n => n.DataProperty.StartsWith("F12")))
                {

                    if (!item.DataFormat.StartsWith("#"))
                    {
                        int rowNo = int.Parse(item.DataFormat.Substring(0, 2));
                        int rowData = int.Parse(item.DataFormat.Substring(2, 2));
                        string dataFormat = item.DataFormat.Substring(5);
                        string tmpData = "";

                        if (item.DataProperty == "F1201")
                        {
                            tmpData = z12_Data[0];
                        }
                        else if (item.DataProperty == "F1202")
                        {
                            tmpData = z12_Data[1];
                        }
                        else if (item.DataProperty == "F1203")
                        {
                            tmpData = z12_Data[2];
                        }
                        else if (item.DataProperty == "F1251")
                        {
                            tmpData = z12_Data[5];
                        }
                        else if (item.DataProperty == "F1252")
                        {
                            tmpData = z12_Data[6];
                        }
                        else if (item.DataProperty == "F1253")
                        {
                            tmpData = z12_Data[7];
                        }
                        else if (item.DataProperty == "F1254")
                        {
                            tmpData = z12_Data[8];
                        }
                        else if (item.DataProperty == "F1255")
                        {
                            tmpData = z12_Data[9];
                        }
                        else if (item.DataProperty == "F1205")          // IMI
                        {
                            if (specNo.StartsWith("H1012") || 
                                specNo.StartsWith("H2012") ||
                                specNo.StartsWith("H82"))
                            {
                                tmpData = specNo.Substring(specNo.Length - 2) + tmpData;
                            }
                            else if (specNo.StartsWith("4010") || 
                                        specNo.StartsWith("4030") ||
                                        specNo.StartsWith("GQ"))
                            {
                                tmpData = specNo.Substring(4, 3) + tmpData;
                            }
                            else if (specNo.StartsWith("GH"))
                            {
                                tmpData = specNo.Substring(4, 2) + tmpData;
                            }
                            else
                            {
                                tmpData = specNo.Substring(specNo.Length - 1);

                                if (tmpData == "1")
                                {
                                    tmpData = "AA";
                                }
                            }
                        }
                        else if (item.DataProperty == "F1206")          // freq
                        {
                            tmpData = freqVal;
                        }
                        else if (item.DataProperty == "F1207")          // Week code
                        {
                            if (qryData.Length > 1 && qryData[1].ToLower().Contains("wc:"))
                            {
                                tmpData = qryData[1].Replace("wc:", "");
                            }
                            else
                            {
                                tmpData = "";
                            }
                        }

                        markingData.Add(new MarkingData()
                        {
                            RowNo = rowNo,
                            RowData = rowData,
                            DataFormat = dataFormat,
                            Value = FormatData(tmpData, dataFormat)
                        });
                    }
                }

                ret = SuccessFailure.Success;
            }
            catch (Exception)
            {
            }

            return ret;
        }

        private string FormatData(string data, string format)
        {
            string ret = "";
            format = format.Trim().ToLower();

            if (!string.IsNullOrEmpty(data))
            {
                if (format.EndsWith("s"))            //string
                {
                    format = format.Replace("s", "");
                    format = format.Replace("%", "");
                    int replacement = int.Parse(format);

                    if (replacement < 0)
                    {
                        ret = data.PadRight(Math.Abs(replacement), ' ');
                    }
                    else
                    {
                        ret = data.PadLeft(Math.Abs(replacement), ' ');
                    }
                }
                else if (format.EndsWith("f"))       //float
                {
                    format = format.Replace("f", "");
                    format = format.Replace("%", "");

                    if (format.Contains("."))
                    {
                        string dataLength = format.Substring(0, format.IndexOf(".")).Trim();
                        string decimalPoint = format.Substring(format.IndexOf(".") + 1).Trim();
                        ret = string.Format("{0:F" + decimalPoint.ToString() + "}", Convert.ToDecimal(data));
                        ret = ret.PadLeft(int.Parse(dataLength), ' ');
                    }
                    else
                    {
                        int replacement = Convert.ToInt32(format);
                        ret = String.Format("{0:D1}", Convert.ToDecimal(data));
                        ret = ret.PadLeft(replacement, ' ');
                    }
                }
                else
                {
                    ret = "";
                }
            }

            return ret;
        }

    }
}