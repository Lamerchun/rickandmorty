import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
	plugins: [vue()],

	server: {
		host: true,
		proxy: {
			'/Api': {
				target: 'https://localhost:44386',
				changeOrigin: false,
				secure: false
			}
		},
		https: {
			pfx: 'devcert.pfx',
			passphrase: '0123456789'
		}
	}
})