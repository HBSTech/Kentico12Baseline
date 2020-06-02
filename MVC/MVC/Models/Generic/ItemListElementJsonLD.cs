using Generic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Generic.Models
{
    public class ItemListElementJsonLD
    {
        [JsonProperty(PropertyName = "@type")]
        public string ContentType { get; set; } = "ListItem";
        [JsonProperty(PropertyName = "position")]
        public int position { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "item")]
        public string Item { get; set; }

        public ItemListElementJsonLD()
        {
            
        }

        public ItemListElementJsonLD(Breadcrumb BreadcrumbItem, int Position)
        {

            position = Position;
            Name = BreadcrumbItem.LinkText;
            if(!string.IsNullOrWhiteSpace(BreadcrumbItem.LinkUrl)) { 
                Item = new Uri(HttpContext.Current.Request.Url, BreadcrumbItem.LinkUrl).AbsoluteUri;
            }
        }
    }
}