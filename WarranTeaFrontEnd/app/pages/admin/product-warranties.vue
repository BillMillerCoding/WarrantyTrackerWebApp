<template>
  <v-container class="py-6" fluid>
    <v-row>
      <v-col cols="12">
        <div class="d-flex align-center mb-4">
          <v-icon class="mr-2" color="primary" size="28"
            >mdi-shield-crown</v-icon
          >
          <span class="text-h5 font-weight-bold"
            >Manage Product Warranties</span
          >
          <v-spacer />
          <v-btn
            color="primary"
            prepend-icon="mdi-plus"
            @click="openCreateDialog"
          >
            Add Product Warranty
          </v-btn>
          <v-btn
            class="ml-2"
            color="secondary"
            prepend-icon="mdi-refresh"
            variant="outlined"
            :loading="reindexing"
            @click="handleReindex"
          >
            Re-index All
          </v-btn>
        </div>

        <v-alert
          v-if="error"
          type="error"
          variant="tonal"
          closable
          class="mb-4"
          @click:close="error = ''"
        >
          {{ error }}
        </v-alert>

        <v-alert
          v-if="successMsg"
          type="success"
          variant="tonal"
          closable
          class="mb-4"
          @click:close="successMsg = ''"
        >
          {{ successMsg }}
        </v-alert>

        <v-card elevation="2" rounded="lg">
          <v-data-table
            :headers="headers"
            :items="warranties"
            :loading="loading"
            :search="search"
            items-per-page="10"
            hover
          >
            <template #top>
              <v-toolbar flat color="transparent">
                <v-text-field
                  v-model="search"
                  density="compact"
                  hide-details
                  placeholder="Filter warranties..."
                  prepend-inner-icon="mdi-magnify"
                  variant="outlined"
                  class="mx-4"
                  max-width="400"
                />
              </v-toolbar>
            </template>

            <template #item.product="{ item }">
              <div>
                <span class="font-weight-medium">{{
                  item.product?.name ?? "N/A"
                }}</span>
                <div class="text-caption text-grey">
                  {{ item.product?.brand }} &bull; {{ item.product?.category }}
                </div>
              </div>
            </template>

            <template #item.durationMonths="{ item }">
              {{ formatDuration(item.durationMonths) }}
            </template>

            <template #item.url="{ item }">
              <a
                v-if="item.url"
                :href="item.url"
                target="_blank"
                rel="noopener"
                class="text-primary"
              >
                <v-icon size="small">mdi-open-in-new</v-icon> Link
              </a>
              <span v-else class="text-grey">—</span>
            </template>

            <template #item.actions="{ item }">
              <v-btn
                icon
                size="small"
                variant="text"
                color="primary"
                @click="openEditDialog(item)"
              >
                <v-icon size="small">mdi-pencil</v-icon>
                <v-tooltip activator="parent" location="top">Edit</v-tooltip>
              </v-btn>
              <v-btn
                icon
                size="small"
                variant="text"
                color="error"
                @click="confirmDelete(item)"
              >
                <v-icon size="small">mdi-delete</v-icon>
                <v-tooltip activator="parent" location="top">Delete</v-tooltip>
              </v-btn>
            </template>
          </v-data-table>
        </v-card>
      </v-col>
    </v-row>

    <!-- Create / Edit Dialog -->
    <v-dialog v-model="formDialog" max-width="600" persistent>
      <v-card>
        <v-card-title class="d-flex align-center pa-4">
          <v-icon class="mr-2" color="primary">{{
            editing ? "mdi-pencil" : "mdi-plus-circle"
          }}</v-icon>
          {{ editing ? "Edit Product Warranty" : "Add Product Warranty" }}
        </v-card-title>
        <v-divider />
        <v-card-text class="pa-4">
          <v-form ref="formRef" v-model="formValid">
            <v-text-field
              v-if="!editing"
              v-model.number="form.productId"
              label="Product ID"
              type="number"
              :rules="[rules.required]"
              variant="outlined"
              density="comfortable"
              class="mb-2"
            />
            <v-text-field
              v-model="form.coverageType"
              label="Coverage Type"
              :rules="[rules.required]"
              variant="outlined"
              density="comfortable"
              class="mb-2"
              placeholder="e.g. Manufacturer, Extended, Accidental"
            />
            <v-text-field
              v-model.number="form.durationMonths"
              label="Duration (months)"
              type="number"
              :rules="[rules.required, rules.positive]"
              variant="outlined"
              density="comfortable"
              class="mb-2"
            />
            <v-text-field
              v-model="form.provider"
              label="Provider"
              :rules="[rules.required]"
              variant="outlined"
              density="comfortable"
              class="mb-2"
              placeholder="e.g. Apple, Best Buy, SquareTrade"
            />
            <v-textarea
              v-model="form.description"
              label="Description"
              variant="outlined"
              density="comfortable"
              rows="3"
              class="mb-2"
            />
            <v-text-field
              v-model="form.url"
              label="URL"
              variant="outlined"
              density="comfortable"
              placeholder="https://..."
            />
          </v-form>
        </v-card-text>
        <v-divider />
        <v-card-actions class="pa-4">
          <v-spacer />
          <v-btn variant="text" @click="formDialog = false">Cancel</v-btn>
          <v-btn
            color="primary"
            variant="flat"
            :disabled="!formValid"
            :loading="saving"
            @click="handleSave"
          >
            {{ editing ? "Update" : "Create" }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation -->
    <v-dialog v-model="deleteDialog" max-width="400">
      <v-card>
        <v-card-title class="text-h6">Delete Product Warranty?</v-card-title>
        <v-card-text>
          Are you sure you want to delete the
          <strong>{{ deleteTarget?.coverageType }}</strong> warranty for
          <strong>{{
            deleteTarget?.product?.name ?? `Product #${deleteTarget?.productId}`
          }}</strong
          >? This action cannot be undone.
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="deleteDialog = false">Cancel</v-btn>
          <v-btn
            color="error"
            variant="flat"
            :loading="deleting"
            @click="handleDelete"
            >Delete</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import type { ProductWarranty } from "~/types";
import {
  getProductWarranties,
  createProductWarranty,
  updateProductWarranty,
  deleteProductWarranty,
  reindexProductWarranties,
} from "~/services/adminProductWarrantyService";

definePageMeta({ layout: "default" });

const { isAdmin } = useAuth();

// Redirect non-admin users
if (import.meta.client && !isAdmin.value) {
  navigateTo("/dashboard");
}

const warranties = ref<ProductWarranty[]>([]);
const loading = ref(true);
const search = ref("");
const error = ref("");
const successMsg = ref("");

const headers = [
  { title: "ID", key: "id", width: "60px" },
  { title: "Product", key: "product", sortable: true },
  { title: "Coverage Type", key: "coverageType" },
  { title: "Duration", key: "durationMonths" },
  { title: "Provider", key: "provider" },
  { title: "Description", key: "description", maxWidth: "250px" },
  { title: "URL", key: "url", width: "80px" },
  { title: "Actions", key: "actions", sortable: false, width: "100px" },
];

// Form state
const formDialog = ref(false);
const formRef = ref();
const formValid = ref(false);
const saving = ref(false);
const editing = ref(false);
const editId = ref<number | null>(null);

const form = ref({
  productId: 0,
  coverageType: "",
  durationMonths: 0,
  provider: "",
  description: "",
  url: "",
});

const rules = {
  required: (v: string | number) =>
    (v !== "" && v !== 0 && v !== null && v !== undefined) || "Required",
  positive: (v: number) => v > 0 || "Must be greater than 0",
};

// Delete state
const deleteDialog = ref(false);
const deleteTarget = ref<ProductWarranty | null>(null);
const deleting = ref(false);

// Reindex
const reindexing = ref(false);

async function loadWarranties() {
  loading.value = true;
  error.value = "";
  try {
    warranties.value = await getProductWarranties();
  } catch {
    error.value = "Failed to load product warranties.";
  } finally {
    loading.value = false;
  }
}

function openCreateDialog() {
  editing.value = false;
  editId.value = null;
  form.value = {
    productId: 0,
    coverageType: "",
    durationMonths: 0,
    provider: "",
    description: "",
    url: "",
  };
  formDialog.value = true;
}

function openEditDialog(pw: ProductWarranty) {
  editing.value = true;
  editId.value = pw.id;
  form.value = {
    productId: pw.productId,
    coverageType: pw.coverageType,
    durationMonths: pw.durationMonths,
    provider: pw.provider,
    description: pw.description ?? "",
    url: pw.url ?? "",
  };
  formDialog.value = true;
}

async function handleSave() {
  saving.value = true;
  error.value = "";
  try {
    if (editing.value && editId.value) {
      await updateProductWarranty(editId.value, {
        coverageType: form.value.coverageType,
        durationMonths: form.value.durationMonths,
        provider: form.value.provider,
        description: form.value.description || undefined,
        url: form.value.url || undefined,
      });
      successMsg.value = "Product warranty updated.";
    } else {
      await createProductWarranty({
        productId: form.value.productId,
        coverageType: form.value.coverageType,
        durationMonths: form.value.durationMonths,
        provider: form.value.provider,
        description: form.value.description || undefined,
        url: form.value.url || undefined,
      });
      successMsg.value = "Product warranty created.";
    }
    formDialog.value = false;
    await loadWarranties();
  } catch {
    error.value = editing.value
      ? "Failed to update product warranty."
      : "Failed to create product warranty. Ensure the Product ID exists.";
  } finally {
    saving.value = false;
  }
}

function confirmDelete(pw: ProductWarranty) {
  deleteTarget.value = pw;
  deleteDialog.value = true;
}

async function handleDelete() {
  if (!deleteTarget.value) return;
  deleting.value = true;
  error.value = "";
  try {
    await deleteProductWarranty(deleteTarget.value.id);
    successMsg.value = "Product warranty deleted.";
    deleteDialog.value = false;
    await loadWarranties();
  } catch {
    error.value = "Failed to delete product warranty.";
  } finally {
    deleting.value = false;
  }
}

async function handleReindex() {
  reindexing.value = true;
  error.value = "";
  try {
    const result = await reindexProductWarranties();
    successMsg.value = result.message;
  } catch {
    error.value = "Failed to re-index product warranties.";
  } finally {
    reindexing.value = false;
  }
}

function formatDuration(months: number): string {
  if (months >= 12 && months % 12 === 0) {
    const years = months / 12;
    return years === 1 ? "1 Year" : `${years} Years`;
  }
  return `${months} Months`;
}

onMounted(loadWarranties);
</script>
