<template>
	<div class="flex flex-col text-center gap-1 items-center md:flex-row md:gap-6">
		<div>
			Results: {{total}}
		</div>
		<div>
			Page {{page}} of {{pages}}
		</div>
		<div class="flex flex-row gap-2 justify-center">
			<div v-if="hasPrev"
				 class="bg-blue-500 pd:hover:bg-blue-700 text-white py-2 px-4 rounded select-none cursor-pointer"
				 @click="onPrev">
				Prev
			</div>
			<div v-else
				 class="bg-gray-300 text-gray-500 py-2 px-4 rounded select-none">
				Prev
			</div>

			<div v-if="hasNext"
				 class="bg-blue-500 pd:hover:bg-blue-700 text-white py-2 px-4 rounded select-none cursor-pointer"
				 @click="onNext">
				Next
			</div>
			<div v-else
				 class="bg-gray-300 text-gray-500 py-2 px-4 rounded select-none">
				Next
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'

	export default {
		props: ['page', 'pages', 'total'],
		emits: ['page'],

		setup(props, { emit }) {
			const hasPrev =
				computed(() => (props.page - 1) >= 1);

			const hasNext =
				computed(() => (props.page + 1) <= props.pages);

			return {
				hasPrev,
				hasNext,

				onPrev() {
					if (hasPrev.value)
						emit('page', props.page - 1);
				},

				onNext() {
					if (hasNext.value)
						emit('page', props.page + 1);
				}
			}
		}
	}
</script>
