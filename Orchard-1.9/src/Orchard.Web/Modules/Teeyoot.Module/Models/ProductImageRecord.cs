using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class ProductImageRecord
    {
        public virtual int Id { get; set; }
        public virtual int Width { get; set; }

        public virtual int Height { get; set; }

        public virtual int Ppi { get; set; }

        public virtual int PrintableFrontLeft { get; set; }

        public virtual int PrintableFrontTop { get; set; }
        public virtual int PrintableFrontWidth { get; set; }
        public virtual int PrintableFrontHeight { get; set; }

        public virtual int ChestLineFront { get; set; }
        public virtual int PrintableBackLeft { get; set; }
        public virtual int PrintableBackTop { get; set; }
        public virtual int PrintableBackWidth { get; set; }
        public virtual int PrintableBackHeight { get; set; }
        public virtual int ChestLineBack { get; set; }
        public virtual string Gender { get; set; }

        public virtual string ProdImgCulture { get; set; }
    }
}