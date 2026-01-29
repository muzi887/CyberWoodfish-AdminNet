// Admin.NET/Admin.NET.Application/Service/WoodFish/WoodFishService.cs

using Admin.NET.Core;
using Admin.NET.Application.Entity;
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
    // 初始化三个变量都为 0
    int addMerit = 0;
    int addLuck = 0;
    int addWisdom = 0;

    // 根据前端传过来的类型，只让对应的变量加1
    switch (input.KnockType?.ToLower()) // ToLower 防止大小写意外
    {
      case "merit":
        addMerit = 1;
        break;
      case "luck":
        addLuck = 1;
        break;
      case "wisdom":
        addWisdom = 1;
        break;
      default:
        // 如果类型不匹配，默认全部加1
        addMerit = 1;
        addLuck = 1;
        addWisdom = 1;
        break;
    }

    // 核心业务： 每敲一次木鱼，就往数据库里插入一条新记录。
    await _repo.InsertAsync(new WoodFishLog 
    { 
        Merit = addMerit,          
        Luck = addLuck,           
        Wisdom = addWisdom,         
        Volume = input.Volume,         
        KnockType = input.KnockType,   
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

  /// <summary>
  /// 分页查询木鱼日志（会自动触发记录级共享过滤器）
  /// </summary>
  [HttpGet]
  public async Task<SqlSugarPagedList<WoodFishLog>> GetPage(int page = 1, int pageSize = 20)
  {
    // _rep是在构造函数里注入的仓储
    // AsQueryable() 会自动被 RecordShareFilter 拦截
    return await _repo.AsQueryable()
      .OrderByDescending(u => u.CreateTime) // 按创建时间降序
      .ToPagedListAsync(page, pageSize); // 分页查询
  }
}