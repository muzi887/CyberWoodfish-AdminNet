// Admin.NET/Admin.NET.Application/Service/SysRecordShare/SysRecordShareService.cs
using Admin.NET.Core;
using Admin.NET.Core.Entity;
using Furion.DatabaseAccessor; // 引用数据库操作能力
using Furion.DependencyInjection; // 引用依赖注入能力
using Furion.DynamicApiController; // 引用动态 Web API 能力
using Microsoft.AspNetCore.Mvc;

namespace Admin.NET.Application.Service;

/// <summary>
/// 记录级共享服务
/// </summary>
[ApiDescriptionSettings("系统记录级共享", Tag = "SysRecordShare", Order = 100)] // 设置在Swagger 文档里的排序
public class SysRecordShareService : IDynamicApiController, ITransient // 实现 ITransient 接口，表示该服务是瞬时生命周期
{
  // 数据库操作对象（数据库管家）
  private readonly ISqlSugarRepository<SysRecordShare> _rep;
  // 用户管理对象（获取当前是谁在操作）
  private readonly UserManager _userManager;

  // 构造函数：注入管家和用户管理器
  public SysRecordShareService(ISqlSugarRepository<SysRecordShare> rep, UserManager userManager)
  {
    _rep = rep;
    _userManager = userManager;
  }

  /// <summary>
  /// 添加共享
  /// </summary>
  /// <param name="input">输入参数</param>
  /// <returns></returns>
  [HttpPost]
  public async Task AddShare(ShareInputDto input)
  {
    // 获取当前登陆人的ID（授权人）
    var currentUserId = _userManager.UserId;

    // 组装要存入数据库的实体对象
    var shareRecord = new SysRecordShare
    {
      BizId = input.BizId,
      BizType = input.BizType,
      GrantUserId = currentUserId,
      ReceiveUserId = input.TargetUserId,
      ExpireAt = input.ExpireAt
    };

    // 写入数据库
    await _rep.InsertAsync(shareRecord);
  }

  /// <summary>
  /// 查看把数据都分享给了谁
  /// </summary>
  [HttpGet]
  public async Task<List<SysRecordShare>> GetMyShareList()
  {
    var myId = _userManager.UserId;
    // 查询 GrantUserId 是当前用户的所有分享记录
    return await _rep.GetListAsync(u => u.GrantUserId == myId);
  }
}

/// <summary>
/// 前端传进来的参数包（DTO）
/// </summary>
public class ShareInputDto
{
/// <summary>
    /// 数据的ID (比如 WoodFishLog 的 Id)
    /// </summary>
    public long BizId { get; set; }

    /// <summary>
    /// 数据类型 (比如填 "WoodFishLog")
    /// </summary>
    public string BizType { get; set; }

    /// <summary>
    /// 目标用户ID (你要分享给那个人的 ID)
    /// </summary>
    public long TargetUserId { get; set; }

    /// <summary>
    /// 过期时间 (不填就是永久)
    /// </summary>
    public DateTime? ExpireAt { get; set; }
}