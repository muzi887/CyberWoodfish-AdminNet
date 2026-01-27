<!-- Web/src/views/woodenfish/index.vue -->
<!-- Client/src/App.vue -->
<template>
	<div class="wooden-fish-container" :style="{ backgroundColor: bgColor }">
		<div class="stats">
			<div v-for="item in WOODFISH_STATS" :key="item.key" class="stat" :class="item.typeClass" @click="handleManualKnock(item.key)">{{ item.label }}：{{ counts[item.key] }}</div>
		</div>
		<div class="right-panel">
			<button class="auto-toggle" :class="{ on: isAuto }" @click="toggleAuto">自动积攒：{{ isAuto ? '开' : '关' }}</button>
			<div class="control-panel">
				<button class="mode-btn" @click="toggleBgMode">
					<span class="mode-label">背景模式</span>
					<span class="mode-value">{{ currentMode.label }}</span>
				</button>
				<div class="speed-group">
					<span class="speed-label">自动档位</span>
					<button v-for="option in speedOptions" :key="option.value" class="speed-btn" :class="{ active: autoInterval === option.value }" @click="setAutoInterval(option.value)">
						{{ option.label }}
					</button>
				</div>
				<div class="volume-group">
					<label class="volume-label">
						手动音量
						<input type="range" min="0" max="1" step="0.01" v-model.number="manualVolume" @input="onManualVolumeChange" />
						<span class="volume-value">{{ manualVolume.toFixed(2) }}</span>
					</label>
					<label class="volume-label">
						自动音量
						<input type="range" min="0" max="1" step="0.01" v-model.number="autoVolume" @input="onAutoVolumeChange" />
						<span class="volume-value">{{ autoVolume.toFixed(2) }}</span>
					</label>
				</div>
				<button class="reset-btn" @click="resetCounts">清零/自定义重置</button>
			</div>
		</div>
		<div class="stage">
			<button class="woodfish" :class="{ 'is-active': isActive }" @click="handleManualKnock()" v-auth="'sys:woodfish:knock'">
				<svg width="240" height="240" viewBox="0 0 240 240" xmlns="http://www.w3.org/2000/svg" aria-label="木鱼">
					<ellipse cx="120" cy="120" rx="92" ry="70" fill="rgb(184,107,50)" stroke="rgb(120,70,35)" stroke-width="8" />
					<ellipse cx="120" cy="120" rx="58" ry="42" fill="rgb(208,135,74)" opacity="0.7" />
					<circle cx="160" cy="90" r="11" fill="rgb(120,70,35)" />
					<path d="M60 140 C90 170 150 170 180 140" fill="none" stroke="rgb(120,70,35)" stroke-width="6" stroke-linecap="round" />
					<rect x="106" y="24" width="28" height="42" rx="10" fill="rgb(140,80,40)" />
				</svg>
			</button>

			<span v-for="ripple in ripples" :key="ripple" class="ripple"></span>

			<div class="merit-layer">
				<span v-for="item in merits" :key="item.id" class="merit" :class="item.typeClass" :style="{ left: `calc(50% + ${item.offset}px)` }">
					{{ item.text }}
				</span>
			</div>
		</div>
	</div>
</template>

<script setup name="woodenFish">
// 或者 defineOptions({ name: 'woodenFish' });
import { useWoodfish } from './composables/useWoodfish';
import { WOODFISH_STATS } from './composables/woodfishConfig';

// 纯静态配置
const speedOptions = [
	{ label: '0.5s', value: 500 },
	{ label: '1s', value: 1000 },
	{ label: '2s', value: 2000 },
];

// 一键解构所有逻辑
const {
	counts,
	merits,
	ripples,
	isActive,
	isAuto,
	autoInterval,
	manualVolume,
	autoVolume,
	bgColor,
	currentMode,
	toggleBgMode,
	toggleAuto,
	setAutoInterval,
	resetCounts,
	handleManualKnock,
	onManualVolumeChange,
	onAutoVolumeChange,
} = useWoodfish();
</script>

