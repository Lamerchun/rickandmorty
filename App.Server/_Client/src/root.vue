<template>
	<div class="max-w-screen-md px-6 mx-auto my-12 flex flex-col gap-12">
		<main>
			<router-view />
		</main>
		<footer class="flex flex-col gap-6">
			<hr />
			<p>
				&copy; 2022
			</p>
		</footer>
	</div>
</template>

<style lang="less">
	@import './_tailwind.less';
</style>

