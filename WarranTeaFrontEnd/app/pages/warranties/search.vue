<template>
  <v-container fluid class="pa-6">
    <h1 class="text-h4 font-weight-bold mb-2">Search Warranties</h1>
    <p class="text-body-1 text-grey-darken-1 mb-6">
      Find specific warranties from your collection
    </p>

    <!-- Search -->
    <v-text-field
      v-model="searchQuery"
      class="mb-4"
      clearable
      hide-details
      placeholder="Search by product name, brand, or category..."
      prepend-inner-icon="mdi-magnify"
      variant="solo"
      @update:model-value="performSearch"
    />

    <!-- Filters -->
    <v-row class="mb-4" dense>
      <v-col cols="6" sm="3">
        <v-select
          v-model="filterBrand"
          :items="brandOptions"
          clearable
          density="compact"
          hide-details
          label="Brand"
          variant="outlined"
          @update:model-value="performSearch"
        />
      </v-col>
      <v-col cols="6" sm="3">
        <v-select
          v-model="filterStatus"
          :items="statusOptions"
          clearable
          density="compact"
          hide-details
          label="Status"
          variant="outlined"
          @update:model-value="performSearch"
        />
      </v-col>
    </v-row>

    <!-- Results -->
    <div v-if="loading" class="text-center py-12">
      <v-progress-circular color="primary" indeterminate />
    </div>

    <div
      v-else-if="results.length === 0 && searchQuery"
      class="text-center py-12"
    >
      <v-icon color="grey" size="80">mdi-magnify-close</v-icon>
      <div class="text-h6 text-grey mt-4">
        No results found for "{{ searchQuery }}"
      </div>
    </div>

    <v-row v-else>
      <v-col
        v-for="warranty in results"
        :key="warranty.id"
        cols="12"
        sm="6"
        lg="4"
      >
        <WarrantyCard
          :warranty="warranty"
          @click="navigateTo(`/warranties/${warranty.id}`)"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import type { Warranty } from "~/types";
import { searchWarranties, getWarranties } from "~/services/warrantyService";

definePageMeta({
  layout: "default",
});

const searchQuery = ref("");
const filterBrand = ref("");
const filterStatus = ref("");
const results = ref<Warranty[]>([]);
const loading = ref(true);

const brandOptions = computed(() => {
  const brands = [...new Set(results.value.map((w) => w.brand))];
  return brands.sort();
});
const statusOptions = [
  { title: "Active", value: "active" },
  { title: "Expiring Soon", value: "expiring-soon" },
  { title: "Expired", value: "expired" },
];

onMounted(async () => {
  results.value = await getWarranties();
  loading.value = false;
});

async function performSearch() {
  loading.value = true;
  try {
    results.value = await searchWarranties(searchQuery.value ?? "", {
      brand: filterBrand.value || undefined,
      status: filterStatus.value || undefined,
    });
  } finally {
    loading.value = false;
  }
}
</script>
