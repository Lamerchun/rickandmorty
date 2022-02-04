<template>
	<ul class="flex flex-row gap-8 select-none">
		<li class="flex flex-row gap-2 cursor-pointer"
			@click="$emit('update:modelValue', true)">
			<div class="relative rounded-full border border-black w-[30px] h-[30px]">
				<div v-if="modelValue"
					 class="m-1 absolute inset-0 rounded-full bg-black">
				</div>
			</div>
			<div>
				GraphQL
			</div>
		</li>
		<li class="flex flex-row gap-2 cursor-pointer"
			@click="$emit('update:modelValue', false)">
			<div class="relative rounded-full border border-black w-[30px] h-[30px]">
				<div v-if="!modelValue"
					 class="m-1 absolute inset-0 rounded-full bg-black">
				</div>
			</div>
			<div>
				REST
			</div>
		</li>
	</ul>
</template>

<script>
	export default {
		props: ['modelValue']
	}
</script>
