using System;
using System.Threading.Tasks;

using Windows.System;
using Windows.Devices.Geolocation;

namespace AppStudio.Services
{
    static public class NokiaMapsServices
    {
        static public async Task MapPosition(string address)
        {
            string url = string.Format("bingmaps:?where=={0}", Uri.EscapeDataString(address));
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        static public async Task HowToGet(string address)
        {
            string posFormat = string.Empty;
            Geolocator locator = new Geolocator();
            var position = await locator.GetGeopositionAsync();
            if (position != null)
            {
                posFormat = string.Format("{0}_{1}", position.Coordinate.Point.Position.Latitude, position.Coordinate.Point.Position.Longitude);
            }
            string url = string.Format("bingmaps:?rtp=adr.{0} pos.{1}", address, posFormat);
            await Launcher.LaunchUriAsync(new Uri(url));
        }
    }
}
