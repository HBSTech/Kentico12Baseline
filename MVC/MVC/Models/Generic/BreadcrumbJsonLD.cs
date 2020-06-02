using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Generic.Models
{
    public class BreadcrumbJsonLD
    {
        [JsonProperty(PropertyName = "@context")]
        public string Context { get; set; } = "https://schema.org";
        [JsonProperty(PropertyName = "@type")]
        public string ContentType { get; set; } = "BreadcrumbList";
        [JsonProperty(PropertyName = "itemListElement")]
        public List<ItemListElementJsonLD> itemListElement { get; set; }
        
        [JsonIgnore]
        public string JsonData { get; set; }

        public BreadcrumbJsonLD()
        {

        }

        public BreadcrumbJsonLD(IEnumerable<Breadcrumb> Breadcrumbs, bool ExcludeFirst = false)
        {
            itemListElement = new List<ItemListElementJsonLD>();
            int Position = 0;
            foreach(Breadcrumb breadcrumb in (ExcludeFirst ? Breadcrumbs.Skip(1) : Breadcrumbs))
            {
                Position++;
                itemListElement.Add(new ItemListElementJsonLD(breadcrumb, Position));
            }
        }
    }
}