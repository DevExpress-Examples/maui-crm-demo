using System.ComponentModel.DataAnnotations.Schema;

namespace CrmDemo.DataModel.Models;

public class CrmImage {
    public int Id { get; set; }
    public virtual ImageData FullImage { get; set; }
    public virtual ImageData ThumbnailImage { get; set; }
    [NotMapped]
    public string FullImageCachePath { get; set; }
    [NotMapped]
    public string ThumbnailImageCachePath { get; set; }

    [NotMapped] private ImageSource thumbnailImageSource;
    [NotMapped] public ImageSource ThumbnailImageSource {
        get {
            if (thumbnailImageSource != null || ThumbnailImage.Data == null || ThumbnailImage.Data.Length == 0)
                return thumbnailImageSource;

            thumbnailImageSource = ImageSource.FromStream(() => new MemoryStream(ThumbnailImage.Data));
            return thumbnailImageSource;
        }
    }

    public void ClearCache() {
        thumbnailImageSource = null;
    }
}

public class ImageData {
    public int Id { get; set; }
    public byte[] Data { get; set; }
}
