using System.Collections.Generic;

namespace Integral.Application.Landing.Data
{
    public class LandingViewModel
    {
        public LandingViewModel()
        {

        }

        public List<LandingProductViewModel> Products { get; set; } = new List<LandingProductViewModel>();
    }
}
