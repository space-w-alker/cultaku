using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using culTAKU.Misc;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace culTAKU.Models
{
    [Serializable()]
    public class Anime : ObservableObject
    {
        
        public ObservableCollection<AnimeEpisode> ListOfEpisodes;
        public ObservableCollection<AnimeEpisode> ListOfUnOrderedEpisodes;

        private string anime_path;
        private const string base_html_string = "https://myanimelist.net/search/all?q=";

        private long id;
        private string name;
        private string synopsis;
        private int episodes;
        private string image_path;

        public long Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string Name { get { return name; } set
            {
                if(name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Synopsis
        {
            get { return synopsis; }
            set
            {
                if (synopsis != value)
                {
                    synopsis = value;
                    OnPropertyChanged("Synopsis");
                }
            }
        }

        public int Episodes
        {
            get { return episodes; }
            set
            {
                if (episodes != value)
                {
                    episodes = value;
                    OnPropertyChanged("Episodes");

                }
            }
        }

        public string ImagePath
        {
            get { return image_path; }
            set
            {
                if (image_path != value)
                {
                    image_path = value;
                    OnPropertyChanged("ImagePath");
                }
            }
        }

        public bool IsPathExists { get { return Directory.Exists(anime_path); } }
        public bool IsImagePathExists { get { return Directory.Exists(ImagePath); } }
        public bool IsKnownAnime { get; set; }

        public Anime(long id, string path)
        {
            Id = id;
            anime_path = path;
            name = new DirectoryInfo(anime_path).Name;
        }

        public void ParseEpisodes()
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

        public void FetchDetails()
        {
            Regex get_target_anime_url = new Regex("information(.|\n)*?href=\"(?<link>.*?)\"", RegexOptions.IgnoreCase);
            Regex get_anime_id = new Regex(@"\d+");
            Regex get_anime_image_url = new Regex("js-scrollfix-bottom\"(.|\n)*?<img src=\"(?<image_link>.*?)\"", RegexOptions.IgnoreCase);
            Regex get_anime_synopsis = new Regex("<span itemprop=\"description\">(?<synopsis>(.|\n)*?)</span>");

            string search_response = Miscelleneous.GetHtml(base_html_string + Name);
            string anime_url = get_target_anime_url.Match(search_response).Groups["link"].Value;
            int anime_id = Int32.Parse(get_anime_id.Match(anime_url).Value);
            string anime_page = Miscelleneous.GetHtml(anime_url);
            string synopsis = get_anime_synopsis.Match(anime_page).Groups["synopsis"].Value;
            Id = anime_id;
            Synopsis = synopsis;
        }
    }
}
