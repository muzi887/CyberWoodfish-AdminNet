<!-- Web/src/views/woodenfish/log.vue -->
<template>
	<div class="log-container">
		<el-card>
			<template #header>
				<span>📜 功德簿 (历史记录)</span>
				<el-button type="primary" @click="fetchData">刷新</el-button>
			</template>
			<el-table :data="tableData" style="width: 100%" v-loading="loading">
				<el-table-column prop="id" label="记录ID" width="180" />
				<el-table-column prop="knockType" label="敲击类型" width="100">
					<template #default="scope">
						<el-tag v-if="scope.row.knockType === 'merit'" type="warning">功德</el-tag>
						<el-tag v-else-if="scope.row.knockType === 'luck'" type="success">好运</el-tag>
						<el-tag v-else type="info">智慧</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="createTime" label="敲击时间" />
				<el-table-column label="来源" align="center">
					<template #default="scope">
						<el-tag v-if="scope.row.createUserId === userInfoStore.userInfos.id" type="success" effect="dark">我的</el-tag>
						<el-tag v-else type="danger" effect="dark">来自分享</el-tag>
					</template>
				</el-table-column>
			</el-table>
			<div class="pagination">
				<el-pagination layout="prev,pager,next" :total="total" @current-change="handlePageChange" />
			</div>
		</el-card>
	</div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useUserInfo } from '/@/stores/userInfo';
import request from '/@/utils/request';
import { getWoodenFishPage } from '/@/api/woodenFish';

const userInfoStore = useUserInfo();
const tableData = ref([]);
const loading = ref(false);
const total = ref(0);
const currentPage = ref(1);

// 调用后端 WoodFishService.cs 的GetPage 接口
const fetchData = async () => {
	loading.value = true;
	try {
		const res = await getWoodenFishPage(currentPage.value, 10);

		// Admin.NET 分页返回结构通常是 { items: [], total: ... }
		const data = res.data?.result ?? res.data;
		tableData.value = data?.items ?? [];
		total.value = data?.total ?? 0;
	} finally {
		loading.value = false;
	}
};

const handlePageChange = (val: number) => {
	currentPage.value = val;
	fetchData();
};

onMounted(() => {
	fetchData();
});
</script>

<style scoped>
.log-container {
	padding: 20px;
}
.card-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
}
.pagination {
	margin-top: 20px;
	display: flex;
	justify-content: center;
}
</style>
