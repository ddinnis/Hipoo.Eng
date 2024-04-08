using Listening.Domain;
using Microsoft.AspNetCore.Mvc;
using Common.ASPNETCore;
using Common.Common;

namespace Listening.Main.WebAPI.Controllers.Albums
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IListeningRepository _repository;
        private readonly IMemoryCacheHelper _cacheHelper; 
        public AlbumController(IListeningRepository listeningRepository,IMemoryCacheHelper memoryCacheHelper)
        {
            _cacheHelper = memoryCacheHelper;
            _repository = listeningRepository;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<AlbumVM>> FindById([RequiredGuid] Guid id) 
        {
            var album = await _cacheHelper.GetOrCreateAsync($"AlbumController.FindById.{id}",
           async (e) => AlbumVM.Create(await _repository.GetAlbumByIdAsync(id)));
            if (album == null)
            {
                return NotFound();
            }
            return album;
        }

        [HttpGet]
        [Route("{categoryId}")]
        public async Task<ActionResult<AlbumVM[]>> FindByCategoryId([RequiredGuid] Guid categoryId)
        {
            var result = await _cacheHelper.GetOrCreateAsync($"AlbumController.FindByCategoryId.{categoryId}", async (e)=>
             AlbumVM.Create(await _repository.GetAlbumsByCategoryIdAsync(categoryId))
              );
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

    }
}
