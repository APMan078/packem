using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class DashboardInventoryFlowGetModel
    {
        public class DataSet
        {
            public DataSet()
            {
                Data = new List<int>();
            }

            public string Label { get; set; }
            public IEnumerable<int> Data { get; set; }
        }

        public DashboardInventoryFlowGetModel()
        {
            DataSets = new List<DataSet>();
        }

        public IEnumerable<string> Labels { get; set; }
        public IEnumerable<DataSet> DataSets { get; set; }
    }
}