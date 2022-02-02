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

		<us-input v-model="input"
				  :suggestions="suggestions"
				  @input="onInput"
				  @suggestion="onSuggestion"
				  @escape="onEscape"
				  @enter="onEnter" />

		<div v-if="results && info?.pages > 1">
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
		<div v-if="results"
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
				<tr v-for="entry in results">
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
	import { ref, watch } from 'vue'
	import axios from 'axios'

	import usInput from './components/input.vue'

	export default {
		components: {
			usInput
		},

		setup() {
			const input = ref();
			const page = ref();
			const useLiveApi = ref(true);
			const info = ref();
			const suggestions = ref(true);
			const results = ref();

			const showSuggestions = ref(false);
			const showResults = ref(false);

			watch(() => input.value, updateUI);

			async function updateUI(value) {
				suggestions.value = null;

				if (!value)
					return;

				const apiResults =
					await queryApi(value);

				if (!apiResults)
					return;

				if (showResults.value)
					results.value = apiResults;

				if (!showSuggestions.value)
					return;

				const names =
					apiResults.reduce((a, x) => {
						if (!a.includes(x.name))
							a.push(x.name)

						return a;
					}, []);

				suggestions.value = names.slice(0, 10);
			}

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

			function resetResults() {
				info.value = null;
				results.value = null;
				page.value = 1;
				showResults.value = false;
			}

			return {
				input,
				useLiveApi,
				info,
				page,
				suggestions,
				results,

				async onInput(newValue) {
					resetResults();
					showSuggestions.value = true;
					await updateUI(newValue);
				},

				onEscape() {
					if (!suggestions.value) {
						input.value = null;
						resetResults();
						return;
					}

					suggestions.value = null;
					showSuggestions.value = false;
				},

				async onEnter() {
					showResults.value = true;
					suggestions.value = null;
					showSuggestions.value = false;
					await updateUI(input.value);
				},

				async onPrev() {
					page.value--;

					if (page.value < 1)
						page.value = 1;

					results.value =
						await queryApi(input.value);
				},

				async onNext() {
					page.value++;

					if (page.value > info.value.pages)
						page.value = info.value.pages;

					results.value =
						await queryApi(input.value);
				},

				async onSuggestion(name) {
					input.value = name;
					suggestions.value = null;
					showSuggestions.value = false;
					showResults.value = true;
					await updateUI(input.value);
				}
			}
		}
	}
</script>
