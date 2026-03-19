<template>
  <v-card class="d-flex flex-column" elevation="2" rounded="xl" height="100%">
    <v-card-title class="d-flex align-center pa-4">
      <v-icon class="mr-2" color="primary">mdi-robot</v-icon>
      <span class="text-h6">Warranty Assistant</span>
      <v-spacer />
      <v-btn
        v-if="messages.length > 0"
        icon
        size="small"
        variant="text"
        @click="emit('clear')"
      >
        <v-icon>mdi-delete-sweep</v-icon>
        <v-tooltip activator="parent" location="bottom">Clear Chat</v-tooltip>
      </v-btn>
    </v-card-title>

    <v-divider />

    <!-- Messages Area -->
    <div
      ref="messagesContainer"
      class="flex-grow-1 overflow-y-auto pa-4"
      style="min-height: 300px; max-height: 60vh"
    >
      <!-- Welcome Message -->
      <div v-if="messages.length === 0" class="text-center py-8">
        <v-icon color="primary" size="64" class="mb-4"
          >mdi-shield-half-full</v-icon
        >
        <div class="text-h6 mb-2">Warranty Assistant</div>
        <div class="text-body-2 text-grey mb-6">
          Ask me anything about your warranties. I can help with:
        </div>
        <v-row dense class="justify-center">
          <v-col
            v-for="suggestion in suggestions"
            :key="suggestion"
            cols="auto"
          >
            <v-chip
              class="ma-1"
              color="primary"
              variant="outlined"
              @click="$emit('send', suggestion)"
            >
              {{ suggestion }}
            </v-chip>
          </v-col>
        </v-row>
      </div>

      <!-- Chat Messages -->
      <div
        v-for="(msg, i) in messages"
        :key="i"
        class="mb-4 d-flex"
        :class="msg.role === 'user' ? 'justify-end' : 'justify-start'"
      >
        <div
          class="pa-3 rounded-xl"
          :class="
            msg.role === 'user' ? 'bg-primary text-white' : 'bg-grey-lighten-4'
          "
          style="max-width: 80%; white-space: pre-wrap; word-break: break-word"
        >
          <div v-if="msg.role === 'assistant'" class="d-flex align-start">
            <v-icon size="18" class="mr-2 mt-1" color="primary"
              >mdi-robot</v-icon
            >
            <div>{{ msg.content }}</div>
          </div>
          <div v-else>{{ msg.content }}</div>
        </div>
      </div>

      <!-- Typing Indicator -->
      <div v-if="loading" class="d-flex justify-start mb-4">
        <div class="bg-grey-lighten-4 pa-3 rounded-xl d-flex align-center">
          <v-icon size="18" class="mr-2" color="primary">mdi-robot</v-icon>
          <v-progress-circular
            size="16"
            width="2"
            indeterminate
            color="primary"
            class="mr-2"
          />
          <span class="text-grey">Thinking...</span>
        </div>
      </div>
    </div>

    <v-divider />

    <!-- Input Area -->
    <div class="pa-4">
      <v-text-field
        v-model="input"
        :disabled="loading"
        hide-details
        placeholder="Ask about a warranty..."
        prepend-inner-icon="mdi-message-text"
        variant="outlined"
        density="comfortable"
        rounded
        @keyup.enter="handleSend"
      >
        <template #append-inner>
          <v-btn
            color="primary"
            icon
            size="small"
            variant="text"
            :disabled="!input.trim() || loading"
            @click="handleSend"
          >
            <v-icon>mdi-send</v-icon>
          </v-btn>
        </template>
      </v-text-field>
    </div>
  </v-card>
</template>

<script setup lang="ts">
import type { AiMessage } from "~/composables/useAiChat";

const props = defineProps<{
  messages: AiMessage[];
  loading: boolean;
}>();

const emit = defineEmits<{
  send: [message: string];
  clear: [];
}>();

const input = ref("");
const messagesContainer = ref<HTMLElement>();

const suggestions = [
  "Is my laptop warranty still valid?",
  "What does manufacturer coverage include?",
  "Extract warranty info from this text",
];

function handleSend() {
  if (!input.value.trim()) return;
  emit("send", input.value);
  input.value = "";
}

watch(
  () => props.messages.length,
  async () => {
    await nextTick();
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  },
);
</script>
