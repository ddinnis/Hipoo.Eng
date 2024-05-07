using Listening.Admin.WebAPI.Categories.Request;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Listening.Admin.WebAPI.Categories;
[Route("[controller]/[action]")]
[Authorize(Roles = "Admin")]
[ApiController]
[UnitOfWork(typeof(ListeningDbContext))]
//供后台用的增删改查接口不用缓存
public class CategoryController : ControllerBase
{
    private IListeningRepository repository;
    private readonly ListeningDbContext dbContext;
    private readonly ListeningDomainService domainService;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CategoryController(ListeningDbContext dbContext, ListeningDomainService domainService, IListeningRepository repository, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        this.dbContext = dbContext;
        this.domainService = domainService;
        this.repository = repository;
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public Task<Category[]> FindAll()
    {
        return repository.GetCategoriesAsync();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<Category?>> FindById([RequiredGuid] Guid id)
    {
        //返回ValueTask的需要await的一下
        var cat = await repository.GetCategoryByIdAsync(id);
        if (cat == null)
        {
            return NotFound($"没有Id={id}的Category");
        }
        else
        {
            return cat;
        }
    }

    [HttpPost]
    public async Task<ActionResult<ResultObject>> Add(CategoryAddRequest req)
    {
        var category = await domainService.AddCategoryAsync(req.Name, req.CoverUrl);
        dbContext.Add(category);
        return new ResultObject
        {
            Code = (int)HttpStatusCode.OK,
            Data = category.Id,
            Ok = true,
        };
    } 

    [HttpPut]
    [Route("{id}")]
    public async Task<ResultObject> Update([RequiredGuid] Guid id, CategoryUpdateRequest request)
    {
        var cat = await repository.GetCategoryByIdAsync(id);
        if (cat == null)
        {
            return new ResultObject
            {
                Code = (int)HttpStatusCode.NotFound,
                Ok = true,
                Message = "id不存在"
            };
        }
        cat.ChangeName(request.Name);
        cat.ChangeCoverUrl(request.CoverUrl);
        return new ResultObject
        {
            Code = (int)HttpStatusCode.OK,
            Ok = true,
        };
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ResultObject> DeleteById([RequiredGuid] Guid id)
    {
        var cat = await repository.GetCategoryByIdAsync(id);
        if (cat == null)
        {
            return new ResultObject
            {
                Code = (int)HttpStatusCode.NotFound,
                Ok = true,
                Message = "$\"没有Id={id}的Category\""
            };
        }
        cat.SoftDelete();//软删除
        return new ResultObject
        {
            Code = (int)HttpStatusCode.OK,
            Ok = true,
        };
    }

    [HttpPut]
    public async Task<ActionResult> Sort(CategoriesSortRequest req)
    {
        await domainService.SortCategoriesAsync(req.SortedCategoryIds);
        return Ok();
    }

    [HttpPost]
    [RequestSizeLimit(60_000_000)]
    public async Task<ActionResult> UploadImage([FromForm] UploadRequest request, CancellationToken cancellationToken = default)
    {
        var file = request.File;

        if (file == null || file.Length == 0)
        {
            return BadRequest("无文件内容，请重新上传");
        }
        string workingDir = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", "images");
        string fullPath = Path.Combine(workingDir, file.FileName);
        string? fullDir = Path.GetDirectoryName(fullPath);

        if (!System.IO.File.Exists(fullDir))
        {
            Directory.CreateDirectory(fullDir);
        }
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }

        using Stream stream = System.IO.File.Create(fullPath);
        await file.CopyToAsync(stream, cancellationToken);
        var req = httpContextAccessor.HttpContext.Request;
        string imageUrl = req.Scheme + "://" + req.Host + "/Listening.Admin/images/" + file.FileName;

        return Ok(new { imageUrl });
    }
}
