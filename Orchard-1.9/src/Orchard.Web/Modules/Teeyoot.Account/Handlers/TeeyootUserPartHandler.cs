using JetBrains.Annotations;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Teeyoot.Module.Models;

namespace Teeyoot.Account.Handlers
{
    [UsedImplicitly]
    public class TeeyootUserPartHandler : ContentHandler
    {
        public TeeyootUserPartHandler(IRepository<TeeyootUserPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}