using EpsonMarkingAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static EpsonMarkingAPI.Common.FunctionStatus;

namespace EpsonMarkingAPI.Services
{
    /// <summary>
    /// Get Data from I.T. Server
    /// </summary>
    public class ExternalDataManagerApi
    {
        private string _connectionString;
        private string _dataTable;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalDataManagerApi()
        {
            this._connectionString = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dataTable"></param>
        public ExternalDataManagerApi(string connectionString, string dataTable)
        {
            this._connectionString = connectionString;
            this._dataTable = dataTable;
        }

        private string GetConnString()
        {
            string sConnStr = string.Empty;

            if (string.IsNullOrEmpty(this._connectionString))
            {
                sConnStr =
                    "Server=" + @"20.10.30.5\SQLEXPRESS" + "; " +
                    "DataBase=" + "Marking" + "; " +
                    "user id=" + "vb-sql" + ";" +
                    "password=" + "Anyn0m0us";
            }
            else
            {
                sConnStr = this._connectionString;
            }

            return sConnStr;
        }

        /// <summary>
        /// Get Taping Lot Form Data from I.T. database
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="tapingLotFormData"></param>
        /// <returns></returns>
        public Exception GetTapingLotFormData(string lotNo, ref List<TapingLotFormData> tapingLotFormData)
        {
            //SuccessFailure ret = SuccessFailure.error;
            //string qry = $"SELECT MarkingData, WeekCode FROM MarkingRecords WHERE LotNo='{lotNo}'";
            string erMsg = string.Empty;

            string qry = "SELECT DISTINCT i.[item_no] AS [SPEC], " +
                "[out_freq] AS [FREQ], " +
                "[cc_item] AS [INA_CODE], " +
                "[cc_lot_no] AS [OGLOTNO], " +
                "[ass_rmlot] AS [LOTNO], " +
                "[dly_marking] AS [WEEKCODE], " +
                "CAST([ass_qty] - (SELECT CASE WHEN sum(dd_qty) IS NULL THEN 0 ELSE sum(dd_qty) END FROM defect_det WHERE dd_lotno=ass_lotno AND dd_lotmat=ass_rmlot AND ass_edit=0) AS int) AS [USEDQTY], " +
                "[cc_product] " +
                $"FROM {this._dataTable} " +
                "JOIN [imi_mstr] AS i ON [cc_item]=[imi_no] " +
                "JOIN [assembly_det] ON [cc_lot_no]=[ass_lotno] " +
                "JOIN [dly_detail] ON [dly_lot_no]=[ass_rmlot] " +
                $"WHERE [ass_edit]=0 and dly_marking IS NOT NULL and [imi_edit]=0 and [imi_delete]=0 AND [cc_lot_no]='{lotNo}' ";


            using (SqlConnection dbConnection = new SqlConnection(GetConnString()))
            {
                try
                {
                    dbConnection.Open();
                    SqlCommand qryCmd = new SqlCommand(qry, dbConnection);
                    SqlDataReader reader = qryCmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        if (tapingLotFormData == null)
                        {
                            tapingLotFormData = new List<TapingLotFormData>();
                        }

                        while (reader.Read())
                        {
                            tapingLotFormData.Add(new TapingLotFormData()
                            {
                                B_SPEC = reader.GetString(0),
                                C_FREQ = reader.GetString(1),
                                D_INA_CODE = reader.GetString(2),
                                E_OGLOTNO = reader.GetString(3),
                                F_LOTNO = reader.GetString(4),
                                G_WKCODE = reader.GetString(5),
                                H_USEDQTY = reader.GetInt32(6),
                                A_PRODUCT = reader.GetString(7)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Get Taping Lot Form Data from I.T. database
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="tapingLotFormData"></param>
        /// <returns></returns>
        public Exception GetTapingLotFormDataPCS(string lotNo, string qry, ref List<TapingLotFormData> tapingLotFormData)
        {
            //SuccessFailure ret = SuccessFailure.error;
            //string qry = $"SELECT MarkingData, WeekCode FROM MarkingRecords WHERE LotNo='{lotNo}'";
            string erMsg = string.Empty;

            using (SqlConnection dbConnection = new SqlConnection(GetConnString()))
            {
                try
                {
                    dbConnection.Open();
                    SqlCommand qryCmd = new SqlCommand(qry, dbConnection);
                    SqlDataReader reader = qryCmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        if (tapingLotFormData == null)
                        {
                            tapingLotFormData = new List<TapingLotFormData>();
                        }

                        while (reader.Read())
                        {
                            tapingLotFormData.Add(new TapingLotFormData()
                            {
                                B_SPEC = reader.GetString(0),
                                F_LOTNO = reader.GetString(1),
                                C_FREQ = reader.GetString(2),
                                G_WKCODE = reader.GetString(3),
                                H_USEDQTY = reader.GetInt32(4),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            return null;
        }

    }
}