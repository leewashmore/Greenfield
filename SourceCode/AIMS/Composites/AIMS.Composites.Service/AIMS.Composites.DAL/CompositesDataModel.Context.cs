﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AIMS.Composites.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class AIMS_MainEntities : DbContext
    {
        public AIMS_MainEntities()
            : base("name=AIMS_MainEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<GF_COMPOSITE_LTHOLDINGS> GF_COMPOSITE_LTHOLDINGS { get; set; }
        public DbSet<GF_PORTFOLIO_HOLDINGS> GF_PORTFOLIO_HOLDINGS { get; set; }
        public DbSet<GF_PORTFOLIO_LTHOLDINGS> GF_PORTFOLIO_LTHOLDINGS { get; set; }
    
        public virtual ObjectResult<GetComposites_Result> GetComposites()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetComposites_Result>("GetComposites");
        }
    
        public virtual ObjectResult<GetCompositePortfolios_Result> GetCompositePortfolios(string compositeID)
        {
            var compositeIDParameter = compositeID != null ?
                new ObjectParameter("compositeID", compositeID) :
                new ObjectParameter("compositeID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCompositePortfolios_Result>("GetCompositePortfolios", compositeIDParameter);
        }
    }
}
