using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLoader.Core.BackendProd;
using System.Diagnostics;
using DataLoader.Core;

namespace GreenField.IssuerShares.Server
{
    public class IssueSharesLoaderExtension : IssuerSharesLoader
    {
        [DebuggerStepThrough]
        public IssueSharesLoaderExtension(
            Int32 numberOfDaysBefore,
            Monitor monitor,
            TargetDataPuller targetPuller,
            SourceDataPuller sourcePuller,
            IssuerSharesTransformer transformer,
            GapsFiller<IssuerShareRecord> filler,
            IssueSharesPusherExtension pusher,
            IIssuerSharesEraser eraser
            )
            : base(numberOfDaysBefore, monitor, targetPuller, sourcePuller, transformer, filler, pusher, eraser)
        {
        }

        public IssueSharesPusherExtension Pusher { get { return (IssueSharesPusherExtension)this.pusher; } }

        public IEnumerable<IssuerSecurityShareRecord> GetShareRecords(String issuerId, IEnumerable<GF_SECURITY_BASEVIEW> allSecurities)
        {
            var securities = this.monitor.MeasureAndFailIfAnything("Pulling the securities of the \"" + issuerId + "\" from the previously loaded security list.", delegate
            {
                return allSecurities.Where(x => x.ISSUER_ID == issuerId);
            });

            var issuerShareRecordsContext = this.monitor.MeasureAndFailIfAnything("Getting shares records for the \"" + issuerId + "\" issuer from the web-service.", delegate
            {
                return this.GetIssuerLevelShareRecords(null, securities);
            });

            var records = issuerShareRecordsContext.IssuerShareRecords
                .GroupBy(x => x.SecurityId)
                .Select(x => new IssuerSecurityShareRecord
                {
                    IssuerId = issuerId,
                    SecurityTicker = securities.Where(y => y.SECURITY_ID == x.Key).FirstOrDefault().TICKER,
                    SecurityId = x.Key,
                    Shares = x.Select(y => new IssuerShareRecord { Date = y.Date, SecurityId = y.SecurityId, SharesOutstanding = y.SharesOutstanding, IssuerId = issuerId })

                });

            var result = new List<IssuerSecurityShareRecord>();

            foreach (var record in records)
            {
                var recordsNoGaps = this.monitor.Scope("Filling the gaps if any.", delegate
                {
                    return this.filler.FillGaps(record.Shares);
                });
                result.Add(new IssuerSecurityShareRecord { IssuerId = record.IssuerId, SecurityId = record.SecurityId, Shares = recordsNoGaps, SecurityTicker = record.SecurityTicker });
            }
            return result;
        }

        public void ExecuteGetDataProc(String issuerId)
        {

        }
    }
}
