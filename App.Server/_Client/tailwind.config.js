module.exports = {
	content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
	theme: {
		extend: {
			screens: {
				'pd': { 'raw': '(hover: hover)' },
			}
		},
	},
	variants: {
		extend: {},
	}
}
