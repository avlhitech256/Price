using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Media.Image;
using Photo.Model;
using Photo.View;
using Photo.ViewModel;

namespace Photo.Service.Implementation
{
    public class PhotoService : IPhotoService
    {
        #region Constructors

        public PhotoService(IMessenger messenger, IImageService imageService)
        {

            Messenger = messenger;
            ImageService = imageService;
        }

        #endregion

        #region Properties

        private IImageService ImageService { get; }

        private IMessenger Messenger { get; }

        #endregion

        #region Methods
        public void ShowPhotos(IEnumerable<byte[]> photos)
        {
            ObservableCollection<BitmapSource> photoCollection = ImageService.Assemble(photos);
            var photoModel = new PhotoModel(photoCollection);
            var photoViewModel = new PhotoViewModel(Messenger, ImageService, photoModel);
            var photoView = new PhotoControl();
            var args = new ChildWindowEventArg(photoView, photoViewModel);
            Messenger.Send(CommandName.ShowImages, args);
        }

        #endregion
    }
}
