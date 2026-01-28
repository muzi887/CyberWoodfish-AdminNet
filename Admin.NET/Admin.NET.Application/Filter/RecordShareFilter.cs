// Admin.NET/Admin.NET.Application/Filter/RecordShareFilter.cs
using System.Linq.Expressions;
using Admin.NET.Application.Entity;
using Admin.NET.Core.Entity; // UserManager
using SqlSugar;
using Furion;
using Admin.NET.Core; // UserManager

namespace Admin.NET.Application.Filter;

/// <summary>
/// 记录级数据共享过滤器
/// 自动拦截查询，拼装共享数据权限条件
/// </summary>
public class RecordShareFilter
{
  /// <summary>
  /// 获取记录级共享数据过滤条件
  /// </summary>
  // 静态方法不能使用实例变量或实例方法
  public static Expression<Func<WoodFishLog, bool>> GetWoodFishFilter()
  {
    // // 获取当前用户ID
    // var userId = UserManager.UserId;

    // // 如果用户没登录，不查任何数据
    // if (userId == 0) return it => false;

    // 返回过滤表达式
    // 服务定位器模式获取当前用户ID
    return it => 
      it.CreateUserId == (long)(App.GetService<UserManager>(null).UserId)
      || SqlFunc.Subqueryable<SysRecordShare>() // 开启子查询
          .Where(s =>
            s.BizId == it.Id && // 业务数据ID匹配
            s.BizType == "WoodFishLog" && // 业务类型匹配
            s.ReceiveUserId == (long)(App.GetService<UserManager>(null).UserId) // 共享用户ID匹配
            ).Any(); // 存在则返回true
  }
}