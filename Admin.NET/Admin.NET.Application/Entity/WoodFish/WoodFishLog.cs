// Admin.NET/Admin.NET.Application/Entity/WoodFish/WoodFishLog.cs
using Admin.NET.Core;
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application.Entity;

[SugarTable("My_WoodFish_Log")] // 数据库表名
public class WoodFishLog : EntityBase
{
  // [SugarColumn(ColumnDescription = "功德数")] // 可选：加上注释，数据库里能看到
  public int Merit { get; set; } //功德数
  public int Luck { get; set; } //好运数
  public int Wisdom { get; set; } //智慧数
}