using Microsoft.Practices.Prism.Regions;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public interface INavigationItem
    {
        void NavigateTo();

        bool IsMe(IRegionNavigationJournalEntry entry);
    }
}