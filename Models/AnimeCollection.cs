using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace culTAKU.Models
{
    public class AnimeCollection
    {
        private string all_anime_path;
        private Random random;

        public ObservableCollection<Anime> ListOfAnime;
        public ObservableCollection<AnimeEpisode> ContinueWatching;

        public AnimeCollection(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);


            //Unit Test This Later
            foreach(DirectoryInfo anime_dir in dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                long anime_id = (long)random.NextDouble() * 1000000000;
                string anime_path = anime_dir.FullName;

                Anime anime = new Anime(anime_id, anime_path);
                anime.ParseEpisodes();
                ListOfAnime.Add(anime);
            }
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
