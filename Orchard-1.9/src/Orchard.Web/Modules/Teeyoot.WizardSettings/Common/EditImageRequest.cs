namespace Teeyoot.WizardSettings.Common
{
    public class EditImageRequest
    {
        public int ProductId { get; set; }
        public int Ppi { get; set; }
        public int PrintableFrontLeft { get; set; }
        public int PrintableFrontTop { get; set; }
        public int PrintableFrontWidth { get; set; }
        public int PrintableFrontHeight { get; set; }
        public int ChestLineFront { get; set; }
        public int PrintableBackLeft { get; set; }
        public int PrintableBackTop { get; set; }
        public int PrintableBackWidth { get; set; }
        public int PrintableBackHeight { get; set; }
        public int ChestLineBack { get; set; }
    }
}