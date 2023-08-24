using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EpsonMarkingAPI.Common
{
    public static class Constant
    {
        //Day Symbol Table
        public const string Day_Table = "123456789ABCDEFGHJKLMNOPQRSTUVW";

        //Month Symbol Table
        public const string Mon_Table = "123456789XYZ";     //Standard   m
        public const string B_Table = "ABCDEFGHJKLM";

        //Year Symbol Table
        public const string Yrs_Table200 = "KABCDEFGHJ";
        public const string Yrs_Table300 = "ZMNRSTUVWX";
        public const string Yrs_Table400 = "Zmnrstuvwx";

        public const string Mon_C1 = "ABCDEFGHIJKL";        //Custom 1   m'
        public const string Mon_C2 = "ABCDEFGHJKLM";        //Custom 2   m''

        public const string tblx001 = "1234567890";
        public const string tblx201 = "ABCDEFGHJK";
        public const string tblx301 = "MNRSTUVWXZ";
        public const string tblx401 = "mnrstuvwxz";

        //array element start from 0
        private static readonly string[,] x = new string[25, 3];


        public static string[,] SunnyTable
        { 
            get
            {
                //Sunny Blank (C003)
                //1=Middle; 2=Bottom; 3=Top //‚PŒ…–Ú’† ‚QŒ…–Ú‰º ‚RŒ…–Úã
                //2010 ~ 2019
                x[9, 2] = "            ---------------------------------------- -";
                x[9, 0] = " -0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZkz-0123456789   ";
                x[9, 1] = " -------------------------------------------------- --";

                x[10, 2] = "                                        --------------";
                x[10, 0] = " ABCDEFGHIJKLMNOPQRSTUVWXYZkz-0123456789ABCDEFGHIJKLMN";
                x[10, 1] = "                                                      ";

                x[11, 2] = " -------------------------                            ";
                x[11, 0] = " OPQRSTUVWXYZkz-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZkz";
                x[11, 1] = "                          ----------------------------";

                x[12, 2] = "            ---------------------------------------- -";
                x[12, 0] = " -0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZkz-0123456789   ";
                x[12, 1] = " -------------------------------------------------- --";

                x[13, 2] = "                                        --------------";
                x[13, 0] = " ABCDEFGHIJKLMNOPQRSTUVWXYZkz-0123456789ABCDEFGHIJKLMN";
                x[13, 1] = "                                                      ";

                x[14, 2] = " -------------------------                            ";
                x[14, 0] = " OPQRSTUVWXYZkz-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZkz";
                x[14, 1] = "                          ----------------------------";

                x[15, 2] = "            ---------------------------------------- -";
                x[15, 0] = " -0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZkz-0123456789   ";
                x[15, 1] = " -------------------------------------------------- --";

                x[16, 2] = "                                        --------------";
                x[16, 0] = " ABCDEFGHIJKLMNOPQRSTUVWXYZkz-0123456789ABCDEFGHIJKLMN";
                x[16, 1] = "                                                      ";

                x[17, 2] = " -------------------------                            ";
                x[17, 0] = " OPQRSTUVWXYZkz-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZkz";
                x[17, 1] = "                          ----------------------------";

                x[18, 2] = "            ---------------------------------------- -";
                x[18, 0] = " -0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZkz-0123456789   ";
                x[18, 1] = " -------------------------------------------------- --";

                return x;
            }
        } 
    }
}