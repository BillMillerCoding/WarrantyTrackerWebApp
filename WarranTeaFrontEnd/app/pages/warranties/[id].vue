<template>
  <v-container class="py-8" max-width="900">
    <!-- Loading -->
    <div v-if="loading" class="text-center py-16">
      <v-progress-circular color="primary" indeterminate size="48" />
    </div>

    <!-- Not Found -->
    <div v-else-if="!warranty" class="text-center py-16">
      <v-icon color="grey" size="80">mdi-file-question</v-icon>
      <h2 class="text-h5 mt-4">Warranty not found</h2>
      <v-btn class="mt-4" color="primary" to="/dashboard"
        >Back to Dashboard</v-btn
      >
    </div>

    <!-- Warranty Details -->
    <div v-else>
      <v-btn
        class="mb-4"
        color="grey"
        prepend-icon="mdi-arrow-left"
        variant="text"
        to="/dashboard"
      >
        Back to Dashboard
      </v-btn>

      <v-row>
        <!-- Main Info -->
        <v-col cols="12" md="8">
          <v-card elevation="2" rounded="xl">
            <v-card-item>
              <template #prepend>
                <v-avatar :color="statusColor" size="56">
                  <v-icon color="white" size="28">mdi-shield-check</v-icon>
                </v-avatar>
              </template>
              <v-card-title class="text-h5">{{
                warranty.productName
              }}</v-card-title>
              <v-card-subtitle class="text-body-1">
                {{ warranty.brand }} &middot; {{ warranty.category }}
              </v-card-subtitle>
              <template #append>
                <v-chip :color="statusColor" label variant="flat">
                  {{ statusLabel }}
                </v-chip>
              </template>
            </v-card-item>

            <v-divider />

            <v-card-text class="pa-6">
              <v-row>
                <v-col cols="6" sm="3">
                  <div class="text-caption text-grey">Purchase Date</div>
                  <div class="text-body-1 font-weight-medium">
                    {{ formatDate(warranty.purchaseDate) }}
                  </div>
                </v-col>
                <v-col cols="6" sm="3">
                  <div class="text-caption text-grey">Expiration Date</div>
                  <div class="text-body-1 font-weight-medium">
                    {{ formatDate(warranty.expirationDate) }}
                  </div>
                </v-col>
                <v-col cols="6" sm="3">
                  <div class="text-caption text-grey">Coverage Type</div>
                  <div class="text-body-1 font-weight-medium">
                    {{ warranty.coverageType }}
                  </div>
                </v-col>
                <v-col cols="6" sm="3">
                  <div class="text-caption text-grey">Days Remaining</div>
                  <div class="text-body-1 font-weight-medium">
                    {{ daysRemaining > 0 ? daysRemaining : "Expired" }}
                  </div>
                </v-col>
              </v-row>

              <v-progress-linear
                class="mt-6"
                :color="statusColor"
                height="8"
                :model-value="coverageProgress"
                rounded
              />
              <div class="text-caption text-right mt-1 text-grey">
                {{ coverageProgress }}% of warranty period used
              </div>

              <div v-if="warranty.notes" class="mt-6">
                <div class="text-caption text-grey mb-1">Notes</div>
                <v-card color="grey-lighten-4" flat rounded="lg">
                  <v-card-text>{{ warranty.notes }}</v-card-text>
                </v-card>
              </div>
            </v-card-text>
          </v-card>
        </v-col>

        <!-- Side Panel -->
        <v-col cols="12" md="4">
          <!-- Receipt Image Card -->
          <v-card
            v-if="warranty.receiptUrl"
            class="mb-4"
            elevation="2"
            rounded="xl"
          >
            <v-card-title class="text-subtitle-1">
              <v-icon class="mr-1" size="small">mdi-receipt</v-icon>
              Receipt
            </v-card-title>
            <v-img
              :src="`/api/warranties/${warranty.id}/receipt`"
              max-height="300"
              cover
              class="mx-4 mb-4 rounded-lg"
            />
          </v-card>

          <!-- Extracted OCR Text Card -->
          <v-card
            v-if="warranty.warrantyDocUrl"
            class="mb-4"
            elevation="2"
            rounded="xl"
          >
            <v-card-title class="text-subtitle-1">
              <v-icon class="mr-1" size="small">mdi-text-recognition</v-icon>
              Extracted Text
            </v-card-title>
            <v-card-text>
              <v-progress-circular
                v-if="loadingText"
                size="20"
                indeterminate
                class="mr-2"
              />
              <pre
                v-else
                class="text-body-2"
                style="white-space: pre-wrap; word-break: break-word"
                >{{ extractedText || "No text extracted." }}</pre
              >
            </v-card-text>
          </v-card>

          <!-- Upload Receipt Card (if no receipt yet) -->
          <v-card
            v-if="!warranty.receiptUrl"
            class="mb-4"
            elevation="2"
            rounded="xl"
          >
            <v-card-title class="text-subtitle-1">Documents</v-card-title>
            <v-card-text class="text-center">
              <v-icon color="grey-lighten-1" size="48">mdi-receipt</v-icon>
              <div class="text-body-2 text-grey mt-2">No receipt uploaded</div>
              <input
                ref="receiptInput"
                accept="image/*"
                hidden
                type="file"
                @change="handleReceiptUpload"
              />
              <v-btn
                class="mt-3"
                color="info"
                prepend-icon="mdi-camera"
                variant="tonal"
                :loading="uploading"
                @click="($refs.receiptInput as HTMLInputElement)?.click()"
              >
                Upload Receipt
              </v-btn>
            </v-card-text>
          </v-card>

          <!-- Actions Card -->
          <v-card elevation="2" rounded="xl">
            <v-card-title class="text-subtitle-1">Actions</v-card-title>
            <v-card-text>
              <input
                ref="receiptInputAction"
                accept="image/*"
                hidden
                type="file"
                @change="handleReceiptUpload"
              />
              <v-btn
                v-if="warranty.receiptUrl"
                block
                class="mb-2"
                color="info"
                prepend-icon="mdi-camera"
                variant="tonal"
                :loading="uploading"
                @click="($refs.receiptInputAction as HTMLInputElement)?.click()"
              >
                Replace Receipt
              </v-btn>
              <v-btn
                block
                color="error"
                prepend-icon="mdi-delete"
                variant="tonal"
                @click="handleDelete"
              >
                Delete Warranty
              </v-btn>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </div>
  </v-container>
