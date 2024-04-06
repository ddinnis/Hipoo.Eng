using Listening.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Main.WebAPI.Controllers.Albums
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IListeningRepository repository;
        private readonly IMemoryCacheHelper cacheHelper;

    }
}
