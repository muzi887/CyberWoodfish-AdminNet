// Admin.NET/Admin.NET.Application/Service/WoodFish/WoodFishService.cs

using Admin.NET.Application.Entity;
using Admin.NET.Core;
using Furion.DynamicApiController; // 动态API( Public method -> HTTP 接口), IDynamicApiController
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel; // DisplayName

namespace Admin.NET.Application.Service.WoodFish;

// [ApiDescriptionSettings]: 这是给 Swagger 看的。
// 加上这行，你的接口在网页文档里就会整整齐齐地归类在 "赛博木鱼" 分组下，而不是乱放。
[ApiDescriptionSettings("赛博木鱼", Tag = "WoodFish", Order = 100)]
public class WoodFishService : IDynamicApiController,ITransient
{
  // _repo: 这是一个数据库操作仓库，专门用来操作 WoodFishLog 表。
  private readonly SqlSugarRepository<WoodFishLog> _repo;

  // 构造函数: 这里通过依赖注入把仓库传进来。
  public WoodFishService(SqlSugarRepository<WoodFishLog> repo)
  {
    _repo = repo;
  }

  // [DisplayName]: 这是给“权限系统”看的。
  // 加上这行，你去系统管理的“菜单配置”里选接口时，就能看到“敲一次木鱼”这几个汉字。
  [DisplayName("敲一次木鱼")]
  public async Task<string> Knock()
  {
    // 核心业务： 每敲一次木鱼，就往数据库里插入一条新记录，记录功德、好运、智慧各+1。
    await _repo.InsertAsync(new WoodFishLog 
    { 
        Merit = 1,          // 功德 +1
        Luck = 1,           // 好运 +1
        Wisdom = 1,         // 智慧 +1
        // CreateTime = DateTime.Now // 记录时间（其实EntityBase会自动处理，但不写也可以）
    });
    return "敲击成功";
  }
}