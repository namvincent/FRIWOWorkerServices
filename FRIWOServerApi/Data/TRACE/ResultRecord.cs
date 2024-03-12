using System;
using System.Collections.Generic;

namespace FRIWOServerApi.Data.TRACE
{
    
    public class ResultRecord
    {
        public DateTime DateTime { get; set; }

        public string[] DataField { get; set; }

        public ResultRecord(DateTime dateTime, string[] args)
        {
            DateTime = dateTime;
            DataField = args;
        }

        public List<ResultRecord> ListBuild(ResultRecord resultRecord, int panes)
        {
            List<ResultRecord> list = new();


            for (int i = 0; i < panes; i++)
            {
                list.Add(resultRecord);
            }
            return list;
        }
    }
}
