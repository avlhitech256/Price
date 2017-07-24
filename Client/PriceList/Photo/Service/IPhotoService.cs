using System.Collections.Generic;

namespace Photo.Service
{
    public interface IPhotoService
    {
        void ShowPhotos(IEnumerable<byte[]> photos);
    }
}
