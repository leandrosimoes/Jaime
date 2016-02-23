using Jaime.Extensions;

namespace Jaime.Models {
    public class FavoritoFirefoxModel {
        public string url { get; set; }
        public string guid { get; set; }
        public string title { get; set; }

        public string urlSemProtocolo {
            get { return url != null ? url.Replace("https://", "").Replace("http://", "") : url; }
        }
    }
}