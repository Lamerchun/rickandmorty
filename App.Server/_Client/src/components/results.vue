<template>
	<div class="w-full border border-gray-400 rounded overflow-hidden">
		<table>
			<tr>
				<th class="w-[60px]">
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
					<div class="w-[60px] h-[60px] bg-center bg-cover rounded relative"
						 :style="{ backgroundImage: `url(${entry.image})` }">
						<div class="text-xs px-1 bg-gray-50 opacity-60 gap-1 rounded absolute right-[2px] bottom-[2px]">
							{{entry.id}}
						</div>
					</div>
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
</template>

<script>
	export default {
		props: ['results']
	}
</script>
