import { describe, it, expect } from "vitest";

describe("auth utility", () => {
  it("authHeaders returns empty object (cookie auth)", async () => {
    const { authHeaders } = await import("~/utils/auth");
    const headers = authHeaders();
    expect(headers).toEqual({});
  });
});
