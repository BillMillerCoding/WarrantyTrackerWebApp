<template>
  <v-row align="center" dense>
    <v-col cols="12" md="5">
      <v-text-field
        :model-value="search"
        clearable
        density="compact"
        hide-details
        placeholder="Search your warranties..."
        prepend-inner-icon="mdi-magnify"
        variant="outlined"
        @update:model-value="$emit('update:search', $event ?? '')"
      />
    </v-col>
    <v-col cols="6" sm="3" md="2">
      <v-select
        :model-value="filterBrand"
        :items="brandOptions"
        clearable
        density="compact"
        hide-details
        label="Brand"
        variant="outlined"
        @update:model-value="$emit('update:filterBrand', $event ?? '')"
      />
    </v-col>
    <v-col cols="6" sm="3" md="2">
      <v-select
        :model-value="filterStatus"
        :items="statusOptions"
        clearable
        density="compact"
        hide-details
        label="Status"
        variant="outlined"
        @update:model-value="$emit('update:filterStatus', $event ?? '')"
      />
    </v-col>
    <v-col cols="6" sm="3" md="2">
      <v-select
        :model-value="sortBy"
        :items="sortOptions"
        density="compact"
        hide-details
        label="Sort By"
        variant="outlined"
        @update:model-value="$emit('update:sortBy', $event)"
      />
    </v-col>
    <v-col cols="auto">
      <v-btn-toggle
        :model-value="viewMode"
        color="primary"
        density="compact"
        mandatory
        variant="outlined"
        @update:model-value="$emit('update:viewMode', $event)"
      >
        <v-btn icon value="grid">
          <v-icon>mdi-view-grid</v-icon>
        </v-btn>
        <v-btn icon value="list">
          <v-icon>mdi-view-list</v-icon>
        </v-btn>
      </v-btn-toggle>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
defineProps<{
  search: string;
  filterBrand: string;
  filterStatus: string;
  sortBy: string;
  viewMode: "grid" | "list";
  brandOptions: string[];
}>();

defineEmits<{
  "update:search": [value: string];
  "update:filterBrand": [value: string];
  "update:filterStatus": [value: string];
  "update:sortBy": [value: string];
  "update:viewMode": [value: "grid" | "list"];
}>();

const statusOptions = [
  { title: "Active", value: "active" },
  { title: "Expiring Soon", value: "expiring-soon" },
  { title: "Expired", value: "expired" },
];

const sortOptions = [
  { title: "Expiration Date", value: "expiration" },
  { title: "Purchase Date", value: "purchase" },
  { title: "Product Name", value: "name" },
  { title: "Brand", value: "brand" },
];
</script>
