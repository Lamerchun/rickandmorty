import router from './router'
import root from './root.vue'
import { createApp } from 'vue'

const app = createApp(root);
app.use(router);
app.mount('#app');