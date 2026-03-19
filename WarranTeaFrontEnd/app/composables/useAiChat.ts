import { useApiBase } from "~/composables/useApi";

export interface AiMessage {
  role: "user" | "assistant";
  content: string;
}

export interface AiQueryResponse {
  message: string;
  success: boolean;
}

export function useAiChat() {
  const apiBase = useApiBase();
  const messages = ref<AiMessage[]>([]);
  const loading = ref(false);
  const error = ref("");

  async function sendMessage(userMessage: string) {
    if (!userMessage.trim()) return;

    error.value = "";
    messages.value.push({ role: "user", content: userMessage });

    loading.value = true;
    try {
      const history = messages.value.slice(0, -1).map((m) => ({
        role: m.role,
        content: m.content,
      }));

      const response = await $fetch<AiQueryResponse>(
        `${apiBase}/ai/warranty-query`,
        {
          method: "POST",
          body: { message: userMessage, history },
          credentials: "include",
        },
      );

      if (response.success) {
        messages.value.push({ role: "assistant", content: response.message });
      } else {
        error.value = response.message;
        messages.value.push({ role: "assistant", content: response.message });
      }
    } catch {
      error.value = "Failed to reach the AI service. Please try again.";
      messages.value.push({
        role: "assistant",
        content: "Sorry, I couldn't process your request. Please try again.",
      });
    } finally {
      loading.value = false;
    }
  }

  function clearChat() {
    messages.value = [];
    error.value = "";
  }

  return {
    messages,
    loading,
    error,
    sendMessage,
    clearChat,
  };
}
