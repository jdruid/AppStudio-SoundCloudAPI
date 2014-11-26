using System;
using System.Threading.Tasks;

using Windows.System;

namespace AppStudio.Services
{
    static public class NokiaMusicServices
    {
        /// <summary>
        /// Launches show artist task.
        /// </summary>
        /// <param name="artist">The artist name</param>
        static public async Task LaunchArtist(string artist)
        {
            string url = string.Format("nokia-music://show/artist/?name={0}", artist);
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        /// <summary>
        /// Launches play artist mix task.
        /// </summary>
        /// <param name="artist">The artist name</param>
        static public async Task PlayArtistMix(string artist)
        {
            string url = string.Format("nokia-music://play/artist/?artist={0}", artist);
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        /// <summary>
        /// Launches search music task.
        /// </summary>
        /// <param name="searchTerm">The search terms</param>
        static public async Task LaunchSearch(string searchTerm)
        {
            string url = string.Format("nokia-music://search/anything/?term={0}", searchTerm);
            await Launcher.LaunchUriAsync(new Uri(url));
        }
    }
}
