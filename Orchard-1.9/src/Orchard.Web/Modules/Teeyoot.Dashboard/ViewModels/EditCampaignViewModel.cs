using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.ViewModels
{
    public class EditCampaignViewModel
    {
        public bool IsError { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Alias { get; set; }

        public bool BackSideByDefault { get; set; }

        public string Tags { get; set; }

        public string FrontImagePath { get; set; }

        public string BackImagePath { get; set; }
    }
}