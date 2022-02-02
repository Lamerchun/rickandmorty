<template>
	<div class="w-full md:w-[480px] text-center relative">
		<div class="flex flex-row gap-4 items-center shadow-md bg-gray-50 px-6 rounded">
			<div class="select-none grayscale text-lg">🔍</div>

			<input class="w-full py-4 bg-gray-50"
				   ref="inputDom"
				   type="text"
				   spellcheck="false"
				   v-model="modelValue"
				   @keyup.escape="onEscape"
				   @keyup.enter="onEnter"
				   @keyup.up="onUp"
				   @keyup.down="onDown"
				   @input="$emit('input', modelValue);$emit('update:modelValue', modelValue);" />

			<div v-if="modelValue"
				 @click="onClear"
				 class="flex p-4 cursor-pointer items-center justify-center text-xl rounded-full bg-gray-300 w-[30px] h-[30px]">
				<div class="relative bottom-[1px]">✕</div>
			</div>
		</div>

		<ul v-if="suggestions?.length > 0"
			class="p-2 absolute inset-x-0 select-none cursor-pointer bg-white rounded border border-gray-400">
			<li v-for="(name, index) in suggestions"
				class="p-2 rounded"
				:class="{'bg-gray-100': suggestionsIndex == index}"
				@mouseenter="suggestionsIndex = index"
				@click="onClickSuggestion(name)">
				{{name}}
			</li>
		</ul>
	</div>
</template>

<script>
	import { ref, watch } from 'vue'

	export default {
		props: ['modelValue', 'suggestions'],
		emits: ['enter', 'escape', 'suggestion', 'input', 'clear'],

		setup(props, { emit }) {
			const inputDom = ref();
			const suggestionsIndex = ref();

			watch(() => props.suggestions, () => suggestionsIndex.value = null);

			async function handleArrows(change) {
				if (!props.suggestions)
					return;

				if (suggestionsIndex.value == undefined)
					suggestionsIndex.value = -1;

				suggestionsIndex.value += change;

				const maxIndex =
					props.suggestions.length - 1;

				if (suggestionsIndex.value < 0)
					suggestionsIndex.value = maxIndex;

				if (suggestionsIndex.value > maxIndex)
					suggestionsIndex.value = 0;
			}

			return {
				inputDom,
				suggestionsIndex,

				onClear() {
					emit('clear');
					inputDom.value.focus();
				},

				onEscape() {
					emit('escape');
				},

				async onEnter() {
					if (props.suggestions)
						if (suggestionsIndex.value != null)
							emit('update:modelValue', props.suggestions[suggestionsIndex.value]);

					emit('enter');
				},

				async onUp() {
					await handleArrows(-1);
				},

				async onDown() {
					await handleArrows(1);
				},

				async onClickSuggestion(name) {
					emit('suggestion', name);
				}
			}
		}
	}
</script>
