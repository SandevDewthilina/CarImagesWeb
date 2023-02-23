using System.IO;
using SixLabors.ImageSharp;

namespace CarImagesWeb.Helpers
{
    public class ImageThumbnail
    {
        public string FileName { get; set; }
        public Image File { get; set; }

        public Stream GetStream()
        {
            var stream = new MemoryStream();
            File.SaveAsJpeg(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}