using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Toolkit.Uwp.UI.Controls;

namespace PacketMessagingTS.Helpers
{
    public class DataGridSortData
    {
        public static Dictionary<string, DataGridSortData> DataGridSortDataDictionary = new Dictionary<string, DataGridSortData>();

        public string PivotName
        { get; set; }

        public int SortColumnNumber
        { get; set; }

        public DataGridSortDirection? SortDirection
        { get; set; }

        public DataGridSortData(string pivotName, int sortColumn, DataGridSortDirection? sortDirection)
        {
            PivotName = pivotName;
            SortColumnNumber = sortColumn;
            SortDirection = sortDirection;
        }

        public async Task SaveDataGridSortSettingsAsync()
        {
            await SettingsStorageExtensions.SaveAsync(SharedData.SettingsContainer, "DataGridSortSettings", DataGridSortDataDictionary);
        }
    }
}
