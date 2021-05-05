using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLS_Video_Generater.Model
{
    public class M3U8Info
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public int MediaSequence { get; set; }
        public string PlaylistType { get; set; }
        public List<MediaSegments> MediaSegments { get; set; }
    }
}
