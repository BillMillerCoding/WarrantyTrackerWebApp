<template>
  <v-card
    class="warranty-card"
    :class="{
      'border-warning': warranty.status === 'expiring-soon',
      'border-error': warranty.status === 'expired',
    }"
    elevation="2"
    hover
    rounded="xl"
    @click="$emit('click', warranty)"
  >
    <v-card-item>
      <template #prepend>
        <v-avatar :color="statusColor" size="48">
          <v-icon color="white">{{ categoryIcon }}</v-icon>
        </v-avatar>
      </template>

      <v-card-title class="text-h6">{{ warranty.productName }}</v-card-title>
      <v-card-subtitle
        >{{ warranty.brand }} &middot;
        {{ warranty.coverageType }}</v-card-subtitle
      >

      <template #append>
        <v-chip :color="statusColor" label size="small" variant="flat">
          {{ statusLabel }}
        </v-chip>
      </template>
    </v-card-item>

    <v-card-text>
      <v-row dense>
        <v-col cols="6">
          <div class="text-caption text-grey">Purchased</div>
          <div class="text-body-2 font-weight-medium">
            {{ formatDate(warranty.purchaseDate) }}
          </div>
        </v-col>
        <v-col cols="6">
          <div class="text-caption text-grey">Expires</div>
          <div class="text-body-2 font-weight-medium">
            {{ formatDate(warranty.expirationDate) }}
          </div>
        </v-col>
      </v-row>

      <v-progress-linear
        class="mt-3"
        :color="statusColor"
        :model-value="coverageProgress"
        rounded
      />
      <div class="text-caption text-right mt-1 text-grey">
        {{ daysRemaining > 0 ? `${daysRemaining} days remaining` : "Expired" }}
      </div>
    </v-card-text>

    <v-card-actions v-if="warranty.notes">
      <v-icon class="ml-2" size="small">mdi-note-text</v-icon>
      <span class="text-caption text-grey ml-1">{{ warranty.notes }}</span>
    </v-card-actions>
  </v-card>
</template>

<script setup lang="ts">
import type { Warranty } from "~/types";

const props = defineProps<{
  warranty: Warranty;
}>();

defineEmits<{
  click: [warranty: Warranty];
}>();

const statusColor = computed(() => {
  switch (props.warranty.status) {
    case "active":
      return "success";
    case "expiring-soon":
      return "warning";
    case "expired":
      return "error";
    default:
      return "grey";
  }
});

const statusLabel = computed(() => {
  switch (props.warranty.status) {
    case "active":
      return "Active";
    case "expiring-soon":
      return "Expiring Soon";
    case "expired":
      return "Expired";
    default:
      return "Unknown";
  }
});

const categoryIcon = computed(() => {
  const cat = props.warranty.category.toLowerCase();
  if (cat.includes("electronic")) return "mdi-laptop";
  if (cat.includes("appliance")) return "mdi-washing-machine";
  return "mdi-shield-check";
});

const daysRemaining = computed(() => {
  const now = new Date();
  const exp = new Date(props.warranty.expirationDate);
  const diff = exp.getTime() - now.getTime();
  return Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)));
});

const coverageProgress = computed(() => {
  const start = new Date(props.warranty.purchaseDate).getTime();
  const end = new Date(props.warranty.expirationDate).getTime();
  const now = Date.now();
  if (now >= end) return 100;
  if (now <= start) return 0;
  return Math.round(((now - start) / (end - start)) * 100);
});

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}
</script>

<style scoped>
.warranty-card {
  transition:
    transform 0.2s,
    box-shadow 0.2s;
}
.warranty-card:hover {
  transform: translateY(-2px);
}
.border-warning {
  border-left: 4px solid rgb(var(--v-theme-warning));
}
.border-error {
  border-left: 4px solid rgb(var(--v-theme-error));
}
</style>
