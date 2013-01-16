using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using GreenField.IssuerShares.Client.Backend.IssuerShares;
using System.Collections.ObjectModel;
using System.Linq;

namespace GreenField.IssuerShares.Controls
{
    public static class IssuerSharesHelper
    {
        public static ObservableCollection<HistoryLineModel> MakeRectangleCollection(ObservableCollection<IssuerSecurityShareRecordModel> shares)
        {
            var names = shares.Select(x => x.SecurityId).ToArray();
            var columnCount = shares.Count();
            var dictionary = new Dictionary<DateTime, Dictionary<int?, Decimal?>>();
            int currentColumn = 0;
            foreach (var share in shares)
            {
                foreach (var shareRecord in share.Shares)
                {
                    Dictionary<int?, Decimal?> record;
                    if (!dictionary.ContainsKey(shareRecord.Date))
                    {
                        record = new Dictionary<int?, Decimal?>();
                        for (int i = 0; i < columnCount; i++)
                        {
                            record.Add(names[i], null);
                        }
                        record.Add(0, null);
                        dictionary.Add(shareRecord.Date, record);
                    }
                    else
                        record = dictionary[shareRecord.Date];
                    record[share.SecurityId] = shareRecord.SharesOutstanding;

                }

                currentColumn++;
            }

            var result = new List<HistoryLineModel>();
            foreach (var date in dictionary.Keys.OrderByDescending(x => x))
            {
                result.Add(new HistoryLineModel { Date = date, Values = dictionary[date] });
            }

            return Helper.ToObservableCollection<HistoryLineModel>(result);
        }

        
    }
}
