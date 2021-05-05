using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLS_Video_Generater.Model
{
    public class MediaSegments
    {
        public int Id { get; set; }
        public string StreamId { get; set; }
        public string FileName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Duration { get; set; }
        public int Size { get; set; }
        public int ByteIndex { get; set; }
        public M3U8Info M3U8Info { get; set; }
    }
}
