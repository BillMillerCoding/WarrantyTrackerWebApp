// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },

  modules: ["vuetify-nuxt-module"],

  nitro: {
    devProxy: {
      "/api/": {
        target: "http://localhost:5000/api/",
        changeOrigin: true,
      },
    },
  },

  vuetify: {
    vuetifyOptions: {
      theme: {
        defaultTheme: "warrantea",
        themes: {
          warrantea: {
            dark: false,
            colors: {
              primary: "#1565C0",
              secondary: "#FFC107",
              accent: "#4CAF50",
              background: "#F5F5F5",
              surface: "#FFFFFF",
              error: "#D32F2F",
              warning: "#FF9800",
              info: "#2196F3",
              success: "#4CAF50",
            },
          },
        },
      },
    },
  },

  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || "/api",
    },
  },

  css: ["@mdi/font/css/materialdesignicons.min.css"],

  app: {
    head: {
      title: "WarranTea - Warranties Made Easy",
      meta: [
        {
          name: "description",
          content: "Track and manage your product warranties with ease.",
        },
      ],
    },
  },
});
