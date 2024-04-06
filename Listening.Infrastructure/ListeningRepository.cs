using Listening.Domain;
using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Listening.Infrastructure
{
    public class ListeningRepository : IListeningRepository
    {
        private readonly ListeningDbContext _dbContext;
        public ListeningRepository(ListeningDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Album?> GetAlbumByIdAsync(Guid albumId)
        {
           return await _dbContext.FindAsync<Album>(albumId);
        }

        public Task<Album[]> GetAlbumsByCategoryIdAsync(Guid categoryId)
        {
            return _dbContext.Albums.OrderBy(e => e.SequenceNumber).Where(a => a.CategoryId == categoryId).ToArrayAsync();
        }


        public Task<Category[]> GetCategoriesAsync()
        {
            return _dbContext.Categories.OrderBy(e => e.SequenceNumber).ToArrayAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _dbContext.FindAsync<Category>(categoryId);
        }

        public async Task<Episode?> GetEpisodeByIdAsync(Guid episodeId)
        {
            return await _dbContext.Episodes.SingleOrDefaultAsync(x=>x.Id == episodeId);
        }

        public async Task<Episode[]> GetEpisodesByAlbumIdAsync(Guid albumId)
        {
            return await _dbContext.Episodes.OrderBy(x => x.SequenceNumber).Where(x => x.AlbumId == albumId).ToArrayAsync();
        }

        public async Task<int> GetMaxSeqOfAlbumsAsync(Guid categoryId)
        {
            return await _dbContext.Set<Album>().AsNoTracking().
                Where(x => x.CategoryId == categoryId)
                .MaxAsync(x => x.SequenceNumber);
        }

        public async Task<int> GetMaxSeqOfCategoriesAsync()
        {
            return await _dbContext.Set<Category>().MaxAsync(x => x.SequenceNumber);
        }

        public async Task<int> GetMaxSeqOfEpisodesAsync(Guid albumId)
        {
            return await _dbContext.Set<Episode>().Where(x=>x.AlbumId == albumId).MaxAsync(x => x.SequenceNumber);
        }
    }
}
