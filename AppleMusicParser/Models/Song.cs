using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleMusicParser.Models
{
    public class Song
    {
        public string Name { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
    }
}
