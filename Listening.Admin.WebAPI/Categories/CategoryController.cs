using Listening.Domain.Entities;
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
    public CategoryController(ListeningDbContext dbContext, ListeningDomainService domainService, IListeningRepository repository)
    {
        this.dbContext = dbContext;
        this.domainService = domainService;
        this.repository = repository;
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
}
