import { ref } from 'vue'
import { useRegisterSW } from 'virtual:pwa-register/vue'

export function usePWA() {
  const needRefresh = ref(false)
  const offlineReady = ref(false)
  const updateAvailable = ref(false)

  const { updateServiceWorker } = useRegisterSW({
    onNeedRefresh() {
      needRefresh.value = true
      updateAvailable.value = true
    },
    onOfflineReady() {
      offlineReady.value = true
    }
  })

  async function updateApp() {
    await updateServiceWorker()
    needRefresh.value = false
    updateAvailable.value = false
  }

  function checkForUpdates() {
    if ('serviceWorker' in navigator) {
      navigator.serviceWorker.getRegistration().then((registration) => {
        if (registration) {
          registration.update()
        }
      })
    }
  }

  return {
    needRefresh,
    offlineReady,
    updateAvailable,
    updateApp,
    checkForUpdates
  }
}
