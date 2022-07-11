using AppleMusicParser.Models;
using Avalonia.Media.Imaging;
using BusinessLogic.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows.Input;

namespace AppleMusicParser.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Bitmap? _image;
        private string _name = string.Empty;
        public string _description = string.Empty;
        public string _imageUrl = string.Empty;

        private ObservableCollection<Song> _songs = new ObservableCollection<Song>();

        public Bitmap Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }
        public string ImageUrl
        {
            get => _imageUrl;
            set
            {

                this.RaiseAndSetIfChanged(ref _imageUrl, value);
                DownloadImage(_imageUrl);
            }

        }

        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set => _songs = this.RaiseAndSetIfChanged(ref _songs, value);
        }

        public MainWindowViewModel()
        {

            GetData = ReactiveCommand.Create((string url) =>
            {
                ClearView();
                try
                {
                    var result = MusicService.GetPlaylist(url);
                    if (result.IsSuccess)
                    {
                        ImageUrl = result.Data.ImageUrl;
                        Name = result.Data.Name;
                        Description = result.Data.Description!;
                        result.Data.Songs!.ForEach((s) =>
                        {
                            var res = new Song
                            {
                                Duration = s.Duration,
                                Name = s.Name,
                                Album = s.Album,
                                Artist = s.Artist

                            };
                            _songs.Add(res);
                        });

                    }
                }
                catch (Exception)
                {

                }

            });

        }
        public async void DownloadImage(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var res = await client.GetByteArrayAsync(new Uri(url));
                    using Stream stream = new MemoryStream(res);

                    Image = new Bitmap(stream);
                }
                catch (UriFormatException)
                {
                    Image = new Bitmap(@"..\..\..\Assets\image-not-found.png"); // change to reletive
                }
            }
        }
        public void ClearView()
        {
            Songs.Clear();
            Name = string.Empty;
            Description = string.Empty;
            ImageUrl = string.Empty;
        }
        public ICommand GetData { get; private set; }

    }
}
