<template>
	<div class="flex flex-col gap-12 items-center">
		<h1 class="text-5xl font-bold select-none">Rick &amp; Morty</h1>

		<us-switch v-model="useLiveApi" />

		<us-input v-model="input"
				  :suggestions="suggestions"
				  @input="onInput"
				  @clear="onClear"
				  @suggestion="onSuggestion"
				  @escape="onEscape"
				  @enter="onEnter" />

		<us-pager v-if="results && info?.pages > 1"
				  :page="page"
				  :total="info?.count"
				  :pages="info?.pages"
				  @page="onPage" />

		<us-results v-if="results"
					:results="results" />

		<div v-if="notFound"
			 class="bg-pink-300 px-4 py-2 rounded">
			No records match your search
		</div>
	</div>
</template>

<script>
	import { ref, watch, onMounted, onUnmounted } from 'vue'
	import axios from 'axios'

	import usSwitch from './components/switch.vue'
	import usInput from './components/input.vue'
	import usPager from './components/pager.vue'
	import usResults from './components/results.vue'

	export default {
		components: {
			usSwitch,
			usInput,
			usPager,
			usResults
		},

		setup() {
			const useLiveApi = ref(true);
			const input = ref();

			const suggestions = ref(true);
			const showSuggestions = ref(false);

			const page = ref();
			const info = ref();
			const results = ref();
			const showResults = ref(false);
			const notFound = ref(false);

			watch(() => input.value, updateUI);

			async function updateUI(value) {
				if (!value) {
					suggestions.value = null;
					notFound.value = false;
					return;
				}

				const apiResults =
					await queryApi(value);

				if (!apiResults) {
					suggestions.value = null;
					notFound.value = true;
					return;
				}

				notFound.value = false;

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

			function onBodyClick() {
				suggestions.value = null;
				showSuggestions.value = false;
			}

			onMounted(() => {
				document.addEventListener("click", onBodyClick);
			});

			onUnmounted(() => {
				document.removeEventListener("click", onBodyClick);
			});

			return {
				useLiveApi,
				input,

				suggestions,

				page,
				info,

				notFound,
				results,

				async onInput(newValue) {
					resetResults();
					showSuggestions.value = true;
				},

				async onClear() {
					resetResults();
					input.value = null;
					suggestions.value = null;
					showSuggestions.value = false;
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
