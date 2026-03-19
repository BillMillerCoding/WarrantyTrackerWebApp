<template>
  <v-container fluid class="pa-6">
    <h1 class="text-h4 font-weight-bold mb-2">Product Warranty Lookup</h1>
    <p class="text-body-1 text-grey-darken-1 mb-6">
      Search our database to find warranty information for thousands of products
    </p>

    <!-- Search Bar -->
    <v-text-field
      v-model="searchQuery"
      autofocus
      class="mb-6"
      clearable
      hide-details
      placeholder="Search products by name, brand, or category..."
      prepend-inner-icon="mdi-magnify"
      variant="solo"
      @keyup.enter="performSearch"
      @click:clear="clearSearch"
    />

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <v-progress-circular color="primary" indeterminate />
      <div class="text-body-1 mt-4 text-grey">Searching products...</div>
    </div>

    <!-- Results -->
    <div v-else>
      <div
        v-if="searchQuery && products.length > 0"
        class="text-body-2 text-grey mb-4"
      >
        {{ total }} result{{ total !== 1 ? "s" : "" }} found
      </div>

      <v-row>
        <v-col
          v-for="product in products"
          :key="product.id"
          cols="12"
          sm="6"
          lg="4"
        >
          <ProductCard :product="product" @click="openProductDetail(product)" />
        </v-col>
      </v-row>

      <div
        v-if="hasSearched && searchQuery && products.length === 0"
        class="text-center py-12"
      >
        <v-icon color="grey" size="80">mdi-package-variant-remove</v-icon>
        <div class="text-h6 text-grey mt-4">
          No products found for "{{ searchQuery }}"
        </div>
        <div class="text-body-2 text-grey">Try adjusting your search terms</div>
      </div>

      <div v-if="!searchQuery" class="text-center py-12">
        <v-icon color="primary" size="80">mdi-database-search</v-icon>
        <div class="text-h6 mt-4">Search for a product</div>
        <div class="text-body-2 text-grey">
          Enter a product name, brand, or category to view warranty information
        </div>
      </div>
    </div>

    <!-- Product Detail Dialog -->
    <v-dialog v-model="showDetail" max-width="600">
      <v-card v-if="selectedProduct" rounded="xl">
        <v-card-item>
          <template #prepend>
            <v-avatar color="primary" size="48">
              <v-icon color="white">mdi-package-variant</v-icon>
            </v-avatar>
          </template>
          <v-card-title class="text-h5">{{
            selectedProduct.name
          }}</v-card-title>
          <v-card-subtitle>{{ selectedProduct.brand }}</v-card-subtitle>
          <template #append>
            <v-btn icon variant="text" @click="showDetail = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </template>
        </v-card-item>

        <v-divider />

        <v-card-text class="pa-6">
          <v-row>
            <v-col cols="6">
              <div class="text-caption text-grey">Category</div>
              <div class="text-body-1 font-weight-medium">
                {{ selectedProduct.category }}
              </div>
            </v-col>
            <v-col cols="6">
              <div class="text-caption text-grey">Warranty Duration</div>
              <div class="text-body-1 font-weight-medium">
                {{ selectedProduct.warrantyDuration }}
              </div>
            </v-col>
            <v-col cols="12">
              <div class="text-caption text-grey">Warranty Type</div>
              <div class="text-body-1 font-weight-medium">
                {{ selectedProduct.warrantyType }}
              </div>
            </v-col>
            <v-col v-if="selectedProduct.description" cols="12">
              <div class="text-caption text-grey">Description</div>
              <div class="text-body-1">{{ selectedProduct.description }}</div>
            </v-col>
          </v-row>
        </v-card-text>

        <v-card-actions class="pa-4">
          <v-spacer />
          <v-btn color="primary" variant="flat" @click="showDetail = false">
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import type { Product } from "~/types";
import { searchProducts } from "~/services/productService";

definePageMeta({
  layout: "default",
});

const route = useRoute();

const searchQuery = ref((route.query.q as string) ?? "");
const products = ref<Product[]>([]);
const total = ref(0);
const loading = ref(false);
const hasSearched = ref(false);

const showDetail = ref(false);
const selectedProduct = ref<Product | null>(null);

// Perform search if query param provided
onMounted(async () => {
  if (searchQuery.value) {
    await performSearch();
  }
});

async function performSearch() {
  if (!searchQuery.value?.trim()) return;
  loading.value = true;
  try {
    const result = await searchProducts(searchQuery.value);
    products.value = result.products;
    total.value = result.total;
  } finally {
    loading.value = false;
    hasSearched.value = true;
  }
}

function clearSearch() {
  products.value = [];
  total.value = 0;
  hasSearched.value = false;
}

function openProductDetail(product: Product) {
  selectedProduct.value = product;
  showDetail.value = true;
}
</script>
