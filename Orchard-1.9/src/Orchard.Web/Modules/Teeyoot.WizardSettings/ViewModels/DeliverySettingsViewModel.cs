using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Orchard.Data;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class DeliverySettingsViewModel
    {
        public IEnumerable<DeliverySettingRecord> DeliverySettings { get; set; }

        public IRepository<CountryRecord> CountryRepository { get; set; }

        public int? CountryId { get; set; }

        public dynamic Pager { get; set; }

    }
}