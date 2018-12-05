using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using culTAKU.Models;

namespace culTAKU.Models
{
    [Serializable()]
    public class AnimeEpisode : INotifyPropertyChanged
    {
        [field:NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;
        public enum STATUS { WATCHING, WATCHED, NEW, MISSING_LINK }
        private readonly Anime parentAnime;

        private TimeSpan stoppedAt;
        private STATUS status;

        public Anime ParentAnime { get { return parentAnime; } }
        public int EpisodeNumber { get; }
        public string EpisodePath { get; }
        public TimeSpan StoppedAt { get { return stoppedAt; } set { stoppedAt = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StoppedAt")); } }
        public STATUS Status { get { return status; } set { status = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status")); } }
        public bool IsPathExists { get { return Directory.Exists(EpisodePath); } }
        
        public AnimeEpisode(Anime parent_anime, int episodeNumber, string episodePath)
        {
            parentAnime = parent_anime;
            EpisodeNumber = episodeNumber;
            EpisodePath = episodePath;
            stoppedAt = new TimeSpan();
            status = STATUS.NEW;
        }
    }
}
