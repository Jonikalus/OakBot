using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OakBot
{
    public class Song
    {
        public string SongName { get; set; }
        public string Link { get; set; }
        public SongType Type { get; }

        public Song(string songname, string link)
        {
            SongName = songname;
            Link = link;
            if (Regex.IsMatch(Link, "soundcloud", RegexOptions.IgnoreCase))
            {
                Type = SongType.SOUNDCLOUD;
            }else
            {
                Type = SongType.YOUTUBE;
            }
        }
    }

    public enum SongType
    {
        YOUTUBE,
        SOUNDCLOUD
    }
}
