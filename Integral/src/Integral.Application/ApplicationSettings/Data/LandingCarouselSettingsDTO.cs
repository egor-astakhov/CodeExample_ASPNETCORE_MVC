using System.Collections.Generic;

namespace Integral.Application.ApplicationSettings.Data
{
    public class LandingCarouselSettingsDTO
    {
        public List<Item> Items { get; } = new List<Item>();

        public class Item
        {
            public string Path { get; set; }

            public string DisplayName { get; set; }

            public string Text { get; set; }
        }
    }
}
