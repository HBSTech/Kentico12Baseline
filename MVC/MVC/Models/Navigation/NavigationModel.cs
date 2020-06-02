using System.Collections.Generic;
using Generic.Models;

namespace Checkpoint.Models
{
    internal class NavigationModel
    {
        public List<NavigationItem> NavItems { get; set; }
        public bool IsSignedIn { get; set; }
    }
}