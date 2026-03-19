<template>
  <v-container fluid class="pa-6">
    <!-- Greeting -->
    <div class="mb-6">
      <h1 class="text-h4 text-secondary font-weight-bold">
        Hello, {{ currentUser?.name ?? "User" }}
      </h1>
      <p class="text-body-1 text-grey-darken-1 mt-1">
        Browse Your Warranties and Registered Products
      </p>
    </div>

    <!-- Stats Summary -->
    <WarrantyStats :warranties="warranties" class="mb-6" />

    <!-- Tabs -->
    <v-tabs v-model="activeTab" color="primary" class="mb-4">
      <v-tab value="warranties">Your Warranties</v-tab>
      <v-tab value="products">Products</v-tab>
      <v-tab value="expiring">Expiring Soon</v-tab>
      <v-tab value="expired">Expired</v-tab>
    </v-tabs>

    <!-- Filter Bar + Add Button -->
    <v-row align="center" class="mb-4">
      <v-col>
        <WarrantyFilterBar
          v-model:search="search"
          v-model:filter-brand="filterBrand"
          v-model:filter-status="filterStatus"
          v-model:sort-by="sortBy"
          v-model:view-mode="viewMode"
          :brand-options="brandOptions"
        />
      </v-col>
      <v-col cols="auto">
        <v-btn
          color="primary"
          icon
          size="large"
          @click="navigateTo('/warranties/add')"
        >
          <v-icon size="32">mdi-plus</v-icon>
          <v-tooltip activator="parent" location="left">Add Warranty</v-tooltip>
        </v-btn>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <v-progress-circular color="primary" indeterminate size="48" />
      <div class="text-body-1 mt-4 text-grey">Loading your warranties...</div>
    </div>

    <!-- Warranty Cards -->
    <v-window v-else v-model="activeTab">
      <!-- Your Warranties Tab -->
      <v-window-item value="warranties">
        <div v-if="filteredWarranties.length === 0" class="text-center py-12">
          <v-icon color="grey-lighten-1" size="80"
            >mdi-shield-off-outline</v-icon
          >
          <div class="text-h6 text-grey mt-4">No warranties found</div>
          <v-btn
            class="mt-4"
            color="primary"
            @click="navigateTo('/warranties/add')"
          >
            Add Your First Warranty
          </v-btn>
        </div>

        <!-- Grid View -->
        <v-row v-else-if="viewMode === 'grid'">
          <v-col
            v-for="warranty in filteredWarranties"
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

        <!-- List View -->
        <v-list v-else lines="three" class="bg-transparent">
          <v-list-item
            v-for="warranty in filteredWarranties"
            :key="warranty.id"
            class="mb-2 bg-surface rounded-lg"
            elevation="1"
            @click="navigateTo(`/warranties/${warranty.id}`)"
          >
            <template #prepend>
              <v-avatar :color="getStatusColor(warranty.status)" class="mr-4">
                <v-icon color="white">mdi-shield-check</v-icon>
              </v-avatar>
            </template>

            <v-list-item-title class="font-weight-bold">
              {{ warranty.productName }}
            </v-list-item-title>
            <v-list-item-subtitle>
              {{ warranty.brand }} &middot; {{ warranty.coverageType }}
            </v-list-item-subtitle>
            <v-list-item-subtitle>
              Expires: {{ formatDate(warranty.expirationDate) }}
            </v-list-item-subtitle>

            <template #append>
              <v-chip
                :color="getStatusColor(warranty.status)"
                label
                size="small"
                variant="flat"
              >
                {{
                  warranty.status === "expiring-soon"
                    ? "Expiring"
                    : warranty.status
                }}
              </v-chip>
            </template>
          </v-list-item>
        </v-list>
      </v-window-item>

      <!-- Products Tab -->
      <v-window-item value="products">
        <v-row>
          <v-col
            v-for="warranty in filteredWarranties"
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
      </v-window-item>

      <!-- Expiring Soon Tab -->
      <v-window-item value="expiring">
        <v-row>
          <v-col
            v-for="warranty in expiringSoon"
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
        <div v-if="expiringSoon.length === 0" class="text-center py-12">
          <v-icon color="success" size="80">mdi-check-circle</v-icon>
          <div class="text-h6 text-grey mt-4">No warranties expiring soon!</div>
        </div>
      </v-window-item>

      <!-- Expired Tab -->
      <v-window-item value="expired">
        <v-row>
          <v-col
            v-for="warranty in expiredWarranties"
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
        <div v-if="expiredWarranties.length === 0" class="text-center py-12">
          <v-icon color="success" size="80">mdi-shield-check</v-icon>
          <div class="text-h6 text-grey mt-4">No expired warranties</div>
        </div>
      </v-window-item>
    </v-window>
  </v-container>
</template>

<script setup lang="ts">
import type { Warranty } from "~/types";
import { getWarranties } from "~/services/warrantyService";

definePageMeta({
  layout: "default",
});

const { currentUser } = useAuth();

const warranties = ref<Warranty[]>([]);
const loading = ref(true);

const activeTab = ref("warranties");
const search = ref("");
const filterBrand = ref("");
const filterStatus = ref("");
const sortBy = ref("expiration");
const viewMode = ref<"grid" | "list">("grid");

// Load data
onMounted(async () => {
  try {
    warranties.value = await getWarranties();
  } finally {
    loading.value = false;
  }
});

const brandOptions = computed(() => {
  const brands = [...new Set(warranties.value.map((w) => w.brand))];
  return brands.sort();
});

const filteredWarranties = computed(() => {
  let result = [...warranties.value];

  if (search.value) {
    const q = search.value.toLowerCase();
    result = result.filter(
      (w) =>
        w.productName.toLowerCase().includes(q) ||
        w.brand.toLowerCase().includes(q),
    );
  }

  if (filterBrand.value) {
    result = result.filter((w) => w.brand === filterBrand.value);
  }

  if (filterStatus.value) {
    result = result.filter((w) => w.status === filterStatus.value);
  }

  result.sort((a, b) => {
    switch (sortBy.value) {
      case "expiration":
        return (
          new Date(a.expirationDate).getTime() -
          new Date(b.expirationDate).getTime()
        );
      case "purchase":
        return (
          new Date(b.purchaseDate).getTime() -
          new Date(a.purchaseDate).getTime()
        );
      case "name":
        return a.productName.localeCompare(b.productName);
      case "brand":
        return a.brand.localeCompare(b.brand);
      default:
        return 0;
    }
  });

  return result;
});

const expiringSoon = computed(() =>
  warranties.value.filter((w) => w.status === "expiring-soon"),
);

const expiredWarranties = computed(() =>
  warranties.value.filter((w) => w.status === "expired"),
);

function getStatusColor(status: string): string {
  switch (status) {
    case "active":
      return "success";
    case "expiring-soon":
      return "warning";
    case "expired":
      return "error";
    default:
      return "grey";
  }
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}
</script>
