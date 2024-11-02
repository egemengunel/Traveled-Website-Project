// Models/GroupedPlaceViewModel.cs

using System.Collections.Generic;

namespace Travel_App.Models
{
    public class GroupedPlaceViewModel
    {
        public string ?Continent { get; set; }
        public List<Place> ?Places { get; set; }
    }
}
