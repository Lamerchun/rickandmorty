<template>
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
</template>

<script>
	export default {
		props: ['results']
	}
</script>
