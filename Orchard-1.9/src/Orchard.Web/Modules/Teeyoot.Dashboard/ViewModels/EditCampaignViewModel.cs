﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.ViewModels
{
    public class EditCampaignViewModel
    {
        public bool IsError { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public string Alias { get; set; }

        public bool BackSideByDefault { get; set; }

        public IEnumerable<TagViewModel> AllTags { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string TagsToSave { get; set; }

        //public string Tags { get; set; }

        public string FrontImagePath { get; set; }

        public string BackImagePath { get; set; }
    }
}