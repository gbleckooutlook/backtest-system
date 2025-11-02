export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBaseUrl as string

  const api = $fetch.create({
    baseURL,
    credentials: 'include',
    headers: {
      'Accept': 'application/json'
    }
  })

  return {
    api,
    baseURL
  }
}

