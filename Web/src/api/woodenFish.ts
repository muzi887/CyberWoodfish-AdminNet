// Web/src/api/woodenFish.ts
import request from '../utils/request' ; // 引用项目自带的请求工具

// 定义接口地址，必须和 Swagger 中的路径一致
enum API { 
  Knock = '/api/woodFish/knock',
  Stats = '/api/woodFish/stats',
  Reset = '/api/woodFish/reset',
  Page = '/api/woodFish/page',
}

// 前端调用接口时的入参类型定义
interface KnockParams {
  volume: number; // 音量
  knockType?: string; // 敲击类型
}

// 导出方法
// 敲击木鱼
export const knockWoodenFish = (data: KnockParams) => {
  // request（axios 实例）收到 config.data
  // axios 自动把 data 序列化成 JSON 并放进 Body，发给后端
  return request({
    // 访问枚举对象
    url: API.Knock,
    method: 'post',
    data:data, 
  });
};

// 获取木鱼统计数据
export const getWoodenFishStats = () => {
  return request({
    url: API.Stats,
    method: 'get',
  });
};

// 重置木鱼记录
export const resetWoodenFish = () => {
  return request({
    url: API.Reset,
    method: 'post',
  });
};

// 分页获取木鱼日志
export const getWoodenFishPage = (page: number, pageSize: number) => {
  return request({
    // 使用模板字符串拼接URL
    url: `${API.Page}/${page}/${pageSize}`,
    method:'get',
  })
}