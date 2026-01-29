<!-- Web/src/components/ShareRecord/index.vue -->
<template>
	<div v-if="visible" class="modal-overlay" @click.self="handleClose">
		<div class="modal-content">
			<div class="modal-header">
				<h3>数据分享</h3>
				<span class="close-btn" @click="handleClose">&times;</span>
			</div>

			<div class="modal-body">
				<div class="form-group">
					<label class="required">分享给</label>
					<select v-model="form.targetUserId" class="native-input" :disabled="loadingUsers">
						<option :value="undefined" disabled>
							{{ loadingUsers ? '加载中...' : '请选择用户' }}
						</option>
						<option v-for="item in userList" :key="item.id" :value="item.id">
							{{ item.realName || item.account }}
						</option>
					</select>
				</div>
				<div class="form-group">
					<label>过期时间</label>
					<input type="datetime-local" v-model="tempDate" class="native-input" />
					<small class="tip">留空则永久有效</small>
				</div>
			</div>
			<div class="modal-footer">
				<button class="btn btn-default" @click="handleClose">取消</button>
				<button class="btn btn-primary" :disabled="submitting" @click="handleSubmit">
					{{ submitting ? '提交中...' : '确定分享' }}
				</button>
			</div>
		</div>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue';
import { shareDataAPI, getSimpleUserListApi } from '../../api/system/share';

// 状态
const visible = ref(false);
const submitting = ref(false);
const loadingUsers = ref(false);
const userList = ref<Array<{ id: number; account: string; realName?: string }>>([]);
const tempDate = ref('');

// 表单数据
const form = reactive({
	bizId: 0 as number | string,
	bizType: '',
	targetUserId: undefined as number | undefined,
	expireAt: null as string | null,
});

// 监听临时日期变化，转换为后端需要的格式
watch(tempDate, (val) => {
	if (!val) {
		form.expireAt = null;
	} else {
		// datetime-local 返回的是 '2026-01-29T14:00'，后端通常需要 '2026-01-29 14:00:00'
		form.expireAt = val.replace('T', ' ') + ':00';
	}
});

// 打开分享记录弹窗
const openDialog = async (id: number | string, type: string) => {
	// 重置表单
	form.bizId = id;
	form.bizType = type;
	form.targetUserId = undefined;
	form.expireAt = null;
	tempDate.value = ''; // 重置日期选择器

	visible.value = true;

	// 加载用户
	if (userList.value.length === 0) {
		await loadUserList();
	}
};

const handleClose = () => {
	visible.value = false;
};

const loadUserList = async () => {
	loadingUsers.value = true;
	try {
		const res = await getSimpleUserListApi();

		const pageData = res.data.result ?? res.data;

		if (Array.isArray(pageData)) {
			// 如果后端直接返回数组（少见）
			userList.value = pageData;
		} else {
			// 如果是分页对象，取 items
			userList.value = pageData?.items ?? [];
		}
	} catch (error) {
		console.error('加载用户列表失败:', error);
		alert('加载用户列表失败');
	} finally {
		loadingUsers.value = false;
	}
};

const handleSubmit = async () => {
	if (!form.targetUserId) {
		alert('请选择目标分享用户');
		return;
	}

	submitting.value = true;
	try {
		await shareDataAPI({
			bizId: form.bizId,
			bizType: form.bizType,
			targetUserId: form.targetUserId,
			expireAt: form.expireAt,
		});
		alert('分享记录添加成功');
		handleClose();
	} catch (error) {
		console.error('分享报错详情:', error);
		// alert('添加分享记录失败');
	} finally {
		submitting.value = false;
	}
};

defineExpose({ openDialog });
</script>

<style scoped>
/* 遮罩层：全屏半透明 */
.modal-overlay {
	position: fixed;
	top: 0;
	left: 0;
	width: 100vw;
	height: 100vh;
	background-color: rgba(0, 0, 0, 0.5);
	display: flex;
	justify-content: center;
	align-items: center;
	z-index: 9999; /* 确保在最上层 */
	backdrop-filter: blur(2px); /* 磨砂效果 */
}

/* 内容框 */
.modal-content {
	background: #fff;
	width: 90%;
	max-width: 400px;
	border-radius: 8px;
	box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
	animation: slideDown 0.3s ease;
	overflow: hidden;
	display: flex;
	flex-direction: column;
}

/* 标题栏 */
.modal-header {
	padding: 15px 20px;
	border-bottom: 1px solid #eee;
	display: flex;
	justify-content: space-between;
	align-items: center;
}

.modal-header h3 {
	margin: 0;
	font-size: 18px;
	color: #333;
}

.close-btn {
	font-size: 24px;
	cursor: pointer;
	color: #999;
	line-height: 1;
}

.close-btn:hover {
	color: #333;
}

/* 表单内容 */
.modal-body {
	padding: 20px;
}

.form-group {
	margin-bottom: 15px;
}

.form-group label {
	display: block;
	margin-bottom: 8px;
	font-size: 14px;
	color: #606266;
	font-weight: 500;
}

.form-group label.required::before {
	content: '*';
	color: #f56c6c;
	margin-right: 4px;
}

/* 原生输入框样式美化 */
.native-input {
	width: 100%;
	padding: 8px 12px;
	border: 1px solid #dcdfe6;
	border-radius: 4px;
	font-size: 14px;
	color: #606266;
	background-color: #fff;
	outline: none;
	transition: border-color 0.2s;
	box-sizing: border-box; /* 防止padding撑大宽度 */
}

.native-input:focus {
	border-color: #409eff;
}

.native-input:disabled {
	background-color: #f5f7fa;
	cursor: not-allowed;
}

.tip {
	display: block;
	margin-top: 4px;
	font-size: 12px;
	color: #909399;
}

/* 底部按钮栏 */
.modal-footer {
	padding: 15px 20px;
	border-top: 1px solid #eee;
	display: flex;
	justify-content: flex-end;
	gap: 10px;
	background-color: #f9f9f9;
}

.btn {
	padding: 8px 16px;
	border-radius: 4px;
	font-size: 14px;
	cursor: pointer;
	border: 1px solid transparent;
	transition: all 0.2s;
}

.btn:disabled {
	opacity: 0.6;
	cursor: not-allowed;
}

.btn-default {
	background: #fff;
	border-color: #dcdfe6;
	color: #606266;
}

.btn-default:hover {
	border-color: #c6e2ff;
	color: #409eff;
	background-color: #ecf5ff;
}

.btn-primary {
	background-color: #409eff;
	color: #fff;
}

.btn-primary:hover {
	background-color: #66b1ff;
}

/* 简单的进场动画 */
@keyframes slideDown {
	from {
		opacity: 0;
		transform: translateY(-20px);
	}
	to {
		opacity: 1;
		transform: translateY(0);
	}
}
</style>
