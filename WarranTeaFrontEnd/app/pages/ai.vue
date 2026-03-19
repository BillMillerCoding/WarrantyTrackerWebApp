<template>
  <v-container class="py-6" fluid>
    <v-row justify="center">
      <v-col cols="12" md="8" lg="6">
        <AiChat
          :messages="messages"
          :loading="loading"
          @send="sendMessage"
          @clear="clearChat"
        />
        <v-alert
          v-if="error"
          type="error"
          variant="tonal"
          class="mt-4"
          closable
          @click:close="error = ''"
        >
          {{ error }}
        </v-alert>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
definePageMeta({ layout: "default" });

const { messages, loading, error, sendMessage, clearChat } = useAiChat();
</script>
