import pageHome from './home.vue'
import pageCharacter from './character.vue'
import { createWebHistory, createRouter } from "vue-router"

const routes = [
	{ path: '', component: pageHome },

	{
		path: '/:id',
		component: pageCharacter,
		props: route => ({
			id: route.params.id
		})
	}
]

const router = createRouter({
	history: createWebHistory(),
	routes,
});

export default router;