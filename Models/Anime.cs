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
using System.Threading;

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
        private long animeListId;
        private string name;
        private string synopsis;
        private int episodes;
        private string image_path;
        private float rating;

        public bool ImageFetched { get; set; }
        public bool DetailsFetched { get; set; }

        private AnimeEpisode lastPlayed;

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

        public long AnimeListId
        {
            get { return animeListId; }
            set
            {
                if (animeListId != value)
                {
                    animeListId = value;
                    OnPropertyChanged("AnimeListId");
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


        public float Rating
        {
            get { return rating; }
            set
            {
                if(rating != value)
                {
                    rating = value;
                    OnPropertyChanged("Rating");
                }
            }
        }

        public AnimeEpisode LastPlayed
        {
            get { return lastPlayed; }
            set { lastPlayed = value; OnPropertyChanged("LastPlayed"); }
        }

        public bool IsPathExists { get { return Directory.Exists(anime_path); } }
        public bool IsImagePathExists { get { return File.Exists(ImagePath); } }
        public bool IsKnownAnime { get; set; }

        public Anime() { }

        public Anime(long id, string path)
        {
            Id = id;
            anime_path = path;
            name = new DirectoryInfo(anime_path).Name;
            rating = 0;
            ListOfEpisodes = new ObservableCollection<AnimeEpisode>();
            ListOfUnOrderedEpisodes = new ObservableCollection<AnimeEpisode>();
            ImagePath = "Icons/unknown.png";
            ImageFetched = false;
            DetailsFetched = false;
        }

        public void ParseEpisodes()
        {
            List<Regex> checks = new List<Regex> {
                new Regex(@"episode.*?(?<EpisodeNumber>\d+).*?((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$", RegexOptions.IgnoreCase),
                new Regex(@"((eps)|(ep))( |-|_)*(?<EpisodeNumber>\d+).*?((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$", RegexOptions.IgnoreCase),
                new Regex(@"(?<EpisodeNumber>\d+)((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$",RegexOptions.IgnoreCase),
                new Regex(@"(?<EpisodeNumber>\d+).*?((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$",RegexOptions.IgnoreCase)
            };
            Regex SearchIsVideoFile = new Regex(@".*?((\.avi)|(\.mp4)|(\.flv)|(\.mkv))$", RegexOptions.IgnoreCase);

            

            foreach (FileInfo file in new DirectoryInfo(anime_path).GetFiles("*", SearchOption.TopDirectoryOnly))
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
                if (num == -1) continue;

                int index = 0;


                foreach(AnimeEpisode episode in ListOfEpisodes)
                {
                    if (num < episode.EpisodeNumber)
                    {
                        break;
                    }
                    index += 1;
                }
                ListOfEpisodes.Insert(index, new AnimeEpisode(this, num, file.FullName));

                
            }
        }

        public void FetchDetails()
        {
            Regex get_target_anime_url = new Regex("information(.|\n)*?href=\"(?<link>.*?)\"", RegexOptions.IgnoreCase);
            Regex get_anime_id = new Regex(@"\d+");
            Regex get_anime_image_url = new Regex("js-scrollfix-bottom\"(.|\n)*?<img src=\"(?<image_link>.*?)\"", RegexOptions.IgnoreCase);
            Regex get_anime_synopsis = new Regex("<span itemprop=\"description\">(?<synopsis>(.|\n)*?)</span>");
            Regex get_anime_rating = new Regex(@"fl-l score(.|\n)*?(?<rating>\d[.]\d+)");
            Regex get_anime_episodes_count = new Regex(@">Episodes:</span>(\s|\n)*?(?<episodes>\d+)");

            string search_response = Miscelleneous.GetHtml(base_html_string + Name, 10);
            if (search_response == null) { return; }


            string anime_url = get_target_anime_url.Match(search_response).Groups["link"].Value;
            int anime_id = Int32.Parse(get_anime_id.Match(anime_url).Value);

            string anime_page = Miscelleneous.GetHtml(anime_url, 10);
            if(String.IsNullOrWhiteSpace(anime_page)) { return; }

            string _synopsis = get_anime_synopsis.Match(anime_page).Groups["synopsis"].Value;

            float _rating = 0;
            try
            {
                _rating = float.Parse(get_anime_rating.Match(anime_page).Groups["rating"].Value);
            }
            catch(Exception e)
            {
                _rating = 0;
            }
            string image_url = get_anime_image_url.Match(anime_page).Groups["image_link"].Value;

            int _episodes = 0;
            try
            {
                _episodes = int.Parse(get_anime_episodes_count.Match(anime_page).Groups["episodes"].Value);
            }
            catch(Exception e) {

            }


            AnimeListId = anime_id;
            Synopsis = _synopsis;
            Rating = _rating;
            DetailsFetched = true;

            ImagePath = Miscelleneous.GetImage(image_url, anime_id, 10, false);
            ImageFetched = ImagePath != null;
        }

        public void FetchDetailsAsync()
        {
            ThreadPool.QueueUserWorkItem(_=> {
                FetchDetails();
            });
        }

    }
}
