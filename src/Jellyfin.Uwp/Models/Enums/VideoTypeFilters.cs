using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models.Enums
{
    public enum VideoTypeFilters
    {
        [DisplayName("Blue-ray")]
        Bluray,
        [DisplayName("DVD")]
        Dvd,
        [DisplayName("HD")]
        HD,
        [DisplayName("4K")]
        FourK,
        [DisplayName("SD")]
        SD,
        [DisplayName("3D")]
        ThreeD
    }
}
