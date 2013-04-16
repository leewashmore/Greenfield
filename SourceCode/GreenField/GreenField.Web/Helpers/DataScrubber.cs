using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DAL;
using System.Diagnostics;
namespace GreenField.Web.Helpers
{
    public class DataScrubber
    {
        public const string RANGE = "Range";
        public const string Multiplier = "Multiplier";

        private  List<DATA_MASTER> dataMaster;
        public List<DATA_MASTER> DataMaster
        {
            get
            {
                if (dataMaster == null)
                {
                    getDataMaster();
                }
                return dataMaster;
            }
            set
            {
                dataMaster = value;

            }
        }

        public DataScrubber()
        {
            getDataMaster();
        }
        private  void getDataMaster()
        {
            ExternalResearchEntities entity = new ExternalResearchEntities();
            dataMaster = entity.DATA_MASTER.ToList();
        }

        public  void DoScrubbing(List<SecurityDataIdScrub> scrubData, int? DataId, string type)
        {
            switch (type)
            {
                case RANGE:
                    doRangeScrubbing(scrubData, DataId);
            

                    break;
            }
           
        }

        private  void doRangeScrubbing(List<SecurityDataIdScrub> scrubData, int? DataId)
        {
            decimal? minvalue = (dataMaster.Where(a => a.DATA_ID == (int)DataId).Select(t => t.MinValue).FirstOrDefault() == null) ? null : dataMaster.Where(a => a.DATA_ID == (int)DataId).Select(t => t.MinValue).FirstOrDefault();
            decimal? maxvalue = (dataMaster.Where(a => a.DATA_ID == (int)DataId).Select(t => t.MaxValue).FirstOrDefault() == null) ? null : dataMaster.Where(a => a.DATA_ID == (int)DataId).Select(t => t.MaxValue).FirstOrDefault();
            List<SecurityDataIdScrub> scrubDataFortheDataId = scrubData.Where(a => a.DataId == DataId).ToList();
            foreach (var data in scrubDataFortheDataId)
            {

                if (data.OriginalValue != null)
                {
                    if (minvalue != null && data.OriginalValue < minvalue)
                    {
                          data.ScrubbedValue= minvalue;
                        
                    }
                    else if(maxvalue != null && data.OriginalValue > maxvalue)
                    {
                        data.ScrubbedValue = maxvalue;
                    }

                }
            }
        }

    }
}