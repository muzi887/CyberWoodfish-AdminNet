// Admin.NET/Admin.NET.Application/Service/WoodFish/WoodFishService.cs

using Admin.NET.Application.Entity;
using Admin.NET.Core;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Furion.DynamicApiController; // 动态API( Public class method -> HTTP API 接口), IDynamicApiController
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel; // DisplayName

namespace Admin.NET.Application.Service.WoodFish;

/// <summary>
/// 赛博木鱼服务
/// </summary>
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

  // 定义一个接收参数的类（DTO），这样扩展性更好
  public class KnockInput
  {
    public double Volume { get; set; } // 音量
    public string? KnockType { get; set; } // 敲击类型
  }

/// <summary>
/// 敲一次木鱼
/// </summary>
/// <param name="input">敲击参数</param>
/// <remarks>
/// 每次调用此接口，功德、好运、智慧均+1。
/// </remarks>
/// <returns>返回成功提示</returns>
  [DisplayName("敲一次木鱼")]
  [HttpPost]
  public async Task<string> Knock([FromBody] KnockInput input) // [FromBody] 表示从 JSONBody 里取参数
  {
    // 核心业务： 每敲一次木鱼，就往数据库里插入一条新记录，记录功德、好运、智慧各+1。
    await _repo.InsertAsync(new WoodFishLog 
    { 
        Merit = 1,          // 功德 +1
        Luck = 1,           // 好运 +1
        Wisdom = 1,         // 智慧 +1
        Volume = input.Volume,         // 存入数据库
        KnockType = input.KnockType,   // 存入数据库
        // CreateTime = DateTime.Now // 记录时间（其实EntityBase会自动处理，但不写也可以）
    });
    return "敲击成功";
  }

  /// <summary>
  /// 获取功德统计
  /// </summary>
  /// <remarks>
  /// 获取当前数据库中所有累积的功德总数。
  /// </remarks>
  /// <returns>包含 merit, luck, wisdom 的对象</returns>
  [DisplayName("获取功德统计")]
  [HttpGet]
  public dynamic GetStats()
  {
    // 计算数据库里所有记录的总和
    var merit = _repo.AsQueryable().Sum(u => u.Merit);
    var luck = _repo.AsQueryable().Sum(u => u.Luck);
    var wisdom = _repo.AsQueryable().Sum(u => u.Wisdom);

    // 返回前端需要的格式
    return new {merit, luck, wisdom};
  }

  /// <summary>
  /// 清空木鱼记录
  /// </summary>
  /// <remarks>
  /// ⚠️ 危险操作：将删除所有历史记录，归零功德。
  /// </remarks>
  [DisplayName("清空木鱼记录")]
  [HttpPost]
  public void Reset()
  {
    _repo.Delete(u => true); // 删除所有记录
  }
}