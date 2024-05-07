 using Common.ASPNETCore;
using FileService.Domain;
using FileService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileService.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    [UnitOfWork(typeof(FSDbContext))]
    public class UploadController : ControllerBase
    {
        private readonly FSDbContext _fSDbContext;
        private readonly FSDomainService _fSDomainService;
        private readonly IFSRepository _fsRepository;

        public UploadController(FSDbContext fSDbContext, FSDomainService fSDomainService, IFSRepository fSRepository)
        {
            _fsRepository = fSRepository;
            _fSDbContext = fSDbContext;
            _fSDomainService = fSDomainService;
        }


        [HttpGet]
        public async Task<FileExistsResponse> FileExist(long fileSize, string sha256Hash) 
        { 
            var file = await _fsRepository.FindFileAsync(fileSize, sha256Hash);
            if (file == null)
            {
                return new FileExistsResponse(false, null);
            }
            else 
            {
            return new FileExistsResponse(true, file.RemoteUrl);

            }
        }

        [HttpPost]
        [RequestSizeLimit(60_000_000)]
        public async Task<ActionResult<Uri>> UploadFile([FromForm] UploadRequest request, CancellationToken cancellationToken = default) 
        { 
            IFormFile? file = request.File;
            string fileName = file.FileName;
            using Stream stream = file.OpenReadStream();
            var result = await _fSDomainService.UploadAsync(stream, fileName, cancellationToken);
            _fSDbContext.Add(result);
            return result.RemoteUrl;
        }
    }
}
