using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingBaskets;
using System.Xml;
using TopDown.Core.ManagingTaxonomies;

namespace TopDown.Core.ManagingBpt.ChangingBt
{
    /// <summary>
    /// Saves any unsaved baskets and modified taxonomy accordingly. So that after saving there is no unsaved baskets.
    /// </summary>
    public class ChangesetApplier
    {
        private ModelToTaxonomyTransformer modelTransformer;
        private IDataManagerFactory managerFactory;

        [DebuggerStepThrough]
        public ChangesetApplier(
            IDataManagerFactory manageFactory,
            ModelToTaxonomyTransformer modelTransformer
        )
        {
            this.managerFactory = manageFactory;
            this.modelTransformer = modelTransformer;
        }

        public void Apply(
            Changeset changeset,
            SqlConnection connection,
            SqlTransaction transaction,
            RepositoryManager repositoryManager
        )
        {
            var manager = this.managerFactory.CreateDataManager(connection, transaction);
            var basketIdRange = manager.ReserveBasketIds(changeset.CountryBasketChanges.Count());
            var countryRepository = repositoryManager.ClaimCountryRepository(manager);
            var basketRepository = manager.GetAllCountryBaskets();

            foreach (var change in changeset.CountryBasketChanges)
            {
                int basketId;
                var basketInfo = basketRepository.Where(b => b.IsoCountryCode == change.UnsavedBasketCountry.Country.IsoCode).SingleOrDefault();
                if (basketInfo != null)
                {
                    basketId = basketInfo.Id;
                }
                else
                {
                    if (!basketIdRange.MoveNext()) throw new ApplicationException("There is no ID for a basket reserved.");
                    basketId = basketIdRange.Current;
                    this.ApplyCountryBasketChange(change, basketId, manager);
                }
                change.UnsavedBasketCountry.BasketId = basketId;


                var country = change.UnsavedBasketCountry.BasketCountry;
                country.Basket = new CountryBasket(basketId, change.UnsavedBasketCountry.Country);
                change.Other.BasketCountries.Add(country);
                change.Other.UnsavedBasketCountries.Remove(change.UnsavedBasketCountry);

            }

            if (changeset.TaxonomyChange != null)
            {
                ApplyTaxonomyChange(changeset, manager);
            }
        }

        private void ApplyTaxonomyChange(Changeset changeset, IDataManager manager)
        {
            var rootModel = changeset.TaxonomyChange.Model;
            var taxonomy = new TopDown.Core.ManagingTaxonomies.Taxonomy(changeset.TaxonomyChange.Taxonomy.Id);
            this.modelTransformer.Populate(taxonomy, rootModel.Globe);

            StringBuilder taxonomyXml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(taxonomyXml))
            {
                TaxonomyToXmlWriter tWriter = new TaxonomyToXmlWriter();
                tWriter.Write(taxonomy, writer);
            }
            manager.UpdateTaxonomy(taxonomyXml.ToString(), changeset.TaxonomyChange.Taxonomy.Id);
            changeset.TaxonomyChange.Taxonomy = taxonomy;
        }

        private void ApplyCountryBasketChange(CountryBasketChange change, int basketId, IDataManager manager)
        {
            manager.CreateBasketAsCountry(basketId);
            manager.CreateBasketCountry(basketId, change.UnsavedBasketCountry.Country.IsoCode);


        }

    }
}
