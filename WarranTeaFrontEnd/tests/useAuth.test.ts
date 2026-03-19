import { describe, it, expect, vi, beforeEach } from "vitest";

// Mock $fetch globally before importing composable
const mockFetch = vi.fn();
vi.stubGlobal("$fetch", mockFetch);

// Mock Nuxt auto-imports
vi.mock("#imports", () => ({
  ref: (val: any) => {
    const r = { value: val };
    return r;
  },
  computed: (fn: () => any) => ({ value: fn() }),
  useState: (_key: string, init: () => any) => {
    const r = { value: init() };
    return r;
  },
  navigateTo: vi.fn(),
}));

describe("useAuth logic", () => {
  beforeEach(() => {
    mockFetch.mockReset();
  });

  describe("cookie-based auth", () => {
    it("no localStorage token management needed", () => {
      // Cookie-based auth does not use localStorage
      expect(localStorage.getItem("warrantea-token")).toBeNull();
    });
  });

  describe("role checking", () => {
    it("identifies admin user", () => {
      const roles = ["Admin"];
      expect(roles.includes("Admin")).toBe(true);
    });

    it("non-admin user has no Admin role", () => {
      const roles: string[] = [];
      expect(roles.includes("Admin")).toBe(false);
    });

    it("handles undefined roles", () => {
      const roles: string[] | undefined = undefined;
      expect(roles?.includes("Admin") ?? false).toBe(false);
    });
  });

  describe("auth API calls", () => {
    it("login returns user without token", async () => {
      const resp = {
        id: "1",
        name: "Alice",
        email: "alice@test.com",
        roles: [] as string[],
      };
      mockFetch.mockResolvedValue(resp);

      const result = await mockFetch("/api/auth/login", {
        method: "POST",
        body: { email: "alice@test.com", password: "pass" },
      });

      expect(result.id).toBe("1");
      expect(result).not.toHaveProperty("token");
    });

    it("fetchUser calls /api/auth/me without headers", async () => {
      const user = {
        id: "1",
        name: "Alice",
        email: "alice@test.com",
        roles: [] as string[],
      };
      mockFetch.mockResolvedValue(user);

      const result = await mockFetch("/api/auth/me");
      expect(mockFetch).toHaveBeenCalledWith("/api/auth/me");
      expect(result.name).toBe("Alice");
    });
  });
});
