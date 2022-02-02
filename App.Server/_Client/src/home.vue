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

		<us-pager v-if="results && info?.pages > 1"
				  :page="page"
				  :total="info?.count"
				  :pages="info?.pages"
				  @page="onPage" />

		<div v-if="results"
			 class="w-full border border-black rounded-md overflow-hidden">
			<us-results :results="results" />
		</div>
	</div>
</template>

<script>
	import { ref, watch } from 'vue'
	import axios from 'axios'

	import usInput from './components/input.vue'
	import usPager from './components/pager.vue'
	import usResults from './components/results.vue'

	export default {
		components: {
			usInput,
			usPager,
			usResults
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

				async onPage(pageNumber) {
					page.value = pageNumber;

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
