using Common.ASPNETCore;
using Common.Common;
using Listening.Domain.Entities;
using Listening.Domain;
using Listening.Main.WebAPI.Controllers.Categories.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Main.WebAPI.Controllers.Categories
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CategoryController : ControllerBase
    {
        private readonly IListeningRepository repository;
        private readonly IMemoryCacheHelper cacheHelper;
        public CategoryController(IListeningRepository repository, IMemoryCacheHelper cacheHelper)
        {
            this.repository = repository;
            this.cacheHelper = cacheHelper;
        }

        [HttpGet]
        public async Task<ActionResult<CategoryVM[]>> FindAll()
        {
            Task<Category[]> FindData()
            {
                return repository.GetCategoriesAsync();
            }
            //用AOP来进行缓存控制看起来更优美（可以用国产的AspectCore或者Castle DynamicProxy），但是这样反而不灵活，因为缓存对于灵活性要求更高，所以用这种直接用ICacheHelper的不优美的方式更实用。
            var task = cacheHelper.GetOrCreateAsync($"CategoryController.FindAll",
                async (e) => CategoryVM.Create(await FindData()));
            return await task;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CategoryVM?>> FindById([RequiredGuid] Guid id)
        {
            var cat = await cacheHelper.GetOrCreateAsync($"CategoryController.FindById.{id}",
                async (e) => CategoryVM.Create(await repository.GetCategoryByIdAsync(id)));
            if (cat == null)
            {
                return NotFound($"没有Id={id}的Category");
            }
            else
            {
                return cat;
            }
        }
    }
}
