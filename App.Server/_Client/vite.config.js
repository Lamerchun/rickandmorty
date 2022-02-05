import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
	plugins: [vue()],

	server: {
		host: true,
		https: {
			pfx: 'devcert.pfx',
			passphrase: '0123456789'
		}
	}
})