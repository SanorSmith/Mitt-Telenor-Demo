import { ref } from 'vue'
// Disabled workbox-window import to fix build issues
// PWA will auto-register via VitePWA injectRegister: 'auto'
// import { useRegisterSW } from 'virtual:pwa-register/vue'

export function usePWA() {
  const needRefresh = ref(false)
  const offlineReady = ref(false)
  const updateAvailable = ref(false)

  // Service worker auto-registers via VitePWA config
  // Manual update functionality disabled for now
  
  async function updateApp() {
    // Reload page to get latest version
    window.location.reload()
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
