using BusinessLogic.Enums;
using BusinessLogic.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace BusinessLogic.Services
{
    public static class MusicService
    {
        public static Result<Playlist> GetPlaylist(string url)
        {
            try
            {
                var html = Load(url);

                if (html == null)
                {
                    return Result<Playlist>.Error("Incorrect Url or html document");
                }
                var playlist = new Playlist();

                playlist.ImageUrl = html.DocumentNode.SelectSingleNode("//*[@class=\"product-lockup\"]/div/div/picture/source[2]") is null ? string.Empty : html.DocumentNode.SelectSingleNode("//*[@class=\"product-lockup\"]/div/div/picture/source[2]").OuterHtml.Split(" ")[3];

                playlist.Name = html.DocumentNode.SelectSingleNode("//h1[@id=\"page-container__first-linked-element\"]") is null ? string.Empty : html.DocumentNode.SelectSingleNode("//h1[@id=\"page-container__first-linked-element\"]").InnerText;

                playlist.Description = html.DocumentNode.SelectSingleNode("//p[@data-test-bidi]") is null ? string.Empty : html.DocumentNode.SelectSingleNode("//p[@data-test-bidi]").InnerText;

                var typeOfData = IsPlaylistOrAlbum(url);
                switch (typeOfData)
                {

                    case TypeOfData.Album:
                        var res = GetSongsFromAlbum(html);
                        if (!res.IsSuccess)
                            return Result<Playlist>.Error(res.Message);
                        playlist.Songs = res.Data;
                        break;
                    case TypeOfData.Playlist:
                        var result = GetSongsFromPlaylist(html);
                        if (!result.IsSuccess)
                            return Result<Playlist>.Error(result.Message);
                        playlist.Songs = result.Data;
                        break;
                    default:
                        return Result<Playlist>.Error("Unknown Url");
                }
                return Result<Playlist>.Success(playlist);

            }
            catch (Exception ex)
            {
                return Result<Playlist>.Error(ex.Message);
            }
        }
        private static HtmlDocument Load(string url)
        {

            var web = new HtmlWeb();
            var res = web.Load(url);
            ;
            return res;
        }
        private static TypeOfData IsPlaylistOrAlbum(string url)
        {
            if (url.Contains("album")) return TypeOfData.Album;
            else if (url.Contains("playlist")) return TypeOfData.Playlist;
            return TypeOfData.Error;
        }
        private static Result<List<Song>> GetSongsFromAlbum(HtmlDocument html)
        {
            var rowSong = html.DocumentNode.SelectNodes("//div[@role=\"row\"]")!;
            List<Song> songLst = new();
            try
            {
                foreach (var row in rowSong)
                {
                    var song = new Song
                    {
                        Name = row.SelectSingleNode(".//div[@role=\"checkbox\"]").InnerText,
                        Duration = Regex.Replace(row.SelectSingleNode(".//time").InnerText, @"\s+", " ")
                    };
                    songLst.Add(song);
                }
                return Result<List<Song>>.Success(songLst);
            }
            catch (Exception ex)
            {
                if (songLst.Any())
                    return Result<List<Song>>.Success(songLst);
                return Result<List<Song>>.Error("Error getting songs from album");
            }
        }
        private static Result<List<Song>> GetSongsFromPlaylist(HtmlDocument html)
        {
            var rowSong = html.DocumentNode.SelectNodes("//div[@role=\"row\"]")!;
            List<Song> songLst = new();
            try
            {
                foreach (var row in rowSong)
                {
                    var song = (new Song
                    {
                        Name = row.SelectSingleNode(".//div[@role=\"checkbox\"]").InnerText,

                        Album = row.SelectSingleNode(".//div[@class=\"songs-list__song-link-wrapper\"]/span/a").InnerText.Contains("Single") ? "Single" : row.SelectSingleNode(".//div[@class=\"songs-list__song-link-wrapper\"]/span/a").InnerText,

                        Artist = Regex.Replace(row.SelectSingleNode(".//div[@class=\"songs-list__song-link-wrapper\"]/a").ParentNode.InnerText, @"\s+", " "),

                        Duration = Regex.Replace(row.SelectSingleNode(".//time").InnerText, @"\s+", " ")
                    });
                    songLst.Add(song);
                }
                return Result<List<Song>>.Success(songLst);
            }
            catch (Exception ex)
            {
                if (songLst.Any())
                    return Result<List<Song>>.Success(songLst);
                return Result<List<Song>>.Error("Error getting songs from playlist");
            }
        }

    }
}
