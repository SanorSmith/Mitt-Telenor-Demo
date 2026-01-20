/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly BASE_URL: string
  readonly VITE_API_BASE_URL: string
  readonly VITE_CONTENTFUL_SPACE_ID: string
  readonly VITE_CONTENTFUL_ACCESS_TOKEN: string
  readonly VITE_ENV: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
