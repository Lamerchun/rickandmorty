<template>
	<div class="flex flex-col gap-12 items-center">
		<h1 class="text-5xl font-bold select-none">Rick &amp; Morty</h1>
		<div>
			<ul class="flex flex-row gap-8 select-none">
				<li class="flex flex-row gap-2 cursor-pointer"
					@click="useLiveApi = true">
					<div class="relative rounded-full border border-black w-[30px] h-[30px]">
						<div v-if="useLiveApi"
							 class="m-1 absolute inset-0 rounded-full bg-black">
						</div>
					</div>
					<div>
						use live API
					</div>
				</li>
				<li class="flex flex-row gap-2 cursor-pointer"
					@click="useLiveApi = false">
					<div class="relative rounded-full border border-black w-[30px] h-[30px]">
						<div v-if="!useLiveApi"
							 class="m-1 absolute inset-0 rounded-full bg-black">
						</div>
					</div>
					<div>
						use local API
					</div>
				</li>
			</ul>
		</div>
		<div class="w-full md:w-[480px] text-center relative">
			<input class="w-full shadow-md bg-gray-50 px-6 py-4 rounded"
				   type="text"
				   spellcheck="false"
				   v-model="input"
				   @keyup.escape="onEscape"
				   @keyup.enter="onEnter"
				   @keyup.up="onUp"
				   @keyup.down="onDown"
				   @input="onInput" />

			<ul v-if="suggestions?.length > 0"
				class="p-2 absolute inset-x-0 select-none cursor-pointer bg-white rounded border border-gray-400">
				<li v-for="(name, index) in suggestions"
					class="p-2 rounded"
					:class="{'bg-gray-100': suggestionsIndex == index}"
					@click="onClickSuggestion(name)">
					{{name}}
				</li>
			</ul>
		</div>
		<div v-if="info?.pages > 1">
			<div class="flex flex-col text-center gap-1 items-center md:flex-row md:gap-6">
				<div>
					Results: {{info.count}}
				</div>
				<div>
					Page {{page}} of {{info.pages}}
				</div>
				<div class="flex flex-row gap-2 justify-center">
					<div v-if="info.prev"
						 class="bg-blue-500 text-white py-2 px-4 rounded select-none cursor-pointer"
						 @click="onPrev">
						Prev
					</div>
					<div v-else
						 class="bg-gray-300 text-gray-500 py-2 px-4 rounded select-none">
						Prev
					</div>

					<div v-if="info.next"
						 class="bg-blue-500 text-white py-2 px-4 rounded select-none cursor-pointer"
						 @click="onNext">
						Next
					</div>
					<div v-else
						 class="bg-gray-300 text-gray-500 py-2 px-4 rounded select-none">
						Next
					</div>
				</div>
			</div>
		</div>
		<div v-if="entries"
			 class="w-full border border-black rounded-md overflow-hidden">
			<table>
				<tr>
					<th>
						ID
					</th>
					<th>
						Name
					</th>
					<th class="text-center hidden sm:table-cell">
						Episodes
					</th>
					<th class="hidden sm:table-cell">
						Origin
					</th>
				</tr>
				<tr v-for="entry in entries">
					<td>
						{{entry.id}}
					</td>
					<td>
						<ul class="flex flex-col gap-1">
							<li>
								{{entry.name}}
							</li>
							<li>
								{{entry.species}}, {{entry.status}}

							</li>
						</ul>
					</td>
					<td class="text-center hidden sm:table-cell">
						{{entry.episode?.length}}
					</td>
					<td class="hidden sm:table-cell">
						{{entry.origin.name}}
					</td>
				</tr>
			</table>
		</div>
	</div>
</template>

<script>
	import { ref } from 'vue'
	import axios from 'axios'

	export default {
		setup() {
			const input = ref();
			const page = ref();
			const useLiveApi = ref(true);
			const info = ref();
			const suggestions = ref(true);
			const suggestionsIndex = ref();
			const entries = ref();

			async function queryApi(name) {
				let apiUrl = 'https://rickandmortyapi.com/api/character/';

				if (!useLiveApi.value)
					apiUrl = '/Api/Character';

				try {
					const response = await axios.get(apiUrl, {
						params: {
							name: name,
							page: page.value
						}
					});

					info.value = response.data.info;
					return response.data.results;
				}
				catch {
					return null;
				}
			}

			function resetSuggestions() {
				suggestions.value = null;
				suggestionsIndex.value = null;
			}

			function resetEntries() {
				info.value = null;
				entries.value = null;
				page.value = 1;
			}

			async function showEntries(name) {
				resetEntries();

				entries.value =
					await queryApi(name);

				resetSuggestions();
			}

			async function handleArrows(change) {
				if (!suggestions.value)
					return;

				if (suggestionsIndex.value == undefined)
					suggestionsIndex.value = -1;

				suggestionsIndex.value += change;

				const maxIndex =
					suggestions.value.length - 1;

				if (suggestionsIndex.value < 0)
					suggestionsIndex.value = maxIndex;

				if (suggestionsIndex.value > maxIndex)
					suggestionsIndex.value = 0;
			}

			return {
				input,
				useLiveApi,
				info,
				page,
				suggestions,
				suggestionsIndex,
				entries,

				onEscape() {
					if (!suggestions.value) {
						input.value = null;
						resetEntries();
						return;
					}

					resetSuggestions();
				},

				async onEnter() {
					if (suggestionsIndex.value != null)
						input.value = suggestions.value[suggestionsIndex.value];

					resetSuggestions();
					await showEntries(input.value);
				},

				async onUp() {
					await handleArrows(-1);
				},

				async onDown() {
					await handleArrows(1);
				},

				async onPrev() {
					page.value--;
					if (page.value < 1)
						page.value = 1;

					entries.value =
						await queryApi(input.value);
				},

				async onNext() {
					page.value++;

					entries.value =
						await queryApi(input.value);
				},

				async onDown() {
					await handleArrows(1);
				},

				async onInput() {
					if (!input.value) {
						resetSuggestions();
						return;
					}

					resetEntries();

					const results =
						await queryApi(input.value);

					if (!results) {
						resetSuggestions();
						return;
					}

					const names =
						results.reduce((a, x) => {
							if (!a.includes(x.name))
								a.push(x.name)

							return a;
						}, []);

					suggestions.value = names.slice(0, 10);
				},

				async onClickSuggestion(name) {
					input.value = name;
					suggestions.value = null;
					await showEntries(name);
				}
			}
		}
	}
</script>
