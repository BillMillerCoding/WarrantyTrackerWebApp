<template>
  <v-card
    elevation="1"
    hover
    rounded="lg"
    class="product-card"
    @click="$emit('click', product)"
  >
    <v-card-item>
      <template #prepend>
        <v-avatar color="primary" variant="tonal" size="44">
          <v-icon>{{ categoryIcon }}</v-icon>
        </v-avatar>
      </template>
      <v-card-title>{{ product.name }}</v-card-title>
      <v-card-subtitle>{{ product.brand }}</v-card-subtitle>
    </v-card-item>

    <v-card-text>
      <v-chip class="mr-2 mb-1" color="info" label size="small" variant="tonal">
        {{ product.category }}
      </v-chip>
      <v-chip class="mb-1" color="success" label size="small" variant="tonal">
        {{ product.warrantyDuration }}
      </v-chip>
      <v-chip
        v-if="product.score != null"
        class="ml-2 mb-1"
        color="purple"
        label
        size="small"
        variant="tonal"
      >
        {{ (product.score * 100).toFixed(1) }}% match
      </v-chip>

      <div class="text-body-2 mt-2 text-grey-darken-1">
        {{ product.warrantyType }}
      </div>

      <div v-if="product.description" class="text-caption mt-1 text-grey">
        {{ product.description }}
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import type { Product } from "~/types";

const props = defineProps<{
  product: Product;
}>();

defineEmits<{
  click: [product: Product];
}>();

const categoryIcon = computed(() => {
  const cat = props.product.category.toLowerCase();
  if (cat.includes("laptop")) return "mdi-laptop";
  if (cat.includes("phone") || cat.includes("smart")) return "mdi-cellphone";
  if (cat.includes("tv")) return "mdi-television";
  if (cat.includes("audio")) return "mdi-headphones";
  if (cat.includes("kitchen")) return "mdi-blender";
  if (cat.includes("appliance") || cat.includes("home"))
    return "mdi-washing-machine";
  return "mdi-package-variant";
});
</script>

<style scoped>
.product-card {
  transition: transform 0.2s;
}
.product-card:hover {
  transform: translateY(-2px);
}
</style>
