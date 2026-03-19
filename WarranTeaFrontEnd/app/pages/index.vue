<template>
  <div>
    <!-- Hero Section -->
    <v-sheet color="primary" class="hero-section">
      <v-container class="py-16">
        <v-row align="center" justify="center">
          <v-col cols="12" class="text-center">
            <h1 class="text-h2 font-weight-bold text-white mb-4">
              Warranties Made Easy
            </h1>
            <p
              class="text-h6 text-blue-lighten-4 mb-8 mx-auto"
              style="max-width: 600px"
            >
              Track, manage, and never miss a warranty claim again. WarranTea
              keeps all your product warranties in one place — powered by AI.
            </p>
          </v-col>
        </v-row>

        <v-row justify="center">
          <v-col cols="12" sm="8" md="5">
            <v-card class="pa-6" elevation="8" rounded="xl">
              <v-img
                class="mx-auto mb-6"
                color="primary"
                height="200"
                rounded="lg"
                src="https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=500&q=80"
                cover
              >
                <template #placeholder>
                  <v-sheet
                    color="primary-lighten-3"
                    height="100%"
                    class="d-flex align-center justify-center"
                  >
                    <v-icon color="white" size="64">mdi-shield-check</v-icon>
                  </v-sheet>
                </template>
              </v-img>

              <v-btn
                v-if="!isAuthenticated"
                block
                color="secondary"
                size="x-large"
                rounded="lg"
                class="text-h6 font-weight-bold"
                @click="navigateTo('/register')"
              >
                SIGN UP
              </v-btn>
              <v-btn
                v-else
                block
                color="secondary"
                size="x-large"
                rounded="lg"
                class="text-h6 font-weight-bold"
                @click="navigateTo('/dashboard')"
              >
                GO TO DASHBOARD
              </v-btn>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-sheet>

    <!-- Features Section -->
    <v-container class="py-16">
      <v-row justify="center" class="mb-8">
        <v-col cols="12" class="text-center">
          <h2 class="text-h4 font-weight-bold mb-2">Everything You Need</h2>
          <p class="text-body-1 text-grey-darken-1">
            From receipt scanning to AI-powered assistance — WarranTea has you
            covered.
          </p>
        </v-col>
      </v-row>

      <v-row justify="center">
        <v-col cols="12" md="5">
          <v-card elevation="2" rounded="xl" height="100%">
            <v-img
              src="https://images.unsplash.com/photo-1586281380349-632531db7ed4?w=500&q=80"
              height="250"
              cover
            >
              <template #placeholder>
                <v-sheet
                  color="grey-lighten-3"
                  height="100%"
                  class="d-flex align-center justify-center"
                >
                  <v-icon size="64" color="grey">mdi-image</v-icon>
                </v-sheet>
              </template>
            </v-img>
            <v-card-text class="pa-6">
              <h3 class="text-h5 font-weight-bold mb-2">Scan &amp; Extract</h3>
              <p class="text-body-1 text-grey-darken-1">
                Snap a photo of your receipt or warranty card. Our OCR
                technology extracts the text and auto-fills warranty details for
                you.
              </p>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" md="5">
          <v-row dense>
            <v-col cols="12">
              <v-card elevation="2" rounded="xl">
                <v-card-text class="pa-6">
                  <v-icon class="mb-2" color="primary" size="40"
                    >mdi-robot</v-icon
                  >
                  <h3 class="text-h6 font-weight-bold mb-1">
                    AI Warranty Assistant
                  </h3>
                  <p class="text-body-2 text-grey-darken-1">
                    Ask questions about your warranties, get coverage details
                    explained, and extract warranty info from text — all with
                    our AI assistant.
                  </p>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12">
              <v-card elevation="2" rounded="xl">
                <v-card-text class="pa-6">
                  <v-icon class="mb-2" color="warning" size="40"
                    >mdi-magnify</v-icon
                  >
                  <h3 class="text-h6 font-weight-bold mb-1">
                    Smart Product Lookup
                  </h3>
                  <p class="text-body-2 text-grey-darken-1">
                    Search our database of product warranties using AI-powered
                    hybrid search. Find coverage details before you even make a
                    purchase.
                  </p>
                </v-card-text>
              </v-card>
            </v-col>
            <v-col cols="12">
              <v-card elevation="2" rounded="xl">
                <v-card-text class="pa-6">
                  <v-icon class="mb-2" color="success" size="40"
                    >mdi-shield-star</v-icon
                  >
                  <h3 class="text-h6 font-weight-bold mb-1">
                    All In One Place
                  </h3>
                  <p class="text-body-2 text-grey-darken-1">
                    No more digging through drawers for receipts. Every
                    warranty, every product — organized on your dashboard and
                    searchable.
                  </p>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
    </v-container>

    <!-- Help Footer -->
    <v-sheet class="bg-grey-darken-4 text-white pa-6">
      <v-row>
        <v-col cols="12" md="4">
          <h4 class="text-h6 mb-2">HELP</h4>
          <div class="text-body-2">Phone: (555) 123-4567</div>
          <div class="text-body-2">Email: support@warrantea.com</div>
        </v-col>
        <v-col cols="12" md="4" class="text-center">
          <div class="text-body-2 text-grey-lighten-1">
            &copy; {{ new Date().getFullYear() }} WarranTea. All rights
            reserved.
          </div>
        </v-col>
        <v-col cols="12" md="4" class="text-right">
          <v-btn icon size="small" variant="text">
            <v-icon>mdi-twitter</v-icon>
          </v-btn>
          <v-btn icon size="small" variant="text">
            <v-icon>mdi-facebook</v-icon>
          </v-btn>
          <v-btn icon size="small" variant="text">
            <v-icon>mdi-instagram</v-icon>
          </v-btn>
        </v-col>
      </v-row>
    </v-sheet>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: "default",
});

const { isAuthenticated } = useAuth();
</script>

<style scoped>
.hero-section {
  background: linear-gradient(135deg, #1565c0 0%, #0d47a1 100%);
}
</style>
