<template>
  <v-container class="py-8" max-width="800">
    <h1 class="text-h4 font-weight-bold mb-6 text-center">Add A Warranty</h1>

    <!-- Upload Zone -->
    <WarrantyUploadZone
      class="mb-4"
      @file-selected="onFileSelected"
      @file-cleared="onFileCleared"
    />

    <!-- OCR Scanning Indicator -->
    <v-alert v-if="scanning" type="info" variant="tonal" class="mb-4">
      <template #prepend>
        <v-progress-circular size="20" indeterminate class="mr-2" />
      </template>
      Scanning receipt for warranty details...
    </v-alert>

    <v-alert
      v-if="scanComplete"
      type="success"
      variant="tonal"
      class="mb-4"
      closable
      @click:close="scanComplete = false"
    >
      Receipt scanned! Form fields have been pre-filled where possible.
    </v-alert>

    <v-alert
      v-if="scanError"
      type="warning"
      variant="tonal"
      class="mb-4"
      closable
      @click:close="scanError = ''"
    >
      {{ scanError }}
    </v-alert>

    <!-- Warranty Details Form -->
    <v-card elevation="2" rounded="xl" class="pa-6">
      <v-card-title class="text-h6 mb-4 px-0">Warranty Details</v-card-title>

      <v-form ref="formRef" v-model="formValid">
        <v-text-field
          v-model="form.productName"
          class="mb-3"
          label="Name of Product"
          :rules="[rules.required]"
          variant="outlined"
        />

        <v-text-field
          v-model="form.brand"
          class="mb-3"
          label="Brand"
          :rules="[rules.required]"
          variant="outlined"
        />

        <v-text-field
          v-model="form.category"
          class="mb-3"
          label="Category"
          :rules="[rules.required]"
          variant="outlined"
        />

        <v-row>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="form.purchaseDate"
              label="Purchase Date"
              :rules="[rules.required]"
              type="date"
              variant="outlined"
            />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="form.expirationDate"
              label="Expiration Date"
              :rules="[rules.required]"
              type="date"
              variant="outlined"
            />
          </v-col>
        </v-row>

        <v-select
          v-model="form.coverageType"
          class="mb-3"
          :items="coverageTypes"
          label="Coverage Type"
          :rules="[rules.required]"
          variant="outlined"
        />

        <v-textarea
          v-model="form.notes"
          label="Notes (optional)"
          rows="3"
          variant="outlined"
        />
      </v-form>

      <v-card-actions class="px-0 pt-4">
        <v-spacer />
        <v-btn color="grey" variant="text" @click="navigateTo('/dashboard')">
          Cancel
        </v-btn>
        <v-btn
          color="primary"
          :disabled="!formValid"
          :loading="submitting"
          size="large"
          variant="flat"
          prepend-icon="mdi-check-circle"
          @click="handleSubmit"
        >
          Save Warranty
        </v-btn>
      </v-card-actions>
    </v-card>

    <!-- Success Dialog -->
    <v-dialog v-model="showSuccess" max-width="400" persistent>
      <v-card class="text-center pa-6" rounded="xl">
        <v-icon class="mb-4" color="success" size="80">mdi-check-circle</v-icon>
        <v-card-title class="text-h5">Warranty Saved!</v-card-title>
        <v-card-text class="text-body-1">
          Your warranty for <strong>{{ form.productName }}</strong> has been
          added successfully.
        </v-card-text>
        <v-card-actions class="justify-center">
          <v-btn color="primary" variant="flat" @click="goToDashboard">
            Go to Dashboard
          </v-btn>
          <v-btn color="primary" variant="outlined" @click="addAnother">
            Add Another
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import type { WarrantyUpload } from "~/types";
import { createWarranty, scanReceipt } from "~/services/warrantyService";

definePageMeta({
  layout: "default",
});

const formRef = ref();
const formValid = ref(false);
const submitting = ref(false);
const showSuccess = ref(false);
const uploadedFile = ref<File | null>(null);
const scanning = ref(false);
const scanComplete = ref(false);
const scanError = ref("");

const form = reactive<WarrantyUpload>({
  productName: "",
  brand: "",
  category: "",
  purchaseDate: "",
  expirationDate: "",
  coverageType: "",
  notes: "",
});

const coverageTypes = [
  "Manufacturer",
  "Extended",
  "Accidental Damage",
  "Theft & Loss",
  "Other",
];

const rules = {
  required: (v: string) => !!v || "This field is required",
};

function onFileSelected(file: File) {
  uploadedFile.value = file;
  scanComplete.value = false;
  scanError.value = "";

  // Run OCR to pre-fill form fields
  scanning.value = true;
  scanReceipt(file)
    .then((result) => {
      if (result.productName && !form.productName)
        form.productName = result.productName;
      if (result.brand && !form.brand) form.brand = result.brand;
      if (result.category && !form.category) form.category = result.category;
      if (result.coverageType && !form.coverageType)
        form.coverageType = result.coverageType;
      if (result.purchaseDate && !form.purchaseDate) {
        form.purchaseDate = result.purchaseDate.substring(0, 10);
      }
      if (result.expirationDate && !form.expirationDate) {
        form.expirationDate = result.expirationDate.substring(0, 10);
      }
      scanComplete.value = true;
    })
    .catch(() => {
      scanError.value =
        "Could not scan receipt. Please fill in the details manually.";
    })
    .finally(() => {
      scanning.value = false;
    });
}

function onFileCleared() {
  uploadedFile.value = null;
  scanning.value = false;
  scanComplete.value = false;
  scanError.value = "";
}

async function handleSubmit() {
  if (!formValid.value) return;

  submitting.value = true;
  try {
    await createWarranty({ ...form, file: uploadedFile.value ?? undefined });
    showSuccess.value = true;
  } finally {
    submitting.value = false;
  }
}

function goToDashboard() {
  showSuccess.value = false;
  navigateTo("/dashboard");
}

function addAnother() {
  showSuccess.value = false;
  form.productName = "";
  form.brand = "";
  form.category = "";
  form.purchaseDate = "";
  form.expirationDate = "";
  form.coverageType = "";
  form.notes = "";
  uploadedFile.value = null;
  scanComplete.value = false;
  scanError.value = "";
}
</script>
