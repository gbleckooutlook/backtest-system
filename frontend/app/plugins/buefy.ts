import { defineNuxtPlugin } from '#app'
import Buefy from '@ntohq/buefy-next'

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.use(Buefy, {
    defaultIconPack: 'mdi'
  })
  
  // Expor $buefy para uso em composables
  return {
    provide: {
      buefy: nuxtApp.vueApp.config.globalProperties.$buefy
    }
  }
})


