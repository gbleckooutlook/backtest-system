import { useNuxtApp } from '#app'

export const useBuefyToast = () => {
  const { $buefy } = useNuxtApp()
  
  return {
    open: (params: any) => {
      if (typeof params === 'string') {
        $buefy.toast.open(params)
      } else {
        $buefy.toast.open(params)
      }
    }
  }
}

export const useBuefyDialog = () => {
  const { $buefy } = useNuxtApp()
  
  return {
    confirm: (params: any) => {
      $buefy.dialog.confirm(params)
    }
  }
}

