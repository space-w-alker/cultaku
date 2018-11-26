using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace culTAKU.Models
{
    [Serializable()]
    class Anime : INotifyPropertyChanged
    {
        [field:NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<AnimeEpisode> ListOfEpisodes;
        public ObservableCollection<AnimeEpisode> ListOfUnOrderedEpisodes;

        private string anime_path;

        public string Name { get; set; }
        public string Synopsis { get; set; }
        public string Year { get; set; }
        public string ImagePath { get; set; }
        public List<string> GenreList { get; set; }
        public long Id { get; set; }
        public long Episodes { get; set; }
        public float Rating { get; set; }
        
        public bool IsPathExists { get { return Directory.Exists(anime_path); } }
        public bool IsImagePathExists { get { return Directory.Exists(ImagePath); } }
        public bool IsKnownAnime { get; set; }

        public Anime(long id, string path)
        {
            Id = id;
            anime_path = path;
        }

        public void parseEpisodes()
        {
            List<Regex> checks = new List<Regex> {
                new Regex(@"episode.*?(?<EpisodeNumber>\d+).*?((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$", RegexOptions.IgnoreCase),
                new Regex(@"((e)|(eps)|(ep))( |-|_)*(?<EpisodeNumber>\d+).*?((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$", RegexOptions.IgnoreCase),
                new Regex(@"(?<EpisodeNumber>\d+)((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$",RegexOptions.IgnoreCase),
                new Regex(@"(?<EpisodeNumber>\d+).*?((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$",RegexOptions.IgnoreCase)
            };
            Regex SearchIsVideoFile = new Regex(@".*?((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$", RegexOptions.IgnoreCase);

            foreach (FileInfo file in new DirectoryInfo(anime_path).GetFiles())
            {
                int num = -1;
                foreach(Regex check in checks)
                {
                    if (check.IsMatch(file.Name))
                    {
                        num = Convert.ToInt32(check.Match(file.Name).Groups["EpisodeNumber"].Value);
                        break;
                    }
                }

                ListOfUnOrderedEpisodes.Add(new AnimeEpisode(num, file.FullName));
            }
        }
    }
}
