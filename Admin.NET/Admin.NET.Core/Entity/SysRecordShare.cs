// Admin.NET/Admin.NET.Core/Entity/SysRecordShare.cs
using Admin.NET.Core;
using SqlSugar;

namespace Admin.NET.Core.Entity;

/// <summary>
/// 记录数据共享表
/// </summary>
[SugarTable("sys_record_share", "记录级数据共享表")]
// [Tenant("Default")] // 指定数据库租户，通常是default; // 如果你的项目是单库，这行也可以去掉；如果是多库，保留它指向默认库
public class SysRecordShare : EntityBase // 继承 EntityBase 自动包含 Id, CreatedTime, UpdatedTime, IsDeleted 等字段，特有字段单独写
{
  /// <summary>
  /// 租户ID：用来支持多租户
  /// </summary>
  [SugarColumn(ColumnDescription = "租户ID", IsNullable = true)]
  public long? TenantId { get; set; }

  /// <summary>
  /// 业务数据Id (被分享的数据主键)
  /// </summary>
  [SugarColumn(ColumnDescription = "业务数据Id", IsNullable = false)]
  public long BizId { get; set; }

  /// <summary>
  /// 业务类型/表名 (如: "Contract")
  /// </summary>
  [SugarColumn(ColumnDescription = "业务类型", Length = 64, IsNullable = false)]
  public string BizType { get; set; }

  /// <summary>
  /// 授权人ID（分享者）
  /// </summary>
  [SugarColumn(ColumnDescription = "授权人ID", IsNullable = false)]
  public long GrantUserId { get; set; }

  /// <summary>
  /// 被授权人ID（接收者）
  /// </summary>
  [SugarColumn(ColumnDescription = "被授权人ID", IsNullable = false)]
  public long ReceiveUserId { get; set; }

  /// <summary>
  /// 过期时间
  /// 如果为空则永久有效，如果不为空则到期自动失效
  /// </summary>
  [SugarColumn(ColumnDescription = "过期时间", IsNullable = true)]
  public DateTime? ExpireAt { get; set; }
}
