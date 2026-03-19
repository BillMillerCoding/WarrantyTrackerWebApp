<template>
  <v-row dense>
    <v-col cols="12" sm="6" md="3">
      <v-card color="success" variant="tonal" rounded="lg">
        <v-card-text class="text-center">
          <v-icon size="32">mdi-shield-check</v-icon>
          <div class="text-h4 font-weight-bold mt-1">{{ activeCount }}</div>
          <div class="text-body-2">Active</div>
        </v-card-text>
      </v-card>
    </v-col>
    <v-col cols="12" sm="6" md="3">
      <v-card color="warning" variant="tonal" rounded="lg">
        <v-card-text class="text-center">
          <v-icon size="32">mdi-alert</v-icon>
          <div class="text-h4 font-weight-bold mt-1">
            {{ expiringSoonCount }}
          </div>
          <div class="text-body-2">Expiring Soon</div>
        </v-card-text>
      </v-card>
    </v-col>
    <v-col cols="12" sm="6" md="3">
      <v-card color="error" variant="tonal" rounded="lg">
        <v-card-text class="text-center">
          <v-icon size="32">mdi-shield-off</v-icon>
          <div class="text-h4 font-weight-bold mt-1">{{ expiredCount }}</div>
          <div class="text-body-2">Expired</div>
        </v-card-text>
      </v-card>
    </v-col>
    <v-col cols="12" sm="6" md="3">
      <v-card color="info" variant="tonal" rounded="lg">
        <v-card-text class="text-center">
          <v-icon size="32">mdi-clipboard-list</v-icon>
          <div class="text-h4 font-weight-bold mt-1">{{ totalCount }}</div>
          <div class="text-body-2">Total</div>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
import type { Warranty } from "~/types";

const props = defineProps<{
  warranties: Warranty[];
}>();

const activeCount = computed(
  () => props.warranties.filter((w) => w.status === "active").length,
);
const expiringSoonCount = computed(
  () => props.warranties.filter((w) => w.status === "expiring-soon").length,
);
const expiredCount = computed(
  () => props.warranties.filter((w) => w.status === "expired").length,
);
const totalCount = computed(() => props.warranties.length);
</script>
