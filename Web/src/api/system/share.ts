// Web/src/api/system/share.ts
import request from '../../utils/request'; // 引用项目自带的请求工具

// 添加分享（AddShare)
export const shareDataAPI = (params: any) => {
	return request({
		url: 'api/sysRecordShare/share',
		method: 'post',
		data: params,
	});
};

// 获取用户列表 API: 使用 Admin.NET 自带的 Selector 接口，或者 baseInfo 接口
export const getSimpleUserListApi = () => {
	return request({
		url: 'api/sysUser/page', // 获取用户选择器列表
		method: 'post',
		data: {
			page: 1,
			pageSize: 1000,
		}
	});
};

// 获取我的分享记录
export const getMyShareListApi = () => {
	return request({
		url: 'api/sysRecordShare/myShareList',
		method: 'get',
	});
}
