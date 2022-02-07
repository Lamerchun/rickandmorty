<template>
	<ul class="flex flex-row gap-8 select-none">
		<li v-for="(label, index) in labels"
			class="flex flex-row gap-2 cursor-pointer"
			@click="$emit('update:modelValue', index)"
			@mouseenter="hover[index]=true"
			@mouseleave="hover[index]=false">
			<div class="relative rounded-full border border-black w-[30px] h-[30px]">
				<div v-if="modelValue == index"
					 class="m-1 absolute inset-0 rounded-full bg-black">
				</div>
				<div v-if="modelValue != index"
					 class="bg-white m-1 absolute inset-0 rounded-full"
					 :class="{ 'pd:bg-gray-200': hover[index] }">
				</div>
			</div>
			<div>
				{{label}}
			</div>
		</li>
	</ul>
</template>

<script>
	import { ref } from 'vue'

	export default {
		props: ['modelValue', 'labels'],

		setup() {
			const hover = ref({});

			return {
				hover
			}
		}
	}
</script>
