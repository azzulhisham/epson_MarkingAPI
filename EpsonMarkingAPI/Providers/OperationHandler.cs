using EpsonMarkingAPI.Models;
using EpsonMarkingAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using static EpsonMarkingAPI.Common.FunctionStatus;

namespace EpsonMarkingAPI.Providers
{
    /// <summary>
    /// Handling request operation
    /// </summary>
    public class OperationHandler : IOperationHandler
    {
        private IDictionary<string, IMaSgMarkingDataApi> _maSgMarkingDataApiInstance = new Dictionary<string, IMaSgMarkingDataApi>();
        private string _serviceName;

        /// <summary>
        /// Handler Constructor
        /// </summary>
        public OperationHandler(string operationKeyname)
        {
            _serviceName = operationKeyname;
            GetMaSgMarkingDataApiInstance(_maSgMarkingDataApiInstance);
        }

        private void GetMaSgMarkingDataApiInstance(IDictionary<string, IMaSgMarkingDataApi> handle)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in TryGetTypes(assembly))
                {
                    if (!type.IsClass || type.IsAbstract || !typeof(IMaSgMarkingDataApi).IsAssignableFrom(type)) continue;
                    IMaSgMarkingDataApi instance = GetInstance<IMaSgMarkingDataApi>(type);

                    handle.Add(type.FullName, instance);
                }
            }
        }

        private IEnumerable<Type> TryGetTypes(Assembly assembly)
        {
            Type[] result = new Type[0];
            if (assembly == null) return result;

            try
            {
                result = assembly.GetTypes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Attempt to retrieve types from assembly failed: " + assembly.FullName + ex.Message);
            }

            return result;
        }

        private TResult GetInstance<TResult>(Type componentType) where TResult : class
        {
            try
            {
                if (componentType == null) throw new ArgumentNullException("componentType");

                if (!typeof(TResult).IsAssignableFrom(componentType))
                    throw new ArgumentException("The requested componentType " + componentType.Name + " cannot be coverted to a " + typeof(TResult).Name + ".");

                TResult component = (TResult)Activator.CreateInstance(componentType);

                return component;

            }
            catch (Exception ex)
            {
                string er = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Handling Request
        /// </summary>
        /// <returns></returns>
        public ServiceReponseData RequestHandle(MarkingDataReq markingDataReq)
        {
            if (_maSgMarkingDataApiInstance == null || _maSgMarkingDataApiInstance.Count == 0)
            {
                return new ServiceReponseData()
                {
                    ResultCode = -1,
                    DataValue = "",
                    Description = "Internal Server Error - No service available."
                };
            }

            string markingData = "";
            IMaSgMarkingDataApi maSgMarkingDataApi = _maSgMarkingDataApiInstance[_serviceName];
            SuccessFailure result = maSgMarkingDataApi.GetMarkingData(markingDataReq.SpecNo, markingDataReq.QueryItem, ref markingData);

            if (result == SuccessFailure.Success)
            {
                return new ServiceReponseData()
                {
                    ResultCode = 0,
                    DataValue = markingData,
                    Description = ""
                };
            }
            else
            {
                return new ServiceReponseData()
                {
                    ResultCode = -1,
                    DataValue = "",
                    Description = "Internal Server Error - Error."
                };
            }
        }
    }
}