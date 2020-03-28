using System.Collections.Generic;
using Soccer.Common.Models;

namespace Soccer.Prism.Helpers
{
    public static class CombosHelper
    {
        public static List<Sex> GetSexs()
        {
            return new List<Sex>
            {
                new Sex { Id = 1, Name = "Hombre" },
                new Sex { Id = 2, Name = "Mujer" }
            };
        }
    }
}