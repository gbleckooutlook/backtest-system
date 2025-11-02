// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  
  modules: ['@primevue/nuxt-module'],
  
  primevue: {
    options: {
      ripple: true
    }
  },
  
  css: [
    'primeicons/primeicons.css',
    '~/assets/css/theme.css'
  ],
  
  runtimeConfig: {
    public: {
      apiBaseUrl: process.env.NUXT_PUBLIC_API_BASE_URL || 'http://localhost:5001'
    }
  }
})
