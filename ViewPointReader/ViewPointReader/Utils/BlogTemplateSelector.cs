using ViewPointReader.Core.Models;
using Xamarin.Forms;

namespace ViewPointReader.Utils
{
    public class BlogTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BlogWithImageTemplate { get; set; }
        public DataTemplate BlogWithoutImageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate (object item, BindableObject container)
        {
            return !string.IsNullOrEmpty(((FeedSubscription)item).ImageUrl) ? BlogWithImageTemplate : BlogWithoutImageTemplate;
        }
    }
}
