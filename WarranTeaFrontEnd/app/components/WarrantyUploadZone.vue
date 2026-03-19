<template>
  <v-card
    class="upload-zone text-center pa-8"
    rounded="xl"
    variant="outlined"
    @click="triggerUpload"
  >
    <input
      ref="fileInput"
      accept="image/*,.pdf"
      hidden
      type="file"
      @change="handleFileChange"
    />

    <div v-if="!previewUrl" class="upload-placeholder">
      <v-icon class="mb-4" color="primary" size="80">mdi-cloud-upload</v-icon>
      <div class="text-h6 mb-2">Upload Warranty Document</div>
      <div class="text-body-2 text-grey mb-4">
        Drag and drop or click to upload a photo or PDF
      </div>
      <v-row class="justify-center" dense>
        <v-col cols="auto">
          <v-btn
            color="primary"
            prepend-icon="mdi-camera"
            variant="tonal"
            @click.stop="triggerUpload"
          >
            Take a Photo
          </v-btn>
        </v-col>
        <v-col cols="auto">
          <v-btn
            color="primary"
            prepend-icon="mdi-file-upload"
            variant="outlined"
            @click.stop="triggerUpload"
          >
            Upload As File
          </v-btn>
        </v-col>
      </v-row>
    </div>

    <div v-else class="preview-container">
      <v-img
        v-if="isImage"
        :src="previewUrl"
        aspect-ratio="16/9"
        cover
        max-height="300"
        rounded="lg"
      />
      <v-icon v-else color="error" size="80">mdi-file-pdf-box</v-icon>
      <div class="text-body-2 mt-2">{{ fileName }}</div>
      <v-btn
        class="mt-2"
        color="error"
        prepend-icon="mdi-delete"
        size="small"
        variant="tonal"
        @click.stop="clearFile"
      >
        Remove
      </v-btn>
    </div>
  </v-card>
</template>

<script setup lang="ts">
const fileInput = ref<HTMLInputElement>();
const selectedFile = ref<File | null>(null);
const previewUrl = ref<string | null>(null);

const emit = defineEmits<{
  "file-selected": [file: File];
  "file-cleared": [];
}>();

const fileName = computed(() => selectedFile.value?.name ?? "");
const isImage = computed(
  () => selectedFile.value?.type.startsWith("image/") ?? false,
);

function triggerUpload() {
  fileInput.value?.click();
}

function handleFileChange(event: Event) {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];
  if (file) {
    selectedFile.value = file;
    if (file.type.startsWith("image/")) {
      previewUrl.value = URL.createObjectURL(file);
    } else {
      previewUrl.value = "pdf";
    }
    emit("file-selected", file);
  }
}

function clearFile() {
  if (previewUrl.value && previewUrl.value !== "pdf") {
    URL.revokeObjectURL(previewUrl.value);
  }
  selectedFile.value = null;
  previewUrl.value = null;
  if (fileInput.value) fileInput.value.value = "";
  emit("file-cleared");
}
</script>

<style scoped>
.upload-zone {
  border: 2px dashed rgb(var(--v-theme-primary));
  cursor: pointer;
  transition: background-color 0.2s;
}
.upload-zone:hover {
  background-color: rgba(var(--v-theme-primary), 0.04);
}
</style>
