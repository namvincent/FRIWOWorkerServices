using FRIWOServerApi.Data.StaticObjects;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using FRIWOServerApi.Data.TRACE;

#nullable enable

namespace FRIWOServerApi.Model
{
    public class Unit
    {
        public string? ID { get; set; }
        public bool IsChecked { get; set; }

        public bool IsTested { get; set; }

        public bool ProcLockResult { get => procLockResult; set => procLockResult = value; }
        string? partNoBB;

        public StationState ResultTest => TestResult.Result ? StationState.PASS : StationState.FAIL;

        public string DataTest => TestResult.Data;

        public string? ExternalBarcode => LinkInfo.ExternalCode;
        public string? InternalBarcode => LinkInfo.InternalCode;
        public LinkInfo LinkInfo { get; set; }

        string? partNoMI;
        string? orderNoBB;
        string? orderNoMI;
        string? revision;
        public TestResult TestResult { get; set; }
        public string? PartNoMI { get => partNoMI; set => partNoMI = value; }
        public string? OrderNoBB { get => orderNoBB; set => orderNoBB = value; }
        public string? OrderNoMI { get => orderNoMI; set => orderNoMI = value; }
        public string? Revision { get => revision; set => revision = value; }
        public string? PartNoBB { get => partNoBB; set => partNoBB = value; }

        public DateTime TestTime { get => testTime; set => testTime = value; }

        public delegate void ChangeEventHandler(int numberArg);
        public event ChangeEventHandler? OnValueChanged;

        List<Station> routing = new();
        List<StationResult> processLockDetail = new();
        private bool procLockResult;
        private DateTime testTime;

        public Unit()
        {
            OrderNoBB = "";
            OrderNoMI = "";
            PartNoBB = "";
            PartNoMI = "";
            ProcLockResult = false;
            IsChecked = false;
            IsTested = false;
            processLockDetail = new();
            TestResult = new();
            LinkInfo = new LinkInfo();
        }

        public string GetBarcode()
        {
            if (LinkInfo.InternalCode != null)
                return LinkInfo.InternalCode;
            else
            {
                throw new Exception();
            }
        }
        public void SetBarcode(string id)
        {
            Initialize(id);
        }
        public string GetOrderNoBB()
        {
            OrderNoBB = GetBarcode().Split("-")[2].ToString();
            return OrderNoBB;
        }

        public string GetPartNoBB()
        {
            PartNoBB = GetBarcode().Split("-")[0].ToString();
            Revision = GetBarcode().Split("-")[3].ToString();
            return PartNoBB;
        }

        public void GetRouting()
        {

        }

        public bool GetProcessLockResult()
        {
            return procLockResult;
        }

        public void ProcessLockChecking(bool res)
        {
            IsChecked = true;
            procLockResult = res;
        }

        public bool DeserializeJSONtoUnit(string json)
        {
            if (json == "")
                return false;
            var Object = JObject.Parse(json);

            if (ID == "" || Object.Count == 0)
                return false;


            Dictionary<Station, StationState> ResultDictionary = new();
            var childObjs = Object.First?.Children<JObject>();
            foreach (var item in childObjs?.Properties())
            {
                Debug.WriteLine(item.Value.ToString());
                StationResult stationResult = new();
                stationResult.Result = item.Value.ToString().Trim() == "true" ? 1 : 0;
                stationResult.Station = item.Name.ToString();
                processLockDetail.Add(stationResult);
            }

            OnValueChanged?.DynamicInvoke();
            return true;
        }

        public void Initialize(string barcode)
        {
            ID = barcode;

            LinkInfo.ExternalCode = ID;
        }


    }

    public partial class StationResult
    {
        public string Station { get; set; } = "";
        public int Result { get; set; } = -1;

        public StationResult()
        {

        }
    }

    public enum StationState : int
    {
        FAIL,
        PASS,
        MISSING = -1
    }

    public class ResultColor
    {
        public static Color PASS = Color.YellowGreen;
        public static Color FAIL = Color.Red;
        public static Color MISSING = Color.Orange;
    }

    public partial class RoutingResults
    {

        public int RoutingState { get; set; }
        public StationResult[] StationResults { get; set; }
        public RoutingResults()
        {
            RoutingState = -1;
            StationResults = new StationResult[0];
        }

    }

    public enum RoutingState : int
    {
        FAIL,
        PASS,
        ERROR = -1
    }

    //public class BooleanRequiredAttribute : ValidationAttribute, IClientValidatable
    //{
    //    public override bool IsValid(object value)
    //    {
    //        return value != null && (bool)value == true;
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        //return new ModelClientValidationRule[] { new ModelClientValidationRule() { ValidationType = "booleanrequired", ErrorMessage = this.ErrorMessage } };
    //        yield return new ModelClientValidationRule()
    //        {
    //            ValidationType = "booleanrequired",
    //            ErrorMessage = this.ErrorMessageString
    //        };
    //    }
    //}
}
