using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Data.Server;
using Aims.Core;

namespace GreenField.IssuerShares.Server
{
    public class Serializer
    {
        private Aims.Data.Server.Serializer serializer;

        [DebuggerStepThrough]
        public Serializer(Aims.Data.Server.Serializer serializer)
        {
            this.serializer = serializer;
        }

        public RootModel SerializeRoot(Core.RootModel model)
        {
            var result = new RootModel(
                this.serializer.SerializeIssuer(model.Issuer),
                this.SerializeItems(model.Items)
            );
            return result;
        }

        private IEnumerable<ItemModel> SerializeItems(IEnumerable<Core.ItemModel> models)
        {
            var result = models.Select(x => this.SerializeItem(x)).ToList();
            return result;
        }

        private ItemModel SerializeItem(Core.ItemModel model)
        {
            var result = new ItemModel(
                this.serializer.SerializeSecurityOnceResolved(model.Security),
                model.Preferred
            );
            return result;
        }

        public IEnumerable<IssuerSecurityShareRecordModel> SerializeShareRecords(IEnumerable<DataLoader.Core.IssuerSecurityShareRecord> records)
        {
            var result = records.Select(x => this.SerializeIssuerSecurityShareRecord(x)).ToList();
            return result;
        }

        private IssuerSecurityShareRecordModel SerializeIssuerSecurityShareRecord(DataLoader.Core.IssuerSecurityShareRecord x)
        {
            var result = new IssuerSecurityShareRecordModel
                            {
                                IssuerId = x.IssuerId,
                                SecurityId = x.SecurityId,
                                SecurityTicker = x.SecurityTicker,
                                Shares = this.SerializeIssuerShareRecords(x.Shares)
                            };

            return result;
        }

        private IEnumerable<IssuerShareRecordModel> SerializeIssuerShareRecords(IEnumerable<DataLoader.Core.IssuerShareRecord> records)
        {
            var result = records.Select(x => this.IssuerShareRecord(x)).ToList();
            return result;
        }

        private IssuerShareRecordModel IssuerShareRecord(DataLoader.Core.IssuerShareRecord x)
        {
            var result = new IssuerShareRecordModel
            {
                IssuerId = x.IssuerId,
                Date = x.Date,
                SecurityId = x.SecurityId,
                SharesOutstanding = x.SharesOutstanding
            };

            return result;
        }
    }
}
