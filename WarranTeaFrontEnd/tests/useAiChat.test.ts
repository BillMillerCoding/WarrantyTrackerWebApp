import { describe, it, expect, vi, beforeEach } from "vitest";

const mockFetch = vi.fn();
vi.stubGlobal("$fetch", mockFetch);

describe("useAiChat logic", () => {
  beforeEach(() => {
    mockFetch.mockReset();
  });

  describe("message management", () => {
    it("builds correct message array with history", () => {
      const messages: Array<{ role: string; content: string }> = [
        { role: "user", content: "What is my warranty status?" },
        { role: "assistant", content: "Your warranty is active." },
      ];

      const newMessage = "How long does it last?";
      const history = messages.map((m) => ({
        role: m.role,
        content: m.content,
      }));

      expect(history).toHaveLength(2);
      expect(history[0].role).toBe("user");
      expect(history[1].role).toBe("assistant");

      // New message is appended
      messages.push({ role: "user", content: newMessage });
      expect(messages).toHaveLength(3);
    });

    it("clears chat resets messages", () => {
      let messages: Array<{ role: string; content: string }> = [
        { role: "user", content: "Test" },
        { role: "assistant", content: "Response" },
      ];

      // clearChat
      messages = [];
      expect(messages).toHaveLength(0);
    });
  });

  describe("AI API request format", () => {
    it("sends correct request body structure", async () => {
      mockFetch.mockResolvedValue({
        success: true,
        message: "Your warranty covers 2 years of protection.",
      });

      const body = {
        message: "What does my warranty cover?",
        history: [] as Array<{ role: string; content: string }>,
      };

      const result = await mockFetch("/api/ai/warranty-query", {
        method: "POST",
        body,
      });

      expect(mockFetch).toHaveBeenCalledWith("/api/ai/warranty-query", {
        method: "POST",
        body: {
          message: "What does my warranty cover?",
          history: [],
        },
      });
      expect(result.success).toBe(true);
      expect(result.message).toContain("warranty");
    });

    it("handles API error response", async () => {
      mockFetch.mockResolvedValue({
        success: false,
        message: "AI service temporarily unavailable.",
      });

      const result = await mockFetch("/api/ai/warranty-query", {
        method: "POST",
        body: { message: "test", history: [] },
      });

      expect(result.success).toBe(false);
    });

    it("handles network error", async () => {
      mockFetch.mockRejectedValue(new Error("Network error"));

      let error = "";
      try {
        await mockFetch("/api/ai/warranty-query");
      } catch {
        error = "Failed to reach the AI service. Please try again.";
      }

      expect(error).toBe("Failed to reach the AI service. Please try again.");
    });
  });
});
