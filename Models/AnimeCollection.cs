using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace culTAKU.Models
{
    [Serializable()]
    public class AnimeCollection
    {
        public List<string> AllAnimePaths;
        private Random random;

        public ObservableCollection<Anime> ListOfAnime;
        public ObservableCollection<AnimeEpisode> ContinueWatching;

        public AnimeCollection()
        {
            AllAnimePaths = new List<string>();
            ListOfAnime = new ObservableCollection<Anime>();
            ContinueWatching = new ObservableCollection<AnimeEpisode>();
            random = new Random();
        }

        public void AddPath(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!Directory.Exists(Path.GetFullPath("/Image")))
            {
                Directory.CreateDirectory(Path.GetFullPath("/Image"));
            }

            //Unit Test This Later
            foreach(DirectoryInfo anime_dir in dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                long anime_id = (long)(random.NextDouble() * 1000000000);
                
                string anime_path;

                try
                {
                    anime_path = anime_dir.FullName;
                }
                catch (NotSupportedException e)
                {
                    continue;
                }
                Anime anime = new Anime(anime_id, anime_path);
                anime.ParseEpisodes();

                anime.FetchDetailsAsync();
                ListOfAnime.Add(anime);
            }

            AllAnimePaths.Add(path);
        }

        public void AddToContinueWatching(AnimeEpisode episode)
        {
            RemoveFromContinueWatching(episode);
            ContinueWatching.Insert(0, episode);
        }

        public void RemoveFromContinueWatching(AnimeEpisode episode)
        {
            for (int i = 0; i < ContinueWatching.Count; i++)
            {
                if (episode == ContinueWatching[i])
                {
                    ContinueWatching.RemoveAt(i);
                }
            }
        }
    }
}
