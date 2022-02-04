<template>
	<div class="max-w-screen-md px-6 mx-auto my-12 flex flex-col gap-12">
		<main>
			<router-view />
		</main>
		<footer class="flex flex-col gap-6 text-center">
			<hr />
			<p>
				<a class="pd:hover:underline underline-offset-4" href="https://github.com/Lamerchun/rickandmorty">Fork me on Github</a>
			</p>
		</footer>
	</div>
</template>

<style lang="less">
	@import './_tailwind.less';
</style>

