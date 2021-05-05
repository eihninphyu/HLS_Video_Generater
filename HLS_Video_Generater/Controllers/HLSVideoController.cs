using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using HLS_Video_Generater.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HLS_Video_Generater.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HLSVideoController : ControllerBase
    {
        private static string exefilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffmpeg.exe");
        private static string inputPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "bunny.mp4");
        private static string outputPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output.m3u8");
        private static string outputM3U8Path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "hlsVideoOutput.m3u8");
        private GeneratorDBContext _db;
        public HLSVideoController(GeneratorDBContext db)
        {
            _db = db;
        }
        // GET: api/<HLSVideoController>
        [HttpGet]
        public string Get()
        {
            //CreateM3U8();
            return "OK";
        }

        [HttpGet,Route("GetM3U8")]
        public FileContentResult GetM3U8([FromQuery]DateTime startTime,[FromQuery]DateTime endTime)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = HttpStatusCode.NoContent;
            byte[] data=new byte[42000000];
            List<MediaSegments> mediaSegments = new List<MediaSegments>();
            if (endTime > startTime) {
                mediaSegments = _db.MediaSegments.Where(m => (m.StartTime >= startTime) && (m.StartTime < endTime)).Select(x=>x).ToList();
                var sortedList = mediaSegments.OrderBy(m => m.StartTime).ToList();
                var str = string.Empty;
                using (StreamWriter sw = new StreamWriter(outputM3U8Path))
                {
                    sw.WriteLine("#EXTM3U\n#EXT-X-VERSION:4\n#EXT-X-TARGETDURATION:8\n#EXT-X-MEDIA-SEQUENCE:0\n#EXT-X-PLAYLIST-TYPE:VOD");
                    foreach(var segment in sortedList)
                    {
                        sw.WriteLine("#EXTINF:"+segment.Duration);
                        sw.WriteLine("#EXT-X-BYTERANGE:"+segment.Size+"@"+segment.ByteIndex);
                        sw.WriteLine(segment.FileName + ".ts");
                    }
                    sw.Write("#EXT-X-ENDLIST");
                }
                
                using (StreamReader sr = new StreamReader(outputM3U8Path))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        sr.BaseStream.CopyTo(ms);
                        data = ms.ToArray();
                    }
                }
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
            }
            return File(data, "application/octet-stream", "OutputHLSVideo.m3u8");
        }
        public void CreateM3U8()
        {
            try
            {
                //Process process;
                //var cmd = "-i " + inputPath + " -c copy -bsf:v h264_mp4toannexb -hls_time 5 -hls_flags single_file -hls_list_size 0 -hls_playlist_type vod " + outputPath;
                var cmd = "-i " + inputPath + " -c copy -hls_time 5 -hls_flags single_file -hls_list_size 0 -hls_playlist_type vod " + outputPath;
                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = false;
                    p.StartInfo.RedirectStandardOutput = false;
                    p.StartInfo.FileName = exefilePath;
                    p.StartInfo.Arguments = cmd;
                    p.Start();
                    p.WaitForExit();
                }
                ReadM3U8File();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        public void ReadM3U8File()
        {
            try
            {               
                DateTime createdTime = new FileInfo(outputPath).CreationTime;
                List<MediaSegments> segments = new List<MediaSegments>();
                M3U8Info m3U8Info = new M3U8Info();
                using (StreamReader sr = new StreamReader(outputPath))
                {                    
                    string line;                    
                    MediaSegments segment = new MediaSegments();
                    segment.StartTime = createdTime;                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        var val = line.Split(":");
                        if (val.Length > 1)
                        {
                            var key = val[0];
                            var value = val[1];
                            
                            switch (key)
                            {
                                case "#EXT-X-VERSION":m3U8Info.Version = int.Parse(value);break;
                                case "#EXT-X-TARGETDURATION": m3U8Info.Version = int.Parse(value); break;
                                case "#EXT-X-MEDIA-SEQUENCE": m3U8Info.MediaSequence = int.Parse(value); break;
                                case "#EXT-X-PLAYLIST-TYPE": m3U8Info.PlaylistType = value; break;
                                case "#EXTINF": {
                                        value = value.Remove(value.Length - 1, 1);
                                        var duration = Convert.ToDouble(value);
                                        segment.Duration = duration;
                                        segment.EndTime = segment.StartTime.AddSeconds(duration);
                                        createdTime = segment.EndTime;
                                    }; break;
                                case "#EXT-X-BYTERANGE":
                                    {
                                        var byterange = value.Split("@");
                                        segment.Size = int.Parse(byterange[0]);
                                        segment.ByteIndex = int.Parse(byterange[1]);
                                    };break;
                                case "#EXT-X-DISCONTINUITY":break;
                                default: break;
                            }
                        }
                        if (val.Length == 1)
                        {
                            if (line.Contains(".ts"))
                            {
                                segment.FileName = line.Split(".")[0];
                                segment.StreamId = "stream";
                                segments.Add(segment);
                               // _db.MediaSegments.Add(segment);
                                segment = new MediaSegments();
                                segment.StartTime = createdTime;
                            }
                        }
                        // Use a tab to indent each line of the file.
                        Debug.WriteLine("\t" + line);
                    }
                }
                _db.M3U8Info.Add(m3U8Info);
                _db.MediaSegments.AddRange(segments);
                _db.SaveChanges();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        // POST api/<HLSVideoController>
        [HttpPost]
        public void Post()
        {
            CreateM3U8();
        }

        // PUT api/<HLSVideoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HLSVideoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
    public class HLSDataVM
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }
}
