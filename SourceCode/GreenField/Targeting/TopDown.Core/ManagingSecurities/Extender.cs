using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingSecurities
{
    public static class Extender
    {
        public static Fund TryAsFund(this ISecurity security)
        {
            var resolver = new TryAsFund_ISecurityResolver();
            security.Accept(resolver);
            return resolver.FundOpt;
        }
        public static Fund AsFund(this ISecurity security)
        {
            var fund = security.TryAsFund();
            if (fund == null) throw new ApplicationException("Given security (" + security + ") cannot be turned into a fund.");
            return fund;
        }

        public static CompanySecurity TryAsCompanySecurity(this ISecurity security)
        {
            var resolver = new TryAsFund_ISecurityResolver();
            security.Accept(resolver);
            return resolver.StockOpt;
        }
        public static CompanySecurity AsCompanySecurity(this ISecurity security)
        {
            var companySecurity = security.TryAsCompanySecurity();
            if (companySecurity == null) throw new ApplicationException("Give security (" + security + ") cannot be turned into a company security.");
            return companySecurity;
        }

        private class TryAsFund_ISecurityResolver : ISecurityResolver
        {
            public CompanySecurity StockOpt { get; private set; }
            public void Resolve(CompanySecurity stock)
            {
                this.StockOpt = stock;
            }

            public Fund FundOpt { get; private set; }
            public void Resolve(Fund fund)
            {
                this.FundOpt = fund;
            }
        }
    }
}
