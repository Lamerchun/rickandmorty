<template>
	<div class="flex flex-col gap-12 items-center">
		<h1 class="text-5xl font-bold select-none">Rick &amp; Morty</h1>

		<div class="flex flex-col gap-4 items-center">
			<us-switch v-model="apiIndex"
					   :labels="['GraphQL', 'REST']" />
			<us-switch v-model="hostIndex"
					   :labels="['live', 'proxy']" />
		</div>

		<us-input v-model="input"
				  :suggestions="suggestions"
				  @input="onInput"
				  @clear="onClear"
				  @suggestion="onSuggestion"
				  @escape="onEscape"
				  @enter="onEnter" />

		<us-pager v-if="showResults && results && info?.pages > 1"
				  :page="page"
				  :total="info?.count"
				  :pages="info?.pages"
				  @page="onPage" />

		<us-results v-if="showResults && results"
					:results="results" />

		<div v-if="notFound"
			 class="bg-pink-300 px-4 py-2 rounded">
			No records match your search
		</div>
	</div>
</template>

<script>
	import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
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
			const apiIndex = ref(0);
			const hostIndex = ref(0);

			const useGraphQL = computed(() => apiIndex.value == 0);
			const useLiveApi = computed(() => hostIndex.value == 0);

			const input = ref();
			const inputTimeout = ref();

			const suggestions = ref(true);
			const showSuggestions = ref(false);

			const page = ref();
			const info = ref();
			const results = ref();
			const showResults = ref(false);
			const notFound = ref(false);

			watch(() => input.value, newValue => {
				if (inputTimeout.value)
					clearTimeout(inputTimeout.value);

				inputTimeout.value = setTimeout(() => {
					updateUI(newValue);
				}, 300);
			});

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
				if (useGraphQL.value) return await queryGraphQL(name);
				else return await queryREST(name);
			}

			async function queryREST(name) {
				let apiUrl = 'https://rickandmortyapi.com/api/character';

				if (!useLiveApi.value)
					apiUrl = '/api/character';

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

			async function queryGraphQL(name) {
				let apiUrl = 'https://rickandmortyapi.com/graphql';

				if (!useLiveApi.value)
					apiUrl = '/graphql';

				try {
					const response = await axios.post(apiUrl, {
						query:
							`
									{
									  characters(page: ${page.value}, filter: { name: "${name}" }) {
										info {
										  count
										  pages
										}
										results {
										  id
										  name
										  image
										  species
										  status
										  episode {
											name
										  }
										  origin {
											name
										  }
										}
									  }
									}
								`
					});

					info.value = response.data.data.characters.info;
					return response.data.data.characters.results;
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
				apiIndex,
				hostIndex,
				input,

				suggestions,

				page,
				info,

				notFound,
				results,
				showResults,

				async onInput() {
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

				async onEnter({ input }) {
					showResults.value = true;
					suggestions.value = null;
					showSuggestions.value = false;
					input.blur();
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
					results.value = null;
					showResults.value = true;
					await updateUI(input.value);
				}
			}
		}
	}
</script>
