using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Transactions;


namespace Common.ASPNETCore
{
    public class UnitOfWorkFilter : IAsyncActionFilter
    {
        public static UnitOfWorkAttribute? GetUoWAttr(ActionDescriptor descriptor) 
        {
            var actionDes = descriptor as ControllerActionDescriptor;
            if (actionDes == null) return null;
            var uowAttr = actionDes.ControllerTypeInfo.GetCustomAttribute<UnitOfWorkAttribute>();
            if (uowAttr == null)
            {
                return actionDes.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();
            }
            else 
            {
                return uowAttr;
            }
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var uowAttr = GetUoWAttr(context.ActionDescriptor);
            if (uowAttr == null)
            {
                await next();
                return;
            }
            else 
            {
                // 安全地使用异步操作
                using TransactionScope txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                List<DbContext> dbCtxs = new List<DbContext>();
                foreach (var dbCtxType in uowAttr.DbContextTypes)
                {
                    var sp = context.HttpContext.RequestServices;
                    DbContext dbCtx = (DbContext)sp.GetRequiredService(dbCtxType);
                    dbCtxs.Add(dbCtx);
                }

                var result = await next();
                if (result.Exception == null) 
                {
                    foreach (var dbCtx in dbCtxs)
                    {
                        await dbCtx.SaveChangesAsync();
                    }
                    txscope.Complete();
                }
            }

        }
    }
}