<style scoped lang="scss">
.wooden-fish-container {
	position: relative; /* 自适应框架内容区 */
	width: 100%;
	min-height: calc(100vh - 120px); /* 计算剩余空间 */
	z-index: 1; /* 确保背景色在框架之上 */
	display: grid;
	place-items: center;
	padding: 20px;
	transition: background-color 0.6s ease-in-out; /* 背景色过渡动画 */
	border-radius: 8px; /* 强制覆盖框架可能的背景色干扰 */
	overflow: hidden; /* 防止波纹溢出 */
}

.wooden-fish-container.gold-mode {
	background: radial-gradient(circle at top, #fff1b8, #d7a94a 70%);
}

.stats {
	position: absolute;
	top: 40px;
	left: 50%;
	transform: translateX(-50%);
	display: flex;
	gap: 12px;
	z-index: 10;
	pointer-events: auto;
}

.stat {
	padding: 8px 14px;
	border-radius: 999px;
	font-size: clamp(16px, 2.6vw, 22px);
	font-weight: 700;
	letter-spacing: 0.5px;
	text-shadow: 0 3px 12px rgba(0, 0, 0, 0.45);
	background: rgba(0, 0, 0, 0.25);
	border: 1px solid rgba(255, 255, 255, 0.2);
	cursor: pointer; /* 鼠标放上去变小手 */
	user-select: none; /* 防止双击选中文字 */
	transition:
		transform 0.1s,
		background 0.2s;
}

/* 鼠标悬停效果 */
.stat:hover {
	transform: translateY(-2px);
	filter: brightness(1.2); /* 稍微变亮 */
}

/* 鼠标按下效果 */
.stat:active {
	transform: scale(0.95); /* 稍微缩小 */
}

.stat-merit {
	color: #ffe9b0;
	border-color: rgba(255, 233, 176, 0.6);
}

.stat-luck {
	color: #9cf5ff;
	border-color: rgba(156, 245, 255, 0.6);
}

.stat-wisdom {
	color: #c9b6ff;
	border-color: rgba(201, 182, 255, 0.6);
}

.right-panel {
	position: fixed;
	top: 88px;
	right: 24px;
	display: grid;
	gap: 10px;
	z-index: 2000;
}

.auto-toggle {
	width: 50%;
	justify-self: end;
	padding: 8px 14px;
	border-radius: 999px;
	border: 1px solid rgba(255, 255, 255, 0.35);
	background: rgba(255, 255, 255, 0.08);
	color: #f6f0e6;
	font-size: 14px;
	letter-spacing: 0.5px;
	cursor: pointer;
	transition:
		transform 150ms ease,
		background 150ms ease,
		border-color 150ms ease;
}

.auto-toggle:hover {
	transform: translateY(-1px);
	background: rgba(255, 255, 255, 0.15);
	border-color: rgba(255, 255, 255, 0.5);
}

.auto-toggle.on {
	background: rgba(255, 233, 176, 0.2);
	border-color: rgba(255, 233, 176, 0.8);
	color: #ffe9b0;
}

.control-panel {
	display: grid;
	gap: 10px;
}

.mode-btn {
	display: flex;
	justify-content: space-between;
	align-items: center;
	width: 100%;
	padding: 10px 14px;
	border-radius: 14px;
	background: rgba(0, 0, 0, 0.4); /* 深色半透明背景 */
	border: 1px solid rgba(255, 255, 255, 0.15);
	color: #f6f0e6;
	font-size: 14px;
	cursor: pointer;
	transition: all 0.2s ease;
	backdrop-filter: blur(4px); /* 毛玻璃效果 */
}

.mode-btn:hover {
	background: rgba(255, 255, 255, 0.1);
	border-color: rgba(255, 255, 255, 0.4);
	transform: translateY(-1px);
}

.mode-btn:active {
	transform: scale(0.98);
}

.mode-label {
	opacity: 0.7;
	font-size: 12px;
}

.mode-value {
	font-weight: 600;
	color: #ffe9b0; /* 金色字体 */
	text-shadow: 0 0 8px rgba(255, 233, 176, 0.3);
}

.speed-group {
	display: flex;
	align-items: center;
	gap: 8px;
	padding: 8px 12px;
	border-radius: 14px;
	background: rgba(0, 0, 0, 0.3);
	border: 1px solid rgba(255, 255, 255, 0.2);
}

.volume-group {
	display: grid;
	gap: 8px;
	padding: 10px 12px;
	border-radius: 14px;
	background: rgba(0, 0, 0, 0.3);
	border: 1px solid rgba(255, 255, 255, 0.2);
}

.volume-label {
	display: grid;
	grid-template-columns: auto 1fr auto;
	align-items: center;
	gap: 8px;
	font-size: 13px;
	color: rgba(255, 255, 255, 0.8);
}

.volume-label input[type='range'] {
	width: 120px;
}

.volume-value {
	font-variant-numeric: tabular-nums;
	color: #ffe9b0;
}

.speed-label {
	font-size: 13px;
	color: rgba(255, 255, 255, 0.75);
	letter-spacing: 0.5px;
}

.speed-btn {
	padding: 6px 10px;
	border-radius: 999px;
	border: 1px solid rgba(255, 255, 255, 0.25);
	background: rgba(255, 255, 255, 0.08);
	color: #f6f0e6;
	font-size: 13px;
	cursor: pointer;
	transition:
		background 150ms ease,
		border-color 150ms ease,
		transform 150ms ease;
}

.speed-btn:hover {
	transform: translateY(-1px);
	background: rgba(255, 255, 255, 0.15);
}

.speed-btn.active {
	background: rgba(255, 233, 176, 0.2);
	border-color: rgba(255, 233, 176, 0.8);
	color: #ffe9b0;
}

.reset-btn {
	padding: 8px 14px;
	border-radius: 999px;
	border: 1px solid rgba(255, 255, 255, 0.35);
	background: rgba(255, 255, 255, 0.08);
	color: #f6f0e6;
	font-size: 14px;
	letter-spacing: 0.5px;
	cursor: pointer;
	transition:
		transform 150ms ease,
		background 150ms ease,
		border-color 150ms ease;
}

.reset-btn:hover {
	transform: translateY(-1px);
	background: rgba(255, 255, 255, 0.15);
	border-color: rgba(255, 255, 255, 0.5);
}
.stage {
	position: relative;
	width: min(70vw, 360px);
	height: min(70vw, 360px);
	display: grid;
	place-items: center;
}

.woodfish {
	appearance: none;
	border: none;
	background: transparent;
	cursor: pointer;
	padding: 0;
	transition: transform 120ms ease;
	filter: drop-shadow(0 12px 20px rgba(0, 0, 0, 0.4));
}

.woodfish:active,
.woodfish.is-active {
	transform: scale(0.95);
}

.merit-layer {
	position: absolute;
	inset: 0;
	pointer-events: none;
}

.merit {
	position: absolute;
	top: 48%;
	transform: translate(-50%, 0);
	font-size: 20px;
	font-weight: 600;
	color: #ffe9b0;
	text-shadow: 0 2px 10px rgba(0, 0, 0, 0.6);
	animation: float-up 1s ease-out forwards;
	white-space: nowrap;
}

.merit-merit {
	color: #ffe9b0;
}

.merit-luck {
	color: #9cf5ff;
}

.merit-wisdom {
	color: #c9b6ff;
}

.ripple {
	position: absolute;
	left: 50%;
	top: 50%;
	width: 40px;
	height: 40px;
	border-radius: 50%;
	border: 2px solid rgba(255, 233, 176, 0.9);
	transform: translate(-50%, -50%);
	opacity: 0;
	pointer-events: none;
	animation: ripple-out 520ms ease-out forwards;
	box-shadow: 0 0 12px rgba(255, 233, 176, 0.5);
}

@keyframes float-up {
	0% {
		opacity: 0;
		transform: translate(-50%, 10px) scale(0.9);
	}
	15% {
		opacity: 1;
	}
	100% {
		opacity: 0;
		transform: translate(-50%, -70px) scale(1.05);
	}
}

@keyframes ripple-out {
	0% {
		opacity: 0.9;
		transform: translate(-50%, -50%) scale(0.4);
	}
	100% {
		opacity: 0;
		transform: translate(-50%, -50%) scale(2.8);
	}
}
</style>
