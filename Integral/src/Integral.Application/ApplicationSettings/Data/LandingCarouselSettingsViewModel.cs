using AutoMapper;
using Integral.Application.Common.Mappings;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Integral.Application.ApplicationSettings.Data
{
    public class LandingCarouselSettingsViewModel : IRequest, IMapFrom<LandingCarouselSettingsDTO>
    {
        //Just to make UI validation happy
        //jQuery Validator throws an error if page doesn't have ViewModel inputs (Items collection can be empty)
        //Even if it doesn't break anything, I will leave it here
        [IgnoreMap]
        public int Id { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();

        public class Item : IMapFrom<LandingCarouselSettingsDTO.Item>
        {
            [IgnoreMap]
            public IFormFile File { get; set; }

            public string Path { get; set; }

            public string DisplayName { get; set; }

            public string Text { get; set; }
        }
    }
}
