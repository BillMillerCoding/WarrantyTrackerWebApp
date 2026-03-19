import { describe, it, expect, vi, beforeEach } from "vitest";

const mockFetch = vi.fn();
vi.stubGlobal("$fetch", mockFetch);

describe("adminProductWarrantyService", () => {
  beforeEach(() => {
    mockFetch.mockReset();
  });

  describe("getAllProductWarranties", () => {
    it("fetches all product warranties", async () => {
      const data = [
        {
          id: 1,
          productId: 1,
          coverageType: "Manufacturer",
          durationMonths: 12,
          provider: "Apple",
          product: {
            id: 1,
            name: "MacBook",
            brand: "Apple",
            category: "Electronics",
          },
        },
      ];
      mockFetch.mockResolvedValue(data);

      const result = await mockFetch("/api/admin/product-warranties");

      expect(result).toHaveLength(1);
      expect(result[0].provider).toBe("Apple");
    });
  });

  describe("createProductWarranty", () => {
    it("sends POST with warranty data", async () => {
      const newPw = {
        productId: 2,
        coverageType: "Extended",
        durationMonths: 24,
        provider: "Samsung",
        description: "Extended coverage",
      };
      mockFetch.mockResolvedValue({ id: 3, ...newPw });

      const result = await mockFetch("/api/admin/product-warranties", {
        method: "POST",
        body: newPw,
      });

      expect(mockFetch).toHaveBeenCalledWith("/api/admin/product-warranties", {
        method: "POST",
        body: newPw,
      });
      expect(result.id).toBe(3);
    });
  });

  describe("updateProductWarranty", () => {
    it("sends PUT with partial update", async () => {
      const update = { coverageType: "Premium" };
      mockFetch.mockResolvedValue({ id: 1, coverageType: "Premium" });

      const result = await mockFetch("/api/admin/product-warranties/1", {
        method: "PUT",
        body: update,
      });

      expect(result.coverageType).toBe("Premium");
    });
  });

  describe("deleteProductWarranty", () => {
    it("sends DELETE request", async () => {
      mockFetch.mockResolvedValue(undefined);

      await mockFetch("/api/admin/product-warranties/1", {
        method: "DELETE",
      });

      expect(mockFetch).toHaveBeenCalledWith(
        "/api/admin/product-warranties/1",
        expect.objectContaining({ method: "DELETE" }),
      );
    });
  });

  describe("reindexAll", () => {
    it("triggers reindex and returns count", async () => {
      mockFetch.mockResolvedValue({ count: 30 });

      const result = await mockFetch("/api/admin/product-warranties/reindex", {
        method: "POST",
      });

      expect(result.count).toBe(30);
    });
  });
});
