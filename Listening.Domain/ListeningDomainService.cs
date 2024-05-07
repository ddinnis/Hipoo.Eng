using Common.Domain.Models;
using Listening.Domain.Entities;

namespace Listening.Domain
{
    public class ListeningDomainService
    {
        private readonly IListeningRepository _repository;
        public ListeningDomainService(IListeningRepository listeningRepository)
        {
            _repository = listeningRepository;
        }


        public async Task<Album> AddAlbumAsync(Guid categoryId, MultilingualString name)
        {
            int maxSeq = await _repository.GetMaxSeqOfAlbumsAsync(categoryId);
            Guid guid = Guid.NewGuid();
            return Album.Create(guid, maxSeq + 1, name, categoryId);
        }

        public async Task SortAlbumsAsync(Guid categoryId, Guid[] sortedAlbumIds)
        {
            var alums = await _repository.GetAlbumsByCategoryIdAsync(categoryId);
            var idsInDB = alums.Select(x => x.Id);

            if (!idsInDB.SequenceIgnoredEqual(sortedAlbumIds))
            {
                throw new Exception($"提交的待排序Id中必须是categoryId={categoryId}分类下所有的Id");
            }
            int seqNum = 1;
            foreach (Guid albumId in sortedAlbumIds)
            {
                var album = await _repository.GetAlbumByIdAsync(albumId);
                if (album == null)
                {
                    throw new Exception($"albumId={albumId}不存在");
                }
                album.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }

        public async Task<Category> AddCategoryAsync(MultilingualString name, Uri coverUrl)
        {
            var maxSeq = await _repository.GetMaxSeqOfCategoriesAsync();
            var id = Guid.NewGuid();
            var cate = Category.Create(id, maxSeq + 1, name, coverUrl);
            return cate;
        }

        public async Task SortCategoriesAsync(Guid[] sortedCategoryIds)
        {
            var categories = await _repository.GetCategoriesAsync();
            var idsInDB = categories.Select(a => a.Id);
            if (!idsInDB.SequenceIgnoredEqual(sortedCategoryIds))
            {
                throw new Exception("提交的待排序Id中必须是所有的分类Id");
            }

            int seqNum = 1;
            foreach (Guid catId in sortedCategoryIds)
            {
                var cat = await _repository.GetCategoryByIdAsync(catId);
                if (cat == null)
                {
                    throw new Exception($"categoryId={catId}不存在");
                }
                cat.ChangeSequenceNumber(seqNum);//顺序改序号
                seqNum++;
            }
        }

        public async Task<Episode> AddEpisodeAsync(MultilingualString name,
            Guid albumId, Uri audioUrl, double durationInSecond,
            string subtitleType, string subtitle)
        {
            int maxSeq = await _repository.GetMaxSeqOfEpisodesAsync(albumId);
            Guid id = Guid.NewGuid();
            var builder = new Episode.Builder();
            builder.Id(id).SequenceNumber(maxSeq + 1).Name(name).AlbumId(albumId)
                .AudioUrl(audioUrl).DurationInSecond(durationInSecond)
                .SubtitleType(subtitleType).Subtitle(subtitle);
            return builder.Build();
        }

        public async Task SortEpisodesAsync(Guid albumId, Guid[] sortedEpisodeIds)
        {
            var episodes = await _repository.GetEpisodesByAlbumIdAsync(albumId);
            var idsInDb = episodes.Select(e => e.Id);
            if (!sortedEpisodeIds.SequenceIgnoredEqual(idsInDb))
            {
                throw new Exception($"提交的待排序Id中必须是albumId={albumId}专辑下所有的Id");
            }

            int seqNum = 0;
            foreach (var episodeId in sortedEpisodeIds)
            {
                var episode = await _repository.GetEpisodeByIdAsync(episodeId);
                if (episode != null)
                {
                    episode.ChangeSequenceNumber(seqNum);
                    seqNum++;
                }
                else
                {
                    throw new Exception($"episodeId={episodeId}不存在");
                }
            }
        }

    }
}
