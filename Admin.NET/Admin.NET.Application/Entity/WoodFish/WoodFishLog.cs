// Admin.NET/Admin.NET.Application/Entity/WoodFish/WoodFishLog.cs

namespace Admin.NET.Application.Entity;

[Table("My_WoodFish_Log")] // 数据库表名
public class WoodFishLog : EntityBase
{
  public int Merit { get; set; } //功德数
  public int Luck { get; set; } //好运数
  public int Wisdom { get; set; } //智慧数
}