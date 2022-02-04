<template>
	<ul class="flex flex-row gap-8 select-none">
		<li v-for="(label, index) in labels"
			class="flex flex-row gap-2 cursor-pointer"
			@click="$emit('update:modelValue', index)">
			<div class="relative rounded-full border border-black w-[30px] h-[30px]">
				<div v-if="modelValue == index"
					 class="m-1 absolute inset-0 rounded-full bg-black">
				</div>
			</div>
			<div>
				{{label}}
			</div>
		</li>
	</ul>
</template>

<script>
	export default {
		props: ['modelValue', 'labels']
	}
</script>