</template>

<script setup lang="ts">
import type { Warranty } from "~/types";
import {
  getWarrantyById,
  getExtractedText,
  uploadReceipt,
  deleteWarranty,
} from "~/services/warrantyService";

definePageMeta({
  layout: "default",
});

const route = useRoute();
const warranty = ref<Warranty | null>(null);
const loading = ref(true);
const extractedText = ref("");
const loadingText = ref(false);
const uploading = ref(false);

onMounted(async () => {
  try {
    const id = route.params.id as string;
    const result = await getWarrantyById(id);
    warranty.value = result ?? null;

    // Load extracted OCR text if available
    if (warranty.value?.warrantyDocUrl) {
      loadingText.value = true;
      try {
        extractedText.value = await getExtractedText(warranty.value.id);
      } catch {
        extractedText.value = "";
      } finally {
        loadingText.value = false;
      }
    }
  } finally {
    loading.value = false;
  }
});

async function handleReceiptUpload(event: Event) {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];
  if (!file || !warranty.value) return;

  uploading.value = true;
  try {
    const updated = await uploadReceipt(warranty.value.id, file);
    warranty.value = updated;

    // Reload extracted text
    if (updated.warrantyDocUrl) {
      loadingText.value = true;
      try {
        extractedText.value = await getExtractedText(updated.id);
      } catch {
        extractedText.value = "";
      } finally {
        loadingText.value = false;
      }
    }
  } catch {
    // handle error silently
  } finally {
    uploading.value = false;
    if (target) target.value = "";
  }
}

async function handleDelete() {
  if (!warranty.value) return;
  try {
    await deleteWarranty(warranty.value.id);
    navigateTo("/dashboard");
  } catch {
    // handle error silently
  }
}

const statusColor = computed(() => {
  switch (warranty.value?.status) {
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
  switch (warranty.value?.status) {
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

const daysRemaining = computed(() => {
  if (!warranty.value) return 0;
  const now = new Date();
  const exp = new Date(warranty.value.expirationDate);
  return Math.max(
    0,
    Math.ceil((exp.getTime() - now.getTime()) / (1000 * 60 * 60 * 24)),
  );
});

const coverageProgress = computed(() => {
  if (!warranty.value) return 0;
  const start = new Date(warranty.value.purchaseDate).getTime();
  const end = new Date(warranty.value.expirationDate).getTime();
  const now = Date.now();
  if (now >= end) return 100;
  if (now <= start) return 0;
  return Math.round(((now - start) / (end - start)) * 100);
});

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("en-US", {
    month: "long",
    day: "numeric",
    year: "numeric",
  });
}
</script>
